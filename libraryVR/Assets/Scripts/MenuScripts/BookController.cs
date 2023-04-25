using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MenuScripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using VersOne.Epub;

public class BookController : MonoBehaviour
{
    [SerializeField] private BookDisplay bookDisplay;
    [SerializeField] private TMP_InputField inputField;
    
    private List<EpubBook> _books;
    
    private int _currentIndex;
    private string _directoryPath;


    private void Start()
    {
        _directoryPath = Application.dataPath + "/Resources/Books";
        _books = new List<EpubBook>();
        
        GetBooksInAllFolders(_directoryPath);
        _currentIndex = 0;
        ChangeDisplayedBook(0);
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
        //using EpubBookRef bookRef = EpubReader.OpenBook(epubFilePath);
        EpubBook book = EpubReader.ReadBook(epubFilePath);
        _books.Add(book);
    }
    
    /**
     * Change book displayed in the main scroll menu
     */
    public void ChangeDisplayedBook(int change)
    {
        _currentIndex += change;
        
        // if it's first/last book -> change index correctly
        if (_currentIndex < 0) _currentIndex = _books.Count - 1;
        else if (_currentIndex > _books.Count - 1) _currentIndex = 0;
        
        if (bookDisplay != null) bookDisplay.DisplayBook((EpubBook)_books[_currentIndex]);
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
            ChangeDisplayedBook(0);
            return;
        }
        
        foreach (var book in _books)
        {
            if (book.Title.Contains(inputStr, StringComparison.OrdinalIgnoreCase)) // if book title contains input string
            {
                _currentIndex = _books.IndexOf(book);
                ChangeDisplayedBook(0);
                break;
            }
        }
    }


    public void StartRead()
    {
        PlayerPrefs.SetString("selectedBook", _books[_currentIndex].FilePath);
        PlayerPrefs.Save();
        Debug.Log("Selected book file path: " + PlayerPrefs.GetString("selectedBook"));
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
