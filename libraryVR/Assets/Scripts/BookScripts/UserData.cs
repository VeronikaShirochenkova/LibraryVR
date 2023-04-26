using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace BookScripts
{
    [System.Serializable]
    public class UserData
    {
        public int fontSizeStandardBook;
        public int fontSizeReadingTablet;
        public int fontSizeScrollingPage;

        public bool openedAsStandardBook;
        public bool openedAsReadingTablet;
        public bool openedAsScrollPage;
        
        public int chaptersCount;
        
        public int chapterBookmarkStandardBook;
        public int pageBookmarkStandardBook;
        
        public int chapterBookmarkTablet;
        public int pageBookmarkTablet;
        
        public int chapterBookmarkScrollPage;
        public int pageBookmarkScrollPage;
        
        public List<ChapterWrapper> notes;
        
        public List<PageWrapper> standardBookPages;
        public List<PageWrapper> tabletPages;
        public List<PageWrapper> scrollPages;



        public UserData(int chapsCount)
        {
            chaptersCount = chapsCount;

            chapterBookmarkStandardBook = 0;
            pageBookmarkStandardBook = 1;
            
            chapterBookmarkTablet = 0;
            pageBookmarkTablet = 1;
            
            chapterBookmarkScrollPage = 0;
            pageBookmarkScrollPage = 1;
            
            fontSizeStandardBook = 5;
            fontSizeReadingTablet = 5;
            fontSizeScrollingPage = 5;
            
            openedAsStandardBook = false;
            openedAsReadingTablet = false;
            openedAsScrollPage = false;
            
            notes = new List<ChapterWrapper>();
            
            standardBookPages = new List<PageWrapper>();
            tabletPages = new List<PageWrapper>();
            scrollPages = new List<PageWrapper>();
            
            for (var i = 0; i < chapsCount; i++)
            {
                notes.Add(new ChapterWrapper());
                notes[i].notes = new List<Note>();
                
                standardBookPages.Add(new PageWrapper());
                standardBookPages[i].pages = new List<int>();
                
                tabletPages.Add(new PageWrapper());
                tabletPages[i].pages = new List<int>();
                
                scrollPages.Add(new PageWrapper());
                scrollPages[i].pages = new List<int>();
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

        public string GetTextToNote(string note, int chapNumber)
        {
            foreach (var n in notes[chapNumber].notes)
            {
                if (n.highlightText == note)
                {
                    return n.note;
                }
            }

            return "";
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