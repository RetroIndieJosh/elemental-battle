using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdvancementMode
{
    Add,
    Mult
}


[CreateAssetMenu(fileName = "New Actor Advancement Def", menuName = "RPG/Actor Advancement Def", order = 1)]
public class ActorAdvancementDef : ScriptableObject
{
    [SerializeField] Advancement m_attackAdvancement = new Advancement();
    [SerializeField] Advancement m_hitPointsAdvancement = new Advancement();
    [SerializeField] Advancement m_speedAdvancement = new Advancement();

    public ActorStats GetStatsForLevel( int a_level ) {
        var stats = new ActorStats {
            attack = GetAttackForLevel( a_level ),
            hitPointsMax = GetHitPointsForLevel( a_level ),
            speed = GetSpeedForLevel( a_level )
        };
        stats.level = a_level;

        return stats;
    }

    public int GetAttackForLevel( int a_level ) {
        return m_attackAdvancement.GetValueForLevel( a_level );
    }

    public int GetHitPointsForLevel( int a_level ) {
        return m_hitPointsAdvancement.GetValueForLevel( a_level );
    }

    public int GetSpeedForLevel( int a_level ) {
        return m_speedAdvancement.GetValueForLevel( a_level );
    }
}
