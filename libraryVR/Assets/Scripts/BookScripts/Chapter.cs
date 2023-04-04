using UnityEngine;

namespace BookScripts
{
    public class Chapter
    {
        public string text;
        public string title;
        public int index;

        public Chapter(string text)
        {
            this.text = text;
            this.title = "";
            GetTitle();
        }
        
        /**
         * Write title to "title" from chapter text
         */
        private void GetTitle()
        {
            for (int i = 0; i < text.Length; i++)
            {
                title += text[i];
                if (text[i] == '\n') break;
            }
        }

    }
}