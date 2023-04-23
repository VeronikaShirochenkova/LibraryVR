using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScrollTextScripts
{
    public class ScrollTableOfContentManager : MonoBehaviour
    {
        [Header("Tablet")]
        public ScrollTextDisplay tablet;
    
        [Header("Chapter parts")]
        public GameObject buttonPrefab;
        public GameObject buttonParent;

        public GameObject tocCanvas;
    
        private List<string> _tableOfContent;
        private List<GameObject> _buttons;
    

        void Start()
        {
            _buttons = new List<GameObject>();
            _tableOfContent = tablet.GetTableOfContent();
        
            // create button for each chapter
            for (var i = 0; i < _tableOfContent.Count; i++)
            {
                var newButton = Instantiate(buttonPrefab, buttonParent.transform);
                newButton.name = i.ToString();
                newButton.GetComponentInChildren<TMP_Text>().text = _tableOfContent[i];
                newButton.GetComponent<Button>().onClick.AddListener(() => SelectChapter(Convert.ToInt32(newButton.name)));
                _buttons.Add(newButton);
            }
        }

        public void ChangeTocVisibility()
        {
            tocCanvas.SetActive(!tocCanvas.activeSelf);
        }
    

        private void SelectChapter(int index)
        {
            tablet.SetChapter(index, true);
        }
    

    }
}

