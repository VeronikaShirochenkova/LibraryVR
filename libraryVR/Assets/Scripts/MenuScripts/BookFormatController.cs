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

        private int _sceneCount;
        [HideInInspector] public int sceneIndex;
        
        void Start()
        {
            _sceneCount = bookFormats.Count;
            sceneIndex = 0;
            
            ShowBookFormat();
        }

        private void ShowBookFormat()
        {
            projector.material.SetTexture("Base_Map", bookFormats[sceneIndex]);
            bookFormatName.text = bookFormatsNames[sceneIndex];
        }
        
        public void ShowNextFormat()
        {
            if (sceneIndex == _sceneCount - 1)
            {
                sceneIndex = 0;
            }
            else
            {
                sceneIndex++;
            }
            //sceneIndex = sceneIndex == _sceneCount - 1 ? 0 : sceneIndex++;
            ShowBookFormat();
        }
        
    }
}
