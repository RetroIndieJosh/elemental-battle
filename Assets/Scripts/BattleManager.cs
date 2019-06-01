using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class BattleManager : MonoBehaviour
{
    static public BattleManager instance = null;

    public int SpeedMax {  get { return m_speedMax; } }

    [SerializeField] private int m_speedMax = 1000;
    [SerializeField] private int m_turnOrderLookAhead = 5;

    [Header( "UI" )]
    [SerializeField] private TextMeshProUGUI m_statsDisplay = null;
    [SerializeField] private TextMeshProUGUI m_turnOrderDisplay = null;

    private List<Actor> m_enemyList = new List<Actor>();
    private List<Actor> m_playerList = new List<Actor>();

    private List<Actor> m_turnOrderList = new List<Actor>();

    private Actor m_activeActor = null;

    private bool m_isRunning = false;

    public void AddEnemy(Actor a_enemy ) {
        Debug.Log( $"[BattleManager] Add enemy {a_enemy}" );
        m_enemyList.Add( a_enemy );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddPlayer(Actor a_player ) {
        Debug.Log( $"[BattleManager] Add player {a_player}" );
        m_playerList.Add( a_player );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddEnemies(List<Actor> a_enemyList ) {
        Debug.Log( $"[BattleManager] Add {a_enemyList.Count} enemies" );
        m_enemyList.AddRange( a_enemyList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddPlayers(List<Actor> a_playerList ) {
        Debug.Log( $"[BattleManager] Add {a_playerList.Count} players" );
        m_playerList.AddRange( a_playerList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void Clear() {
        m_enemyList.Clear();
        m_playerList.Clear();
    }

    public void Next() {
        ReviseTurnOrder();
        m_turnOrderList.RemoveAt( 0 );
        m_activeActor = m_turnOrderList[0];
    }

    public void StartBattle() {
        if( m_enemyList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no enemies set; ignoring." );
            return;
        }

        if( m_playerList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no players set; ignoring." );
            return;
        }

        // TODO find fastest actor and set as active actor
        m_activeActor = m_playerList[0];

        Debug.Log( $"[BattleManager] Start {m_playerList.Count} vs {m_enemyList.Count}, {m_activeActor} first" );

        ReviseTurnOrder();
        m_isRunning = true;
    }

    private void Awake() {
        instance = this;
    }

    private void Update() {
        UpdateHud();
    }

    private void UpdateHud() {
        if ( m_isRunning == false ) return;

        var statsStr = "";
        foreach ( var actor in m_playerList ) {
            if ( actor == m_activeActor ) statsStr += "=> ";
            statsStr += $"<color=green>{actor.Stats}</color>\n\n";
        }
        foreach ( var actor in m_enemyList ) {
            if ( actor == m_activeActor ) statsStr += "=> ";
            statsStr += $"<color=red>{actor.Stats}</color>\n\n";
        }
        m_statsDisplay.text = statsStr;

        var turnOrderStr = "";
        foreach ( var actor in m_turnOrderList ) {
            var color = m_playerList.Contains( actor ) ? "green" : "red";
            turnOrderStr += $"<color={color}>{actor.name}</color> / ";
        }
        m_turnOrderDisplay.text = turnOrderStr.Substring(0, turnOrderStr.Length - 2 );
    }

    private void ReviseTurnOrder() {
        var fullList = new List<Actor>();
        fullList.AddRange( m_enemyList );
        fullList.AddRange( m_playerList );

        m_turnOrderList.Clear();
        m_turnOrderList.Add( m_activeActor );

        var turn = m_activeActor.Speed + 1;
        while( m_turnOrderList.Count < m_turnOrderLookAhead ) {
            var potentialActorList = new List<Actor>();
            foreach ( var actor in fullList )
                if ( turn % actor.Speed == 0 )
                    potentialActorList.Add( actor );
            if ( potentialActorList.Count > 0 ) {
                while( potentialActorList.Count > 0 ) {
                    var roll = Random.Range( 0, potentialActorList.Count );
                    var actor = potentialActorList[roll];
                    m_turnOrderList.Add( actor );
                    potentialActorList.RemoveAt( roll );

                    Debug.Log( $"Turn {turn}: {actor}" );
                }
            }
            ++turn;
            if( turn > 10000 ) {
                Debug.LogError( "Infinite loop detected in determining turn order" );
                return;
            }
        }
    }
}
