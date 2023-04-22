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
        public List<PageWrapper> standardBookPages;
        public List<PageWrapper> tabletPages;



        public UserData(int chapsCount)
        {
            chaptersCount = chapsCount;
            fontSize = 5;
            notes = new List<ChapterWrapper>();
            standardBookPages = new List<PageWrapper>();
            tabletPages = new List<PageWrapper>();
            
            for (var i = 0; i < chapsCount; i++)
            {
                notes.Add(new ChapterWrapper());
                notes[i].notes = new List<Note>();
                
                standardBookPages.Add(new PageWrapper());
                standardBookPages[i].pages = new List<int>();
                
                tabletPages.Add(new PageWrapper());
                tabletPages[i].pages = new List<int>();
            }
        }

        public void SaveNewNote(Note note, int chapNumber)
        {
            notes[chapNumber].notes.Add(note);
        }

        public void DeleteExistingNote(string note, int chapNumber)
        {
            foreach (var n in notes[chapNumber].notes)
            {
                if (n.highlightText == note)
                {
                    notes[chapNumber].notes.Remove(n);
                    break;
                }
            }
            
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