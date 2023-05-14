using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace MenuScripts
{
    public class SceneTransition : MonoBehaviour
    {
        public TMP_Text loadingPercentage;
        public Image loadingProgressBar;
        
        private static SceneTransition _instance;
        private static bool _shouldPlayOpeningAnimation = false;
        
        private Animator componentAnimator;
        private AsyncOperation loadingSceneOperation;
        

        void Start()
        {
            _instance = this;

            componentAnimator = GetComponentInChildren<Animator>();

            if (_shouldPlayOpeningAnimation)
            {
                componentAnimator.SetTrigger("sceneOpening");
            }
        }

        private void Update()
        {
            if (loadingSceneOperation != null)
            { 
                loadingPercentage.text = Mathf.RoundToInt(loadingSceneOperation.progress * 100) + "%";
                loadingProgressBar.fillAmount = loadingSceneOperation.progress; 
            }
            
        }

        public static void SwitchToScene(int sceneID)
        {
            _instance.componentAnimator.SetTrigger("sceneClosing");
            _instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneID);
            _instance.loadingSceneOperation.allowSceneActivation = false;
        }

        public void OnAnimationOver()
        {
            _shouldPlayOpeningAnimation = true;
            loadingSceneOperation.allowSceneActivation = true;
        }
        
    }
}
