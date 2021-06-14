using UnityEngine;


namespace CB.SceneSystem
{

    public class SetStartScene : MonoBehaviour
    {
        [SerializeField]
        private SceneReference scene;


        void Awake ()
        {
            Set ();
        }


        public void Set ()
        {
            SceneSystem.instance.SetStartScene (scene);
        }
    }

}