using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor( typeof( ActorAdvancementDef ) )]
public class ActorAdvancementDefEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();

        DrawDefaultInspector();

        var actorAdvancementDef = target as ActorAdvancementDef;

        GUILayout.Label( "Advancement", EditorStyles.boldLabel );
        GUILayout.BeginHorizontal();
        GUILayout.Label( "Level" );
        GUILayout.Label( "HP" );
        GUILayout.Label( "Atk" );
        GUILayout.Label( "Spd" );
        GUILayout.EndHorizontal();
        for( var level = 1; level <= 5; ++level ) {
            var attack = actorAdvancementDef.GetAttackForLevel( level );
            var hitPoints = actorAdvancementDef.GetHitPointsForLevel( level );
            var speed = actorAdvancementDef.GetSpeedForLevel( level );

            GUILayout.BeginHorizontal();
            GUILayout.Label( $"{level}" );
            GUILayout.Label( $"{hitPoints}" );
            GUILayout.Label( $"{attack}" );
            GUILayout.Label( $"{speed}" );
            GUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif // UNITY_EDITOR
