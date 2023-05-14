using UnityEngine;

namespace MenuScripts
{
    public class SceneController : MonoBehaviour
    {   
        public void ChangeScene()
        {
            //SceneManager.LoadScene(0);
            SceneTransition.SwitchToScene(0);
        }
    }
}
