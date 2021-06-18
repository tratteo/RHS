using UnityEditor;

//[CustomEditor(typeof(Level), true)]
//public class Level_CE : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        var picker = target as Level;
//        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.Scene);
//        serializedObject.Update();
//        EditorGUI.BeginChangeCheck();
//        var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

//        if (EditorGUI.EndChangeCheck())
//        {
//            var newPath = AssetDatabase.GetAssetPath(newScene);
//            var scenePathProperty = serializedObject.FindProperty("scenePath");
//            scenePathProperty.stringValue = newPath;
//        }
//        serializedObject.ApplyModifiedProperties();
//    }
//}