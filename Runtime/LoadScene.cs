using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem
{

    public class LoadScene : MonoBehaviour
    {
        [SerializeField]
        private SceneReference scene;

        [SerializeField]
        private LoadSceneMode mode;

        [Space]
        [SerializeField]
        private bool loadOnStart;


        void Start ()
        {
            if (loadOnStart)
            {
                Load ();
            }
        }


        /// <summary>
        /// Load the referenced scene.
        /// </summary>
        public void Load ()
        {
            scene.LoadScene (mode);
        }
    }

}