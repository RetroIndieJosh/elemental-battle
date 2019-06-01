using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell", order = 1)]
public class Spell : ScriptableObject
{
    [SerializeField] private int m_cost = 1;
    [SerializeField] private Element m_element = Element.None;
    [SerializeField] private int m_damage = 1;

    public int Cost {  get { return m_cost; } }
    public int Damage {  get { return m_damage; } }
    public Element Element {  get { return m_element; } }
}
