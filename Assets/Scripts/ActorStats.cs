using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorStats
{
    public string name = "";
    public List<Spell> spellList = new List<Spell>();

    public int attack = 0;
    public int hitPointsMax = 0;
    public int level = 0;
    public int speed = 0;

    public ActorStats() { }

    public ActorStats(ActorStats a_other ) {
        attack = a_other.attack;
        hitPointsMax = a_other.hitPointsMax;
        speed = a_other.speed;

        foreach ( var spell in a_other.spellList )
            spellList.Add( spell );
    }
}
