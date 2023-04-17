using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using chatGPT;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BookScripts
{
    public class TextDisplay : MonoBehaviour
    {
        public SceneController sceneController;
        
        [Header("Book pages")]
        [SerializeField] private TMP_Text leftDisplayedPage;
        [SerializeField] private TMP_Text rightDisplayedPage;
        [SerializeField] private TMP_Text leftPageNumber;
        [SerializeField] private TMP_Text rightPageNumber;
        
        [Header("Page Settings")]
        public TextSettings settings;
        
        [Header("Note-taking tools")]
        [SerializeField] private GameObject noteSaveButton;
        [SerializeField] private GameObject noteCancelButton;
        [SerializeField] private GameObject noteDeleteButton;               // delete existing selected note
        [SerializeField] private GameObject noteRetellingButton;
        [SerializeField] private GameObject noteText;
        [SerializeField] private GameObject noteKeyboard;
        
        [Header("Note parts")]
        public GameObject noteButtonPrefab;
        public GameObject noteButtonParent;
        private List<GameObject> _buttonNotes;

        [HideInInspector] public string selectedNote;           // the text of the selected existing note
        public OpenAIController aiController;
        
        public List<(int, int)> words;                          // start and end index of each word selected by the marker
        
        private string _tagStart;                               // tags for highlighting notes in text
        private string _tagEnd;                                 //
    
        // JSON
        private string _jsonFilePath;
        private UserData _userData;
    
        // Book
        private string _filePath;
        [HideInInspector] public int currentChapter;
        public List<Chapter> stringChapters;
    
        private int _pageCount;
        private int _currentPage;
        
        [Header("Book Search")]
        [SerializeField] private TMP_InputField searchBox;
        public GameObject searchButtonPrefab;
        public GameObject searchButtonParent;
        private List<GameObject> _buttonSearchResults;

        
        [Header("Animation")]
        public GameObject bookMesh;
        public GameObject forwardSheet;
        public GameObject backSheet;
        
        private AlembicStreamPlayer abcPlayer;
        
        void Start()
        {
            _filePath = PlayerPrefs.GetString("selectedBook");

            // _filePath = Application.dataPath + "/Resources/Books/Fahrenheit451/Fahrenheit451_EN.epub";       // ok
            // _filePath = Application.dataPath + "/Resources/Books/BraveNewWorld/BraveNewWorld_EN.epub";       // ok
            //_filePath = Application.dataPath + "/Resources/Books/NineteenEightyFour/1984_EN.epub";             // ok
            
            EPubBookReader reader = new EPubBookReader(_filePath);
            stringChapters = reader.GetAllChapters();
            
            // Notes
            words = new List<(int, int)>();
            selectedNote = "";
            _tagStart = "<color=red>";
            _tagEnd = "</color>";

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
                _userData = new UserData(stringChapters.Count);
            }

            // set text
            currentChapter = 0;
            SetChapter(currentChapter, false);
            
            
            noteKeyboard.SetActive(false);
            noteText.SetActive(false);
            
            AddAllNotesToNotePage();
            
            // search
            _buttonSearchResults = new List<GameObject>();
            
            
            // book mesh anim
            abcPlayer = bookMesh.GetComponent<AlembicStreamPlayer>();

            forwardSheet.SetActive(false);
            backSheet.SetActive(false);
        }
        //=================== ANIMATION ===========================
        private IEnumerator TurnPageEvent()
        {
            float elapsedTime = 0;
            float pageTurnTime = 0.9f;
            
            leftDisplayedPage.alpha = 0;
            rightDisplayedPage.alpha = 0;
            
            while (abcPlayer.CurrentTime < abcPlayer.EndTime - 0.05f)
            {
                abcPlayer.CurrentTime = Mathf.Lerp(abcPlayer.StartTime, abcPlayer.EndTime, elapsedTime/pageTurnTime);
                //float norm = Mathf.Lerp(0f, 255f, elapsedTime / pageTurnTime);
                
                //soupIconMat.SetFloat("_Opacity", norm);
                //titleBox.color = new Color(titleBox.color.r, titleBox.color.g, titleBox.color.b, norm);
                //receiptBox.color = new Color(receiptBox.color.r, receiptBox.color.g, receiptBox.color.b, norm);
            
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            ResetAnim();
        }

        private void ResetAnim()
        {
            abcPlayer.CurrentTime = 0;
            forwardSheet.SetActive(false);
            backSheet.SetActive(false);
            leftDisplayedPage.alpha = 255f;
            rightDisplayedPage.alpha = 255f;
        }
        
        //=================== SEARCH ========================

        public void SearchWholeBook()
        {
            string text = searchBox.text;
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
            
            for (int i = 0; i < _userData.chaptersCount; i++)
            {
                if (allSentences[i].Count == 0) continue;
                foreach (var sentence in allSentences[i])
                {
                    var newButton = Instantiate(searchButtonPrefab, searchButtonParent.transform);
                    newButton.GetComponentInChildren<TMP_Text>().text = sentence;
                    int chap = i;
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
            int pageIndex = leftDisplayedPage.textInfo.characterInfo[charIndex].pageNumber + 1;

            if (pageIndex % 2 != 0)
            {
                _currentPage = pageIndex;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
            else
            {
                _currentPage = pageIndex - 1;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
            ShowPageNumber();
        }
        
        //=================== PAGES ========================
        /**
         * Called only the first time the book is opened.
         * Counts how many pages each chapter takes with each font size
         */
        public void CountPagesNumber()
        {
            for(var chapter = 0; chapter < stringChapters.Count; chapter++)
            {
                leftDisplayedPage.text = stringChapters[chapter].text;

                foreach (var fontSize in settings.sizes)
                {
                    leftDisplayedPage.fontSize = fontSize;
                    leftDisplayedPage.ForceMeshUpdate();
                    var pc = (leftDisplayedPage.textInfo.pageCount % 2 == 0) ? leftDisplayedPage.textInfo.pageCount : leftDisplayedPage.textInfo.pageCount + 1;
                    _userData.pages[chapter].pages.Add(pc);
                }
            }
        }
        
        /**
         * Displays the left and right page numbers
         */
        public void ShowPageNumber()
        {
            int n = 0;

            for (int i = 0; i < currentChapter; i++)
            {
                n += _userData.pages[i].pages[settings.currentFontSize];
            }

            leftPageNumber.text = (n + leftDisplayedPage.pageToDisplay).ToString();
            rightPageNumber.text = (n + rightDisplayedPage.pageToDisplay).ToString();
        }

        /**
         * Set the chapter by given index; Get two parameters:
         *  1) index of chapter
         *  2) bool var "byContent": "true" if chapter opens by table of content
         */
        public void SetChapter(int index, bool byContent)
        {
            if (index < 0) return;
            if (index >= stringChapters.Count) return;
            
            leftDisplayedPage.text = stringChapters[index].text;
            rightDisplayedPage.text = stringChapters[index].text;
        
            // Need it because in the first frame it's null 
            ForceUpdate();
            
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
        
        /**
         * show previous page
         */
        public void PreviousPage()
        {
            backSheet.SetActive(true);
            StartCoroutine(TurnPageEvent());

            if (_currentPage >= 3)
            {
                leftDisplayedPage.pageToDisplay = _currentPage - 2;
                rightDisplayedPage.pageToDisplay = _currentPage - 1;
                
                _currentPage -= 2;
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
            forwardSheet.SetActive(true);
            StartCoroutine(TurnPageEvent());

            int pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;
            if (_currentPage <= pageCount - 3)
            {
                leftDisplayedPage.pageToDisplay = _currentPage + 2;
                rightDisplayedPage.pageToDisplay = _currentPage + 3;
                
                _currentPage += 2;
            }
            else
            {
                GetNextChapter();
            }
            ShowPageNumber();
        }    
        
        /**
         * Reset current page number/page count;
         * Set the first page
         */
        private void SetStartPages()
        {
            _currentPage = 1;
            leftDisplayedPage.pageToDisplay = _currentPage;
            
            _pageCount = leftDisplayedPage.textInfo.pageCount;

            if (_pageCount == 1)
            {
                rightDisplayedPage.text = "";
                rightDisplayedPage.pageToDisplay = 2;
            }
            else
            {
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
        }
        
        /**
         * Reset current page number/page count if it was opened back from the next chapter
         */
        private void SetPrevPages()
        {
            _currentPage = 1;
            leftDisplayedPage.pageToDisplay = _currentPage;
            
            _pageCount = leftDisplayedPage.textInfo.pageCount;
            
            _pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;

            _currentPage = _pageCount - 1;
            
            leftDisplayedPage.pageToDisplay = _currentPage;
            rightDisplayedPage.pageToDisplay = _currentPage + 1;
            ForceUpdate();
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
    
        
        //=================== NOTES ========================

        public void DeleteSelectedNote()
        {
            if (selectedNote == "") return;

            string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");

            leftDisplayedPage.text = leftDisplayedPage.text.Replace(selectedNote, clearNote);
            rightDisplayedPage.text = rightDisplayedPage.text.Replace(selectedNote, clearNote);
            
            _userData.DeleteExistingNote(clearNote, currentChapter);
            selectedNote = "";
            noteDeleteButton.SetActive(false);
            DeleteNoteFormNotePage(clearNote);
        }

        public void DeleteNoteFormNotePage(string t)
        {
            foreach (var b in _buttonNotes)
            {
                if (b.GetComponentInChildren<TMP_Text>().text == t)
                {
                    Destroy(b);
                    _buttonNotes.Remove(b);
                    break;
                }
            }
        }
        
        /**
         * Add all existing notes as buttons to one page
         */
        private void AddAllNotesToNotePage()
        {
            _buttonNotes = new List<GameObject>();
            
            // create button for each note
            for (var i = 0; i < _userData.chaptersCount; i++)
            {
                foreach (var note in _userData.notes[i].notes)
                {
                    var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
                    newButton.GetComponentInChildren<TMP_Text>().text = note.highlightText;
                    int chap = i;
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
            newButton.GetComponentInChildren<TMP_Text>().text = note.highlightText;
            int chap = currentChapter;
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedNote(note, chap));
            _buttonNotes.Add(newButton);
        }
        
        /**
         * Allows to jump to a selected note in the book
         */
        private void ShowPageWithSelectedNote(Note note, int chapter)
        {
            SetChapter(chapter, true);
            int charIndex = stringChapters[currentChapter].text.IndexOf(note.highlightText, StringComparison.Ordinal);
            int pageIndex = leftDisplayedPage.textInfo.characterInfo[charIndex].pageNumber + 1;

            if (pageIndex % 2 != 0)
            {
                _currentPage = pageIndex;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
            else
            {
                _currentPage = pageIndex - 1;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
            ShowPageNumber();
        }

        /**
         * Show all notes on the open page
         */
        public void ShowAllNotesOnPage()
        {
            // if chapter doesn't have notes
            if (_userData.notes[currentChapter].notes.Count == 0) return;

            string left = leftDisplayedPage.text;
            string right = rightDisplayedPage.text;
            
            foreach (var note in _userData.notes[currentChapter].notes)
            {
                left = left.Replace(note.highlightText, _tagStart+note.highlightText+_tagEnd);
                right = right.Replace(note.highlightText, _tagStart+note.highlightText+_tagEnd);
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
            
            leftDisplayedPage.text = leftDisplayedPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);
            rightDisplayedPage.text = rightDisplayedPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);

            words.Clear();
            
            var inputNote = noteText.GetComponent<TMP_InputField>();

            // Save new note and change its color
            Note note = new Note(oldText, inputNote.text);
            _userData.SaveNewNote(note, currentChapter);
            AddNewNoteToNotePage(note);

            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            // deactivate "Cancel note" button
            noteCancelButton.SetActive(false);
            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            inputNote.text = "";
            noteText.SetActive(false);
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
            
            string noteForRetelling = leftDisplayedPage.text.Substring(start, end - start + 1);
            aiController.GetResponse(noteForRetelling);
            
            
            words.Clear();

            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            // deactivate "Cancel note" button
            noteCancelButton.SetActive(false);
            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = noteText.GetComponent<TMP_InputField>();
            inputNote.text = "";
            noteText.SetActive(false);
            
            
            leftDisplayedPage.ForceMeshUpdate();
            rightDisplayedPage.ForceMeshUpdate();
        }
        
        /**
         * Deletes the started note selection
         */
        public void RemoveNoteOnPage()
        {
            words.Clear();
            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            // deactivate "remove note" button
            noteCancelButton.SetActive(false);
            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = noteText.GetComponent<TMP_InputField>();
            inputNote.text = "";
            noteText.SetActive(false);
            
            ForceUpdate();
        }
        
        /**
         * Update pages 
         */
        private void ForceUpdate()
        {
            leftDisplayedPage.ForceMeshUpdate();
            rightDisplayedPage.ForceMeshUpdate();
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
            _userData.fontSize = settings.currentFontSize;
            
            string json = JsonUtility.ToJson(_userData);
            
            using StreamWriter writer = new StreamWriter(_jsonFilePath);
            writer.Write(json);


            sceneController.ChangeScene();
        }

        public void SetFontSizeFromJson()
        {
            leftDisplayedPage.fontSize = settings.sizes[_userData.fontSize];
            rightDisplayedPage.fontSize = settings.sizes[_userData.fontSize];
            settings.currentFontSize = _userData.fontSize;
            settings.SetButtonsVisibility();
        }
    }
}
