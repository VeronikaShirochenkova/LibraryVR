using UnityEngine;

namespace MenuScripts
{
    public class SceneController : MonoBehaviour
    {

        public void ChangeScene()
        {
            SceneTransition.SwitchToScene(0);
        }
        
        public void ExitGame() {
            Application.Quit();
        }
    }
}
