using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem
{

    [CreateAssetMenu (menuName = "CB/Scene System/Scene")]
    public class SceneReference : ScriptableObject
    {
        #region Fields

        [Tooltip ("The scene name that will be loaded.")]
        [SerializeField]
        private SceneReferenceAsset scene;

        [Tooltip ("The mode to load the scene.")]
        [SerializeField]
        private LoadSceneMode mode;


        /// <summary>
        /// The scene name to load.
        /// </summary>
        public string Name => scene?.Name;

        
        /// <summary>
        /// The current state of this scene reference.
        /// </summary>
        public SceneState State { get; internal set; } = SceneState.NotLoaded;
        
        #endregion


        #region Events

        /// <summary>
        /// Is called on scene load start. 
        /// </summary>
        public event Action OnSceneLoadStart;


        /// <summary>
        /// Is sequentially called while scene loading and returns the current progress. 
        /// </summary>
        public event Action<float> OnSceneLoading;


        /// <summary>
        /// Is called on scene load done. 
        /// </summary>
        public event Action OnSceneLoadDone;


        /// <summary>
        /// Is called on scene unload start. 
        /// </summary>
        public event Action OnSceneUnloadStart;


        /// <summary>
        /// Is sequentially called while scene unloading and returns the current progress. 
        /// </summary>
        public event Action<float> OnSceneUnloading;


        /// <summary>
        /// Is called on scene unload done. 
        /// </summary>
        public event Action OnSceneUnloadDone;

        #endregion


        #region Internal

        internal void SceneLoadStart ()
        {
            OnSceneLoadStart?.Invoke ();
        }


        internal void SceneLoading (float progress)
        {
            OnSceneLoading?.Invoke (progress);
        }


        internal void SceneLoadDone ()
        {
            OnSceneLoadDone?.Invoke ();
        }


        internal void SceneUnloadStart ()
        {
            OnSceneUnloadStart?.Invoke ();
        }


        internal void SceneUnloading (float progress)
        {
            OnSceneUnloading?.Invoke (progress);
        }


        internal void SceneUnloadDone ()
        {
            OnSceneUnloadDone?.Invoke ();
        }

        #endregion


        #region Public

        /// <summary>
        /// Load the scene with the load mode.
        /// </summary>
        public void Load ()
        {
            SceneSystem.LoadScene (this, mode);
        }


        /// <summary>
        /// Unload all scenes and load this scene.
        /// </summary>
        public void Unload ()
        {
            SceneSystem.UnloadScene (this);
        }


        public override string ToString ()
        {
            return Name;
        }


        /// <summary>
        /// Implicit for the scene name.
        /// </summary>
        public static implicit operator string (SceneReference scene)
        {
            return scene.Name;
        }

        #endregion
    }

}