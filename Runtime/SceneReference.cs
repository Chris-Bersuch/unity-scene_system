using UnityEngine;
using UnityEngine.SceneManagement;


namespace CB.SceneSystem
{

    [CreateAssetMenu (menuName = "CB/Scene System/Scene")]
    public class SceneReference : ScriptableObject
    {
        [Tooltip ("The scene name that will be loaded.")]
        [SerializeField]
        private SceneReferenceAsset scene;


        /// <summary>
        /// The scene name to load.
        /// </summary>
        public string Name
        {
            get
            {
                if (scene != null)
                {
                    return scene.Name;
                }

                return "Not_Set";
            }
        }


        /// <summary>
        /// Load this scene the given scene mode.
        /// </summary>
        public void LoadScene (LoadSceneMode mode)
        {
            SceneSystem.LoadScene (this, mode);
        }


        /// <summary>
        /// Unload all scenes and load this scene.
        /// </summary>
        public void LoadSceneSingle ()
        {
            LoadScene (LoadSceneMode.Single);
        }


        /// <summary>
        /// Load this scene additive to all existing ones.
        /// </summary>
        public void LoadSceneAdditive ()
        {
            LoadScene (LoadSceneMode.Additive);
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
    }

}