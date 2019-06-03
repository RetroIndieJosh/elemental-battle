#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( ActorDef ) )]
[CanEditMultipleObjects]
public class ActorDefEditor : Editor
{
    public override void OnInspectorGUI() {
        serializedObject.Update();

        DrawDefaultInspector();

        var actorAdvancementDef = serializedObject.FindProperty( "m_advancementDef" ).objectReferenceValue as ActorAdvancementDef;
        var levelCap = serializedObject.FindProperty( "m_levelCap" ).intValue;
        ActorAdvancementDefEditor.ShowAdvancementTable( actorAdvancementDef, levelCap );

        serializedObject.ApplyModifiedProperties();
    }
}

#endif // UNITY_EDITOR

