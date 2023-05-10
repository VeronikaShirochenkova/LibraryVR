using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookScripts;
using chatGPT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TabletScripts
{
    public class TabletTextDisplay : MonoBehaviour
    {
        public SceneController sceneController;
        public GameObject TOC;
        [Header("Tablet pages")] 
        [SerializeField] private TMP_Text tabletPage;
        [SerializeField] private TMP_Text tabletPageNumber;

        [Header("Page Settings")] 
        public GameObject settingsObject;
        public TabletTextSettings settings;
        
        [Header("Note-taking tools")]
        [SerializeField] private GameObject noteSaveButton;
        [SerializeField] private GameObject noteCancelButton;
        [SerializeField] private GameObject noteWriteButton;
        
        [SerializeField] private GameObject notePaper;
        private TMP_InputField notePaperInputField;
        [SerializeField] private GameObject noteKeyboard;
        [SerializeField] private GameObject textToNotePaper;
        private TMP_Text _textToNote;
        
        [SerializeField] private GameObject noteDeleteButton;               // delete existing selected note
        [SerializeField] private GameObject noteRetellingButton;

        [Header("Note parts")] 
        public GameObject highlightedWindow;
        public GameObject noteButtonPrefab;
        public GameObject noteButtonParent;
        private List<GameObject> _buttonNotes;
        
        [Header("Book Search")]
        [SerializeField] private TMP_InputField searchInputField;
        public GameObject searchResultButtonPrefab;
        public GameObject searchResultButtonParent;
        public GameObject noMatchFound;
        private List<GameObject> _buttonSearchResults;
        public GameObject search;
        
        // Book
        private string _filePath;
        [HideInInspector] public int currentChapter;
        public List<Chapter> stringChapters;
        
        private int _pageCount;
        private int _currentPage;
        
        [HideInInspector] public string selectedNote;           // the text of the selected existing note
        public List<(int, int)> words;                          // start and end index of each word selected by the marker
        private string _tagStart;                               // tags for highlighting notes in text
        private string _tagEnd; 
        
        [Header("Retelling")]
        public OpenAIController aiController;
        public GameObject retellingTablet;
        
        // JSON
        private string _jsonFilePath;
        private UserData _userData;
        
        void Start()
        {
            _filePath = PlayerPrefs.GetString("selectedBook");
            EPubBookReader reader = new EPubBookReader(_filePath);
            stringChapters = reader.GetAllChapters();
            
            // Notes
            words = new List<(int, int)>();
            selectedNote = "";
            _tagStart = "<font=\"Brass Mono\"><mark=#767EE190>";
            _tagEnd = "</mark>";
            
            //JSON
            _jsonFilePath = Path.GetDirectoryName(_filePath) + "\\" + Path.GetFileNameWithoutExtension(_filePath) + ".json";
            if (File.Exists(_jsonFilePath))
            {
                LoadUserData();
            }
            else
            {
                _userData = new UserData(stringChapters.Count);
            }
            
            SetChapter(_userData.chapterBookmarkTablet, false);
            SetPage(_userData.pageBookmarkTablet);
            
            // add all highlighted notes to page with them
            AddAllNotesToNotePage();
            
            // search
            _buttonSearchResults = new List<GameObject>();


            noteKeyboard.SetActive(false);
            notePaper.SetActive(false);
            _textToNote = textToNotePaper.GetComponentInChildren<TMP_Text>();
            textToNotePaper.SetActive(false);

            settingsObject.SetActive(true);
            TOC.SetActive(true);
            
            notePaperInputField = notePaper.GetComponentInChildren<TMP_InputField>();
        }
        
        //=================== TABLE OF CONTENT ========================
        public List<string> GetTableOfContent()
        {
            List<string> tableOfContent = new List<string>();
        
            for (int i = 0; i < stringChapters.Count; i++)
            {
                tableOfContent.Add(stringChapters[i].title);
            }
        
            return tableOfContent;
        }
        
        //==================== PAGES ======================
        public void ShowPageNumber()
        {
            int n = 0;

            for (int i = 0; i < currentChapter; i++)
            {
                n += _userData.tabletPages[i].pages[settings.currentFontSize];
            }

            tabletPageNumber.text = (n + tabletPage.pageToDisplay).ToString();
        }
        
        public void CountPagesNumber()
        {
            for(var chapter = 0; chapter < stringChapters.Count; chapter++)
            {
                tabletPage.text = stringChapters[chapter].text;

                foreach (var fontSize in settings.sizes)
                {
                    tabletPage.fontSize = fontSize;
                    tabletPage.ForceMeshUpdate();
                    _userData.tabletPages[chapter].pages.Add(tabletPage.textInfo.pageCount);
                }
            }

            tabletPage.text = stringChapters[currentChapter].text;
        }        
        
        public void SetChapter(int index, bool byContent)
        {
            if (index < 0) return;
            if (index >= stringChapters.Count) return;
            
            tabletPage.text = stringChapters[index].text;
            tabletPage.ForceMeshUpdate();
            
            if (index >= currentChapter || byContent)
            {
                currentChapter = index;
                SetStartPages();
            }
            else if(index < currentChapter)
            {
                currentChapter = index;
                SetPrevPages();
            }
            
            ShowAllNotesOnPage();
            ShowPageNumber();
        }
        
        public void SetPage(int index)
        {

            _currentPage = index;
            tabletPage.pageToDisplay = _currentPage;
            
            ShowPageNumber();
        }
        
        public void PreviousPage()
        {
            if (_currentPage > 1)
            {
                _currentPage -= 1;
                tabletPage.pageToDisplay = _currentPage;
            }
            else
            {
                GetPreviousChapter();
            }
            
            ShowPageNumber();
        }
        
        /**
         * show next page
         */
        public void NextPage()
        {
            if (_currentPage < _pageCount )
            {
                _currentPage += 1;
                tabletPage.pageToDisplay = _currentPage;
            }
            else
            {
                GetNextChapter();
            }
            ShowPageNumber();
        }            
        /**
         * Displays the page numbers
         */
        
        private void SetStartPages()
        {
            _currentPage = 1;
            tabletPage.pageToDisplay = _currentPage;
            tabletPage.ForceMeshUpdate();
            
            _pageCount = tabletPage.textInfo.pageCount;
        }
        
        private void SetPrevPages()
        {
            _currentPage = 1;
            tabletPage.pageToDisplay = _currentPage;
            
            _pageCount = tabletPage.textInfo.pageCount;

            _currentPage = _pageCount;
            
            tabletPage.pageToDisplay = _currentPage;
            tabletPage.ForceMeshUpdate();
        }
        
        /**
         *  Set next chapter
         */
        private void GetNextChapter()
        {
            SetChapter(currentChapter + 1, false);
        }

        /**
         * Set previous chapter
         */
        private void GetPreviousChapter()
        {
            SetChapter(currentChapter - 1, false);
        }
        
        //=================== NOTES =======================

        public void ChangeHighlightedWindowVisibility()
        {
            highlightedWindow.SetActive(!highlightedWindow.activeSelf);
        }
        
        private void AddAllNotesToNotePage()
        {
            _buttonNotes = new List<GameObject>();
            
            // create button for each note
            for (var i = 0; i < _userData.chaptersCount; i++)
            {
                foreach (var note in _userData.notes[i].notes)
                {
                    int chap = i;
                    
                    var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
                    //newButton.GetComponentInChildren<TMP_Text>().text = stringChapters[chap].title.TrimEnd() + ": " + note.highlightText;
                    newButton.GetComponent<HighlightedButtonController>().CreateButton(note, stringChapters[chap].title.TrimEnd());
                    newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedNote(note, chap));
                    
                    _buttonNotes.Add(newButton);
                }
            }
            
        }
        
        
        /**
         * Adds new saved note to page with all existing notes
         */
        private void AddNewNoteToNotePage(Note note)
        {
            var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
            int chap = currentChapter;
            newButton.GetComponent<HighlightedButtonController>().CreateButton(note, stringChapters[chap].title.TrimEnd());
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedNote(note, chap));
            _buttonNotes.Add(newButton);
        }    
        
        public void DeleteNoteFormNotePage(string t)
        {
            foreach (var b in _buttonNotes)
            {
                if (b.GetComponentInChildren<TMP_Text>().text.Contains(t))
                {
                    Destroy(b);
                    _buttonNotes.Remove(b);
                    break;
                }
            }
        }  
        
        public void DeleteSelectedNote()
        {
            if (selectedNote == "") return;

            string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");

            tabletPage.text = tabletPage.text.Replace(selectedNote, clearNote);
            
            _userData.DeleteExistingNote(clearNote, currentChapter);
            selectedNote = "";
            
            noteDeleteButton.SetActive(false);
            DeleteNoteFormNotePage(clearNote);
            
            noteSaveButton.SetActive(false);
            noteWriteButton.SetActive(false);
            
            noteKeyboard.SetActive(false);
            notePaperInputField.text = "";
            notePaper.SetActive(false);
        }
        
        /**
         * Allows to jump to a selected note in the book
         */
        private void ShowPageWithSelectedNote(Note note, int chapter)
        {
            SetChapter(chapter, true);
            int charIndex = stringChapters[currentChapter].text.IndexOf(note.highlightText, StringComparison.Ordinal);
            int pageIndex = tabletPage.textInfo.characterInfo[charIndex].pageNumber + 1;


            _currentPage = pageIndex;
            tabletPage.pageToDisplay = _currentPage;

            ShowPageNumber();
            highlightedWindow.SetActive(false);
        }
        
        public void ShowAllNotesOnPage()
        {
            // if chapter doesn't have notes
            if (_userData.notes[currentChapter].notes.Count == 0) return;

            string left = tabletPage.text;

            
            foreach (var note in _userData.notes[currentChapter].notes)
            {
                left = left.Replace(note.highlightText, _tagStart+note.highlightText+_tagEnd);
            }

            tabletPage.text = left;
        }
        
        private void UpdateNoteButtonInfo(string text, bool note)
        {
            foreach (var b in _buttonNotes)
            {
                if (b.GetComponentInChildren<TMP_Text>().text.Contains(text))
                {
                    b.GetComponent<HighlightedButtonController>().SetNoteIndicator(note);
                    break;
                }
            }
        }
        public void SaveNoteOnPage()
        {
            var inputNote = notePaperInputField.text;
            string date = DateTime.UtcNow.GetDateTimeFormats('d')[0];

            if (selectedNote.Length != 0)
            {
                string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");
                _userData.UpdateExistingNote(clearNote, inputNote, currentChapter);
                UpdateNoteButtonInfo(clearNote, inputNote.Length != 0);
                selectedNote = "";
                ShowTextToSelectedNote();
                noteDeleteButton.SetActive(false);
            }
            else
            {
                var start = words[0].Item1;
                var end = words[0].Item2;
                
                foreach (var word in words)
                {
                    start = (word.Item1 < start) ? word.Item1 : start;
                    end = (word.Item2 > end) ? word.Item2 : end;
                }
                
                string oldText = tabletPage.text.Substring(start, end - start + 1);
                tabletPage.text = tabletPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);
                words.Clear();
                
                Note note = new Note(oldText, inputNote, date);
                _userData.SaveNewNote(note, currentChapter);
                AddNewNoteToNotePage(note);
                
                // deactivate "Cancel note" button
                noteCancelButton.SetActive(false);
                
                // deactivate "retelling" button
                noteRetellingButton.SetActive(false);
            }
            
            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            
            // deactivate "Write mark" button
            noteWriteButton.SetActive(false);
            

            //noteWriteTools.SetActive(false);
            noteKeyboard.SetActive(false);
            notePaperInputField.text = "";
            notePaper.SetActive(false);
            selectedNote = "";
            //textToNotePaper.SetActive(false);
            
            //// deactivate keyboard
            //noteKeyboard.SetActive(false);
            //// deactivate input field
            //inputNote.text = "";
            //notePaper.SetActive(false);
        }
        
        public void GetNoteForRetelling()
        {
            var start = words[0].Item1;
            var end = words[0].Item2;
            
            foreach (var word in words)
            {
                start = (word.Item1 < start) ? word.Item1 : start;
                end = (word.Item2 > end) ? word.Item2 : end;
            }
            
            string noteForRetelling = tabletPage.text.Substring(start, end - start + 1);
            aiController.GetResponse(noteForRetelling);
            retellingTablet.SetActive(true);
            
            
            words.Clear();

            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            // deactivate "Cancel note" button
            noteCancelButton.SetActive(false);
            // deactivate "Write mark" button
            noteWriteButton.SetActive(false);
            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);
            
            //noteWriteTools.SetActive(false);
            noteKeyboard.SetActive(false);
            notePaper.SetActive(false);
            textToNotePaper.SetActive(false);
            
            //// deactivate keyboard
            //noteKeyboard.SetActive(false);
            //// deactivate input field
            //var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            //inputNote.text = "";
            //notePaper.SetActive(false);
            
            
            tabletPage.ForceMeshUpdate();
        }
        
        public void RemoveNoteOnPage()
        {
            words.Clear();
            // deactivate "Save note" button
            noteSaveButton.SetActive(false);

            // deactivate "Cancel note" button
            noteCancelButton.SetActive(false);

            // deactivate "Write mark" button
            noteWriteButton.SetActive(false);

            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);
            
            //noteWriteTools.SetActive(false);
            noteKeyboard.SetActive(false);
            notePaper.SetActive(false);
            textToNotePaper.SetActive(false);
            
            
            // // deactivate keyboard
            // noteKeyboard.SetActive(false);
            // // deactivate input field
            // var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            // inputNote.text = "";
            // notePaper.SetActive(false);
            
            tabletPage.ForceMeshUpdate();
        }
        
        public void KeyboardAndNotePaperVisibility()
        {
            // if (notePaper.activeSelf)
            // {
            //     notePaper.GetComponentInChildren<TMP_InputField>().text = "";
            // }
            // noteWriteTools.SetActive(!noteWriteTools.activeSelf);
            noteKeyboard.SetActive(!noteKeyboard.activeSelf);

            if (notePaper.activeSelf)
            {
                if (selectedNote.Length != 0)
                {
                    notePaper.GetComponentInChildren<TMP_InputField>().text = "";
                }
                notePaper.SetActive(false);
                if (textToNotePaper.GetComponentInChildren<TMP_Text>().text.Length != 0)
                {
                    textToNotePaper.SetActive(true);
                }
            }
            else
            {
                notePaper.SetActive(true);
                textToNotePaper.SetActive(false);
            }
            
        }

        public void ShowTextToSelectedNote()
        {
            if (selectedNote == "")
            {
                textToNotePaper.SetActive(false);
                _textToNote.text = "";
            }
            
            string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");
            string t = _userData.GetTextToNote(clearNote, currentChapter);
            if (t != "")
            {
                textToNotePaper.SetActive(true); 
                _textToNote.text = t;
            }
            else
            {
                textToNotePaper.SetActive(false); 
            }
        }
        
        public void PaperTextUpdate()
        {
            if (selectedNote == "")
            {
                return;
            }
            
            string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");
            string t = _userData.GetTextToNote(clearNote, currentChapter);
            if (t != "")
            {
                notePaperInputField.text = t;
            }

        }
        //=================== SEARCH ========================
        
        public void SearchWholeBook()
        {
            string text = searchInputField.text;
            if (text.Length < 2)
            {
                Debug.Log("here");
                DestroySearchResults();
                searchInputField.text = "";
                noMatchFound.SetActive(true);
                return;
            }

            noMatchFound.SetActive(false);
            
            List<List<string>> allSentences = new List<List<string>>();
            

            for (int i = 0; i < _userData.chaptersCount; i++)
            {
                string[] sentences = stringChapters[i].text.Split(new char[] { '.', '?', '!' });
                List<string> chapterSentences = new List<string>();

                foreach (string sentence in sentences)
                {
                    if (sentence.Contains(text))
                    {
                        chapterSentences.Add(sentence);
                    }
                }
                allSentences.Add(chapterSentences);
            }
            
            
            ShowSearchResults(allSentences);
        }
        
        private void ShowSearchResults(List<List<string>> allSentences)
        {
            DestroySearchResults();
            //listOfLists.All(lst => lst.Count == 0)
            //if (allSentences.Count == 0)
            if (allSentences.All(lst => lst.Count == 0))
            {
                noMatchFound.SetActive(true);
                return;
            }
            for (int i = 0; i < _userData.chaptersCount; i++)
            {
                if (allSentences[i].Count == 0) continue;
                foreach (var sentence in allSentences[i])
                {
                    int chap = i;
                    var newButton = Instantiate(searchResultButtonPrefab, searchResultButtonParent.transform);
                    newButton.GetComponentInChildren<TMP_Text>().text = sentence;
                    newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedSentence(sentence, chap));
                    
                    _buttonSearchResults.Add(newButton);
                }
            }
        }

        private void DestroySearchResults()
        {
            if (_buttonSearchResults.Count == 0) return;
            
            foreach (var res in _buttonSearchResults)
            {
                Destroy(res);
            }
            _buttonSearchResults.Clear();
        }
        
        private void ShowPageWithSelectedSentence(string sentence, int chapter)
        {
            SetChapter(chapter, true);
            int charIndex = stringChapters[currentChapter].text.IndexOf(sentence, StringComparison.Ordinal);
            int pageIndex = tabletPage.textInfo.characterInfo[charIndex].pageNumber + 1;
            
            _currentPage = pageIndex;
            tabletPage.pageToDisplay = _currentPage;

            ShowPageNumber();
            search.SetActive(false);
        }   
        
        public void UpdateSearchResult()
        {
            if (searchInputField.text.Length == 0)
            {
                DestroySearchResults();
            }
            noMatchFound.SetActive(false);
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
            _userData.openedAsReadingTablet = true;
            
            _userData.fontSizeReadingTablet = settings.currentFontSize;

            _userData.pageBookmarkTablet = _currentPage;
            _userData.chapterBookmarkTablet = currentChapter;
            
            string json = JsonUtility.ToJson(_userData);
            
            using StreamWriter writer = new StreamWriter(_jsonFilePath);
            writer.Write(json);


            sceneController.ChangeScene();
        }
        
        public void SetFontSizeFromJson()
        {
            tabletPage.fontSize = settings.sizes[_userData.fontSizeReadingTablet];
            settings.currentFontSize = _userData.fontSizeReadingTablet;
            settings.SetButtonsVisibility();
            ShowPageNumber();
        }
    }
}
