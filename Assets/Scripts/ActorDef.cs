using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SpellLevel
{
    public Spell spell = null;
    public int level = 0;
}

[CreateAssetMenu(fileName = "New Actor Def", menuName = "Actor Def", order = 1)]
public class ActorDef : ScriptableObject
{
    [Header( "Base Attributes" )]
    [SerializeField] private Element m_element = Element.None;
    [SerializeField] private ActorAttributes m_baseAttributes = new ActorAttributes();

    [Header( "Visual" )]
    [SerializeField] private Sprite m_fieldSprite = null;
    [SerializeField] private Sprite m_portraitSprite = null;

    [Header("Leveling")]
    [SerializeField] private float m_strengthIncPerLevel = 1f;
    [SerializeField] private float m_hitPointsIncPerLevel = 1f;
    [SerializeField] private float m_speedIncPerLevel = 1f;

    [SerializeField, Tooltip("0 to use universal level cap")]
    private int m_levelCap = 100;

    [Header("Spells")]
    [SerializeField] private List<SpellLevel> m_spellList = new List<SpellLevel>();

    [Header("Level Tweaking")]
    [SerializeField] private List<ActorAttributes> m_attributeLevelList = new List<ActorAttributes>();

    public Element Element {  get { return m_element; } }
    public Sprite FieldSprite {  get { return m_fieldSprite; } }
    public Sprite PortraitSprite {  get { return m_portraitSprite; } }

    public void GenerateAttributeLevelList() {
        m_attributeLevelList.Clear();
        for ( var i = 1; i <= m_levelCap; ++i )
            m_attributeLevelList.Add( CreateAtLevel( i ) );
    }

    public ActorAttributes GetAttributesForLevel( int a_level ) {
        if ( m_attributeLevelList.Count != m_levelCap )
            GenerateAttributeLevelList();

        a_level = Mathf.Clamp( a_level, 1, m_levelCap );
        return m_attributeLevelList[a_level - 1];
    }

    private ActorAttributes CreateAtLevel(int a_level ) {
        if ( a_level > m_levelCap ) return null;

        var attributes = new ActorAttributes( m_baseAttributes );

        attributes.AddHitPoints( a_level * m_hitPointsIncPerLevel );
        attributes.AddSpeed( a_level * m_speedIncPerLevel );
        attributes.AddStrength( a_level * m_strengthIncPerLevel );
        foreach( var spellLevel in m_spellList ) {
            if ( spellLevel.level <= a_level )
                attributes.spellList.Add( spellLevel.spell );
        }
        attributes.Level = a_level;

        return attributes;
    }
}
