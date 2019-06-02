using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;
using System.Linq;

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

    // TODO move these to a visual handling component that queries battle manager
    [Header( "Visual" )]
    [SerializeField] private GameObject m_statusPortraitsParent = null;
    [SerializeField] private GameObject m_statusPortraitPrefab = null;
    [SerializeField] private FillBarUI m_chargePointsBar = null;
    [SerializeField] private TextMeshProUGUI m_chargePointsLabel = null;
    [SerializeField] private Image m_activeActorDisplay = null;
    [SerializeField] private GameObject m_turnOrderDisplayParent = null;
    [SerializeField] private Material m_fieldPrimaryMaterial = null;
    [SerializeField] private Material m_fieldSecondaryMaterial = null;
    [SerializeField] List<ActorSprite> m_enemyActorSpriteList = new List<ActorSprite>();
    [SerializeField] List<ActorSprite> m_playerActorSpriteList = new List<ActorSprite>();

    [Header("Charge Points")]
    [SerializeField] private int m_chargePointsPerAttack = 1;
    [SerializeField] private int m_chargePointsMax = 10;

    [Header( "Field Effect on CP Costs" )]
    [SerializeField] private float m_primaryFieldEffectMatchMult = 0.5f;
    [SerializeField] private float m_secondaryFieldEffectMatchMult = 0.75f;
    [SerializeField] private float m_primaryFieldEffectOpposeMult = 2f;
    [SerializeField] private float m_secondaryFieldEffectOpposeMult = 1.5f;

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
        foreach ( var enemy in a_enemyList )
            AddEnemy( enemy, false );
        if ( m_isRunning ) ReviseTurnOrder();
    }

    public void AddEnemy( Actor a_enemy , bool a_reviseTurnOrder = true ) {
        Output( $"Add enemy {a_enemy.name}" );

        var index = m_enemyList.Count;
        m_enemyList.Add( a_enemy );

        if ( index < m_enemyActorSpriteList.Count )
            a_enemy.ActorSprite = m_enemyActorSpriteList[index];
        else Debug.LogWarning( $"[BattleManager] Missing enemy sprite index {index}" );

        if ( a_reviseTurnOrder && m_isRunning ) ReviseTurnOrder();
    }

    public void AddPlayer( Actor a_player, bool a_reviseTurnOrder = true ) {
        Output( $"Add player {a_player.name}" );

        var index = m_playerList.Count;
        m_playerList.Add( a_player );

        if( index < m_playerActorSpriteList.Count )
            a_player.ActorSprite = m_playerActorSpriteList[index];
        else Debug.LogWarning( $"[BattleManager] Missing player sprite index {index}" );

        if ( m_isRunning && a_reviseTurnOrder ) ReviseTurnOrder();
    }

    public void AddPlayers( List<Actor> a_playerList ) {
        foreach ( var player in a_playerList )
            AddPlayer( player, false );
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

        for( var i = 0; i < m_playerList.Count; ++i ) {
            var player = m_playerList[i];

            if ( i >= m_playerActorSpriteList.Count ) break;
            m_playerActorSpriteList[i].Sprite = player.Sprite;

            // TODO make this less hacky
            var statusPortrait = Instantiate( m_statusPortraitPrefab );
            statusPortrait.GetComponentInChildren<Image>().sprite = player.Sprite;
            statusPortrait.GetComponentInChildren<TextMeshProUGUI>().text = player.Stats;
            statusPortrait.transform.SetParent( m_statusPortraitsParent.transform, false );
        }

        for( var i = 0; i < m_enemyList.Count; ++i ) {
            if ( i >= m_enemyActorSpriteList.Count ) break;
            m_enemyActorSpriteList[i].Sprite = m_enemyList[i].Sprite;
        }

        m_isRunning = true;

        ReviseTurnOrder();
        Next();

        Output( $"Start {m_playerList.Count} vs {m_enemyList.Count}, {m_activeActor} first" );
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
        if ( m_isRunning == false ) return;

        RemoveDeadEnemies();

        if( m_activeActor != null )
            m_activeActor.ActorSprite.IsSelected = false;

        do {
            m_activeActor = m_turnOrderList[0].actor;
            m_currentTurn = m_turnOrderList[0].turnValue;
            m_turnOrderList.RemoveAt( 0 );
        } while ( m_activeActor.IsDead );

        m_activeActor.ActorSprite.IsSelected = true;
        m_activeActorDisplay.sprite = m_activeActor.Sprite;

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
        foreach( var player in m_playerList )
            player.ActorSprite.Color = player.IsDead ? Color.gray : Color.white;
        foreach ( var enemy in m_enemyList )
            if ( enemy.IsDead ) enemy.ActorSprite.Color = Color.clear;

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

        // TODO reimplement showing turn numbers for debug
        // TODO optimization: create lookahead number of images then set them
        // TODO show active player larger
        foreach ( Transform child in m_turnOrderDisplayParent.transform )
            Destroy( child.gameObject );
        foreach ( var turnData in m_turnOrderList ) {
            var imageObj = new GameObject();
            var image = imageObj.AddComponent<Image>();
            image.sprite = turnData.actor.ActorSprite.Sprite;
            imageObj.transform.SetParent( m_turnOrderDisplayParent.transform, false );
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
                cost = Mathf.FloorToInt( cost * m_primaryFieldEffectMatchMult );
            else if ( element == OpposingElement[primary] )
                cost = Mathf.FloorToInt( cost * m_primaryFieldEffectOpposeMult );
        }

        var secondary = FieldElementSecondary;
        if ( secondary != Element.None ) {
            if ( element == secondary )
                cost = Mathf.FloorToInt( cost * m_secondaryFieldEffectMatchMult );
            else if ( element == OpposingElement[secondary] )
                cost = Mathf.FloorToInt( cost * m_secondaryFieldEffectOpposeMult );
        }

        return Mathf.Max( 1, cost );
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
            if ( actorList.Count( a => a.name.Contains( "Enemy" ) && a.IsDead == false ) == 0 ) return;
            if ( actorList.Count( a => a.name.Contains( "Player" ) && a.IsDead == false ) == 0 ) return;
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
        m_chargePointsBar.FillPercent = (float)m_playerChargePoints / m_chargePointsMax;
        m_chargePointsLabel.text = $"{m_playerChargePoints}/{m_chargePointsMax} CP";

        UpdateHud();

        m_timeSinceTurnStart += Time.deltaTime;

        if ( m_playerList.Contains( m_activeActor ) )
            UpdateTurnPlayer();
        else UpdateTurnAi();
    }

    private void UpdateHud() {
        if ( m_isRunning == false ) return;

        m_fieldPrimaryMaterial.color = ElementColor( FieldElementPrimary );
        m_fieldSecondaryMaterial.color = ElementColor( FieldElementSecondary );

        var statsStr = "";
        if ( m_airEarthSpectrum < -m_fieldEffectThreshold )
            statsStr += $"Air: {-m_airEarthSpectrum}\n";
        else if( m_airEarthSpectrum > m_fieldEffectThreshold )
            statsStr += $"Earth: {m_airEarthSpectrum}\n";

        if ( m_fireWaterSpectrum < -m_fieldEffectThreshold )
            statsStr += $"Fire: {-m_fireWaterSpectrum}\n";
        else if( m_fireWaterSpectrum > m_fieldEffectThreshold )
            statsStr += $"Water: {m_fireWaterSpectrum}\n";

        // TODO display field effect
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
