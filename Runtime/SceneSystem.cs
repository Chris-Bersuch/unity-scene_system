using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem
{

    public enum SceneState
    {
        NotLoaded,
        Loaded,
        Loading,
        Unloading
    }


    struct SceneLoadEntry
    {
        public SceneReference Scene;
        public LoadSceneMode Mode;
    }


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


        #region Fields

        private List<string> loadingScenes = new List<string> ();
        private List<string> unloadingScenes = new List<string> ();

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
                SceneState state = GetSceneState (scene);

                if (state == SceneState.Loaded || state == SceneState.Loading)
                {
                    Debug.LogWarning ($"Error while loading the scene {scene}. This scene cannot be loaded multiple times.");

                    yield break;
                }
            }

            loadingScenes.Add (scene);

            Debug.Log ($"Start loading scene: {scene} with mode {mode}");

            OnSceneLoadStart?.Invoke (scene);
            scene.SceneLoadStart ();

            AsyncOperation load = SceneManager.LoadSceneAsync (scene.Name, mode);

            while (load != null && !load.isDone)
            {
                OnSceneLoading?.Invoke (scene, load.progress);
                scene.SceneLoading (load.progress);

                yield return null;
            }

            loadingScenes.Remove (scene);

            Debug.Log ($"Scene {scene} loaded with mode {mode}");

            OnSceneLoadDone?.Invoke (scene);
            scene.SceneLoadDone ();
        }


        IEnumerator Unload (SceneReference scene)
        {
            SceneState state = GetSceneState (scene);

            if (state != SceneState.Loaded)
            {
                Debug.LogWarning ($"Error while unloading the scene {scene}. The given scene is not loaded!");

                yield break;
            }

            unloadingScenes.Add (scene);

            Debug.Log ($"Start unloading scene: {scene}");

            OnSceneUnloadStart?.Invoke (scene);
            scene.SceneUnloadStart ();

            AsyncOperation unload = SceneManager.UnloadSceneAsync (scene.Name);

            while (unload != null && !unload.isDone)
            {
                OnSceneUnloading?.Invoke (scene, unload.progress);
                scene.SceneUnloading (unload.progress);

                yield return null;
            }

            unloadingScenes.Remove (scene);

            Debug.Log ($"Scene {scene} unloaded");

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

            // instance.loadingQueue.Enqueue (new SceneLoadEntry
            // {
            //     Scene = scene,
            //     Mode = mode
            // });
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

            // instance.unloadingQueue.Enqueue (scene);
            instance.StartCoroutine (instance.Unload (scene));
        }

        #endregion


        #region Public

        /// <summary>
        /// Returns the current state of the given scene.
        /// </summary>
        public static SceneState GetSceneState (SceneReference scene)
        {
            if (instance.unloadingScenes.Contains (scene))
            {
                return SceneState.Unloading;
            }

            if (instance.loadingScenes.Contains (scene))
            {
                return SceneState.Loading;
            }

            Scene s = SceneManager.GetSceneByName (scene);

            if (s.IsValid ())
            {
                return SceneState.Loaded;
            }

            return SceneState.NotLoaded;
        }

        #endregion
    }

}