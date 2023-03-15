using System.IO;
using System.Text;

using UnityEngine;
using TMPro;
using VersOne.Epub;
using HtmlAgilityPack;
using VersOne.Epub.Options;


public class SelectableText : MonoBehaviour
{
    private TMP_InputField _inputField;
    private RectTransform rectTransform;
    private int _cursorIndex;
    private EpubBook book;
    

    void Start()
    {
        //_inputField = transform.GetComponent<TMP_InputField>();
        //rectTransform = _inputField.GetComponent<RectTransform>();
        //var rect = rectTransform.rect;
        //float width = rect.width;
        //float height = rect.height;
        
        //Debug.Log(width + " " + height);
   
        book = EpubReader.ReadBook("Assets/Scripts/451.epub");
        
        using StreamWriter file = new("WriteLines.txt");
        file.WriteLine($"Title: {book.Title}");
        file.WriteLine($"Author: {book.Author}");
        

        foreach (EpubTextContentFile textContentFile in book.ReadingOrder)
        {
            PrintTextContentFile(textContentFile, file);
        }
    }

    private void PrintTextContentFile(EpubTextContentFile textContentFile, StreamWriter file)
    {
        HtmlDocument htmlDocument = new();
        htmlDocument.LoadHtml(textContentFile.Content);
        StringBuilder sb = new();
        foreach (HtmlNode node in htmlDocument.DocumentNode.SelectNodes("//text()"))
        {
            sb.AppendLine(node.InnerText.Trim());
        }
        
        string contentText = sb.ToString();
        
        file.WriteLine(contentText);

    }
    
    
}
