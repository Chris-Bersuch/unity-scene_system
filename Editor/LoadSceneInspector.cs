using UnityEditor;
using UnityEngine;


namespace CB.SceneSystem.Editor
{

    [CustomEditor (typeof (LoadScene))]
    public class LoadSceneInspector : UnityEditor.Editor
    {
        private SerializedProperty scene;
        private SerializedProperty loadOnStart;
        private SerializedProperty onSceneLoadStart;
        private SerializedProperty onSceneLoading;
        private SerializedProperty onSceneLoadDone;
        private SerializedProperty onSceneUnloadStart;
        private SerializedProperty onSceneUnloading;
        private SerializedProperty onSceneUnloadDone;

        private bool foldout;


        void OnEnable ()
        {
            scene = serializedObject.FindProperty ("scene");
            loadOnStart = serializedObject.FindProperty ("loadOnStart");
            onSceneLoadStart = serializedObject.FindProperty ("onSceneLoadStart");
            onSceneLoading = serializedObject.FindProperty ("onSceneLoading");
            onSceneLoadDone = serializedObject.FindProperty ("onSceneLoadDone");
            onSceneUnloadStart = serializedObject.FindProperty ("onSceneUnloadStart");
            onSceneUnloading = serializedObject.FindProperty ("onSceneUnloading");
            onSceneUnloadDone = serializedObject.FindProperty ("onSceneUnloadDone");
        }


        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (scene);
            EditorGUILayout.PropertyField (loadOnStart);

            foldout = EditorGUILayout.Foldout (foldout, "Events", true);

            if (foldout)
            {
                EditorGUILayout.PropertyField (onSceneLoadStart);
                EditorGUILayout.PropertyField (onSceneLoading);
                EditorGUILayout.PropertyField (onSceneLoadDone);
                EditorGUILayout.PropertyField (onSceneUnloadStart);
                EditorGUILayout.PropertyField (onSceneUnloading);
                EditorGUILayout.PropertyField (onSceneUnloadDone);
            }

            serializedObject.ApplyModifiedProperties ();

            // Gui to execute scene loading inside the editor.
            if (Application.isPlaying)
            {
                EditorGUILayout.Space (50);

                EditorGUILayout.BeginHorizontal ();

                if (GUILayout.Button ("Load"))
                {
                    LoadScene loadScene = target as LoadScene;

                    if (loadScene)
                    {
                        loadScene.Load ();
                    }
                }

                if (GUILayout.Button ("Unload"))
                {
                    LoadScene loadScene = target as LoadScene;

                    if (loadScene)
                    {
                        loadScene.Unload ();
                    }
                }

                EditorGUILayout.EndHorizontal ();
            }
        }
    }

}