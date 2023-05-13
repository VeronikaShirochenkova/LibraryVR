using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BookScripts
{
    public class SearchResultButtonController : MonoBehaviour
    {
        [SerializeField] private TMP_Text searchResult;

        
        public void CreateButton(string chapter, string text, string request)
        {
            string output = new string(text.Where(c => !char.IsControl(c)).ToArray());
            
            int idx = output.IndexOf(request, StringComparison.Ordinal);
            if (idx > 15)
            { 
                output = output[idx..];
            }
            
            
            searchResult.text = chapter + "\n" + output;
        }
    }
}
