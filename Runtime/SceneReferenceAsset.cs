using UnityEngine;


namespace CB.SceneSystem
{

    [System.Serializable]
    public class SceneReferenceAsset
    {
        /// <summary>
        /// This object will be used to store the scene asset object inside the property drawer.
        /// </summary>
        [SerializeField]
        private Object sceneAsset;

        
        [SerializeField, HideInInspector]
        private string sceneName;


        /// <summary>
        /// The scene name
        /// </summary>
        public string Name => sceneName;


        /// <summary>
        /// Implicit for the scene name.
        /// </summary>
        public static implicit operator string (SceneReferenceAsset scene)
        {
            return scene.Name;
        }
    }

}