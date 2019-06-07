using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class EncounterGroup
{
    public List<Encounter> encounterList = new List<Encounter>();

    public List<Actor> EnemyList {
        get { var list = new List<Actor>();
            foreach ( var encounter in encounterList )
                list.AddRange( encounter.EnemyList );
            return list;
        }
    }
}

[CreateAssetMenu(fileName = "New Encounter Region", menuName = "RPG/Encounter Region", order = 1)]
public class EncounterRegion: ScriptableObject
{
    [SerializeField] List<EncounterGroup> m_enemyPartyList = new List<EncounterGroup>();

    public List<Actor> GenerateEncounter() {
        var roll = Random.Range( 0, m_enemyPartyList.Count );
        return m_enemyPartyList[roll].EnemyList;
    }
}
