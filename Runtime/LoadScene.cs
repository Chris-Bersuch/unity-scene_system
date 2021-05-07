using UnityEngine;
using UnityEngine.Events;


namespace CB.SceneSystem
{

    public class LoadScene : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private SceneReference scene;

        [SerializeField]
        private bool loadOnStart;

        #endregion


        #region Events

        [Tooltip ("Is called on scene load start")]
        [SerializeField]
        private UnityEvent onSceneLoadStart;


        [Tooltip ("Is sequentially called while scene loading and returns the current progress")]
        [SerializeField]
        private UnityEvent<float> onSceneLoading;


        [Tooltip ("Is called on scene load done")]
        [SerializeField]
        private UnityEvent onSceneLoadDone;


        [Tooltip ("Is called on scene unload start")]
        [SerializeField]
        private UnityEvent onSceneUnloadStart;


        [Tooltip ("Is sequentially called while scene unloading and returns the current progress")]
        [SerializeField]
        private UnityEvent<float> onSceneUnloading;


        [Tooltip ("Is called on scene unload done")]
        [SerializeField]
        private UnityEvent onSceneUnloadDone;

        #endregion


        #region Unity

        void OnEnable ()
        {
            scene.OnSceneLoadStart += OnSceneLoadStart;
            scene.OnSceneLoading += OnSceneLoading;
            scene.OnSceneLoadDone += OnSceneLoadDone;

            scene.OnSceneUnloadStart += OnSceneUnloadStart;
            scene.OnSceneUnloading += OnSceneUnloading;
            scene.OnSceneUnloadDone += OnSceneUnloadDone;
        }


        void OnDisable ()
        {
            scene.OnSceneLoadStart -= OnSceneLoadStart;
            scene.OnSceneLoading -= OnSceneLoading;
            scene.OnSceneLoadDone -= OnSceneLoadDone;

            scene.OnSceneUnloadStart -= OnSceneUnloadStart;
            scene.OnSceneUnloading -= OnSceneUnloading;
            scene.OnSceneUnloadDone -= OnSceneUnloadDone;
        }


        void Start ()
        {
            if (loadOnStart)
            {
                Load ();
            }
        }

        #endregion


        #region Private

        void OnSceneLoadStart ()
        {
            onSceneLoadStart?.Invoke ();
        }


        void OnSceneLoading (float progress)
        {
            onSceneLoading?.Invoke (progress);
        }


        void OnSceneLoadDone ()
        {
            onSceneLoadDone?.Invoke ();
        }


        void OnSceneUnloadStart ()
        {
            onSceneUnloadStart?.Invoke ();
        }


        void OnSceneUnloading (float progress)
        {
            onSceneUnloading?.Invoke (progress);
        }


        void OnSceneUnloadDone ()
        {
            onSceneUnloadDone?.Invoke ();
        }

        #endregion


        #region Public

        /// <summary>
        /// Load the referenced scene.
        /// </summary>
        public void Load ()
        {
            scene.Load ();
        }


        /// <summary>
        /// Unload the referenced scene.
        /// </summary>
        public void Unload ()
        {
            scene.Unload ();
        }

        #endregion
    }

}