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

        public static event Action<SceneReference> OnSceneLoadStart;


        public static event Action<SceneReference, float> OnSceneLoading;


        public static event Action<SceneReference> OnSceneLoadDone;

        #endregion


        #region Private

        IEnumerator Load (SceneReference scene, LoadSceneMode mode)
        {
            OnSceneLoadStart?.Invoke (scene);

            AsyncOperation load = SceneManager.LoadSceneAsync (scene.Name, mode);

            while (!load.isDone)
            {
                OnSceneLoading?.Invoke (scene, load.progress);

                yield return null;
            }

            OnSceneLoadDone?.Invoke (scene);
        }

        #endregion


        #region Public

        /// <summary>
        /// Load the given scene with the given scene mode.
        /// </summary>
        public static void LoadScene (SceneReference scene, LoadSceneMode mode)
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

        #endregion
    }

}