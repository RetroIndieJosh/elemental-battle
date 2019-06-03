using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor( typeof( ActorAdvancementDef ) )]
public class ActorAdvancementDefEditor : Editor
{
    static public void ShowAdvancementTable( ActorAdvancementDef a_advancement, int a_levelCap = 5 ) {
        if ( a_advancement == null ) return;

        GUILayout.Label( "Advancement", EditorStyles.boldLabel );
        GUILayout.BeginHorizontal();
        GUILayout.Label( "Level" );
        GUILayout.Label( "HP" );
        GUILayout.Label( "Atk" );
        GUILayout.Label( "Spd" );
        GUILayout.EndHorizontal();
        for( var level = 1; level <= a_levelCap; ++level ) {
            var attack = a_advancement.GetAttackForLevel( level );
            var hitPoints = a_advancement.GetHitPointsForLevel( level );
            var speed = a_advancement.GetSpeedForLevel( level );

            GUILayout.BeginHorizontal();
            GUILayout.Label( $"{level}" );
            GUILayout.Label( $"{hitPoints}" );
            GUILayout.Label( $"{attack}" );
            GUILayout.Label( $"{speed}" );
            GUILayout.EndHorizontal();
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        DrawDefaultInspector();

        var actorAdvancementDef = target as ActorAdvancementDef;
        ShowAdvancementTable( actorAdvancementDef );

        serializedObject.ApplyModifiedProperties();
    }
}

#endif // UNITY_EDITOR
