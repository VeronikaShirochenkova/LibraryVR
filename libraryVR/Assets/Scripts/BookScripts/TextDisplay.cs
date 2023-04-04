using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace BookScripts
{
    public class TextDisplay : MonoBehaviour
    {
        // Pages
        [SerializeField] private TMP_Text leftPage;
        [SerializeField] private TMP_Text rightPage;
    
        // Notes
        [SerializeField] private TMP_InputField noteInputField;
        [SerializeField] private GameObject noteButton;

        private Color newNoteColor;
        private Color savedNoteColor;
    
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
            //_filePath = PlayerPrefs.GetString("selectedBook");
            //_book = EpubReader.ReadBook(_filePath);
        
            // _filePath = Application.dataPath + "/Resources/Books/Fahrenheit451/Fahrenheit451_EN.epub";       // ok
            // _filePath = Application.dataPath + "/Resources/Books/BraveNewWorld/BraveNewWorld_EN.epub";       // ok
            _filePath = Application.dataPath + "/Resources/Books/NineteenEightyFour/1984_EN.epub";             // ok


            EPubBookReader reader = new EPubBookReader(_filePath);
            _stringChapters = reader.GetAllChapters();
            
            // Notes
            newNoteColor = new Color32(255, 128, 0, 255);
            savedNoteColor = new Color32(76, 255, 147, 255);
        
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
        }
        
        
        //=================== NOTES ========================
        
        /**
         * Set the chapter by given index; Get two parameters:
         *  1) index of chapter
         *  2) bool var "byContent": "true" if chapter opens by table of content
         */
        public void SetChapter(int index, bool byContent)
        {
            if (index < 0) return;
            if (index >= _stringChapters.Count) return;
            
            leftPage.text = _stringChapters[index].text;
            rightPage.text = _stringChapters[index].text;
        
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
                leftPage.pageToDisplay = _currentPage - 2;
                rightPage.pageToDisplay = _currentPage - 1;
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
                leftPage.pageToDisplay = _currentPage + 2;
                rightPage.pageToDisplay = _currentPage + 3;
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
            leftPage.pageToDisplay = _currentPage;
            _pageCount = leftPage.textInfo.pageCount;

            if (_pageCount == 1)
            {
                rightPage.text = "";
                rightPage.pageToDisplay = 2;
            }
            else
            {
                rightPage.pageToDisplay = _currentPage + 1;
            }
        }
        
        /**
         * Reset current page number/page count if it was opened back from the next chapter
         */
        private void SetPrevPages()
        {
            _currentPage = 1;
            leftPage.pageToDisplay = _currentPage;
            _pageCount = leftPage.textInfo.pageCount;
            
            _pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;

            _currentPage = _pageCount - 1;

            leftPage.pageToDisplay = _currentPage;
            rightPage.pageToDisplay = _currentPage + 1;
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
            if (_userData.notes[_currentChapter].Count == 0) return;
        
            // need for render all notes color
            ForceUpdate();
        
            // get indices of first and last char on both pages
            var firstL = leftPage.textInfo.pageInfo[leftPage.pageToDisplay - 1].firstCharacterIndex;
            var lastL = leftPage.textInfo.pageInfo[leftPage.pageToDisplay - 1].lastCharacterIndex;
        
            var firstR = rightPage.textInfo.pageInfo[rightPage.pageToDisplay - 1].firstCharacterIndex;
            var lastR = rightPage.textInfo.pageInfo[rightPage.pageToDisplay - 1].lastCharacterIndex;
        
        
            foreach (var note in _userData.notes[_currentChapter])
            {
                if (note.head >= firstL && note.head <= lastL)
                {
                    ChangeNoteColor(note, leftPage);
                }
            
                if (note.tail >= firstR && note.tail <= lastR)          // <= lastR ???
                {
                    ChangeNoteColor(note, rightPage);
                }
            }
        }
        
        /**
         * Saves the highlighted note after pressing the save button
         */
        public void SaveNoteOnPage()
        {
            bool left = false;
            bool right = false;
        
            int head = -1;
            int tail = -1;
        
        
            (int headL, int tailL) = FindNote(leftPage);
            (int headR, int tailR) = FindNote(rightPage);
        
            // if left page doesn't contain note
            if (headL == -1 && headR != -1)
            {
                head = headR;
                tail = tailR;
                right = true;
            }
        
            // if right page doesn't contain note
            if (headR == -1 && headL != -1)
            {
                head = headL;
                tail = tailL;
                left = true;
            }
        
            // if both pages have notes
            if (headL != -1 && headR != -1)
            {
                head = headL;
                tail = tailR;
                left = true;
                right = true;
            }
        
        
            // Save new note and change its color
            Note note = new Note(head, tail, noteInputField.text);
            _userData.SaveNewNote(note, _currentChapter);
            if (left) {ChangeNoteColor(note, leftPage);}
            if (right) {ChangeNoteColor(note, rightPage);}
        
            // deactivate "Save note" button
            noteButton.SetActive(false);
        }
    
        /**
         * Find the beginning and the end of a highlighted note for further saving
         */
        private (int, int) FindNote(TMP_Text page)
        {
            int head = -1;
            int tail = -1;
        
            var first = page.textInfo.pageInfo[page.pageToDisplay - 1].firstCharacterIndex;
            var last = page.textInfo.pageInfo[page.pageToDisplay - 1].lastCharacterIndex;
        
            TMP_CharacterInfo ch;
        
            // search end of new note
            for (int i = first; i <= last; i++)
            {
                ch = page.textInfo.characterInfo[i];
                // if char is a letter or punctuation mark and has "highlighting" color
                if ((int)page.text[i] >= 33 && page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex] == newNoteColor)
                {
                    tail = i;
                }
            }
        
            // search head of new note
            for (int i = last; i >= first; i--)
            {
                ch = page.textInfo.characterInfo[i];
                // if char is a letter or punctuation mark and has "highlighting" color
                if ((int)page.text[i] >= 33 && page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex] == newNoteColor)
                {
                    head = i;
                }
            }
        
            return (head, tail);
        }

        /**
         * Find first and last word of new note and paint the whole note
         */
        private void ChangeNoteColor(Note note, TMP_Text page)
        {
            // get indices of first and last char on page
            var first = page.textInfo.pageInfo[page.pageToDisplay - 1].firstCharacterIndex;
            var last = page.textInfo.pageInfo[page.pageToDisplay - 1].lastCharacterIndex;
        
            // get indices of first and last char of note
            var start = (note.head < first) ? first : note.head;
            var end = (note.tail > last) ? last : note.tail;
            
            TMP_CharacterInfo ch;
        
            // paint whole note
            for (int i = start; i <= end; i++)
            {
                ch = page.textInfo.characterInfo[i];
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 0] = savedNoteColor;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = savedNoteColor;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = savedNoteColor;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = savedNoteColor;
                page.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }           
        
        /**
         * Update pages 
         */
        private void ForceUpdate()
        {
            leftPage.ForceMeshUpdate();
            rightPage.ForceMeshUpdate();
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
        }

    }
}
