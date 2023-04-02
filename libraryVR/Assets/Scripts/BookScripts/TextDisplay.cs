using System.Collections;
using System.Collections.Generic;
using System.Text;
using BookScripts;
using HtmlAgilityPack;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;
using VersOne.Epub;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text leftPage;
    [SerializeField] private TMP_Text rightPage;

    private string _filePath;                   // book file path
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

        _currentChapter = 0;
        SetChapter(_currentChapter, false);


    }

    public List<string> GetTableOfContent()
    {
        List<string> tableOfContent = new List<string>();
        
        for (int i = 0; i < _stringChapters.Count; i++)
        {
            tableOfContent.Add(_stringChapters[i].title);
        }
        
        return tableOfContent;
    }

    // PAGES / CHAPTERS
    public void SetChapter(int index, bool byContent)
    {
        if (index < 0) return;
        if (index >= _stringChapters.Count) return;

        
        leftPage.text = _stringChapters[index].text;
        rightPage.text = _stringChapters[index].text;
        // Need it because in the first frame it's null 
        leftPage.ForceMeshUpdate();
        rightPage.ForceMeshUpdate();

        if (index >= _currentChapter || byContent == true)
        {
            _currentChapter = index;
            SetStartPages();
        }
        else if(index < _currentChapter)
        {
            _currentChapter = index;
            SetPrevPages();
        }
        
    }
    
    /**
     * Reset current page number/page count
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
    
    
    private void GetNextChapter()
    {
        SetChapter(_currentChapter + 1, false);
    }

    private void GetPreviousChapter()
    {
        SetChapter(_currentChapter - 1, false);
    }
    
    
    
}
