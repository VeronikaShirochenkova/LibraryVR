using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VersOne.Epub;

public class BookDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text bookName;
    [SerializeField] private TMP_Text bookDescription;
    [SerializeField] private Image bookCover;

    public void DisplayBook(EpubBook book)
    {
        bookName.text = book.Title;
        bookDescription.text = book.Author;
        //bookCover.sprite = 
    }
}
