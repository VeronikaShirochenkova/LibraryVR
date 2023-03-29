using UnityEngine;

namespace BookScripts
{
    public class Chapter
    {
        public string name;
        public string text;
        public int index;

        public Chapter(string text)
        {
            this.text = text;
            this.name = "";
            WriteChapterName();
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