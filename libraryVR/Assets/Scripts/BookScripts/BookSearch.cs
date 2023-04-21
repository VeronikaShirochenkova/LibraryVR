using UnityEngine;

namespace BookScripts
{
    public class BookSearch : MonoBehaviour
    {
        public GameObject searchTools;

        void Start()
        {
         searchTools.SetActive(false);
        }

        public void ChangeSearchToolsVisibility()
        {
            searchTools.SetActive(!searchTools.activeSelf);
        }
        
    }
}
