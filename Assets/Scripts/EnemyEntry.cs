using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEntry
{
    [SerializeField] private ActorDef m_enemyDef = null;
    [SerializeField] private int m_level = 1;
    [SerializeField] private int m_count = 1;

    public List<Actor> EnemyList {
        get {
            var enemyList = new List<Actor>();
            for ( var i = 0; i < m_count; ++i ) {
                var obj = new GameObject();
                var actor = obj.AddComponent<Actor>();
                actor.Set( m_enemyDef, m_level );
                enemyList.Add( actor );
            }
            return enemyList;
        }
    }
}

