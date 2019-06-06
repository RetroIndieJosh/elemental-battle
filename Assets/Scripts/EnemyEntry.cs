using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EditorGUITable;

[System.Serializable]
public class LevelCount
{
    public int levelMin = 1;
    public int levelMax = 1;

    public int countMin = 1;
    public int countMax = 1;
} 

[System.Serializable]
public class EnemyEntry
{
    [SerializeField] private ActorDef m_enemyDef = null;
    [SerializeField] List<LevelCount> m_levelCount = new List<LevelCount>();

    public List<Actor> EnemyList {
        get {
            var enemyList = new List<Actor>();
            foreach ( var levelCount in m_levelCount ) {
                var count = Random.Range( levelCount.countMin, levelCount.countMax );
                for ( var i = 0; i < count; ++i ) {
                    var obj = new GameObject();
                    var actor = obj.AddComponent<Actor>();
                    var level = Random.Range( levelCount.levelMin, levelCount.levelMax );
                    actor.Set( m_enemyDef, level );
                    enemyList.Add( actor );
                }
            }
            return enemyList;
        }
    }
}

