using UnityEngine;

namespace BookScripts
{
    public class Chapter
    {
        public string name;
        public string text;
        public string title;
        public int index;

        public Chapter(string text)
        {
            this.text = text;
            this.name = "";
            WriteChapterName();
            GetTitle();
        }

        private void GetTitle()
        {
            for (int i = 0; i < text.Length; i++)
            {
                title += text[i];
                if (text[i] == '\n') break;
            }
        }
        private void WriteChapterName()
        {
            foreach (char ch in text)
            {
                name += ch;
                if (ch == '\n')
                {
                    break;
                }
            }
        }

    }
}