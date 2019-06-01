using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : MonoBehaviour
{
    public Element InnateElement {  get { return m_innateElement; } }
    public bool IsDead {  get { return m_hitPoints <= 0; } }
    public int Speed { get { return BattleManager.instance.SpeedMax / m_speed; } }

    public string Stats {
        get {
            var stats = $"{name} {m_hitPoints}/{m_hitPointsMax}hp";
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

    private int m_hitPoints = 0;
    private bool m_isDefending = false;

    public int Attack( Actor a_target ) {
        return a_target.Damage( m_attack );
    }

    public int CastSpell( int a_index, Actor a_target ) {
        var spell = m_spellList[a_index];
        var damage = spell.Damage;
        if ( m_innateElement == a_target.m_innateElement )
            damage = Mathf.FloorToInt( damage * BattleManager.instance.ResistMultiplier );
        else if ( BattleManager.instance.OpposingElement[m_innateElement] == a_target.m_innateElement )
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
        m_hitPoints = m_hitPointsMax;
    }
}
