using UnityEditor;
using UnityEngine;


namespace CB.SceneSystem.Editor
{

    [CustomEditor (typeof (SceneSystem))]
    public class SceneSystemInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            SceneSystem system = target as SceneSystem;

            if (Application.isPlaying)
            {
                GUILayout.Label ("History");

                if (system)
                {
                    foreach (SceneReference scene in system.GetSceneHistory ())
                    {
                        GUILayout.BeginHorizontal (GUI.skin.box);
                        GUILayout.Label (scene.Name);
                        GUILayout.EndHorizontal ();
                    }
                }
            }
        }
    }

}