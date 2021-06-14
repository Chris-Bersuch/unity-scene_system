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


    public class SceneSystem : MonoBehaviour
    {
        #region Singleton

        private static SceneSystem _instance;


        internal static SceneSystem instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject ("Scene System");
                    _instance = go.AddComponent<SceneSystem> ();
                    SceneManager.sceneLoaded += _instance.OnSceneLoaded;
                    SceneManager.sceneUnloaded += _instance.OnSceneUnloaded;
                    DontDestroyOnLoad (go);
                }

                return _instance;
            }
        }

        #endregion


        #region Fields

        private List<string> loadingScenes = new List<string> ();
        private List<string> unloadingScenes = new List<string> ();
        private SceneReference startScene;
        private List<SceneReference> history = new List<SceneReference> ();

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


        #region Unity

        void OnDestroy ()
        {
            if (this == _instance)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
            }
        }

        #endregion


        #region Private

        void OnSceneUnloaded (Scene scene)
        {
            Debug.Log ($"Unloaded the scene {scene.name}.");
            unloadingScenes.Remove (scene.name);
        }


        void OnSceneLoaded (Scene scene, LoadSceneMode mode)
        {
            Debug.Log ($"Loaded the scene {scene.name} with mode {mode}.");
            loadingScenes.Remove (scene.name);
        }


        IEnumerator LoadAsync (SceneReference scene, LoadSceneMode mode)
        {
            Debug.Log ($"Start loading scene: {scene} with mode {mode}");

            scene.State = SceneState.Loading;
            loadingScenes.Add (scene.Name);

            OnSceneLoadStart?.Invoke (scene);
            scene.SceneLoadStart ();

            AsyncOperation load = SceneManager.LoadSceneAsync (scene.Name, mode);

            while (load != null && !load.isDone)
            {
                OnSceneLoading?.Invoke (scene, load.progress);
                scene.SceneLoading (load.progress);

                yield return null;
            }

            scene.State = SceneState.Loaded;

            OnSceneLoadDone?.Invoke (scene);
            scene.SceneLoadDone ();
        }


        IEnumerator UnloadAsync (SceneReference scene)
        {
            Debug.Log ($"Start unloading scene: {scene}");

            scene.State = SceneState.Unloading;
            unloadingScenes.Add (scene.Name);

            OnSceneUnloadStart?.Invoke (scene);
            scene.SceneUnloadStart ();

            AsyncOperation unload = SceneManager.UnloadSceneAsync (scene.Name);

            while (unload != null && !unload.isDone)
            {
                OnSceneUnloading?.Invoke (scene, unload.progress);
                scene.SceneUnloading (unload.progress);

                yield return null;
            }

            scene.State = SceneState.NotLoaded;

            OnSceneUnloadDone?.Invoke (scene);
            scene.SceneUnloadDone ();
        }


        void AddToHistory (SceneReference scene)
        {
            if (scene.Mode == LoadSceneMode.Single)
            {
                if (scene == startScene)
                {
                    if (history.Count > 0 && history[0] == scene)
                    {
                        // Do not add the start scene twice.
                        return;
                    }
                }

                Debug.Log ($"Scene {scene} was added to the history");
                history.Add (scene);
            }
        }


        void ClearSceneHistory ()
        {
            Debug.Log ("Scene history cleared");
            history.Clear ();

            if (startScene)
            {
                history.Add (startScene);
            }
        }

        #endregion


        #region Public

        /// <summary>
        /// Returns the history of the scene loading.
        /// </summary>
        public SceneReference[] GetSceneHistory ()
        {
            return history.ToArray ();
        }

        #endregion


        #region Internal

        /// <summary>
        /// Load the given scene with the given scene mode.
        /// </summary>
        internal void LoadScene (SceneReference scene, LoadSceneMode mode, bool clearHistory, bool addToHistory)
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

            // Check duplicate loading only in additive loading mode.
            if (mode == LoadSceneMode.Additive)
            {
                SceneState state = GetSceneState (scene);

                if (state == SceneState.Loaded || state == SceneState.Loading)
                {
                    Debug.LogWarning ($"Error while loading the scene {scene}. This scene cannot be loaded multiple times.");

                    return;
                }
            }

            if (clearHistory)
            {
                ClearSceneHistory ();
            }

            if (addToHistory)
            {
                AddToHistory (scene);
            }

            StartCoroutine (LoadAsync (scene, mode));
        }


        /// <summary>
        /// Unloads the given scene.
        /// </summary>
        internal void UnloadScene (SceneReference scene)
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

            SceneState state = GetSceneState (scene);

            if (state != SceneState.Loaded)
            {
                Debug.LogWarning ($"Error while unloading the scene {scene}. The given scene is not loaded!");

                return;
            }

            StartCoroutine (UnloadAsync (scene));
        }


        /// <summary>
        /// Loads the previous scene.
        /// Returns true if any previous scene exists.
        /// </summary>
        internal bool LoadPreviousScene ()
        {
            if (history.Count > 1)
            {
                // Remove the current scene.
                history.RemoveAt (history.Count - 1);

                // Get the reference to the previous scene
                SceneReference previousScene = history[history.Count - 1];
                LoadScene (previousScene, LoadSceneMode.Single, previousScene.ClearHistoryOnLoad, false);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Set the start scene that will always be the first entry of the history
        /// </summary>
        internal void SetStartScene (SceneReference scene)
        {
            // Do nothing if the given scene is already the start scene.
            if (startScene == scene)
            {
                return;
            }

            // Add the given scene to the first entry of the history
            if (history.Count > 0)
            {
                if (history[0] == startScene)
                {
                    // The first entry is the current start scene.
                    // Change the start scene
                    history[0] = scene;
                }
                else
                {
                    // The first entry is not the current start scene.
                    // Add the given scene to the first entry.
                    history.Insert (0, scene);
                }
            }
            else
            {
                // The history is empty so we can add it to the end.
                history.Add (scene);
            }

            startScene = scene;
        }

        #endregion


        #region Public

        /// <summary>
        /// Returns the current state of the given scene.
        /// </summary>
        public static SceneState GetSceneState (SceneReference scene)
        {
            // prevent the creation of the SceneSystem in Edit mode. 
            if (!Application.isPlaying)
            {
                return SceneState.NotLoaded;
            }

            if (instance.unloadingScenes.Contains (scene))
            {
                return SceneState.Unloading;
            }

            if (instance.loadingScenes.Contains (scene))
            {
                return SceneState.Loading;
            }

            Scene s = SceneManager.GetSceneByName (scene);

            if (s.IsValid () && s.isLoaded)
            {
                return SceneState.Loaded;
            }

            return SceneState.NotLoaded;
        }


        /// <summary>
        /// Clears the scene history
        /// </summary>
        public static void ClearHistory ()
        {
            // prevent the creation of the SceneSystem in Edit mode. 
            if (!Application.isPlaying)
            {
                return;
            }

            instance.ClearSceneHistory ();
        }


        /// <summary>
        /// Returns the history of the scene loading.
        /// </summary>
        public static SceneReference[] GetHistory ()
        {
            // prevent the creation of the SceneSystem in Edit mode. 
            if (!Application.isPlaying)
            {
                return new SceneReference[0];
            }

            return instance.GetSceneHistory ();
        }

        #endregion
    }

}