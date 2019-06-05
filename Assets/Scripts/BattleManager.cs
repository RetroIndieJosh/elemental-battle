﻿using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private GameObject m_help = null;

    [Header( "Sound Effects" )]
    [SerializeField] private AudioMixerGroup m_soundMixerGroup = null;
    [SerializeField] private AudioClip m_hitSound = null;
    [SerializeField] private AudioClip m_hurtSound = null;
    [SerializeField] private AudioClip m_castSound = null;
    [SerializeField] private AudioClip m_deathSound = null;
    [SerializeField] private AudioClip m_menuSound = null;
    [SerializeField] private AudioClip m_victorySound = null;
    [SerializeField] private AudioClip m_gameOverSound = null;

    [Header( "Animation" )]
    [SerializeField] private float m_attackMoveTime = 0.4f;
    [SerializeField] private float m_attackStayTime = 0.1f;

    [Header( "Element Sprites" )]
    [SerializeField] private Sprite m_airSprite = null;
    [SerializeField] private Sprite m_earthSprite = null;
    [SerializeField] private Sprite m_fireSprite = null;
    [SerializeField] private Sprite m_waterSprite = null;

    // TODO move these to a visual handling component that queries battle manager
    [Header( "Visual" )]
    [SerializeField] private FloatText m_damageText = null;
    [SerializeField] private GameObject m_fieldEffectDisplayParent = null;
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

    [Header( "Charge Points" )]
    [SerializeField] private int m_chargePointsPerAttack = 1;
    [SerializeField] private int m_chargePointsMax = 10;

    [Header( "Field Effect on CP Costs" )]
    [SerializeField] private float m_primaryFieldEffectMatchMult = 0.5f;
    [SerializeField] private float m_secondaryFieldEffectMatchMult = 0.75f;
    [SerializeField] private float m_primaryFieldEffectOpposeMult = 2f;
    [SerializeField] private float m_secondaryFieldEffectOpposeMult = 1.5f;

    [SerializeField, Tooltip( "The point at which element field effects (+/-) take effect" )]
    private int m_fieldEffectThreshold = 2;

    [SerializeField, Tooltip( "The maximum field effect (+/-) per element" )]
    private int m_fieldEffectMax = 3;

    [Header( "Weakness/Resistance" )]
    [SerializeField] private float m_resistMultiplier = 0.75f;
    [SerializeField] private float m_weaknessMultiplier = 1.5f;

    [Header( "Debug" )]
    [SerializeField] private bool m_showTurnNumbers = false;

    [Header( "Game State" )]
    [SerializeField] private GameObject m_gameOver = null;
    [SerializeField] private GameObject m_win = null;

    [Header( "UI" )]
    [SerializeField] private TextMeshProUGUI m_menuHeaderTextMesh = null;
    [SerializeField] private GameObject m_menuEntryPrefab = null;
    [SerializeField] private GameObject m_menuParent = null;
    [SerializeField] private TextMeshProUGUI m_outputDisplay = null;
    [SerializeField] private int m_outputMaxLines = 10;

    public float AttackMoveTime {  get { return m_attackMoveTime; } }
    public float AttackStayTime {  get { return m_attackStayTime; } }

    public UnityEvent OnFinish = new UnityEvent();

    private Image[] m_playerPortraitImage = null;
    private TextMeshProUGUI[] m_playerPortraitTextMesh = null;

    private Image[] m_menuEntryImage = null;
    private TextMeshProUGUI[] m_menuEntryTextMesh = null;

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

    public void AddEnemy( Actor a_enemy, bool a_reviseTurnOrder = true ) {
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

        if ( index < m_playerActorSpriteList.Count )
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

        foreach ( Transform child in m_statusPortraitsParent.transform )
            Destroy( child.gameObject );

        foreach ( Transform child in m_turnOrderDisplayParent.transform )
            Destroy( child.gameObject );
    }

    public void LoadBattle() {
        m_gameOver.SetActive( false );
        m_win.SetActive( false );

        if ( m_enemyList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no enemies set; ignoring." );
            return;
        }

        if ( m_playerList.Count == 0 ) {
            Debug.LogWarning( "[BattleManager] Tried to start battle but no players set; ignoring." );
            return;
        }

        m_playerPortraitImage = new Image[m_playerList.Count];
        m_playerPortraitTextMesh = new TextMeshProUGUI[m_playerList.Count];

        for ( var i = 0; i < m_playerList.Count; ++i ) {
            var player = m_playerList[i];

            if ( i >= m_playerActorSpriteList.Count ) break;
            m_playerActorSpriteList[i].Sprite = player.FieldSprite;

            // TODO make this less hacky
            var statusPortrait = Instantiate( m_statusPortraitPrefab );
            m_playerPortraitImage[i] = statusPortrait.GetComponentInChildren<Image>();
            m_playerPortraitTextMesh[i] = statusPortrait.GetComponentInChildren<TextMeshProUGUI>();
            statusPortrait.transform.SetParent( m_statusPortraitsParent.transform, false );
        }

        m_menuEntryImage = new Image[5];
        m_menuEntryTextMesh = new TextMeshProUGUI[5];
        for( var i = 0; i < 5; ++i ) {
            var menuEntry = Instantiate( m_menuEntryPrefab );
            m_menuEntryImage[i] = menuEntry.GetComponentInChildren<Image>();
            m_menuEntryTextMesh[i] = menuEntry.GetComponentInChildren<TextMeshProUGUI>();
            menuEntry.transform.SetParent( m_menuParent.transform, false );
        }

        for ( var i = 0; i < m_enemyList.Count; ++i ) {
            if ( i >= m_enemyActorSpriteList.Count ) break;
            m_enemyActorSpriteList[i].Sprite = m_enemyList[i].FieldSprite;
        }

        ReviseTurnOrder();
        Next();

        UpdateHud();
    }

    public void StartBattle() {
        m_isRunning = true;

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

        // clear field effect
        for ( var i = 0; i < m_fieldEffectDisplayImage.Length; ++i )
            m_fieldEffectDisplayImage[i].sprite = null;

        var imageIndex = 0;

        // air
        for ( var i = 0; i < -m_airEarthSpectrum; ++i ) {
            m_fieldEffectDisplayImage[imageIndex].sprite = m_airSprite;
            ++imageIndex;
        }

        // earth
        for ( var i = 0; i < m_airEarthSpectrum; ++i ) {
            m_fieldEffectDisplayImage[imageIndex].sprite = m_earthSprite;
            ++imageIndex;
        }

        // fire
        for ( var i = 0; i < -m_fireWaterSpectrum; ++i ) {
            m_fieldEffectDisplayImage[imageIndex].sprite = m_fireSprite;
            ++imageIndex;
        }

        // water
        for ( var i = 0; i < m_fireWaterSpectrum; ++i ) {
            m_fieldEffectDisplayImage[imageIndex].sprite = m_waterSprite;
            ++imageIndex;
        }

        for ( var i = 0; i < m_fieldEffectDisplayImage.Length; ++i ) {
            if ( m_fieldEffectDisplayImage[i].sprite == null )
                m_fieldEffectDisplayImage[i].color = Color.clear;
            else m_fieldEffectDisplayImage[i].color = Color.white;
        }
    }

    private Image[] m_fieldEffectDisplayImage = null;

    private void Awake() {
        instance = this;

        var fieldEffectDisplayCount = m_fieldEffectMax * 2;
        m_fieldEffectDisplayImage = new Image[fieldEffectDisplayCount];
        for ( var i = 0; i < fieldEffectDisplayCount; ++i ) {
            var go = new GameObject();
            m_fieldEffectDisplayImage[i] = go.AddComponent<Image>();
            m_fieldEffectDisplayImage[i].color = Color.clear;
            go.transform.SetParent( m_fieldEffectDisplayParent.transform, false );
        }
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

    private Actor m_nextActor = null;

    private void PlaySound(AudioClip a_sound ) {
        if ( a_sound == null ) return;
        var source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = m_soundMixerGroup;
        source.clip = a_sound;
        source.Play();

        Destroy( source, a_sound.length );
    }

    private void Next() {
        RemoveDeadEnemies();

        if ( m_activeActor != null )
            m_activeActor.ActorSprite.IsSelected = false;

        do {
            m_nextActor = m_turnOrderList[0].actor;
            m_currentTurn = m_turnOrderList[0].turnValue;
            m_turnOrderList.RemoveAt( 0 );
        } while ( m_nextActor.IsDead );
    }

    private void StartTurn() {
        if ( m_activeActor != null )
            m_activeActor.ActorSprite.Color = Color.white;
        m_activeActor = m_nextActor;
        m_nextActor = null;

        m_activeActor.ActorSprite.IsSelected = true;
        m_activeActorDisplay.sprite = m_activeActor.FieldSprite;

        ReviseTurnOrder();

        if ( m_playerList.Contains( m_activeActor ) )
            CurPlayerMenuState = PlayerMenuState.TopMenu;
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

    private void CastSpell( int a_spellIndex, Actor a_target ) {
        var color = ElementColor( m_activeActor.Element );
        m_activeActor.ActorSprite.Color = color;
        if ( TryAttack( a_target, color, true ) == false ) return;

        ApplyElement( m_activeActor.Element, m_activeActor.Spells[a_spellIndex].ElementPower );
        if ( IsEnemy( m_activeActor ) ) m_enemyChargePoints -= SpellCost( a_spellIndex );
        else m_playerChargePoints -= SpellCost( a_spellIndex );
    }

    private void PlayerSpellMenu() {
        if ( m_backButton.wasPressedThisFrame || m_backKey.wasPressedThisFrame ) {
            CurPlayerMenuState = PlayerMenuState.TopMenu;
            return;
        }

        var spells = m_activeActor.Spells;
        for ( var i = 0; i < spells.Length; ++i ) {
            if ( m_castButton[i].wasPressedThisFrame || m_castKey[i].wasPressedThisFrame ) {
                PlaySound( m_menuSound );
                if ( CanCastSpell( i ) ) {
                    var target = m_enemyList.GetRandomElement();
                    CastSpell( i, target );
                    Next();
                    return;
                } else {
                    Output( $"{m_activeActor.name} cannot cast {spells[i].name} (not enough CP)" );
                }

                return;
            }
        }
    }

    private void PlayerTopMenu() {
        if ( m_attackButton.wasPressedThisFrame || m_attackKey.wasPressedThisFrame ) {
            PlaySound( m_menuSound );
            var target = m_enemyList.GetRandomElement();
            TryAttack( target );
            Next();
        } else if ( m_defendButton.wasPressedThisFrame || m_defendKey.wasPressedThisFrame ) {
            PlaySound( m_menuSound );
            m_activeActor.Defend();
            Output( $"{m_activeActor} is now defending" );
            Next();
        } else if ( m_spellButton.wasPressedThisFrame || m_spellKey.wasPressedThisFrame ) {
            PlaySound( m_menuSound );
            if ( m_activeActor.Spells.Length == 0 ) {
                Output( $"{m_activeActor.name} has no spells" );
                return;
            }

            CurPlayerMenuState = PlayerMenuState.SpellMenu;
        }

        m_help.SetActive( m_backButton.isPressed || m_backKey.isPressed );
    }

    // TODO set player back to white color when revived
    private void RemoveDeadEnemies() {
        foreach ( var player in m_playerList )
            if ( player.IsDead ) player.ActorSprite.Color = Color.gray;
        foreach ( var enemy in m_enemyList )
            enemy.ActorSprite.IsVisible = ( enemy.IsDead == false );

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
        var turnOrderListClamped = m_turnOrderList.Take( m_turnOrderLookAhead );
        foreach ( var turnData in turnOrderListClamped ) {
            var o = new GameObject();
            var image = o.AddComponent<Image>();
            image.sprite = turnData.actor.PortraitSprite;
            o.transform.SetParent( m_turnOrderDisplayParent.transform, false );
        }
    }

    private void ShowControlsSpellMenu() {
        for ( var i = 0; i < m_activeActor.Spells.Length; ++i ) {
            var spell = m_activeActor.Spells[i];
            var color = CanCastSpell( i ) ? "green" : "red";
            var spellName = spell.GetNameForElement( m_activeActor.Element );
            var cost = SpellCost( i );
            var keyboardInput = $"{m_castKey[i].displayName}";
            m_menuEntryTextMesh[i].text = $"{keyboardInput} <color={color}>{spellName}: {cost} CP</color>\n";
            m_menuEntryImage[i].sprite = UiManager.instance.GetSpriteFor( m_castButton[i] );
        }

        m_menuHeaderTextMesh.text = $"{m_activeActor.name} Spells";
        m_menuEntryImage[m_activeActor.Spells.Length].sprite = UiManager.instance.GetSpriteFor( Gamepad.current.leftShoulder );
        m_menuEntryTextMesh[m_activeActor.Spells.Length].text = $"\n{m_backKey.displayName} Back";
    }

    private void ShowControlsTopMenu() {
        m_menuEntryTextMesh[0].text = $"{m_attackKey.displayName} Attack";
        m_menuEntryImage[0].sprite = UiManager.instance.GetSpriteFor( m_attackButton );

        m_menuEntryTextMesh[1].text = $"{m_defendKey.displayName} Defend";
        m_menuEntryImage[1].sprite = UiManager.instance.GetSpriteFor( m_defendButton );

        m_menuEntryTextMesh[2].text = $"{m_spellKey.displayName} Spells";
        m_menuEntryImage[2].sprite = UiManager.instance.GetSpriteFor( m_spellButton );

        m_menuEntryTextMesh[3].text = $"{m_backKey.displayName} Help";
        m_menuEntryImage[3].sprite = UiManager.instance.GetSpriteFor( m_backButton );

        m_menuHeaderTextMesh.text = $"{m_activeActor.name}";
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

    private void UpdatePlayerDisplay() {
        for ( var i = 0; i < m_playerList.Count; ++i ) {
            m_playerPortraitImage[i].sprite = m_playerList[i].PortraitSprite;
            m_playerPortraitTextMesh[i].text = m_playerList[i].Stats;
        }
    }

    private IEnumerator FinishAfter( float a_seconds ) {
        yield return new WaitForSeconds( a_seconds );
        OnFinish.Invoke();
    }

    private void Update() {
        if ( m_activeActorDisplay.sprite == null ) m_activeActorDisplay.color = Color.clear;
        else m_activeActorDisplay.color = Color.white;

        if ( m_isRunning == false ) return;

        if ( m_enemyList.Count == 0 ) {
            PlaySound( m_victorySound );
            m_win.SetActive( true );
            m_isRunning = false;
            StartCoroutine( FinishAfter( 2f ) );
            return;
        }

        var gameOver = true;
        foreach ( var player in m_playerList ) {
            if ( player.IsDead == false ) {
                gameOver = false;
                break;
            }
        }
        if ( gameOver ) {
            PlaySound( m_gameOverSound );
            m_gameOver.SetActive( true );
            m_isRunning = false;
            return;
        }

        // wait for animation before switching actor
        if ( m_activeActor == null ) StartTurn();
        else if ( m_nextActor != null ) {
            if ( m_activeActor.ActorSprite.IsAnimating ) return;

            StartTurn();
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

        UpdatePlayerDisplay();

        m_fieldPrimaryMaterial.color = ElementColor( FieldElementPrimary );
        m_fieldSecondaryMaterial.color = ElementColor( FieldElementSecondary );

        var statsStr = "";
        if ( m_airEarthSpectrum < -m_fieldEffectThreshold )
            statsStr += $"Air: {-m_airEarthSpectrum}\n";
        else if ( m_airEarthSpectrum > m_fieldEffectThreshold )
            statsStr += $"Earth: {m_airEarthSpectrum}\n";

        if ( m_fireWaterSpectrum < -m_fieldEffectThreshold )
            statsStr += $"Fire: {-m_fireWaterSpectrum}\n";
        else if ( m_fireWaterSpectrum > m_fieldEffectThreshold )
            statsStr += $"Water: {m_fireWaterSpectrum}\n";

        // hide unused menu slots
        foreach ( var menuEntry in m_menuEntryImage ) {
            if ( menuEntry.sprite == null )
                menuEntry.color = Color.clear;
            else menuEntry.color = Color.white;
        }
    }

    private void UpdateTurnAi() {
        var target = m_playerList.GetRandomElement();
        TryAttack( target );
        Next();
    }

    private bool IsEnemy( Actor a_actor ) {
        return m_enemyList.Contains( a_actor );
    }

    private bool TryAttack( Actor a_target ) {
        return TryAttack( a_target, Color.black );
    }

    private void PlaySoundDelayed( AudioClip a_sound, float a_delaySec ) {
        StartCoroutine( PlaySoundDelayedCoroutine( a_sound, a_delaySec ) );
    }

    private IEnumerator PlaySoundDelayedCoroutine( AudioClip a_sound, float a_delaySec ) {
        yield return new WaitForSeconds( a_delaySec );
        PlaySound( a_sound );
    }

    private bool TryAttack( Actor a_target, Color a_flashColor, bool a_isSpell = false ) {
        if ( a_target.IsDead ) return false;

        var damage = m_activeActor.TryAttack( a_target );
        if ( damage < 0 ) return false;

        var offset = IsEnemy( a_target ) ? Vector3.left : Vector3.right;
        var targetPos = a_target.ActorSprite.transform.position + offset;
        m_activeActor.ActorSprite.AnimateAttack( targetPos );
        a_target.ActorSprite.Flash( a_flashColor, AttackMoveTime / 2f );

        PlaySoundDelayed( a_isSpell ? m_castSound : m_hitSound, AttackMoveTime / 2f );
        PlaySoundDelayed( a_target.IsDead ? m_deathSound : m_hurtSound, AttackMoveTime / 2f + 0.2f );

        if ( a_isSpell == false ) {
            if ( IsEnemy( m_activeActor ) ) m_enemyChargePoints += m_chargePointsPerAttack;
            else m_playerChargePoints += m_chargePointsPerAttack;
        }

        m_damageText.transform.position = a_target.ActorSprite.transform.position + Vector3.up;
        m_damageText.Text = $"{damage}";
        m_damageText.Show();

        return true;
    }

    private void UpdateTurnPlayer() {
        switch ( m_curPlayerMenuState ) {
            case PlayerMenuState.SpellMenu: PlayerSpellMenu(); break;
            case PlayerMenuState.TopMenu: PlayerTopMenu(); break;
        }
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
