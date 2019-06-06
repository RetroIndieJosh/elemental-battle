using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounter", order = 1)]
public class Encounter: ScriptableObject
{
    [SerializeField] private List<EnemyEntry> m_enemyEntryList = new List<EnemyEntry>();

    public List<Actor> EnemyList {
        get {
            var enemyList = new List<Actor>();
            foreach ( var entry in m_enemyEntryList )
                enemyList.AddRange( entry.EnemyList );
            return enemyList;
        }
    }
}

