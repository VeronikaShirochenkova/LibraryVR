using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using VersOne.Epub;

namespace MenuScripts
{
    public class BookController : MonoBehaviour
    {
        [SerializeField] private BookDisplay bookDisplay;
        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private BookFormatController bookFormatController;
        [SerializeField] private RoomFormatController roomFormatController;
        
        [SerializeField] private TMP_Text bookNumber;
        
        private List<EpubBook> _books;
        private List<EpubBook> _displayedBooks;

        private int _currentIndex;
        private string _directoryPath;

        private int _sceneID;
        private int _bookFormatID;


        private void Start()
        {
            _directoryPath = Application.dataPath + "/Resources/Books";
            
            _books = new List<EpubBook>();
            _displayedBooks = new List<EpubBook>();
            _currentIndex = 0;
            
            GetBooksInAllFolders(_directoryPath);
            ChangeDisplayedBook(0);
        }
        
        
        private void ShowBookNumber()
        {
            // string text = index + " / " + count;
            // bookNumber.text = text;
            string text = (_currentIndex+1) + " / " + _displayedBooks.Count;
            bookNumber.text = text;
        }
        
    
        /**
        * Go through all folders in the directory and find all books
        */
        private void GetBooksInAllFolders(string directoryPath)
        {
            foreach (string subdirectoryPath in Directory.EnumerateDirectories(directoryPath))
            {
                GetBooksInAllFolders(subdirectoryPath);
            }
        
            foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.epub"))
            {
                AddBook(filePath);
            }
        }
    
        /**
        * Create book object and add it into list of books
        */
        private void AddBook(string epubFilePath)
        {
            EpubBook book = EpubReader.ReadBook(epubFilePath);
            _books.Add(book);
            _displayedBooks.Add(book);
        }
    
        /**
        * Change book displayed in the main scroll menu
        */
        public void ChangeDisplayedBook(int change)
        {
            if (_currentIndex < 0) return;
            _currentIndex += change;
        
            // if it's first/last book -> change index correctly
            if (_currentIndex < 0) _currentIndex = _displayedBooks.Count - 1;
            else if (_currentIndex > _displayedBooks.Count - 1) _currentIndex = 0;
        
            if (bookDisplay != null) bookDisplay.DisplayBook((EpubBook)_displayedBooks[_currentIndex]);
            
            ShowBookNumber();
        }

        private void AddAllBooks()
        {
            _displayedBooks.Clear();
            foreach (var book in _books)
            {
                _displayedBooks.Add(book);
            }
        }
    
        /**
        * Display search result from search box in the main scroll menu
        */
        public void ShowSearchResult()
        {
            String inputStr = inputField.text;
        
            // if input field is empty -> display first book
            if (inputStr.Length == 0)
            {
                _currentIndex = 0;
                AddAllBooks();
                ChangeDisplayedBook(0);
                return;
            }
            
            _displayedBooks.Clear();
            
            foreach (var book in _books)
            {
                if (book.Title.Contains(inputStr, StringComparison.OrdinalIgnoreCase) || book.Author.Contains(inputStr, StringComparison.OrdinalIgnoreCase)) // if book title contains input string
                {
                    //_currentIndex = _books.IndexOf(book);
                    //ChangeDisplayedBook(0);
                    //break;
                    _displayedBooks.Add(book);
                }
            }


            _currentIndex = _displayedBooks.Count == 0 ? -1 : 0;
            if (_currentIndex == 0)
            {
                ChangeDisplayedBook(0);
            }
            else
            {
                bookDisplay.DisplayBook(null);
            }
            ShowBookNumber();
        }


        public void StartRead()
        {
            if (_displayedBooks.Count == 0)
            {
                return;
            }
            
            _sceneID = roomFormatController.roomIndex + 1;
            _bookFormatID = bookFormatController.bookIndex;
            
            PlayerPrefs.SetString("selectedBook", _displayedBooks[_currentIndex].FilePath);
            PlayerPrefs.SetInt("selectedBookFormat", _bookFormatID);
            PlayerPrefs.Save();
            
            Debug.Log("Selected book file path: " + PlayerPrefs.GetString("selectedBook"));
            
            ChangeScene();
        }

        private void ChangeScene()
        {
            //SceneManager.LoadScene(_sceneID);
            SceneTransition.SwitchToScene(_sceneID);
        }
    }
}
