using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Linq;

public enum Element
{
    Air,
    Earth,
    Fire,
    Water,
    None
}

static public class ListExt
{
    public static string ToString<T>( this List<T> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var listStr = "";
        foreach ( var str in a_list )
            listStr += $"{str}{a_delimiter}";

        listStr = listStr.Substring( 0, listStr.Length - 2 );
        if ( a_endDelimiter != null )
            listStr += a_endDelimiter;
        return listStr;
    }

    public static string ToString( this List<MonoBehaviour> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var strList = new List<string>();
        foreach ( var o in a_list )
            strList.Add( o.name );

        return strList.ToString();
    }
}

[DisallowMultipleComponent]
public class BattleManager : MonoBehaviour
{
    class TurnData
    {
        public Actor actor = null;
        public int turnValue = 0;

        public TurnData( Actor a_actor, int a_turnValue ) {
            actor = a_actor;
            turnValue = a_turnValue;
        }
    }

    static public BattleManager instance = null;

    private List<string> m_outputList = new List<string>();

    public int SpeedMax { get { return m_speedMax; } }

    [SerializeField] private int m_speedMax = 1000;
    [SerializeField] private int m_turnOrderLookAhead = 5;
    [SerializeField] private float m_aiTurnlengthSec = 0.5f;

    [Header( "Debug" )]
    [SerializeField] private bool m_enterBattleOnStart = false;
    [SerializeField] private bool m_showTurnNumbers = false;

    [Header( "UI" )]
    [SerializeField] private TextMeshProUGUI m_outputDisplay = null;
    [SerializeField] private int m_outputMaxLines = 10;
    [SerializeField] private TextMeshProUGUI m_statsDisplay = null;
    [SerializeField] private TextMeshProUGUI m_turnOrderDisplay = null;

    private ButtonControl m_attackButton = null;
    private ButtonControl m_backButton = null;
    private ButtonControl m_defendButton = null;
    private ButtonControl m_spellButton = null;
    private ButtonControl[] m_castButton = null;

    private KeyControl m_attackKey = null;
    private KeyControl m_backKey = null;
    private KeyControl m_defendKey = null;
    private KeyControl m_spellKey = null;
    private KeyControl[] m_castKey = null;

    private List<Actor> m_enemyList = new List<Actor>();
    private List<Actor> m_playerList = new List<Actor>();

    private List<TurnData> m_turnOrderList = new List<TurnData>();

    private Actor m_activeActor = null;
    private float m_timeSinceTurnStart = 0f;

    private bool m_isRunning = false;
    private int m_currentTurn = 0;

    private int m_airEarthSpectrum = 0;
    private int m_fireWaterSpectrum = 0;

    public void AddEnemy( Actor a_enemy ) {
        Output( $"Add enemy {a_enemy.name}" );
        m_enemyList.Add( a_enemy );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddPlayer( Actor a_player ) {
        Output( $"Add player {a_player.name}" );
        m_playerList.Add( a_player );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddEnemies( List<Actor> a_enemyList ) {
        Output( $"Add enemies: {a_enemyList.ToString( ", " )} " );
        m_enemyList.AddRange( a_enemyList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddPlayers( List<Actor> a_playerList ) {
        Output( $"Add players: {a_playerList.ToString( ", " )}" );
        m_playerList.AddRange( a_playerList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void Clear() {
        m_enemyList.Clear();
        m_playerList.Clear();
    }

    private void RemoveDeadEnemies() {
        m_turnOrderList.RemoveAll( data => m_enemyList.Contains( data.actor ) && data.actor.IsDead );
        m_enemyList.RemoveAll( enemy => enemy.IsDead );
    }

    private void Next() {
        RemoveDeadEnemies();

        m_activeActor = m_turnOrderList[0].actor;
        m_currentTurn = m_turnOrderList[0].turnValue;
        m_turnOrderList.RemoveAt( 0 );

        ReviseTurnOrder();

        if ( m_playerList.Contains( m_activeActor ) ) {
            CurPlayerMenuState = PlayerMenuState.TopMenu;
        }
        m_activeActor.StartTurn();
        m_timeSinceTurnStart = 0f;
    }

    public void StartBattle() {
        if ( m_enemyList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no enemies set; ignoring." );
            return;
        }

        if ( m_playerList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no players set; ignoring." );
            return;
        }

        // TODO find fastest actor and set as active actor
        m_activeActor = m_playerList[0];

        Output( $"Start {m_playerList.Count} vs {m_enemyList.Count}, {m_activeActor} first" );

        m_isRunning = true;

        ReviseTurnOrder();
        Next();
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        m_attackButton = Gamepad.current.aButton;
        m_backButton = Gamepad.current.leftShoulder;
        m_defendButton = Gamepad.current.bButton;
        m_spellButton = Gamepad.current.yButton;

        m_castButton = new ButtonControl[4];
        m_castButton[0] = Gamepad.current.aButton;
        m_castButton[1] = Gamepad.current.xButton;
        m_castButton[2] = Gamepad.current.yButton;
        m_castButton[3] = Gamepad.current.bButton;

        m_attackKey = Keyboard.current.xKey;
        m_backKey = Keyboard.current.escapeKey;
        m_defendKey = Keyboard.current.cKey;
        m_spellKey = Keyboard.current.vKey;

        m_castKey = new KeyControl[4];
        m_castKey[0] = Keyboard.current.xKey;
        m_castKey[1] = Keyboard.current.cKey;
        m_castKey[2] = Keyboard.current.vKey;
        m_castKey[3] = Keyboard.current.bKey;
    }

    private void Update() {
        if ( m_isRunning == false ) {
            if ( m_enterBattleOnStart == false ) return;

            // do this here so all Start()s have run
            var actorList = FindObjectsOfType<Actor>();
            foreach ( var actor in actorList ) {
                if ( actor.name.Contains( "Player" ) )
                    AddPlayer( actor );
                else
                    AddEnemy( actor );
            }
            StartBattle();
            return;
        }

        UpdateHud();

        m_timeSinceTurnStart += Time.deltaTime;

        if ( m_playerList.Contains( m_activeActor ) )
            UpdateTurnPlayer();
        else UpdateTurnAi();
    }

    private void Output( string a_output ) {
        var newOutputLines = a_output.Split( '\n' );
        m_outputList.AddRange( newOutputLines );
        while ( m_outputList.Count > m_outputMaxLines )
            m_outputList.RemoveAt( 0 );

        var output = "";
        foreach ( var line in m_outputList )
            output += $"{line}\n";
        m_outputDisplay.text = output;
    }

    private void UpdateHud() {
        if ( m_isRunning == false ) return;

        var statsStr = $"Air << {m_airEarthSpectrum} >> Earth\n"
            + $"Fire << {m_fireWaterSpectrum} >> Water\n\n";
        foreach ( var actor in m_playerList ) {
            statsStr += $"<color=green>{actor.Stats}</color>";
            if ( actor == m_activeActor ) statsStr += " <=";
            statsStr += "\n\n";
        }
        foreach ( var actor in m_enemyList ) {
            statsStr += $"<color=red>{actor.Stats}</color>";
            if ( actor == m_activeActor ) statsStr += " <=";
            statsStr += "\n\n";
        }
        m_statsDisplay.text = statsStr;

        var startColor = m_playerList.Contains( m_activeActor ) ? "green" : "red";
        var turnOrderStr = "";
        if ( m_showTurnNumbers )
            turnOrderStr += $"[{m_currentTurn}] ";
        turnOrderStr += $"<color={startColor}>{m_activeActor}</color>\n";
        foreach ( var turnData in m_turnOrderList ) {
            var color = m_playerList.Contains( turnData.actor ) ? "green" : "red";
            if ( m_showTurnNumbers ) turnOrderStr += $"[{turnData.turnValue}] ";
            turnOrderStr += $"<color={color}>{turnData.actor.name}</color> / ";
        }
        m_turnOrderDisplay.text = turnOrderStr.Substring( 0, turnOrderStr.Length - 2 );
    }

    private void UpdateTurnAi() {
        if ( m_timeSinceTurnStart < m_aiTurnlengthSec ) return;

        var roll = Random.Range( 0, m_playerList.Count );
        var target = m_playerList[roll];
        var damage = m_activeActor.Attack( target );
        Output( $"{m_activeActor} attacks {target} for {damage} damage" );
        Next();
    }

    private enum PlayerMenuState
    {
        TopMenu,
        SpellMenu
    }

    private PlayerMenuState m_curPlayerMenuState = PlayerMenuState.TopMenu;
    private PlayerMenuState CurPlayerMenuState {
        set {
            m_curPlayerMenuState = value;

            switch ( m_curPlayerMenuState ) {
                case PlayerMenuState.SpellMenu: ShowControlsSpellMenu(); break;
                case PlayerMenuState.TopMenu: ShowControlsTopMenu(); break;
            }
        }
    }

    private void ShowControlsSpellMenu() {
        var keyboardStr = "(Keyboard) ";
        var gamepadStr = "(Gamepad) ";
        for ( var i = 0; i < m_activeActor.Spells.Length; ++i ) {
            var spell = m_activeActor.Spells[i];
            var color = m_activeActor.CanCastSpell( i ) ? "green" : "red";
            var spellStr = $"<color={color}>{spell.name} ({spell.Cost})</color>, ";
            keyboardStr += $"({m_castKey[i].displayName}) {spellStr}";
            gamepadStr += $"({m_castButton[i].displayName}) {spellStr}";
        }

        Output( keyboardStr );
        Output( gamepadStr );
    }

    private void ShowControlsTopMenu() {
        Output( $"[Keyboard] ({m_attackKey.displayName}) Attack / ({m_defendKey.displayName}) Defend" +
            $"/ ({m_spellKey.displayName}) Spell" );
        Output( $"[Gamepad] ({m_attackButton.displayName}) Attack / ({m_defendButton.displayName}) Defend" +
            $"/ ({m_spellButton.displayName}) Spell" );
    }

    private void UpdateTurnPlayer() {
        switch ( m_curPlayerMenuState ) {
            case PlayerMenuState.SpellMenu: PlayerSpellMenu(); break;
            case PlayerMenuState.TopMenu: PlayerTopMenu(); break;
        }
    }

    private void PlayerSpellMenu() {
        if ( m_backButton.wasPressedThisFrame || m_backKey.wasPressedThisFrame ) {
            CurPlayerMenuState = PlayerMenuState.TopMenu;
            return;
        }

        var spells = m_activeActor.Spells;
        for ( var i = 0; i < spells.Length; ++i ) {
            if ( m_castButton[i].wasPressedThisFrame || m_castKey[i].wasPressedThisFrame ) {
                var wasCast = m_activeActor.CastSpell( i );
                if ( wasCast ) {
                    Output( $"{m_activeActor.name} casts {spells[i].name} for {spells[i].Cost} CP" );
                    Next();
                } else {
                    Output( $"{m_activeActor.name} cannot cast {spells[i].name} (not enough CP)" );
                }
            }
        }
    }

    private void PlayerTopMenu() {
        if ( m_attackButton.wasPressedThisFrame || m_attackKey.wasPressedThisFrame ) {
            var roll = Random.Range( 0, m_enemyList.Count );
            var target = m_enemyList[roll];
            var damage = m_activeActor.Attack( target );
            Output( $"{m_activeActor} attacks {target} for {damage} damage" );
            Next();
        } else if ( m_defendButton.wasPressedThisFrame || m_defendKey.wasPressedThisFrame ) {
            m_activeActor.Defend();
            Output( $"{m_activeActor} is now defending" );
            Next();
        } else if ( m_spellButton.wasPressedThisFrame || m_spellKey.wasPressedThisFrame ) {
            if ( m_activeActor.Spells.Length == 0 ) {
                Output( $"{m_activeActor.name} has no spells" );
                return;
            }

            CurPlayerMenuState = PlayerMenuState.SpellMenu;
        }
    }

    private void ReviseTurnOrder() {
        var fullList = new List<Actor>();
        fullList.AddRange( m_enemyList );
        fullList.AddRange( m_playerList );

        var turn = 1;
        if ( m_turnOrderList.Count > 0 )
            turn = m_turnOrderList[m_turnOrderList.Count - 1].turnValue + 1;
        while ( m_turnOrderList.Count < m_turnOrderLookAhead ) {
            var potentialActorList = new List<Actor>();
            foreach ( var actor in fullList )
                if ( turn % actor.Speed == 0 )
                    potentialActorList.Add( actor );
            if ( potentialActorList.Count > 0 ) {
                while ( potentialActorList.Count > 0 ) {
                    var roll = Random.Range( 0, potentialActorList.Count );
                    var actor = potentialActorList[roll];
                    potentialActorList.RemoveAt( roll );

                    if ( m_enemyList.Contains( actor ) && actor.IsDead )
                        continue;
                    m_turnOrderList.Add( new TurnData( actor, turn + 1 ) );
                }
            }
            ++turn;
            if ( turn > 10000 ) {
                Debug.LogError( "Infinite loop detected in determining turn order" );
                return;
            }
        }
    }
}
