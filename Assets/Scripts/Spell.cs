using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell", order = 1)]
public class Spell : ScriptableObject
{
    [SerializeField] private int m_cost = 1;
    [SerializeField] private Element m_element = Element.None;

    public Element Element {  get { return m_element; } }
    public int Cost {  get { return m_cost; } }
}
