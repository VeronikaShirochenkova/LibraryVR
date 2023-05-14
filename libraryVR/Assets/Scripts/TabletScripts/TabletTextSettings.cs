using System.Collections.Generic;
using System.IO;
using BookScripts;
using TMPro;
using UnityEngine;

namespace TabletScripts
{
    public class TabletTextSettings : MonoBehaviour
    {
        [Header("Book")]
        [SerializeField] private TMP_Text tabletPage;

        [Header("Settings Tools")] 
        public GameObject settingCanvas;
        [SerializeField] private GameObject minButton;
        [SerializeField] private GameObject maxButton;
        
        [HideInInspector] public int currentFontSize;
        [HideInInspector] public List<float> sizes;
        
        private TabletTextDisplay _textDisplay;

        void Start()
        {
            sizes = new List<float>()
                { 0.010f, 0.011f, 0.012f, 0.013f, 0.014f, 0.015f, 0.016f, 0.017f, 0.018f, 0.019f, 0.020f };

            for (var i = 0; i < sizes.Count; i++)
            {
                if (Mathf.Approximately(tabletPage.fontSize, sizes[i]))
                {
                    currentFontSize = i;
                }
            }
            
            // JSON
            string _jsonFilePath = Path.GetDirectoryName(PlayerPrefs.GetString("selectedBook")) + "\\" + Path.GetFileNameWithoutExtension(PlayerPrefs.GetString("selectedBook")) + ".json";

            _textDisplay = transform.GetComponentInParent<TabletTextDisplay>();
            
            if (!File.Exists(_jsonFilePath))
            {
                StartSetUp();
            }
            else
            {
                using StreamReader reader = new StreamReader(_jsonFilePath);
                string json = reader.ReadToEnd();

                UserData userData = JsonUtility.FromJson<UserData>(json);
                if (userData.openedAsReadingTablet)
                {
                    _textDisplay.SetFontSizeFromJson();
                }
                else
                {
                    StartSetUp();
                }
            }
            
        }
        
        private void StartSetUp()
        {
            _textDisplay.CountPagesNumber();
            _textDisplay.SetFontSizeFromJson();
        }

        public void SetButtonsVisibility()
        {
            if (currentFontSize == 0) minButton.SetActive(false);
            if (currentFontSize == sizes.Count - 1) maxButton.SetActive(false);
        }
        
        /**
         * Increases font size of displayed text
         */
        public void IncreaseFont()
        {
            // if font size is increased from the smallest -> show "min" button
            if (currentFontSize == 0)
            {
                minButton.SetActive(true);
            }
        
            // increase font size
            currentFontSize++;
            tabletPage.fontSize = sizes[currentFontSize];

            // if it's the biggest font size -> hide "max" button
            if (currentFontSize == sizes.Count-1)
            {
                maxButton.SetActive(false);
            }
        
            _textDisplay.ShowPageNumber();
        }

        /**
         * Reduces font size of displayed text
         */
        public void ReduceFont()
        {
            // if font size is reduced from the biggest -> show "max" button
            if (currentFontSize == sizes.Count - 1)
            {
                maxButton.SetActive(true);
            }
        
            // reduce font size
            currentFontSize--;
            tabletPage.fontSize = sizes[currentFontSize];

            // if it's the smallest font size -> hide "min" button
            if (currentFontSize == 0)
            {
                minButton.SetActive(false);
            }
        
            _textDisplay.ShowPageNumber();
        }

        public void ChangeWindowVisibility()
        {
            settingCanvas.SetActive(!settingCanvas.activeSelf);
        }
    }
}
