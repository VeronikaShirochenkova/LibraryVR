using TMPro;
using UnityEngine;

namespace BookScripts
{
    public class HighlightedButtonController : MonoBehaviour
    {
        [SerializeField] private TMP_Text highlighted;
        [SerializeField] private TMP_Text date;
        [SerializeField] private GameObject noteIndicator;
        

        public void CreateButton(Note note, string chapter)
        {
            highlighted.text = chapter + "\n" + note.highlightText;
            date.text = note.date.Replace("-"," / ");
            noteIndicator.SetActive(note.note.Length > 0);
        }
        
    }
}
