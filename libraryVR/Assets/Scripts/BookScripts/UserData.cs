using System.Collections.Generic;

namespace BookScripts
{
    public class UserData
    {
        public int chaptersCount;
        public List<List<Note>> notes;

        public UserData(int chapsCount)
        {
            chaptersCount = chapsCount;
            notes = new List<List<Note>>();
            for (var i = 0; i < chapsCount; i++)
            {
                notes.Add(new List<Note>());
            }
        }

        public void SaveNewNote(Note note, int chapNumber)
        {
            notes[chapNumber].Add(note);
        }

    }

    public class Note
    {
        public int head;
        public int tail;
        public string text;

        public Note(int startChar, int lastChar, string note)
        {
            head = startChar;
            tail = lastChar;
            text = note;
        }
    }
}