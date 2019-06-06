using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "RPG/Spell", order = 1)]
public class Spell : ScriptableObject
{
    [SerializeField] private int m_cost = 1;
    [SerializeField] private int m_elementPower = 1;
    [SerializeField] private int m_damage = 1;

    [Header( "Elemental Names" )]
    [SerializeField] private string m_nameAir = "";
    [SerializeField] private string m_nameEarth = "";
    [SerializeField] private string m_nameFire = "";
    [SerializeField] private string m_nameWater = "";

    public string GetNameForElement(Element a_element ) {
        if ( a_element == Element.None ) return name;

        var elementalName = name;
        switch( a_element ) {
            case Element.Air: elementalName = m_nameAir; break;
            case Element.Earth: elementalName = m_nameEarth; break;
            case Element.Fire: elementalName = m_nameFire; break;
            case Element.Water: elementalName = m_nameWater; break;
        }
        return string.IsNullOrEmpty( elementalName ) ? $"{name} ({a_element})" : elementalName;
    }

    public int Cost {  get { return m_cost; } }
    public int Damage {  get { return m_damage; } }
    public int ElementPower {  get { return m_elementPower; } }
}
