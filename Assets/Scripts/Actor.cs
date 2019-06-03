using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : MonoBehaviour
{
    [SerializeField] private ActorDef m_actorDef = null;
    [SerializeField] private int m_startLevel = 1;

    public Element Element {  get { return m_actorDef.Element; } }
    public bool IsDead {  get { return m_hitPoints <= 0; } }
    public Sprite FieldSprite {  get { return m_actorDef.FieldSprite; } }
    public Sprite PortraitSprite {  get { return m_actorDef.PortraitSprite; } }
    public int TurnStep { get { return BattleManager.instance.SpeedMax / Speed; } }

    public string Stats {
        get {
            var healthPercent = (float)m_hitPoints / HitPointsMax;
            var healthColor = Color.white;
            if ( m_hitPoints == HitPointsMax ) healthColor = Color.green;
            else if ( healthPercent < 0.2f ) healthColor = Color.red;
            else if ( healthPercent < 0.4f ) healthColor = Color.yellow;

            var healthColorStr = healthColor.ToHexString();
            var stats = $"{name} ({Element.ToString()[0]})\n" +
                $"<color={healthColorStr}>{m_hitPoints}</color>/{HitPointsMax} HP";

            if ( m_isDefending ) stats += " (defending)";
            return stats;
        }
    }

    public Spell[] Spells {  get { return SpellList.ToArray(); } }

    [HideInInspector] public ActorSprite ActorSprite = null;

    private int Strength { get { return m_attributes.Strength; } }
    private int HitPointsMax { get { return m_attributes.HitPointsMax; } }
    private int Speed { get { return m_attributes.Speed; } }
    private List<Spell> SpellList { get { return m_attributes.spellList; } }

    private ActorAttributes m_attributes = null;
    private int m_hitPoints = 0;
    private bool m_isDefending = false;

    public int TryAttack( Actor a_target ) {
        return a_target.Damage( Strength );
    }

    public int CastSpell( int a_index, Actor a_target ) {
        var spell = SpellList[a_index];
        var damage = spell.Damage;
        if ( Element == a_target.Element )
            damage = Mathf.FloorToInt( damage * BattleManager.instance.ResistMultiplier );
        else if ( BattleManager.instance.OpposingElement[Element] == a_target.Element )
            damage = Mathf.FloorToInt( damage * BattleManager.instance.WeaknessMultiplier );

        return a_target.Damage( damage );
    }

    public void Defend() {
        m_isDefending = true;
        ActorSprite.transform.localScale = new Vector3( -1f, 1f );
    }

    public void Set(ActorDef a_def, int a_level, bool a_resetHitPoints = true ) {
        m_actorDef = a_def;
        m_attributes = m_actorDef.GetAttributesForLevel( a_level );
        if ( a_resetHitPoints ) m_hitPoints = HitPointsMax;
    }

    public void StartTurn() {
        m_isDefending = false;
        ActorSprite.transform.localScale = Vector3.one;
    }

    private int Damage( int a_damage ) {
        if ( m_isDefending ) a_damage /= 2;
        a_damage = Mathf.Max( a_damage, 1 );
        m_hitPoints -= a_damage;
        if ( m_hitPoints < 0 ) m_hitPoints = 0;
        return a_damage;
    }

    private void Start() {
        if( m_actorDef == null ) {
            Debug.LogError( $"[Actor] Failed to create {name}; missing ActorDef. Destroying." );
            Destroy( this );
            return;
        }

        Set( m_actorDef, m_startLevel );
    }
}
