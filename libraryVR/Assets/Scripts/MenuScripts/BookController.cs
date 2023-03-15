using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VersOne.Epub;

public class BookController : MonoBehaviour
{
    private List<EpubBook> _books;
    [SerializeField] private BookDisplay bookDisplay;
    private int _currentIndex;
    private string _directoryPath;
    private int _totalFiles;

    private void Start()
    {
        _directoryPath = Application.dataPath + "/Resources/Books";
        _totalFiles = 0;
        _books = new List<EpubBook>();
        
        TestEpubDirectory(_directoryPath);
        
        
        ChangeBook(0);
    }
    
    private void TestEpubDirectory(string directoryPath)
    {
        foreach (string subdirectoryPath in Directory.EnumerateDirectories(directoryPath))
        {
            TestEpubDirectory(subdirectoryPath);
        }
        foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.epub"))
        {
            TestEpubFile(filePath);
            _totalFiles++;
        }
    }
    
    private void TestEpubFile(string epubFilePath)
    {
        using EpubBookRef bookRef = EpubReader.OpenBook(epubFilePath);
        EpubBook book = EpubReader.ReadBook(epubFilePath);
        _books.Add(book);
    }

    public void ChangeBook(int change)
    {
        _currentIndex += change;

        if (_currentIndex < 0) _currentIndex = _books.Count - 1;
        else if (_currentIndex > _books.Count - 1) _currentIndex = 0;
        
        if (bookDisplay != null) bookDisplay.DisplayBook((EpubBook)_books[_currentIndex]);
    }
}
