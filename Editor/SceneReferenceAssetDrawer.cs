using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace CB.SceneSystem.Editor
{

    [CustomPropertyDrawer (typeof (SceneReferenceAsset))]
    public class SceneReferenceAssetDrawer : PropertyDrawer
    {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneProperty = property.FindPropertyRelative ("sceneAsset");
            SerializedProperty sceneNameProperty = property.FindPropertyRelative ("sceneName");

            if (sceneProperty != null)
            {
                if (sceneProperty.objectReferenceValue != null)
                {
                    SceneAsset sceneAsset = (SceneAsset) sceneProperty.objectReferenceValue;
                    string sceneAssetPath = AssetDatabase.GetAssetPath (sceneAsset);
                    bool inBuildSettings = false;
                    bool isEnabled = false;

                    // Check if the asset exists inside the build setting.
                    // Check if the asset is enabled inside the build settings.
                    foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
                    {
                        if (buildScene.path == sceneAssetPath)
                        {
                            isEnabled = buildScene.enabled;
                            inBuildSettings = true;

                            break;
                        }
                    }

                    EditorGUI.BeginProperty (position, GUIContent.none, property);
                    position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);

                    if (!inBuildSettings)
                    {
                        DrawInfoButton (MessageType.Error, "The scene is not in the build settings. Click to add", ref position, () => { AddSceneToBuildSettings (sceneAssetPath); });
                    }
                    else if (!isEnabled)
                    {
                        DrawInfoButton (MessageType.Warning, "The scene is not enabled in the build settings. Click to enable", ref position, () => { EnableSceneInBuildSettings (sceneAssetPath); });
                    }

                    sceneProperty.objectReferenceValue = EditorGUI.ObjectField (position, sceneProperty.objectReferenceValue, typeof (SceneAsset), false);
                    sceneNameProperty.stringValue = ((SceneAsset) sceneProperty.objectReferenceValue).name;
                }
                else
                {
                    EditorGUI.BeginProperty (position, GUIContent.none, property);
                    position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
                    sceneProperty.objectReferenceValue = EditorGUI.ObjectField (position, sceneProperty.objectReferenceValue, typeof (SceneAsset), false);

                    if (sceneProperty.objectReferenceValue != null)
                    {
                        sceneNameProperty.stringValue = ((SceneAsset) sceneProperty.objectReferenceValue).name;
                    }
                }
            }

            EditorGUI.EndProperty ();
        }


        void DrawInfoButton (MessageType type, string tooltip, ref Rect position, Action onClick)
        {
            position.width -= 50;

            Rect r1 = position;
            r1.width = 40;
            r1.x = position.xMax + 10;
            GUIContent error;

            switch (type)
            {
                case MessageType.Error:
                    error = EditorGUIUtility.IconContent ("console.erroricon@2x");

                    break;
                case MessageType.Warning:
                    error = EditorGUIUtility.IconContent ("console.warnicon@2x");

                    break;
                default:
                    error = EditorGUIUtility.IconContent ("console.warnicon.inactive.sml@2x");

                    break;
            }

            error.tooltip = tooltip;

            if (GUI.Button (r1, error))
            {
                onClick ();
            }
        }


        void EnableSceneInBuildSettings (string scenePath)
        {
            List<EditorBuildSettingsScene> tempScenes = EditorBuildSettings.scenes.ToList ();

            foreach (EditorBuildSettingsScene scene in tempScenes)
            {
                if (scene.path == scenePath)
                {
                    scene.enabled = true;

                    break;
                }
            }

            EditorBuildSettings.scenes = tempScenes.ToArray ();
        }


        void AddSceneToBuildSettings (string scenePath)
        {
            EditorBuildSettingsScene newScene = new EditorBuildSettingsScene (new GUID (AssetDatabase.AssetPathToGUID (scenePath)), true);
            List<EditorBuildSettingsScene> tempScenes = EditorBuildSettings.scenes.ToList ();
            tempScenes.Add (newScene);
            EditorBuildSettings.scenes = tempScenes.ToArray ();
        }
    }

}