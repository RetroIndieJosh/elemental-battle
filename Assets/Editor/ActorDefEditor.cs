using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( ActorDef ) )]
public class ActorDefEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();

        /*
        EditorGUILayout.LabelField( "State" );
        var stateProp = FindProperty( "m_stateIndex", SerializedPropertyType.Integer );
        stateProp.intValue = EditorGUILayout.Popup( stateProp.intValue, stateComp.StateNameList.ToArray() );
        */

        DrawDefaultInspector();

        var actorDef = target as ActorDef;
        GUILayout.Label( "WARNING: Overwrites all Level Tweaking!", EditorStyles.boldLabel );
        if( GUILayout.Button("Generate Attributes" ) ) {
            actorDef.GenerateAttributeLevelList();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
