using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem.Editor
{

    [CustomEditor (typeof (SceneReference))]
    public class SceneReferenceInspector : UnityEditor.Editor
    {
        private SerializedProperty scene;
        private SerializedProperty mode;
        private SerializedProperty addToHistory;
        private SerializedProperty clearHistoryOnLoad;


        void OnEnable ()
        {
            scene = serializedObject.FindProperty ("scene");
            mode = serializedObject.FindProperty ("mode");
            addToHistory = serializedObject.FindProperty ("addToHistory");
            clearHistoryOnLoad = serializedObject.FindProperty ("clearHistoryOnLoad");
        }


        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (scene);
            EditorGUILayout.PropertyField (mode);

            if ((LoadSceneMode) mode.enumValueIndex == LoadSceneMode.Single)
            {
                EditorGUILayout.PropertyField (addToHistory);
                EditorGUILayout.PropertyField (clearHistoryOnLoad);
            }
            
            serializedObject.ApplyModifiedProperties ();

            // Gui to execute scene loading inside the editor.
            GUI.enabled = Application.isPlaying;
            {
                EditorGUILayout.Space (50);

                EditorGUILayout.BeginHorizontal ();

                if (GUILayout.Button ("Load"))
                {
                    SceneReference sceneReference = target as SceneReference;

                    if (sceneReference)
                    {
                        sceneReference.Load ();
                    }
                }

                if (GUILayout.Button ("Unload"))
                {
                    SceneReference sceneReference = target as SceneReference;

                    if (sceneReference)
                    {
                        sceneReference.Unload ();
                    }
                }

                EditorGUILayout.EndHorizontal ();
            }
            GUI.enabled = true;
        }
    }

}