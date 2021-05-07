using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem
{

    public class SceneSystem : MonoBehaviour
    {
        #region Singleton

        private static SceneSystem _instance;


        private static SceneSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject ("Scene System");
                    _instance = go.AddComponent<SceneSystem> ();
                    DontDestroyOnLoad (go);
                }

                return _instance;
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Is called on scene load start. 
        /// </summary>
        public static event Action<SceneReference> OnSceneLoadStart;


        /// <summary>
        /// Is sequentially called while scene loading and returns the current progress. 
        /// </summary>
        public static event Action<SceneReference, float> OnSceneLoading;


        /// <summary>
        /// Is called on scene load done. 
        /// </summary>
        public static event Action<SceneReference> OnSceneLoadDone;


        /// <summary>
        /// Is called on scene unload start. 
        /// </summary>
        public static event Action<SceneReference> OnSceneUnloadStart;


        /// <summary>
        /// Is sequentially called while scene unloading and returns the current progress. 
        /// </summary>
        public static event Action<SceneReference, float> OnSceneUnloading;


        /// <summary>
        /// Is called on scene unload done. 
        /// </summary>
        public static event Action<SceneReference> OnSceneUnloadDone;

        #endregion


        #region Private

        IEnumerator Load (SceneReference scene, LoadSceneMode mode)
        {
            // Check duplicate loading only in additive loading mode.
            if (mode == LoadSceneMode.Additive)
            {
                if (!scene.AllowDuplicate)
                {
                    Scene s = SceneManager.GetSceneByName (scene.Name);

                    // Is the scene already loaded?
                    if (s.IsValid ())
                    {
                        Debug.LogError ($"Error while loading the scene {scene}. This scene cannot be loaded multiple times.");

                        yield break;
                    }
                }
            }

            OnSceneLoadStart?.Invoke (scene);
            scene.SceneLoadStart ();

            AsyncOperation load = SceneManager.LoadSceneAsync (scene.Name, mode);

            while (!load.isDone)
            {
                OnSceneLoading?.Invoke (scene, load.progress);
                scene.SceneLoading (load.progress);

                yield return null;
            }

            OnSceneLoadDone?.Invoke (scene);
            scene.SceneLoadDone ();
        }


        IEnumerator Unload (SceneReference scene)
        {
            Scene s = SceneManager.GetSceneByName (scene.Name);

            if (!s.IsValid ())
            {
                Debug.LogError ($"Error while unloading the scene {scene}. The given scene is not loaded!");

                yield break;
            }

            OnSceneUnloadStart?.Invoke (scene);
            scene.SceneUnloadStart ();

            AsyncOperation unload = SceneManager.UnloadSceneAsync (scene.Name);

            while (!unload.isDone)
            {
                OnSceneUnloading?.Invoke (scene, unload.progress);
                scene.SceneUnloading (unload.progress);

                yield return null;
            }

            OnSceneUnloadDone?.Invoke (scene);
            scene.SceneUnloadDone ();
        }

        #endregion


        #region Internal

        /// <summary>
        /// Load the given scene with the given scene mode.
        /// </summary>
        internal static void LoadScene (SceneReference scene, LoadSceneMode mode)
        {
            if (scene == null)
            {
                Debug.LogError ($"Error while loading a scene. The given scene is null!");

                return;
            }

            if (string.IsNullOrEmpty (scene.Name))
            {
                Debug.LogError ($"Error while loading a scene. The given scene is empty!");

                return;
            }

            instance.StartCoroutine (instance.Load (scene, mode));
        }


        internal static void UnloadScene (SceneReference scene)
        {
            if (scene == null)
            {
                Debug.LogError ($"Error while unloading a scene. The given scene is null!");

                return;
            }

            if (string.IsNullOrEmpty (scene.Name))
            {
                Debug.LogError ($"Error while unloading a scene. The given scene is empty!");

                return;
            }

            instance.StartCoroutine (instance.Unload (scene));
        }

        #endregion
    }

}