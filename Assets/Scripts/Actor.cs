using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : MonoBehaviour
{
    public bool IsDead {  get { return m_hitPoints <= 0; } }
    public int Speed { get { return BattleManager.instance.SpeedMax / m_speed; } }

    public string Stats {
        get {
            var stats = $"{name} {m_hitPoints}/{m_hitPointsMax}hp {m_chargePoints}cp";
            if ( m_isDefending ) stats += " (defending)";
            return stats;
        }
    }

    public Spell[] Spells {  get { return m_spellList.ToArray(); } }

    [SerializeField] private int m_hitPointsMax = 10;
    [SerializeField] private int m_attack = 1;
    [SerializeField] private int m_speed = 10;
    [SerializeField] private Element m_innateElement = Element.None;

    [SerializeField] private List<Spell> m_spellList = new List<Spell>();

    [Header( "Debug" )]
    [SerializeField] private int m_startChargePoints = 0;

    private int m_hitPoints = 0;
    private bool m_isDefending = false;
    private int m_chargePoints = 0;

    public int Attack( Actor a_target ) {
        m_chargePoints += 1;
        return a_target.Damage( m_attack );
    }

    public bool CanCastSpell(int a_index ) {
        return m_chargePoints >= m_spellList[a_index].Cost;
    }

    // returns -1 if cannot cast
    public int CastSpell( int a_index, Actor a_target ) {
        if( CanCastSpell(a_index) == false )
            return -1;

        var spell = m_spellList[a_index];
        var damage = spell.Damage;
        if ( spell.Element == a_target.m_innateElement )
            damage = Mathf.FloorToInt( damage * BattleManager.instance.ResistMultiplier );
        else if ( BattleManager.instance.OpposingElement[spell.Element] == a_target.m_innateElement )
            damage = Mathf.FloorToInt( damage * BattleManager.instance.WeaknessMultiplier );

        return a_target.Damage( damage );
    }

    public void Defend() {
        m_isDefending = true;
    }

    public void StartTurn() {
        m_isDefending = false;
    }

    private int Damage( int a_damage ) {
        if ( m_isDefending ) a_damage /= 2;
        a_damage = Mathf.Max( a_damage, 1 );
        m_hitPoints -= a_damage;
        return a_damage;
    }

    private void Start() {
        m_chargePoints = m_startChargePoints;
        m_hitPoints = m_hitPointsMax;
    }
}
