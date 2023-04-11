using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using VersOne.Epub;

namespace BookScripts
{
    public class EPubBookReader
    {
        private string _filePath;                   // book file path
        private static EpubBook _book;
        
        private List<Chapter> _stringChapters;
        private Dictionary<string, EpubTextContentFile> _htmlChapters;

        public EPubBookReader(string filePath)
        {
            _book = EpubReader.ReadBook(filePath);
            _stringChapters = new List<Chapter>();
            _htmlChapters = _book.Content.Html;
            GetChapters();
        }
        
        /**
         * Return all chapters as list of strings
         */
        public List<Chapter> GetAllChapters()
        {
            return _stringChapters;
        }

        /**
         * Get each chapter from epub file 
         */
        private void GetChapters()
        {
            foreach (string key in _htmlChapters.Keys)
            {
                ConvertHtmlChapterToString(_htmlChapters[key]);
            }
        }
        
        /**
         * Convert ech chapter from html to string
         */
        private void ConvertHtmlChapterToString(EpubTextContentFile htmlText)
        {
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(htmlText.Content);
            StringBuilder sb = new();
            sb.AppendLine(htmlDocument.DocumentNode.SelectSingleNode("//body").InnerText.Trim());

            string contentText = sb.ToString();
            if (contentText.Length != 0)
            {
                Chapter ch = new Chapter(contentText);
                if (ch.title.Length != contentText.Length)
                {
                    _stringChapters.Add(ch);
                    ch.index = _stringChapters.Count - 1;
                    ch.text += ".";
                }
            }
        }
    }
}