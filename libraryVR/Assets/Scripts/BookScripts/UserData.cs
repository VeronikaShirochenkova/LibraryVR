using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace BookScripts
{
    [System.Serializable]
    public class UserData
    {
        public int chaptersCount;
        
         //public List<List<Note>> notes;
         public List<ChapterWrapper> notes;


        public UserData(int chapsCount)
        {
            chaptersCount = chapsCount;
            notes = new List<ChapterWrapper>();
            
            for (var i = 0; i < chapsCount; i++)
            {
                notes.Add(new ChapterWrapper());
                notes[i].notes = new List<Note>();
            }
        }

        public void SaveNewNote(Note note, int chapNumber)
        {
            notes[chapNumber].notes.Add(note);
        }

    }

    [System.Serializable]
    public class Note
    {
        //public int head;
        //public int tail;
        public string highlightText;
        public string note;

        public Note(string text, string note)
        {
            highlightText = text;
            this.note = note;
        }
    }
    
    [System.Serializable]
    public class ChapterWrapper
    {
        public List<Note> notes;
    }
}