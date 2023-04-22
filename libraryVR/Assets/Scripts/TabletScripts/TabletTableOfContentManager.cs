using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TabletScripts
{
    public class TabletTableOfContentManager : MonoBehaviour
    {
        
        [Header("Tablet")]
        public TabletTextDisplay tablet;
        
        [Header("Chapter parts")]
        public GameObject buttonPrefab;
        public GameObject buttonParent;

        public GameObject tocCanvas;
        
        [Header("Table of content Tools")]
        public GameObject previousButton;
        public GameObject nextButton;
        
        private List<string> _tableOfContent;
        private List<GameObject> _buttons;

        private int _numOfChapters;
        private int _pageCount;
        private int _currentPage;
        
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

            _currentPage = 0;
            //SetPages();
        }

        public void ChangeTocVisibility()
        {
            tocCanvas.SetActive(!tocCanvas.activeSelf);
        }
        
        /**
         * Show the next part of the chapter buttons in table of content
         */
        public void ShowNextPage()
        {
            if(_currentPage == _pageCount-1) return;
            ChangeVisible();
            _currentPage++;
            ChangeVisible();
        }
        
        /**
         * Show the previous part of the chapter buttons in table of content
         */
        public void ShowPreviousPage()
        {
            if(_currentPage == 0) return;
            ChangeVisible();
            _currentPage--;
            ChangeVisible();
        }
        
        /**
         * Check the count of buttons in table of content;
         * Show buttons for turning pages if there are more that 10 buttons
         */
        private void SetPages()
        {
            _pageCount = _buttons.Count / 10;
            if (_pageCount > 1)
            {
                _pageCount = (_buttons.Count % 10 == 0) ? _pageCount : _pageCount + 1;
            
                previousButton.SetActive(true);
                nextButton.SetActive(true);
            
                for (int i = 10; i < _buttons.Count; i++)
                {
                    _buttons[i].SetActive(false);
                }
            }
        }
        
        /**
         * Call the function that display the selected chapter
         */
        private void SelectChapter(int index)
        {
            tablet.SetChapter(index, true);
        }
        
        /**
         * Change "activeSelf" parameter of the currently shown buttons
         */
        private void ChangeVisible()
        {
            int start = _currentPage * 10;
            int end = (start+10 > _buttons.Count) ? _buttons.Count : start+10;
        
            for (int i = start; i < end; i++)
            {
                _buttons[i].SetActive(!_buttons[i].activeSelf);
            }
        }

    }
}
