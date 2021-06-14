using System;
using UnityEngine;
using UnityEngine.Events;


namespace CB.SceneSystem
{

    public class LoadPreviousScene : MonoBehaviour
    {
        [SerializeField]
        private bool loadOnStart;


        [Tooltip ("Is called on loading the previous scene")]
        [SerializeField]
        private UnityEvent onLoadPreviousScene;


        /// <summary>
        /// Is called if the previous scene starts loading.
        /// </summary>
        public event Action OnLoadPreviousScene;


        void Start ()
        {
            if (loadOnStart)
            {
                Load ();
            }
        }


        public void Load ()
        {
            // prevent the creation of the SceneSystem in Edit mode. 
            if (!Application.isPlaying)
            {
                return;
            }

            if (SceneSystem.instance.LoadPreviousScene ())
            {
                onLoadPreviousScene?.Invoke ();
                OnLoadPreviousScene?.Invoke ();
            }
        }
    }

}