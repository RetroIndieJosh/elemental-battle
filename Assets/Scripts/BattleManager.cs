using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;

[DisallowMultipleComponent]
public class BattleManager : MonoBehaviour
{
    private class TurnData
    {
        public Actor actor = null;
        public int turnValue = 0;
        public TurnData( Actor a_actor, int a_turnValue ) {
            actor = a_actor;
            turnValue = a_turnValue;
        }
    }

    private enum PlayerMenuState
    {
        TopMenu,
        SpellMenu
    }

    static public BattleManager instance = null;

    public Dictionary<Element, Element> OpposingElement = new Dictionary<Element, Element>() {
        {Element.Air, Element.Earth },
        {Element.Earth, Element.Air },
        {Element.Fire, Element.Water },
        {Element.Water, Element.Fire }
    };

    [SerializeField] private int m_speedMax = 1000;
    [SerializeField] private int m_turnOrderLookAhead = 5;
    [SerializeField] private float m_aiTurnlengthSec = 0.5f;

    [Header("Charge Points")]
    [SerializeField] private int m_chargePointsPerAttack = 1;
    [SerializeField] private int m_chargePointsMax = 10;

    [Header( "Field Effect" )]
    [SerializeField] private float m_primaryFieldEffectMatchMult = 2f;
    [SerializeField] private float m_secondaryFieldEffectMatchMult = 2f;
    [SerializeField] private float m_primaryFieldEffectOpposeMult = 2f;
    [SerializeField] private float m_secondaryFieldEffectOpposeMult = 2f;

    [SerializeField, Tooltip("The point at which element field effects (+/-) take effect")]
    private int m_fieldEffectThreshold = 2;

    [SerializeField, Tooltip( "The maximum field effect (+/-) per element" )]
    private int m_fieldEffectMax = 3;


    [Header( "Weakness/Resistance" )]
    [SerializeField] private float m_resistMultiplier = 0.75f;
    [SerializeField] private float m_weaknessMultiplier = 1.5f;

    [Header( "Debug" )]
    [SerializeField] private bool m_enterBattleOnStart = false;
    [SerializeField] private bool m_showTurnNumbers = false;

    [Header("Game State")]
    [SerializeField] private GameObject m_gameOver = null;
    [SerializeField] private GameObject m_win = null;

    [Header( "UI" )]
    [SerializeField] private TextMeshProUGUI m_menuDisplay = null;
    [SerializeField] private TextMeshProUGUI m_outputDisplay = null;
    [SerializeField] private int m_outputMaxLines = 10;
    [SerializeField] private TextMeshProUGUI m_statsDisplay = null;
    [SerializeField] private TextMeshProUGUI m_turnOrderDisplay = null;

    private Actor m_activeActor = null;
    private int m_airEarthSpectrum = 0;
    private ButtonControl m_attackButton = null;
    private KeyControl m_attackKey = null;
    private ButtonControl m_backButton = null;
    private KeyControl m_backKey = null;
    private ButtonControl[] m_castButton = null;
    private KeyControl[] m_castKey = null;
    private PlayerMenuState m_curPlayerMenuState = PlayerMenuState.TopMenu;
    private int m_currentTurn = 0;
    private ButtonControl m_defendButton = null;
    private KeyControl m_defendKey = null;
    private int m_enemyChargePoints = 0;
    private List<Actor> m_enemyList = new List<Actor>();
    private int m_fireWaterSpectrum = 0;
    private bool m_isRunning = false;
    private List<string> m_outputList = new List<string>();
    private int m_playerChargePoints = 0;
    private List<Actor> m_playerList = new List<Actor>();
    private ButtonControl m_spellButton = null;
    private KeyControl m_spellKey = null;
    private float m_timeSinceTurnStart = 0f;
    private List<TurnData> m_turnOrderList = new List<TurnData>();

    public Element FieldElementPrimary {
        get { return FieldElements[0]; }
    }

    public Element FieldElementSecondary {
        get { return FieldElements[1]; }
    }

    public float ResistMultiplier {
        get { return m_resistMultiplier; }
    }

    public int SpeedMax {
        get { return m_speedMax; }
    }

    public float WeaknessMultiplier {
        get { return m_weaknessMultiplier; }
    }

    private PlayerMenuState CurPlayerMenuState {
        set {
            m_curPlayerMenuState = value;

            switch ( m_curPlayerMenuState ) {
                case PlayerMenuState.SpellMenu: ShowControlsSpellMenu(); break;
                case PlayerMenuState.TopMenu: ShowControlsTopMenu(); break;
            }
        }
    }

    private Element[] FieldElements {
        get {
            var fieldElements = new Element[2];
            fieldElements[0] = fieldElements[1] = Element.None;

            if ( Mathf.Abs( m_airEarthSpectrum ) > Mathf.Abs( m_fireWaterSpectrum ) ) {
                if ( m_airEarthSpectrum <= -m_fieldEffectThreshold )
                    fieldElements[0] = Element.Air;
                else if ( m_airEarthSpectrum >= m_fieldEffectThreshold )
                    fieldElements[0] = Element.Earth;
            } else {
                if ( m_fireWaterSpectrum <= -m_fieldEffectThreshold )
                    fieldElements[0] = Element.Fire;
                else if ( m_fireWaterSpectrum >= m_fieldEffectThreshold )
                    fieldElements[0] = Element.Water;
            }

            if ( fieldElements[0] == Element.Fire || fieldElements[0] == Element.Water ) {
                if ( m_airEarthSpectrum <= -m_fieldEffectThreshold )
                    fieldElements[1] = Element.Air;
                else if ( m_airEarthSpectrum >= m_fieldEffectThreshold )
                    fieldElements[1] = Element.Earth;
            } else {
                if ( m_fireWaterSpectrum <= -m_fieldEffectThreshold )
                    fieldElements[1] = Element.Fire;
                else if ( m_fireWaterSpectrum >= m_fieldEffectThreshold )
                    fieldElements[1] = Element.Water;
            }

            return fieldElements;
        }
    }

    public void AddEnemies( List<Actor> a_enemyList ) {
        Output( $"Add enemies: {a_enemyList.ToString( ", " )} " );
        m_enemyList.AddRange( a_enemyList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

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

    public void AddPlayers( List<Actor> a_playerList ) {
        Output( $"Add players: {a_playerList.ToString( ", " )}" );
        m_playerList.AddRange( a_playerList );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void Clear() {
        m_enemyList.Clear();
        m_playerList.Clear();
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

    private void ApplyElement( Element a_element, int a_power = 1 ) {
        switch ( a_element ) {
            case Element.Air:
                if ( m_airEarthSpectrum > 0 ) m_airEarthSpectrum = 0;
                else m_airEarthSpectrum -= a_power;
                break;
            case Element.Earth:
                if ( m_airEarthSpectrum < 0 ) m_airEarthSpectrum = 0;
                else m_airEarthSpectrum += a_power;
                break;
            case Element.Fire:
                if ( m_fireWaterSpectrum > 0 ) m_fireWaterSpectrum = 0;
                else m_fireWaterSpectrum -= a_power;
                break;
            case Element.Water:
                if ( m_fireWaterSpectrum < 0 ) m_fireWaterSpectrum = 0;
                else m_fireWaterSpectrum += a_power;
                break;
        }

        m_airEarthSpectrum = Mathf.Clamp( m_airEarthSpectrum, -m_fieldEffectMax, m_fieldEffectMax );
        m_fireWaterSpectrum = Mathf.Clamp( m_fireWaterSpectrum, -m_fieldEffectMax, m_fieldEffectMax );
    }

    private void Awake() {
        instance = this;
    }

    private bool CanCastSpell( int a_spellIndex ) {
        if ( m_enemyList.Contains( m_activeActor ) )
            return SpellCost( a_spellIndex ) <= m_enemyChargePoints;
        else return SpellCost( a_spellIndex ) <= m_playerChargePoints;
    }

    private Color ElementColor( Element a_element ) {
        switch ( a_element ) {
            case Element.Air: return Color.yellow;
            case Element.Earth: return Color.green;
            case Element.Fire: return Color.red;
            case Element.Water: return Color.cyan;
            default: return Color.white;
        }
    }

    private void Next() {
        RemoveDeadEnemies();

        do {
            m_activeActor = m_turnOrderList[0].actor;
            m_currentTurn = m_turnOrderList[0].turnValue;
            m_turnOrderList.RemoveAt( 0 );
        } while ( m_activeActor.IsDead );

        ReviseTurnOrder();

        if ( m_playerList.Contains( m_activeActor ) ) {
            CurPlayerMenuState = PlayerMenuState.TopMenu;
        }
        m_activeActor.StartTurn();
        m_timeSinceTurnStart = 0f;
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

    private void PlayerSpellMenu() {
        if ( m_backButton.wasPressedThisFrame || m_backKey.wasPressedThisFrame ) {
            CurPlayerMenuState = PlayerMenuState.TopMenu;
            return;
        }

        var spells = m_activeActor.Spells;
        for ( var i = 0; i < spells.Length; ++i ) {
            if ( m_castButton[i].wasPressedThisFrame || m_castKey[i].wasPressedThisFrame ) {
                if ( CanCastSpell( i ) ) {
                    var roll = Random.Range( 0, m_enemyList.Count );
                    var target = m_enemyList[roll];
                    var damage = m_activeActor.CastSpell( i, target );
                    Output( $"{m_activeActor.name} casts {spells[i].name} for {spells[i].Cost} CP ~ {damage} damage" );
                    ApplyElement( m_activeActor.Element, spells[i].ElementPower );

                    if ( m_enemyList.Contains( m_activeActor ) )
                        m_enemyChargePoints -= SpellCost( i );
                    else m_playerChargePoints -= SpellCost( i );
                    Next();
                } else {
                    Output( $"{m_activeActor.name} cannot cast {spells[i].name} (not enough CP)" );
                }

                return;
            }
        }
    }

    private void PlayerTopMenu() {
        if ( m_attackButton.wasPressedThisFrame || m_attackKey.wasPressedThisFrame ) {
            var roll = Random.Range( 0, m_enemyList.Count );
            var target = m_enemyList[roll];
            var damage = m_activeActor.Attack( target );
            if ( damage >= 0 ) {
                m_playerChargePoints += m_chargePointsPerAttack;
                Output( $"{m_activeActor} attacks {target} for {damage} damage" );
            }
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

    private void RemoveDeadEnemies() {
        m_turnOrderList.RemoveAll( data => data.actor.IsDead );
        m_enemyList.RemoveAll( enemy => enemy.IsDead );
    }

    // TODO when reviving player characters, must reconstruct entire turn order
    // (otherwise will only be tacked onto end rather than injected in turn order)
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
                if ( turn % actor.TurnStep == 0 )
                    potentialActorList.Add( actor );
            if ( potentialActorList.Count > 0 ) {
                while ( potentialActorList.Count > 0 ) {
                    var roll = Random.Range( 0, potentialActorList.Count );
                    var actor = potentialActorList[roll];
                    potentialActorList.RemoveAt( roll );

                    if ( actor.IsDead ) continue;
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

    private void ShowControlsSpellMenu() {
        var controlStr = "";
        for ( var i = 0; i < m_activeActor.Spells.Length; ++i ) {
            var spell = m_activeActor.Spells[i];
            var color = CanCastSpell( i ) ? "green" : "red";
            var spellName = spell.GetNameForElement( m_activeActor.Element );
            var cost = SpellCost( i );
            var input = $"{m_castKey[i].displayName} ({m_castButton[i].displayName})";
            controlStr += $"{input} <color={color}>{spellName}: {cost} CP</color>\n";
        }

        m_menuDisplay.text = $"{m_activeActor.name} Spells\n\n" + controlStr
            + $"\n{m_backKey.displayName} ({m_backButton.displayName}) Back";
    }

    private void ShowControlsTopMenu() {
        var attackInputStr = $"{m_attackKey.displayName} ({m_attackButton.displayName}) Attack";
        var defendInputStr = $"{m_defendKey.displayName} ({m_defendButton.displayName}) Defend";
        var spellInputStr = $"{m_spellKey.displayName} ({m_spellButton.displayName}) Spells";

        m_menuDisplay.text = $"{m_activeActor.name}\n\n{attackInputStr}\n{defendInputStr}\n{spellInputStr}";
    }

    private int SpellCost( int a_spellIndex ) {
        var spell = m_activeActor.Spells[a_spellIndex];
        var cost = spell.Cost;

        var element = m_activeActor.Element;
        var primary = FieldElementPrimary;
        if ( primary != Element.None ) {
            if ( element == primary )
                cost = Mathf.FloorToInt( cost * 0.5f );
            else if ( element == OpposingElement[primary] )
                cost = cost * 2;
        }

        var secondary = FieldElementSecondary;
        if ( secondary != Element.None ) {
            if ( element == secondary )
                cost = Mathf.FloorToInt( cost * 0.75f );
            else if ( element == OpposingElement[secondary] )
                cost = Mathf.FloorToInt( cost * 1.5f );
        }

        return cost;
    }

    private void Start() {
        m_win.SetActive( false );
        m_gameOver.SetActive( false );

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

        if( m_enemyList.Count == 0 ) {
            m_win.SetActive( true );
            m_isRunning = false;
            return;
        }

        var gameOver = true;
        foreach( var player in m_playerList ) {
            if( player.IsDead == false ) {
                gameOver = false;
                break;
            }
        }
        if ( gameOver ) {
            m_gameOver.SetActive( true );
            m_isRunning = false;
            return;
        }

        m_enemyChargePoints = Mathf.Clamp( m_enemyChargePoints, 0, m_chargePointsMax );
        m_playerChargePoints = Mathf.Clamp( m_playerChargePoints, 0, m_chargePointsMax );

        UpdateHud();

        m_timeSinceTurnStart += Time.deltaTime;

        if ( m_playerList.Contains( m_activeActor ) )
            UpdateTurnPlayer();
        else UpdateTurnAi();
    }

    private void UpdateHud() {
        if ( m_isRunning == false ) return;

        var airEarthColor = "white";
        if ( m_airEarthSpectrum <= -m_fieldEffectThreshold )
            airEarthColor = ElementColor( Element.Air ).ToHexString();
        else if ( m_airEarthSpectrum >= m_fieldEffectThreshold )
            airEarthColor = ElementColor( Element.Earth ).ToHexString();

        var fireWaterColor = "white";
        if ( m_fireWaterSpectrum <= -m_fieldEffectThreshold )
            fireWaterColor = ElementColor( Element.Fire ).ToHexString();
        else if ( m_fireWaterSpectrum >= m_fieldEffectThreshold )
            fireWaterColor = ElementColor( Element.Water ).ToHexString();

        var statsStr = $"<color={airEarthColor}>Air << {m_airEarthSpectrum} >> Earth</color>\n" +
            $"<color={fireWaterColor}>Fire << {m_fireWaterSpectrum} >> Water</color>\n\n" +
            $"{m_playerChargePoints}/{m_chargePointsMax} CP\n\n";
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
        if ( target.IsDead ) {
            Output( $"{m_activeActor} tries to attack {target} but they're already dead" );
        } else {
            var damage = m_activeActor.Attack( target );
            if ( damage > 0 ) {
                Output( $"{m_activeActor} attacks {target} for {damage} damage" );
                m_enemyChargePoints += m_chargePointsPerAttack;
            }
        }
        Next();
    }

    private void UpdateTurnPlayer() {
        switch ( m_curPlayerMenuState ) {
            case PlayerMenuState.SpellMenu: PlayerSpellMenu(); break;
            case PlayerMenuState.TopMenu: PlayerTopMenu(); break;
        }
    }
}

static public class ListExt
{
    public static string ToHexString( this Color c ) {
        var r = Mathf.FloorToInt( c.r * 255 );
        var g = Mathf.FloorToInt( c.g * 255 );
        var b = Mathf.FloorToInt( c.b * 255 );
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    public static string ToString( this List<MonoBehaviour> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var strList = new List<string>();
        foreach ( var o in a_list )
            strList.Add( o.name );

        return strList.ToString();
    }

    public static string ToString<T>( this List<T> a_list, string a_delimiter, string a_endDelimiter = null ) {
        var listStr = "";
        foreach ( var str in a_list )
            listStr += $"{str}{a_delimiter}";

        listStr = listStr.Substring( 0, listStr.Length - 2 );
        if ( a_endDelimiter != null )
            listStr += a_endDelimiter;
        return listStr;
    }
}

public enum Element
{
    Air,
    Earth,
    Fire,
    Water,
    None
}
