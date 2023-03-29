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
    [SerializeField] private TMP_InputField leftPage;
    [SerializeField] private TMP_InputField rightPage;
    [SerializeField] private TMP_InputField contentPage;
    
    private string _filePath;                   // book file path
    //private static EpubBook _book;              
    
    //private List<string> _tableOfContent;
    private List<Chapter> _stringChapters;
    //private Dictionary<string, EpubTextContentFile> _htmlChapters;


    private TMP_Text _leftText;
    private TMP_Text _rightText;
    
    private int _pageCount;
    private int _currentPage;


    void Start()
    {
        //_filePath = PlayerPrefs.GetString("selectedBook");
        //_book = EpubReader.ReadBook(_filePath);
        
        // _filePath = Application.dataPath + "/Resources/Books/Fahrenheit451/Fahrenheit451_EN.epub";       // ok
         _filePath = Application.dataPath + "/Resources/Books/BraveNewWorld/BraveNewWorld_EN.epub";       // ok
        // _filePath = Application.dataPath + "/Resources/Books/NineteenEightyFour/1984_EN.epub";             // ok


        EPubBookReader reader = new EPubBookReader(_filePath);
        _stringChapters = reader.GetAllChapters();
        
        
        // get text component
        _leftText = leftPage.GetComponentInChildren<TMP_Text>();
        _rightText = rightPage.GetComponentInChildren<TMP_Text>();
        
        leftPage.text = _stringChapters[1].text;
        rightPage.text = _stringChapters[1].text;
        
        // Need it because in the first frame it's null 
        _leftText.ForceMeshUpdate();
        _rightText.ForceMeshUpdate();
        
        SetStartPages();
        

        // _book = EpubReader.ReadBook(_filePath);
        //
        // _stringChapters = new List<Chapter>();
        // _htmlChapters = _book.Content.Html;

        //GetChapters();
    }
    
    private void SetStartPages()
    {
        _currentPage = 1;
        _leftText.pageToDisplay = _currentPage;
        _pageCount = _leftText.textInfo.pageCount;

        if (_pageCount == 1)
        {
            rightPage.text = "";
            _rightText.pageToDisplay = 0;
        }
        else
        {
            _rightText.pageToDisplay = _currentPage + 1;
        }
    }
    
    public void PreviousPage()
    {
        if (_currentPage >= 3)
        {
            _leftText.pageToDisplay = _currentPage - 2;
            _rightText.pageToDisplay = _currentPage - 1;
            _currentPage -= 2;
        }
        else
        {
            GetPreviousChapter();
        }
    }
    
    public void NextPage()
    {
        int pageCount = (_pageCount % 2 == 0) ? _pageCount : _pageCount + 1;
        if (_currentPage <= pageCount - 3)
        {
            _leftText.pageToDisplay = _currentPage + 2;
            _rightText.pageToDisplay = _currentPage + 3;
            _currentPage += 2;
        }
        else
        {
            GetNextChapter();
        }
    }

    private void GetNextChapter()
    {
        Debug.Log("Get Next Chapter");
    }

    private void GetPreviousChapter()
    {
        Debug.Log("Get Previous Chapter");
    }
    
    
    
    
    
    
    
    
    
    
    
    
    

    // private void ShowTableOfContent()
    // {
    //     throw new System.NotImplementedException();
    // }


    // /**
    //  * go through the book get table of content
    //  */
    // void GetTableOfContent()
    // {
    //     using (EpubBookRef bookRef = EpubReader.OpenBook(_filePath))
    //     {
    //         foreach (EpubNavigationItemRef navigationItemRef in bookRef.GetNavigation())
    //         {
    //             PrintNavigationItem(navigationItemRef);
    //         }
    //     }
    // }
    //
    // /**
    //  * get parts of table of contents
    //  */
    // private void PrintNavigationItem(EpubNavigationItemRef navigationItemRef)
    // {
    //     _tableOfContent.Add(navigationItemRef.Title + '\n');
    //     foreach (EpubNavigationItemRef nestedNavigationItemRef in navigationItemRef.NestedItems)
    //     {
    //         PrintNavigationItem(nestedNavigationItemRef);
    //     }
    // }
    
    // /**
    //  * Go through all chapters
    //  */
    // private void GetChapters()
    // {
    //     foreach (string key in _htmlChapters.Keys)
    //     {
    //         ConvertHtmlChapterToString(_htmlChapters[key]);
    //     }
    // }
    //
    //
    // private void ConvertHtmlChapterToString(EpubTextContentFile htmlText)
    // {
    //     HtmlDocument htmlDocument = new();
    //     htmlDocument.LoadHtml(htmlText.Content);
    //     StringBuilder sb = new();
    //     sb.AppendLine(htmlDocument.DocumentNode.SelectSingleNode("//body").InnerText.Trim());
    //
    //     string contentText = sb.ToString();
    //     if (contentText.Length != 0)
    //     {
    //         Chapter ch = new Chapter(contentText);
    //         if (ch.name.Length != contentText.Length)
    //         {
    //             _stringChapters.Add(ch);
    //             ch.index = _stringChapters.Count - 1;
    //         }
    //     }
    // }

  
}
