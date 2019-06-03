using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ArenaLevel
{
    public string name = "Arena Level";
    public string openMessage = "";
    public EncounterRegion encounterRegion = null;
}

public class Arena : MonoBehaviour
{
    [SerializeField] private float m_transitionFadeTimeSec = 2f;
    [SerializeField] private Image m_transitionBackground = null;
    [SerializeField] private TextMeshProUGUI m_transitionTextMesh = null;

    [SerializeField] private bool m_runOnStart = false;
    [SerializeField] private List<ArenaLevel> m_arenaLevelList = new List<ArenaLevel>();
    [SerializeField] private List<ArenaPlayer> m_arenaPlayerList = new List<ArenaPlayer>();

    private ArenaLevel CurrentLevel {  get { return m_arenaLevelList[m_arenaLevelIndex]; } }

    private int m_arenaLevelIndex = -1;

    private void Start() {
        m_transitionTextMesh.text = "";
        if ( m_runOnStart ) NextLevel();
        BattleManager.instance.OnFinish.AddListener( NextLevel );
    }

    private void NextLevel() {
        ++m_arenaLevelIndex;
        StartCoroutine( Transition() );
    }

    private IEnumerator Transition() {
        if ( LoadBattle() == false ) yield break;

        if ( m_transitionTextMesh != null && string.IsNullOrEmpty( CurrentLevel.openMessage ) == false )
            m_transitionTextMesh.text = $"~ {CurrentLevel.name} ~\n{CurrentLevel.openMessage}";

        var fadeBackground = m_transitionBackground.FadeOutCoroutine( m_transitionFadeTimeSec );
        StartCoroutine( fadeBackground );
        yield return new WaitForSeconds( m_transitionFadeTimeSec );

        m_transitionTextMesh.text = "";

        BattleManager.instance.StartBattle();
    }

    private bool LoadBattle() {
        var encounterRegion = CurrentLevel.encounterRegion;
        if( encounterRegion == null ) {
            for( var i = m_arenaLevelIndex; i >= 0; --i ) {
                encounterRegion = m_arenaLevelList[i].encounterRegion;
                if ( encounterRegion != null ) break;
            }

            if( encounterRegion == null ) {
                Debug.LogError( $"[Arena] No encounter region for level {m_arenaLevelIndex} ({CurrentLevel.name})" +
                    $" and no fallback. Aborting battle load." );
                return false;
            }
        }

        BattleManager.instance.Clear();

        var enemyList = encounterRegion.GenerateEncounter();
        BattleManager.instance.AddEnemies( enemyList );

        var playerList = new List<Actor>();
        foreach ( var arenaPlayer in m_arenaPlayerList )
            if ( arenaPlayer.firstLevel <= m_arenaLevelIndex )
                playerList.Add( arenaPlayer.player );
        BattleManager.instance.AddPlayers( playerList );

        BattleManager.instance.LoadBattle();
        return true;
    }
}
