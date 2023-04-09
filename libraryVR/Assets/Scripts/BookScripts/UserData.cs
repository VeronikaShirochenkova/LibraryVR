using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace BookScripts
{
    [System.Serializable]
    public class UserData
    {
        public int fontSize;
        public int chaptersCount;
        public List<ChapterWrapper> notes;
        public List<PageWrapper> pages;



        public UserData(int chapsCount)
        {
            chaptersCount = chapsCount;
            notes = new List<ChapterWrapper>();
            pages = new List<PageWrapper>();
            
            for (var i = 0; i < chapsCount; i++)
            {
                notes.Add(new ChapterWrapper());
                notes[i].notes = new List<Note>();
                
                pages.Add(new PageWrapper());
                pages[i].pages = new List<int>();
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
    
    [System.Serializable]
    public class PageWrapper
    {
        public List<int> pages;
    }
}