using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace MenuScripts
{
    public class BookFormatController : MonoBehaviour
    {
        public List <Texture2D> bookFormats;
        public List <string> bookFormatsNames;
        
        public DecalProjector projector;
        public TMP_Text bookFormatName;
        public TMP_Text bookFormatNumber;

        private int _bookCount;
        [HideInInspector] public int bookIndex;
        
        void Start()
        {
            _bookCount = bookFormats.Count;
            bookIndex = 0;
            
            ShowBookFormat();
        }

        private void ShowBookFormatNumber()
        {
            string text = (bookIndex+1) + " / " + _bookCount;
            bookFormatNumber.text = text;
        }

        private void ShowBookFormat()
        {
            projector.material.SetTexture("Base_Map", bookFormats[bookIndex]);
            bookFormatName.text = bookFormatsNames[bookIndex];
            ShowBookFormatNumber();
        }
        
        public void ShowNextFormat()
        {
            if (bookIndex == _bookCount - 1)
            {
                bookIndex = 0;
            }
            else
            {
                bookIndex++;
            }
            ShowBookFormat();
        }
        
    }
}
