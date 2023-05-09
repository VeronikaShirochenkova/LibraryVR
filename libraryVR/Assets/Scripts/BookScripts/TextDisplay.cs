using System;
using TMPro;
using chatGPT;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Formats.Alembic.Importer;
using UnityEngine.Serialization;


namespace BookScripts
{
    public class TextDisplay : MonoBehaviour
    {
        public SceneController sceneController;
        public GameObject tableOfContentObject;
        
        [Header("Book pages")]
        [SerializeField] private TMP_Text leftDisplayedPage;
        [SerializeField] private TMP_Text rightDisplayedPage;
        [SerializeField] private TMP_Text leftPageNumber;
        [SerializeField] private TMP_Text rightPageNumber;
        
        [Header("Book Settings paper")]
        public GameObject settingsObject;
        public TextSettings settings;
        
        [Header("Book Search")]
        [SerializeField] private TMP_InputField searchInputField;
        public GameObject searchResultButtonPrefab;
        public GameObject searchResultButtonParent;
        public GameObject noMatchFound;
        private List<GameObject> _buttonSearchResults;
        
        [Header("Note-taking tools")]
        [SerializeField] private GameObject noteSaveButton;
        [SerializeField] private GameObject noteCancelButton;
        [SerializeField] private GameObject noteWriteButton;
        
        [SerializeField] private GameObject notePaper;
        private TMP_InputField notePaperInputField;
        [SerializeField] private GameObject noteKeyboard;

        [SerializeField] private GameObject textToNotePaper;
        private TMP_Text _textToNote;
        
        [SerializeField] private GameObject noteDeleteButton;
        private MoveObject _deleteButton;
        [SerializeField] private GameObject noteRetellingButton;
        
        
        [Header("Page with highlighted notes")]
        public GameObject noteButtonPrefab;
        public GameObject noteButtonParent;
        public GameObject notePrevButton;
        public GameObject noteNextButton;
        public AudioSource sound;
        private int _allNotesPageCount;
        private int _allNotesCurrPage;
        private List<GameObject> _buttonNotes;

        
        [Header("Retelling")]
        public OpenAIController aiController;
        public GameObject retellingTablet;

        [Header("Animation")]
        public GameObject bookMesh;
        public GameObject forwardSheet;
        public GameObject backSheet;
        private AlembicStreamPlayer abcPlayer;
        
        // Notes & Retelling
        public List<(int, int)> words;                          // start and end index of each word selected by the marker
        [HideInInspector] public string selectedNote;           // the text of the selected existing note
        
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
        
        
        
        void Start()
        {
            _filePath = PlayerPrefs.GetString("selectedBook");
            //_filePath = Application.dataPath + "/Resources/Books/NineteenEightyFour/1984_EN.epub";             // ok
            
            EPubBookReader reader = new EPubBookReader(_filePath);
            stringChapters = reader.GetAllChapters();
            
            // Notes
            words = new List<(int, int)>();
            selectedNote = "";
            _tagStart = "<font=\"Brass Mono\"><mark=#767EE190>";
            _tagEnd = "</mark>";
            

            // JSON
            _jsonFilePath = Path.GetDirectoryName(_filePath) + "\\" + Path.GetFileNameWithoutExtension(_filePath) + ".json";
        
            if (File.Exists(_jsonFilePath))
            {
                LoadUserData();
            }
            else
            {
                _userData = new UserData(stringChapters.Count);
            }

            // set text
            SetChapter(_userData.chapterBookmarkStandardBook, false);
            SetPage(_userData.pageBookmarkStandardBook);
            
            // add all highlighted notes to page with them
            AddAllNotesToNotePage();
            
            // search
            _buttonSearchResults = new List<GameObject>();
            
            // book mesh anim
            abcPlayer = bookMesh.GetComponent<AlembicStreamPlayer>();

            forwardSheet.SetActive(false);
            backSheet.SetActive(false);

            _deleteButton = noteDeleteButton.GetComponent<MoveObject>();
            
            retellingTablet.SetActive(false);
            
            noteKeyboard.SetActive(false); 
            notePaper.SetActive(false);
            
            settingsObject.SetActive(true);
            tableOfContentObject.SetActive(true);

            _textToNote = textToNotePaper.GetComponentInChildren<TMP_Text>();
            textToNotePaper.SetActive(false);
            
            // other
            notePaperInputField = notePaper.GetComponentInChildren<TMP_InputField>();
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
            string text = searchInputField.text;
            if (text.Length < 2)
            {
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
                    var newButton = Instantiate(searchResultButtonPrefab, searchResultButtonParent.transform);
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

        public void UpdateSearchResult()
        {
            if (searchInputField.text.Length == 0)
            {
                DestroySearchResults();
            }
            noMatchFound.SetActive(false);
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
                    _userData.standardBookPages[chapter].pages.Add(pc);
                }
            }
            
            leftDisplayedPage.text = stringChapters[currentChapter].text;
        }
        
        /**
         * Displays the left and right page numbers
         */
        public void ShowPageNumber()
        {
            int n = 0;

            for (int i = 0; i < currentChapter; i++)
            {
                n += _userData.standardBookPages[i].pages[settings.currentFontSize];
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
            
            ShowAllNotesInChapter();
            ShowPageNumber();
        }

        public void SetPage(int index)
        {
            if (index % 2 != 0)
            {
                _currentPage = index;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
            else
            {
                _currentPage = index - 1;
                leftDisplayedPage.pageToDisplay = _currentPage;
                rightDisplayedPage.pageToDisplay = _currentPage + 1;
            }
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
                    int chap = i;
                    
                    var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
                    newButton.GetComponent<HighlightedButtonController>().CreateButton(note, stringChapters[chap].title.TrimEnd());
                    newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedNote(note, chap));
                    
                    _buttonNotes.Add(newButton);
                }
            }

            _allNotesCurrPage = 0;
            SetAllNotesPages();
        }

        private void SetAllNotesPages()
        {
            if (_buttonNotes.Count > 4)
            {
                _allNotesPageCount = (_buttonNotes.Count % 4 == 0) ? _buttonNotes.Count / 4 : (_buttonNotes.Count / 4) + 1;
            
                //notePrevButton.SetActive(true);
                noteNextButton.SetActive(true);

                for (int i = 0; i < 4; i++)
                {
                    _buttonNotes[i].SetActive(true);
                }

                for (int i = 4; i < _buttonNotes.Count; i++)
                {
                    _buttonNotes[i].SetActive(false);
                }
            }
            else
            {
                _allNotesCurrPage = 0;
                for (int i = 0; i < _buttonNotes.Count; i++)
                {
                    _buttonNotes[i].SetActive(true);
                }
                
                notePrevButton.SetActive(false);
                noteNextButton.SetActive(false);
            }
        }
        
        /**
         * Show the next part of the note buttons in "Highlighted" paper
         */
        public void ShowNextPageHighlighted()
        {
            if (_allNotesCurrPage == _allNotesPageCount - 1) return;

            sound.Play();
            ChangeVisible();
            if (_allNotesCurrPage == 0) notePrevButton.SetActive(true);
            _allNotesCurrPage++;
            if (_allNotesCurrPage == _allNotesPageCount - 1) noteNextButton.SetActive(false);
            ChangeVisible();
        }
        
        /**
         * Show the previous part of the note buttons in "Highlighted" paper
         */
        public void ShowPreviousPageHighlighted()
        {
            if (_allNotesCurrPage == 0) return;
            
            sound.Play();
            ChangeVisible();
            if (_allNotesCurrPage == _allNotesPageCount - 1) noteNextButton.SetActive(true);
            _allNotesCurrPage--;
            if (_allNotesCurrPage == 0) notePrevButton.SetActive(false);
            ChangeVisible();
        }
        
        private void ChangeVisible()
        {
            int start = _allNotesCurrPage * 4;
            int end = (start+4 > _buttonNotes.Count) ? _buttonNotes.Count : start+4;
        
            for (int i = start; i < end; i++)
            {
                _buttonNotes[i].SetActive(!_buttonNotes[i].activeSelf);
            }
        }
        
        /**
         * Adds new saved note to page with all existing notes
         */
        private void AddNewNoteToNotePage(Note note)
        {
            var needReset = _buttonNotes.Count == 4;
            var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
            int chap = currentChapter;
            newButton.GetComponent<HighlightedButtonController>().CreateButton(note, stringChapters[chap].title.TrimEnd());
            //newButton.GetComponentInChildren<TMP_Text>().text = stringChapters[chap].title.TrimEnd() + ": " + note.highlightText;
            
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowPageWithSelectedNote(note, chap));
            _buttonNotes.Add(newButton);
            
            if (needReset) { SetAllNotesPages(); }
        }        
        
        public void DeleteNoteFormNotePage(string t)
        {
            foreach (var b in _buttonNotes)
            {
                if (b.GetComponentInChildren<TMP_Text>().text.Contains(t))
                {
                    int count = _buttonNotes.Count;
                    Destroy(b);
                    _buttonNotes.Remove(b);
                    if (count == 5) {SetAllNotesPages();}
                    break;
                }
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
        

        public void DeleteSelectedNote()
        {
            if (selectedNote == "") return;

            string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");

            leftDisplayedPage.text = leftDisplayedPage.text.Replace(selectedNote, clearNote);
            rightDisplayedPage.text = rightDisplayedPage.text.Replace(selectedNote, clearNote);
            
            _userData.DeleteExistingNote(clearNote, currentChapter);
            selectedNote = "";
            
            
            _deleteButton.MoveObjectOnClick();
            DeleteNoteFormNotePage(clearNote);
            
            // deactivate "Save note" button
            noteSaveButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Write mark" button
            noteWriteButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate keyboard
            noteKeyboard.SetActive(false);
            
            // deactivate input field
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
        public void ShowAllNotesInChapter()
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
        
        /**
         * Saves the highlighted note after pressing the save button
         */
        public void SaveNoteOnPage()
        {
            
            var inputNote = notePaperInputField.text;
            string date = DateTime.UtcNow.GetDateTimeFormats('d')[0];
            
            if (selectedNote.Length != 0 )
            {
                string clearNote = selectedNote.Replace(_tagStart, "").Replace(_tagEnd, "");
                _userData.UpdateExistingNote(clearNote, inputNote, currentChapter);
                UpdateNoteButtonInfo(clearNote, inputNote.Length != 0);
                selectedNote = "";
                ShowTextToSelectedNote();
                noteDeleteButton.GetComponent<MoveObject>().MoveObjectOnClick();
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
                
                string oldText = leftDisplayedPage.text.Substring(start, end - start + 1);
                leftDisplayedPage.text = leftDisplayedPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);
                rightDisplayedPage.text = rightDisplayedPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);
                words.Clear();
                
                // Save new note and change its color
                Note note = new Note(oldText, inputNote, date);
                _userData.SaveNewNote(note, currentChapter);
                AddNewNoteToNotePage(note);
                
                // deactivate "Cancel note" button
                noteCancelButton.GetComponent<MoveObject>().MoveObjectOnClick();
                
                // deactivate "retelling" button
                noteRetellingButton.GetComponent<MoveObject>().MoveObjectOnClick();
            }
            
            
            //var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();

            // deactivate "Save note" button
            noteSaveButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Write mark" button
            noteWriteButton.GetComponent<MoveObject>().MoveObjectOnClick();
            
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            
            // deactivate input field
            notePaperInputField.text = "";
            notePaper.SetActive(false);
            
            selectedNote = "";
            
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
            retellingTablet.SetActive(true);
            
            
            words.Clear();

            // deactivate "Save note" button
            noteSaveButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Cancel note" button
            noteCancelButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Write mark" button
            noteWriteButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "retelling" button
            noteRetellingButton.GetComponent<MoveObject>().MoveObjectOnClick();
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            inputNote.text = "";
            notePaper.SetActive(false);
            
            
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
            noteSaveButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Cancel note" button
            noteCancelButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "Write mark" button
            noteWriteButton.GetComponent<MoveObject>().MoveObjectOnClick();

            // deactivate "retelling" button
            noteRetellingButton.GetComponent<MoveObject>().MoveObjectOnClick();
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            inputNote.text = "";
            notePaper.SetActive(false);
            
            ForceUpdate();
        }

        public void KeyboardAndNotePaperVisibility()
        {
            noteKeyboard.SetActive(!noteKeyboard.activeSelf);

            if (notePaper.activeSelf)
            {
                if (selectedNote.Length != 0)
                {
                    notePaper.GetComponentInChildren<TMP_InputField>().text = "";
                }
                notePaper.SetActive(false);
                if (textToNotePaper.GetComponent<TMP_Text>().text.Length != 0)
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
            _userData.openedAsStandardBook = true;
            
            _userData.fontSizeStandardBook = settings.currentFontSize;
            
            _userData.pageBookmarkStandardBook = _currentPage;
            _userData.chapterBookmarkStandardBook = currentChapter;
            
            string json = JsonUtility.ToJson(_userData);
            
            using StreamWriter writer = new StreamWriter(_jsonFilePath);
            writer.Write(json);
            
            sceneController.ChangeScene();
        }

        public void SetFontSizeFromJson()
        {
            leftDisplayedPage.fontSize = settings.sizes[_userData.fontSizeStandardBook];
            rightDisplayedPage.fontSize = settings.sizes[_userData.fontSizeStandardBook];
            settings.currentFontSize = _userData.fontSizeStandardBook;
            settings.SetButtonsVisibility();
            ShowPageNumber();
        }
    }
}
