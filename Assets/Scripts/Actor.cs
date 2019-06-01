using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : MonoBehaviour
{
    public bool IsDead {  get { return m_hp <= 0; } }
    public int Speed { get { return BattleManager.instance.SpeedMax / m_speed; } }
    public string Stats {
        get {
            var stats = $"{name} {m_hp}/{m_hpMax}";
            if ( m_isDefending ) stats += " (defending)";
            return stats;
        }
    }

    [SerializeField] private int m_hpMax = 10;
    [SerializeField] private int m_attack = 1;
    [SerializeField] private int m_speed = 10;

    private int m_hp = 0;
    private bool m_isDefending = false;

    public int Attack( Actor a_target ) {
        return a_target.Damage( m_attack );
    }

    public void Defend() {
        m_isDefending = true;
    }

    public void StartTurn() {
        m_isDefending = false;
    }

    private int Damage( int a_damage ) {
        if ( m_isDefending ) a_damage /= 2;
        m_hp -= a_damage;
        return a_damage;
    }

    private void Start() {
        m_hp = m_hpMax;
    }
}
