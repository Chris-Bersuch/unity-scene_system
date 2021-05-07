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
        private SerializedProperty allowDuplicate;


        void OnEnable ()
        {
            scene = serializedObject.FindProperty ("scene");
            mode = serializedObject.FindProperty ("mode");
            allowDuplicate = serializedObject.FindProperty ("allowDuplicate");
        }


        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.PropertyField (scene);
            EditorGUILayout.PropertyField (mode);

            if ((LoadSceneMode) mode.enumValueIndex == LoadSceneMode.Additive)
            {
                EditorGUILayout.Space ();
                EditorGUILayout.PropertyField (allowDuplicate);
            }

            serializedObject.ApplyModifiedProperties ();

            // Gui to execute scene loading inside the editor.
            if (Application.isPlaying)
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
        }
    }

}