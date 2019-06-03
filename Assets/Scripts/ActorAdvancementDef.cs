using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdvancementMode
{
    Add,
    Mult
}

[System.Serializable]
public class Advancement
{
    public AdvancementMode valueAdvancementMode = AdvancementMode.Add;
    public AdvancementMode modAdvancementMode = AdvancementMode.Add;

    public float valueBase = 1f;
    public float modBase = 1f;
    public float modMod = 1f;

    public int GetValueForLevel(int a_level ) {
        var val = valueBase;
        for( var level = 1; level < a_level; ++level ) {
            var mod = modBase;
            if ( modAdvancementMode == AdvancementMode.Add )
                mod += modMod;
            else mod *= modMod;

            if ( valueAdvancementMode == AdvancementMode.Add )
                val += mod;
            else val *= mod;
        }

        return Mathf.FloorToInt( val );
    }
}

[CreateAssetMenu(fileName = "New Actor Advancement Def", menuName = "Actor Advancement Def", order = 1)]
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
