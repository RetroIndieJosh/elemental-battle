using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private int m_turnOrderLookAhead = 5;

    private List<Actor> m_enemyList = new List<Actor>();
    private List<Actor> m_playerList = new List<Actor>();

    private List<Actor> m_turnOrderList = new List<Actor>();

    private Actor m_activeActor = null;

    private bool m_isRunning = false;

    public void AddEnemy(Actor a_enemy ) {
        m_enemyList.Add( a_enemy );
        if ( m_isRunning ) UpdateTurnOrder();
    }

    public void AddPlayer(Actor a_player ) {
        m_playerList.Add( a_player );
        if ( m_isRunning ) UpdateTurnOrder();
    }

    public void AddEnemies(List<Actor> a_enemyList ) {
        m_enemyList.AddRange( a_enemyList );
        if ( m_isRunning ) UpdateTurnOrder();
    }

    public void AddPlayers(List<Actor> a_playerList ) {
        m_playerList.AddRange( a_playerList );
        if ( m_isRunning ) UpdateTurnOrder();
    }

    public void Clear() {
        m_enemyList.Clear();
        m_playerList.Clear();
    }

    public void Next() {
        UpdateTurnOrder();
        m_turnOrderList.RemoveAt( 0 );
        m_activeActor = m_turnOrderList[0];
    }

    public void StartBattle() {
        if( m_enemyList.Count == 0 ) {
            Debug.LogWarning( "Tried to start battle but no enemies set; ignoring." );
            return;
        }

        if( m_playerList.Count == 0 ) {
            Debug.LogWarning( "Tried to start battle but no players set; ignoring." );
            return;
        }

        // TODO find fastest actor and set as active actor
        m_activeActor = m_playerList[0];

        Debug.Log( $"Start battle: {m_playerList} vs {m_enemyList}, {m_activeActor} first." );

        UpdateTurnOrder();
        m_isRunning = true;
    }

    private void UpdateTurnOrder() {
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
                var roll = Random.Range( 0, potentialActorList.Count );
                var actor = potentialActorList[roll];
                m_turnOrderList.Add( actor );
                Debug.Log( $"Turn {turn}: {actor}" );
            }
            ++turn;
            if( turn > 10000 ) {
                Debug.LogError( "Infinite loop detected in determining turn order" );
                return;
            }
        }

        Debug.Log( $"Turn order: {m_turnOrderList}" );
    }
}
