using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace BookScripts
{
    public class TextHighlighter : MonoBehaviour
    {
        [SerializeField] private XRRayInteractor rayUI;                     // used like marker
        [SerializeField] private InputActionReference actionReference;      // trigger button action
        [SerializeField] private TMP_Text displayedPage;                    // ref to page which display text
        
        
        [SerializeField] private GameObject marker;                         // marker mesh
    
        // Notes
        [SerializeField] private GameObject noteButtonSave;
        [SerializeField] private GameObject noteButtonRemove;
        [SerializeField] private GameObject noteText;
        [SerializeField] private GameObject keyboard;

        public TextDisplay textDisplay;
    
        // User data
        private UserData _userData;

        // indices of first and last char of word which will be highlighted
        private int _head;
        private int _tail;
    
        // highlighting colors
        private Color _color;
        private Color _alfaColor;
        
        private void Start()
        {
            _head = -1;
            _tail = -1;
            _color = new Color32(255, 128, 0, 255);
            _alfaColor = new Color32(255, 128, 0, 0);

            // cause in 1st frame it's null
            displayedPage.ForceMeshUpdate();
        }
    
        private void Update()
        {
            float value = actionReference.action.ReadValue<float>();
        
            // if trigger button almost pressed -> start highlighted text
            if (value >= 0.99f && marker.activeSelf) 
            {
                HighlightText();
            }
        }

        /**
         * If char was highlighted, paint whole word
         */
        private void HighlightText()
        {
            RaycastResult hitInfo;
            bool result = rayUI.TryGetCurrentUIRaycastResult(out hitInfo);
        
            // if ray hit UI element
            if (result)
            {
                // get indices of first and last char on current page 
                var firstChar = displayedPage.textInfo.pageInfo[displayedPage.pageToDisplay - 1].firstCharacterIndex;
                var lastChar = displayedPage.textInfo.pageInfo[displayedPage.pageToDisplay - 1].lastCharacterIndex;

                int i = firstChar;
                while(i <= lastChar)
                {
                    TMP_CharacterInfo cInfo = displayedPage.textInfo.characterInfo[i];

                    // get word position of char
                    Vector3 worldBottomLeft = transform.TransformPoint(cInfo.bottomLeft);
                    Vector3 worldBottomRight = transform.TransformPoint(cInfo.bottomRight);
                    Vector3 worldTopLeft = transform.TransformPoint(cInfo.topLeft);
                    Vector3 worldTopRight = transform.TransformPoint(cInfo.topRight);
                    
                    // check if ray hit this char
                    bool res = PointInsideRectangle(hitInfo.worldPosition, 
                        worldBottomLeft, 
                        worldTopLeft, 
                        worldTopRight, 
                        worldBottomRight);
                
                    if (res)
                    {
                        // if char is a letter or punctuation mark and char isn't highlighted yet
                        if (char.IsLetter(cInfo.character) && displayedPage.textInfo.meshInfo[cInfo.materialReferenceIndex].colors32[cInfo.vertexIndex] != _color)
                        {
                            // find all chars in this word
                            FindWordBorders(i, firstChar, lastChar);
                            if (_tail == -1 || _head == -1)
                            {
                                // reset indices
                                _head = -1;
                                _tail = -1;
                                break;
                            }
                    
                            // highlight word on alfa page
                            HighlightWord(displayedPage, _color);
                            // highlight word on displayed page
                            ShowOnDisplayedPage();
                            
                            // Show buttons which save/remove new note
                            if (!noteButtonSave.activeSelf) {noteButtonSave.SetActive(true);}
                            if (!noteButtonRemove.activeSelf) {noteButtonRemove.SetActive(true);}
                            
                            // Show keyboard for typing text for note
                            if (!keyboard.activeSelf) {keyboard.SetActive(true);}
                            // Show input field for show text for note
                            if (!noteText.activeSelf) { noteText.SetActive(true); }
                        
                            // reset indices
                            _head = -1;
                            _tail = -1;
                        }
                    }

                    i++;
                }
            }
        }

        private void HighlightWord(TMP_Text page, Color color)
        {
            TMP_CharacterInfo ch;
            for (var i = _head; i <= _tail; i++)
            {
                ch = page.textInfo.characterInfo[i];
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 0] = color;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = color;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = color;
                page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = color;
            }
            page.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void ShowOnDisplayedPage()
        {
            textDisplay.words.Add((displayedPage.textInfo.characterInfo[_head].index, displayedPage.textInfo.characterInfo[_tail].index));
        }

        /**
         * Find word boundaries from the highlighted character
         */
        private void FindWordBorders(int charIdx, int start, int end)
        {
            

            var ch = displayedPage.textInfo.characterInfo[charIdx];
            _head = charIdx;
            _tail = charIdx;

            if (!char.IsLetter(ch.character))
            {
                _head = -1;
                _tail = -1;
                return;
            }
        
            // find head
            for (var i = charIdx; i >= start; i--)
            {
                if (!char.IsLetter(displayedPage.textInfo.characterInfo[i].character)) break;
                _head = i;
            }
        
            // find tail 
            for (var i = charIdx; i <= end; i++)
            {
                if (!char.IsLetter(displayedPage.textInfo.characterInfo[i].character)) break;
                _tail = i;
            }
        }

        /**
         * Check if the point is inside a given rectangle
         */
        private bool PointInsideRectangle(Vector3 point, Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight)
        {
            bool isInside = false;
            float angleSum = 0f;
        
            Vector3[] rectangleVertices = { bottomLeft, topLeft, topRight, bottomRight };

            for (int i = 0; i < rectangleVertices.Length; i++)
            {
                Vector3 v1 = rectangleVertices[i] - point;
                Vector3 v2 = rectangleVertices[(i + 1) % rectangleVertices.Length] - point;
                angleSum += Vector3.Angle(v1, v2);
            }

            if (Mathf.Approximately(angleSum, 360f))
            {
                isInside = true;
            }

            return isInside;
        }
    }
}
