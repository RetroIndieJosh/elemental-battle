using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Actor : MonoBehaviour
{
    public int Speed { get { return BattleManager.instance.SpeedMax / m_speed; } }
    public string Stats { get { return $"{name} {m_hp}/{m_hpMax}"; } }

    [SerializeField] private int m_hpMax = 10;
    [SerializeField] private int m_attack = 1;
    [SerializeField] private int m_speed = 10;

    private int m_hp = 0;

    private void Start() {
        m_hp = m_hpMax;
    }
}
