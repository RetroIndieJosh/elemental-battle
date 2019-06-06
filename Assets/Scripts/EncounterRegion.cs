using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter Region", menuName = "RPG/Encounter Region", order = 1)]
public class EncounterRegion: ScriptableObject
{
    [SerializeField] List<Encounter> m_enemyPartyList = new List<Encounter>();

    public List<Actor> GenerateEncounter() {
        var roll = Random.Range( 0, m_enemyPartyList.Count );
        return m_enemyPartyList[roll].EnemyList;
    }
}
