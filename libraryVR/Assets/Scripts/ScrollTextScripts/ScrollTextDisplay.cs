using System;
using System.Collections.Generic;
using System.IO;
using BookScripts;
using chatGPT;
using ScrollView;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ScrollTextScripts
{
    public class ScrollTextDisplay : MonoBehaviour
    
    {
        public SceneController sceneController;
        public GameObject TOC;
        
        [FormerlySerializedAs("tabletPage")]
        [Header("Scroll pages")] 
        [SerializeField] private TMP_Text scrollPage;
        [SerializeField] private TMP_Text scrollPageNumber;

        //[SerializeField] private GameObject content;
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject content;

        [Header("Page Settings")] 
        public GameObject settingsObject;
        public ScrollTextSettings settings;
        
        [Header("Note-taking tools")]
        [SerializeField] private GameObject noteSaveButton;
        [SerializeField] private GameObject noteCancelButton;
        [SerializeField] private GameObject noteWriteButton;
        
        [SerializeField] private GameObject noteWriteTools;
        [SerializeField] private GameObject notePaper;
        [SerializeField] private GameObject noteKeyboard;
        
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
            _tagStart = "<font=\"Brass Mono SDF\"><mark=#767EE190>";
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
            
            SetChapter(_userData.chapterBookmarkScrollPage, false);
            _currentPage = 1;
            //SetPage(_userData.pageBookmarkScrollPage);
            
            AddAllNotesToNotePage();
            
            // search
            _buttonSearchResults = new List<GameObject>();
            
            noteWriteTools.SetActive(false);
            
            // set text
            settingsObject.SetActive(true);
            TOC.SetActive(true);
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
                n += _userData.scrollPages[i].pages[settings.currentFontSize];
            }
        
            scrollPageNumber.text = (n + _currentPage).ToString();
        }
        
        public void CountPagesNumber()
        {
            for(var chapter = 0; chapter < stringChapters.Count; chapter++)
            {
                scrollPage.text = stringChapters[chapter].text;
        
                foreach (var fontSize in settings.sizes)
                {
                    scrollPage.fontSize = fontSize;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(scrollPage.GetComponent<RectTransform>());
                    scrollPage.ForceMeshUpdate();
                    
                    RectTransform viewport = scrollView.GetComponent<ScrollRect>().viewport;
                    float num = scrollPage.rectTransform.rect.height / viewport.rect.height;
                    num = ((num * 10) % 10 != 0) ? num + 1 : num;
                    
                    _userData.scrollPages[chapter].pages.Add((int)num);
                    Debug.Log(fontSize + " " + scrollPage.rectTransform.rect.height + " " + viewport.rect.height );
                }
            }

            scrollPage.text = stringChapters[currentChapter].text;
        }

        public void GetCurrentPage()
        {
            int numOfPages = _userData.scrollPages[currentChapter].pages[settings.currentFontSize];
            float currScrollbarPos = scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition;
            
            for (int i = 0; i <= numOfPages; i++)
            {
                if (1.0f - i * (1.0f / numOfPages) >= currScrollbarPos)
                {
                    _currentPage = i;
                }
                else
                {
                    break;
                }
            }

            ShowPageNumber();
        }
        
        public void SetChapter(int index, bool byContent)
        {
            if (index < 0) return;
            if (index >= stringChapters.Count) return;
            
            scrollPage.text = stringChapters[index].text;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.GetComponent<RectTransform>());
            scrollPage.ForceMeshUpdate();

            if (index >= currentChapter || byContent)
            {
                currentChapter = index;
                if (scrollPage.rectTransform.rect.height > scrollView.GetComponent<ScrollRect>().viewport.rect.height)
                {
                    scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;
                }
                
            }
            else if(index < currentChapter)
            {
                currentChapter = index;
                scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
            }
            
            ShowAllNotesOnPage();
            ShowPageNumber();
        }


        /**
         *  Set next chapter
         */
        public void GetNextChapter()
        {
            SetChapter(currentChapter + 1, false);
        }

        /**
         * Set previous chapter
         */
        public void GetPreviousChapter()
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
                    var newButton = Instantiate(noteButtonPrefab, noteButtonParent.transform);
                    int chap = i;
                    newButton.GetComponentInChildren<TMP_Text>().text = stringChapters[chap].title.TrimEnd() + ": " + note.highlightText;
                    
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
            newButton.GetComponentInChildren<TMP_Text>().text = stringChapters[chap].title.TrimEnd() + ": " + note.highlightText;
            
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

            scrollPage.text = scrollPage.text.Replace(selectedNote, clearNote);
            
            _userData.DeleteExistingNote(clearNote, currentChapter);
            selectedNote = "";
            
            noteDeleteButton.SetActive(false);
            DeleteNoteFormNotePage(clearNote);
        }
        
        /**
         * Allows to jump to a selected note in the book
         */
        private void ShowPageWithSelectedNote(Note note, int chapter)
        {
            SetChapter(chapter, true);
            scrollPage.ForceMeshUpdate();
            int charIndex = stringChapters[currentChapter].text.IndexOf(note.highlightText, StringComparison.Ordinal);

            TMP_Text text = scrollPage;

            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[charIndex];

            float charYPos = charInfo.bottomLeft.y;
            
            var rect = text.rectTransform.rect;
            float times = rect.height - (rect.height/2.0f - charYPos);
            float scrollPos = times/rect.height;

            scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = scrollPos;
        }
        
        public void ShowAllNotesOnPage()
        {
            // if chapter doesn't have notes
            if (_userData.notes[currentChapter].notes.Count == 0) return;

            string left = scrollPage.text;

            
            foreach (var note in _userData.notes[currentChapter].notes)
            {
                left = left.Replace(note.highlightText, _tagStart+note.highlightText+_tagEnd);
            }

            scrollPage.text = left;
        }
        
        public void SaveNoteOnPage()
        {
            var start = words[0].Item1;
            var end = words[0].Item2;
            
            foreach (var word in words)
            {
                start = (word.Item1 < start) ? word.Item1 : start;
                end = (word.Item2 > end) ? word.Item2 : end;
            }

            string oldText = scrollPage.text.Substring(start, end - start + 1);
            
            scrollPage.text = scrollPage.text.Replace(oldText, _tagStart + oldText + _tagEnd);

            words.Clear();
            
            var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();

            // Save new note and change its color
            Note note = new Note(oldText, inputNote.text);
            _userData.SaveNewNote(note, currentChapter);
            AddNewNoteToNotePage(note);

            // deactivate "Save note" button
            noteSaveButton.SetActive(false);
            // deactivate "Cancel note" button
            noteCancelButton.SetActive(false);
            // deactivate "Write mark" button
            noteWriteButton.SetActive(false);
            // deactivate "retelling" button
            noteRetellingButton.SetActive(false);

            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            inputNote.text = "";
            notePaper.SetActive(false);
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
            
            string noteForRetelling = scrollPage.text.Substring(start, end - start + 1);
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
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            inputNote.text = "";
            notePaper.SetActive(false);
            
            
            scrollPage.ForceMeshUpdate();
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
            
            // deactivate keyboard
            noteKeyboard.SetActive(false);
            // deactivate input field
            var inputNote = notePaper.GetComponentInChildren<TMP_InputField>();
            inputNote.text = "";
            notePaper.SetActive(false);
            
            scrollPage.ForceMeshUpdate();
        }
        
        public void KeyboardAndNotePaperVisibility()
        {
            noteKeyboard.SetActive(!noteKeyboard.activeSelf);

            if (notePaper.activeSelf)
            {
                notePaper.GetComponentInChildren<TMP_InputField>().text = "";
                notePaper.SetActive(false);
            }
            else
            {
                notePaper.SetActive(true);
            }
        }
        
        
        //=================== SEARCH ========================
        
        public void SearchWholeBook()
        {
            string text = searchInputField.text;
            List<List<string>> allSentences = new List<List<string>>();
            if (text.Length != 0)
            {
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
            }
            
            ShowSearchResults(allSentences);
        }

        private void ShowSearchResults(List<List<string>> allSentences)
        {
            DestroySearchResults();
            if (allSentences.Count == 0)
            {
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
            scrollPage.ForceMeshUpdate();
            int charIndex = stringChapters[currentChapter].text.IndexOf(sentence, StringComparison.Ordinal);
            
            TMP_Text text = scrollPage;
            TMP_CharacterInfo charInfo = text.textInfo.characterInfo[charIndex];

            float charYPos = charInfo.bottomLeft.y;

            var rect = text.rectTransform.rect;
            float times = rect.height - (rect.height/2.0f - charYPos);
            float scrollPos = times/rect.height;
            
            scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = scrollPos ;

            search.SetActive(false);
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
            _userData.fontSizeStandardBook = settings.currentFontSize;
            
            string json = JsonUtility.ToJson(_userData);
            
            using StreamWriter writer = new StreamWriter(_jsonFilePath);
            writer.Write(json);


            sceneController.ChangeScene();
        }
        
        public void SetFontSizeFromJson()
        {
            scrollPage.fontSize = settings.sizes[_userData.fontSizeScrollingPage];
            settings.currentFontSize = _userData.fontSizeScrollingPage;
            settings.SetButtonsVisibility();
            ShowPageNumber();
        }
    }
}

