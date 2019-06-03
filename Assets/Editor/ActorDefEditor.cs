#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( ActorDef ) )]
[CanEditMultipleObjects]
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

        GUILayout.Label( "WARNING: Overwrites all Level Tweaking!", EditorStyles.boldLabel );
        if( GUILayout.Button("Generate Attributes" ) ) {
            foreach( ActorDef actorDef in targets )
                actorDef.GenerateAttributeLevelList();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif // UNITY_EDITOR

