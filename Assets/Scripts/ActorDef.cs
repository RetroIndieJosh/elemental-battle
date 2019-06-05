using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SpellEntry
{
    public Spell spell = null;
    public int level = 0;
}

[CreateAssetMenu(fileName = "New Actor Def", menuName = "Actor Def", order = 1)]
public class ActorDef : ScriptableObject
{
    [Header( "Visual" )]
    [SerializeField] private Sprite m_fieldSprite = null;
    [SerializeField] private Sprite m_portraitSprite = null;

    [Header( "Leveling" )]
    [SerializeField] ActorAdvancementDef m_advancementDef = null;

    [SerializeField, Tooltip("0 to use universal level cap")]
    private int m_levelCap = 100;

    [Header("Spells")]
    [SerializeField] private Element m_element = Element.None;
    [SerializeField] private List<SpellEntry> m_spellList = new List<SpellEntry>();

    public Element Element {  get { return m_element; } }
    public Sprite FieldSprite {  get { return m_fieldSprite; } }
    public Sprite PortraitSprite {  get { return m_portraitSprite; } }

    public ActorStats GetStatsForLevel( int a_level ) {
        a_level = Mathf.Clamp( a_level, 1, m_levelCap );

        var stats = m_advancementDef.GetStatsForLevel( a_level );

        foreach( var spellEntry in m_spellList ) {
            if ( a_level >= spellEntry.level )
                stats.spellList.Add( spellEntry.spell );
        }

        return stats;
    }
}
