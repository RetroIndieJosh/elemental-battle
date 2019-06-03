﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class EnemyEntry
{
    [SerializeField] private ActorDef m_enemyDef = null;
    [SerializeField] private int m_level = 1;
    [SerializeField] private int m_count = 1;

    public List<Actor> EnemyList {
        get {
            var enemyList = new List<Actor>();
            for ( var i = 0; i < m_count; ++i ) {
                var obj = new GameObject();
                var actor = obj.AddComponent<Actor>();
                actor.Set( m_enemyDef, m_level );
                enemyList.Add( actor );
            }
            return enemyList;
        }
    }
}

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter", order = 1)]
public class Encounter: ScriptableObject
{
    [SerializeField] private List<EnemyEntry> m_enemyEntryList = new List<EnemyEntry>();

    public List<Actor> EnemyList {
        get {
            var enemyList = new List<Actor>();
            foreach ( var entry in m_enemyEntryList )
                enemyList.AddRange( entry.EnemyList );
            return enemyList;
        }
    }
}

[CreateAssetMenu(fileName = "New Encounter Region", menuName = "Encounter Region", order = 1)]
public class EncounterRegion: ScriptableObject
{
    [SerializeField] List<Encounter> m_enemyPartyList = new List<Encounter>();

    public List<Actor> GenerateEncounter() {
        var roll = Random.Range( 0, m_enemyPartyList.Count );
        return m_enemyPartyList[roll].EnemyList;
    }
}

[System.Serializable]
public class ArenaPlayer
{
    public string Name {  get { return player == null ? "Unset" : player.name; } }
    public Actor player = null;
    public int firstLevel = 0;
}

public class Arena : MonoBehaviour
{
    [SerializeField] private float m_transitionFadeTimeSec = 2f;
    [SerializeField] private Image m_transitionBackground = null;
    [SerializeField] private TextMeshProUGUI m_transitionTextMesh = null;

    [SerializeField] private bool m_runOnStart = false;
    [SerializeField] private EncounterRegion m_encounterRegion = null;
    [SerializeField] private List<ArenaPlayer> m_arenaPlayerList = new List<ArenaPlayer>();

    [SerializeField] private List<string> m_battleStartMessageList = new List<string>();

    private int m_arenaLevel = 0;

    private void Start() {
        m_transitionTextMesh.text = "";
        if ( m_runOnStart ) NextLevel();
        BattleManager.instance.OnFinish.AddListener( NextLevel );
    }

    private void NextLevel() {
        StartCoroutine( Transition() );
    }

    private IEnumerator Transition() {
        var msg = "";
        if( m_transitionTextMesh != null && m_battleStartMessageList.Count > m_arenaLevel )
            msg = m_battleStartMessageList[m_arenaLevel];

        ++m_arenaLevel;

        if ( string.IsNullOrEmpty( msg ) == false )
            m_transitionTextMesh.text = $"~ Level {m_arenaLevel} ~\n{msg}";

        LoadBattle();

        var fadeBackground = m_transitionBackground.FadeOutCoroutine( m_transitionFadeTimeSec );
        StartCoroutine( fadeBackground );
        yield return new WaitForSeconds( m_transitionFadeTimeSec );

        m_transitionTextMesh.text = "";

        /*
        var fadeInBackground = m_transitionBackground.FadeInCoroutine( fadeTime );
        //StartCoroutine( fadeInBackground );
        var fadeInText = m_transitionTextMesh.FadeInCoroutine( fadeTime );
        //StartCoroutine( fadeInText );
        //yield return new WaitForSeconds( fadeTime );

        //var fadeOutBackground = m_transitionBackground.FadeOutCoroutine( fadeTime );
        var fadeOutBackground = m_transitionBackground.FadeCoroutineHandler( fadeTime, false );
        StartCoroutine( fadeOutBackground );
        //var fadeOutText = m_transitionTextMesh.FadeOutCoroutine( fadeTime );
        var fadeOutText = m_transitionTextMesh.FadeCoroutineHandler( fadeTime, false );
        StartCoroutine( fadeOutText );
        yield return new WaitForSeconds( fadeTime );
        */

        BattleManager.instance.StartBattle();
    }

    private void LoadBattle() {
        if( m_encounterRegion == null ) {
            Debug.LogWarning( "[Arena] Tried to start without an encounter region. Aborting." );
            return;
        }

        BattleManager.instance.Clear();

        var enemyList = m_encounterRegion.GenerateEncounter();
        BattleManager.instance.AddEnemies( enemyList );

        var playerList = new List<Actor>();
        foreach ( var arenaPlayer in m_arenaPlayerList )
            if ( arenaPlayer.firstLevel <= m_arenaLevel )
                playerList.Add( arenaPlayer.player );
        BattleManager.instance.AddPlayers( playerList );

        BattleManager.instance.LoadBattle();
    }
}