using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace BookScripts
{
    public class TextDisplay : MonoBehaviour
    {
        public SceneController sc;
        
        // Pages
        [SerializeField] private TMP_Text leftAlphaPage;
        [SerializeField] private TMP_Text leftDisplayedPage;
        
        [SerializeField] private TMP_Text rightAlphaPage;
        [SerializeField] private TMP_Text rightDisplayedPage;
    
        // Notes
        [SerializeField] private GameObject noteButton;
        [SerializeField] private GameObject noteText;
        [SerializeField] private GameObject keyboard;

        public List<(int, int)> words;

        private string tagStart;
        private string tagEnd;
    
        // JSON
        private string _jsonFilePath;
        private UserData _userData;
    
        // Book
        private string _filePath;
        private List<Chapter> _stringChapters;
    
        private int _pageCount;
        private int _currentPage;
        private int _currentChapter;


        void Start()
        {
            
            _filePath = PlayerPrefs.GetString("selectedBook");

            // _filePath = Application.dataPath + "/Resources/Books/Fahrenheit451/Fahrenheit451_EN.epub";       // ok
            // _filePath = Application.dataPath + "/Resources/Books/BraveNewWorld/BraveNewWorld_EN.epub";       // ok
            //_filePath = Application.dataPath + "/Resources/Books/NineteenEightyFour/1984_EN.epub";             // ok


            EPubBookReader reader = new EPubBookReader(_filePath);
            _stringChapters = reader.GetAllChapters();
            
            // Notes
            words = new List<(int, int)>();
            tagStart = "<color=red>";
            tagEnd = "</color>";

            // JSON
            _jsonFilePath = Path.GetDirectoryName(_filePath) + "\\" + Path.GetFileNameWithoutExtension(_filePath) + ".json";
        
            if (File.Exists(_jsonFilePath))
            {
                Debug.Log("Load data from JSON");
                LoadUserData();
            }
            else
            {
                Debug.Log("Create new user data");
                _userData = new UserData(_stringChapters.Count);
            }

            // set text
            _currentChapter = 0;
            SetChapter(_currentChapter, false);
            
            
            keyboard.SetActive(false);
            noteText.SetActive(false);
        }
        
        
        //=================== PAGES ========================
        
        /**
         * Set the chapter by given index; Get two parameters:
         *  1) index of chapter
         *  2) bool var "byContent": "true" if chapter opens by table of content
         */
        public void SetChapter(int index, bool byContent)
        {
            if (index < 0) return;
            if (index >= _stringChapters.Count) return;
            
            leftAlphaPage.text = _stringChapters[index].text;
            leftDisplayedPage.text = _stringChapters[index].text;
            
            rightAlphaPage.text = _stringChapters[index].text;
            rightDisplayedPage.text = _stringChapters[index].text;
        
            // Need it because in the first frame it's null 
            ForceUpdate();
            
            if (index >= _currentChapter || byContent)
            {
                _currentChapter = index;
                SetStartPages();
            }
            else if(index < _currentChapter)
            {
                _currentChapter = index;
                SetPrevPages();
            }
            
            ShowAllNotesOnPage();
        }
        
        /**
         * show previous page
         */
        public void PreviousPage()
        {
            if (_currentPage >= 3)
            {
                leftAlphaPage.pageToDisplay = _currentPage - 2;
                leftDisplayedPage.pageToDisplay = _currentPage - 2;
                
                rightAlphaPage.pageToDisplay = _currentPage - 1;
                rightDisplayedPage.pageToDisplay = _currentPage - 1;
                
                _currentPage -= 2;
            }
            else
            {
                GetPreviousChapter();
            }
        }
    
        /**
         * show next page
         */
        public void NextPage()
        {
            int pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;
            if (_currentPage <= pageCount - 3)
            {
                leftAlphaPage.pageToDisplay = _currentPage + 2;
                leftDisplayedPage.pageToDisplay = _currentPage + 2;
                
                rightAlphaPage.pageToDisplay = _currentPage + 3;
                rightDisplayedPage.pageToDisplay = _currentPage + 3;
                
                _currentPage += 2;
            }
            else
            {
                GetNextChapter();
            }
        }    
        
        /**
         * Reset current page number/page count;
         * Set the first page
         */
        private void SetStartPages()
        {
            _currentPage = 1;
            leftAlphaPage.pageToDisplay = _currentPage;
            leftDisplayedPage.pageToDisplay = _currentPage;
            
            _pageCount = leftAlphaPage.textInfo.pageCount;

            if (_pageCount == 1)
            {
                rightAlphaPage.text = "";
                rightDisplayedPage.text = "";
                
                rightAlphaPage.pageToDisplay = 2;
                rightDisplayedPage.pageToDisplay = 2;
            }
            else
            {
                rightAlphaPage.pageToDisplay = _currentPage + 1;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
        }
        
        /**
         * Reset current page number/page count if it was opened back from the next chapter
         */
        private void SetPrevPages()
        {
            _currentPage = 1;
            leftAlphaPage.pageToDisplay = _currentPage;
            leftDisplayedPage.pageToDisplay = _currentPage;
            
            _pageCount = leftAlphaPage.textInfo.pageCount;
            
            _pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;

            _currentPage = _pageCount - 1;

            leftAlphaPage.pageToDisplay = _currentPage;
            leftDisplayedPage.pageToDisplay = _currentPage;
            
            rightAlphaPage.pageToDisplay = _currentPage + 1;
            rightDisplayedPage.pageToDisplay = _currentPage + 1;
        }
        
        /**
         *  Set next chapter
         */
        private void GetNextChapter()
        {
            SetChapter(_currentChapter + 1, false);
        }

        /**
         * Set previous chapter
         */
        private void GetPreviousChapter()
        {
            SetChapter(_currentChapter - 1, false);
        }
    
        
        //=================== NOTES ========================
        /**
         * Show all notes on the open page
         */
        public void ShowAllNotesOnPage()
        {
            // if chapter doesn't have notes
            if (_userData.notes[_currentChapter].notes.Count == 0) return;

            string left = leftDisplayedPage.text;
            string right = rightDisplayedPage.text;
            
            foreach (var note in _userData.notes[_currentChapter].notes)
            {
                left = left.Replace(note.highlightText, tagStart+note.highlightText+tagEnd);
                right = right.Replace(note.highlightText, tagStart+note.highlightText+tagEnd);
            }

            leftDisplayedPage.text = left;
            rightDisplayedPage.text = right;
        }
        
        /**
         * Saves the highlighted note after pressing the save button
         */
        public void SaveNoteOnPage()
        {
            var start = words[0].Item1;
            var end = words[0].Item2;
            
            foreach (var word in words)
            {
                start = (word.Item1 < start) ? word.Item1 : start;
                end = (word.Item2 > end) ? word.Item2 : end;
            }

            string oldText = leftDisplayedPage.text.Substring(start, end - start + 1);
            
            leftDisplayedPage.text = leftDisplayedPage.text.Replace(oldText, tagStart + oldText + tagEnd);
            rightDisplayedPage.text = rightDisplayedPage.text.Replace(oldText, tagStart + oldText + tagEnd);

            words.Clear();
            
            var inputNote = noteText.GetComponent<TMP_InputField>();

            // Save new note and change its color
            Note note = new Note(oldText, inputNote.text);
            _userData.SaveNewNote(note, _currentChapter);

            // deactivate "Save note" button
            noteButton.SetActive(false);
            // deactivate keyboard
            keyboard.SetActive(false);
            // deactivate input field
            inputNote.text = "";
            noteText.SetActive(false);
        }
        
        
        /**
         * Update pages 
         */
        private void ForceUpdate()
        {
            leftAlphaPage.ForceMeshUpdate();
            leftDisplayedPage.ForceMeshUpdate();
            
            rightAlphaPage.ForceMeshUpdate();
            rightDisplayedPage.ForceMeshUpdate();
        }
        
        
        //=================== TABLE OF CONTENT ========================
        public List<string> GetTableOfContent()
        {
            List<string> tableOfContent = new List<string>();
        
            for (int i = 0; i < _stringChapters.Count; i++)
            {
                tableOfContent.Add(_stringChapters[i].title);
            }
        
            return tableOfContent;
        }
            
        
        //=================== JSON ========================
        public void LoadUserData()
        {
            using StreamReader reader = new StreamReader(_jsonFilePath);
            string json = reader.ReadToEnd();

           _userData = JsonUtility.FromJson<UserData>(json);
            Debug.Log(_userData.ToString());
        }

        public void SaveUserData()
        {
            string json = JsonUtility.ToJson(_userData);
            
            using StreamWriter writer = new StreamWriter(_jsonFilePath);
            writer.Write(json);


            sc.ChangeScene();
        }

    }
}
