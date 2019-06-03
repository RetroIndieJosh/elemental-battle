using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorAttributes
{
    public string name = "";
    public List<Spell> spellList = new List<Spell>();

    public int Strength {  get { return Mathf.FloorToInt( m_strength ); } }
    public int HitPointsMax { get { return Mathf.FloorToInt( m_hitPointsMax ); } }
    public int Speed {  get { return Mathf.FloorToInt( m_speed ); } }

    public int Level {
        get { return m_level; }
        set {
            m_level = value;
            name = $"Level {m_level}";
        }
    }

    private int m_level = 0;
    
    [SerializeField] private float m_hitPointsMax = 0f;
    [SerializeField] private float m_speed = 0f;
    [SerializeField] private float m_strength = 0f;

    public ActorAttributes() { }

    public ActorAttributes(ActorAttributes a_other ) {
        m_hitPointsMax = a_other.m_hitPointsMax;
        m_speed = a_other.m_speed;
        m_strength = a_other.m_strength;

        foreach ( var spell in a_other.spellList )
            spellList.Add( spell );
    }

    public void AddHitPoints(float a_hitPoints ) {
        m_hitPointsMax += a_hitPoints;
    }

    public void AddStrength(float a_strength ) {
        m_strength += a_strength;
    }

    public void AddSpeed(float a_speed ) {
        m_speed += a_speed;
    }
}
