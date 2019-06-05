using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        var mod = modBase;
        for( var level = 1; level < a_level; ++level ) {
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
