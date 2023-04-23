using System.Text.RegularExpressions;
using ScrollTextScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

namespace ScrollView
{
    public class ScrollTextHighlighter : MonoBehaviour
    {
        [Header("UI rays")]
        [SerializeField] private XRRayInteractor markerRay;                     // used like marker
        [SerializeField] private XRRayInteractor handRay;                     // used like marker
        [SerializeField] private GameObject pencil;                         // marker mesh
        
        [Header("XR Controller")]
        [SerializeField] private InputActionReference actionReference;      // trigger button action
        

        [Header("Book")]
        public ScrollTextDisplay textDisplay;
        [SerializeField] private TMP_Text page;                    // ref to page which display text
        [SerializeField] private ScrollRect scrollView;
        public GameObject content;
        
        
        [Header("Note-taking tools")]
        [SerializeField] private GameObject noteSaveButton;
        [SerializeField] private GameObject noteCancelButton;               // cancel orange word selection
        [SerializeField] private GameObject noteWriteButton;
        [SerializeField] private GameObject noteDeleteButton;               // delete existing note
        [SerializeField] private GameObject noteRetellingButton;

        // indices of first and last char of word which will be highlighted
        private int _head;
        private int _tail;

        private int _startIndex;
        private int _endIndex;
    
        // highlighting colors
        private Color _color;
        
        private Regex _tagsRegex;
        private MatchCollection _tagsMatches;
        
        void Start()
        {
            _head = -1;
            _tail = -1;
            _color = new Color32(0, 105, 203, 251);
            _tagsRegex = new Regex(@"<font=""Brass Mono SDF""><mark=#767EE190>(.*?)<\/mark>", RegexOptions.Singleline);
            
            page.ForceMeshUpdate();
        }


        void Update()
        {
            float value = actionReference.action.ReadValue<float>();

            // if trigger button almost pressed -> start highlighted text
            if (value >= 0.99f) 
            {
                GetFirstAndLastVisibleChar();
                
                if (pencil.activeSelf)
                {
                    if (noteDeleteButton.activeSelf)
                    {
                        textDisplay.selectedNote = "";
                        noteDeleteButton.SetActive(false);
                    } 
                    HighlightText();  
                }
                else
                {
                    InteractWithNote();
                }
            }
        }
        
        /**
         * Checks if there was an attempt to interact with an existing note;
         * If so, displays the attached text;
         * Also allows to delete this note;
         */
        private void InteractWithNote()
        {
            RaycastResult handHitInfo;
            bool result = handRay.TryGetCurrentUIRaycastResult(out handHitInfo);
            
            _tagsMatches = _tagsRegex.Matches(page.text);
            
            // if ray hit UI element
            if (result)
            {
                // get indices of first and last char on current page 
                var firstChar = _startIndex;
                var lastChar = _endIndex;
                
                
                int i = firstChar;
                while(i <= lastChar)
                {
                    
                    TMP_CharacterInfo cInfo = page.textInfo.characterInfo[i];

                    // get word position of char
                    Vector3 worldBottomLeft = transform.TransformPoint(cInfo.bottomLeft);
                    Vector3 worldBottomRight = transform.TransformPoint(cInfo.bottomRight);
                    Vector3 worldTopLeft = transform.TransformPoint(cInfo.topLeft);
                    Vector3 worldTopRight = transform.TransformPoint(cInfo.topRight);
                    
                    // check if ray hit this char
                    bool res = PointInsideRectangle(handHitInfo.worldPosition, 
                        worldBottomLeft, 
                        worldTopLeft, 
                        worldTopRight, 
                        worldBottomRight);

                    if (res)
                    {
                        if (ClickOnNote(cInfo))
                        {
                            break;
                        }

                        if (noteDeleteButton.activeSelf)
                        {
                            textDisplay.selectedNote = "";
                            noteDeleteButton.SetActive(false);
                        }
                        break;
                    }

                    i++;
                }
            }
            else
            {
                if (noteDeleteButton.activeSelf)
                {
                    textDisplay.selectedNote = "";
                    noteDeleteButton.SetActive(false);
                }
            }
            
        }
        
        /**
         * Checks if the selected character is in the selection
         */
        private bool ClickOnNote(TMP_CharacterInfo cInfo)
        {
            foreach (Match tagsMatch in _tagsMatches)
            {
                int startIndex = tagsMatch.Index;
                int endIndex = startIndex + tagsMatch.Length - 1;
                
                if (cInfo.index > startIndex && cInfo.index < endIndex)
                {
                    if (pencil.activeSelf)
                    {
                        return true;
                    }
                    textDisplay.selectedNote = page.text.Substring(startIndex, endIndex - startIndex + 1);
                    noteDeleteButton.SetActive(true);
                    return true;
                }
            }

            return false;
        }
        
        /**
         * If char was highlighted, paint whole word
         */
        private void HighlightText()
        {
            RaycastResult hitInfo;
            bool result = markerRay.TryGetCurrentUIRaycastResult(out hitInfo);
            
            _tagsMatches = _tagsRegex.Matches(page.text);

            // if ray hit UI element
            if (result)
            {
                // get indices of first and last char on current page 
                var firstChar = _startIndex;
                var lastChar = _endIndex;

                int i = firstChar;
                while(i < lastChar)
                {
                    
                    TMP_CharacterInfo cInfo = page.textInfo.characterInfo[i];

                    if (!cInfo.isVisible)
                    {
                        i++;
                        continue;
                    }

                    // get word position of char
                    Vector3 worldBottomLeft = content.transform.TransformPoint(cInfo.bottomLeft);
                    Vector3 worldBottomRight = content.transform.TransformPoint(cInfo.bottomRight);
                    Vector3 worldTopLeft = content.transform.TransformPoint(cInfo.topLeft);
                    Vector3 worldTopRight = content.transform.TransformPoint(cInfo.topRight);
                    
                    // check if ray hit this char
                    bool res = PointInsideRectangle(hitInfo.worldPosition, 
                        worldBottomLeft, 
                        worldTopLeft, 
                        worldTopRight, 
                        worldBottomRight);
                    
                    if (res)
                    {
                        // if char is a letter or punctuation mark and char isn't highlighted yet
                        if (char.IsLetterOrDigit(cInfo.character) && page.textInfo.meshInfo[cInfo.materialReferenceIndex].colors32[cInfo.vertexIndex] != _color)
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
                            HighlightWord(page, _color);
                            // add word in list 
                            AddWordInList();
                            
                            // Show buttons which save/remove new note
                            if (!noteSaveButton.activeSelf)
                            {
                                noteSaveButton.SetActive(true);
                            }
                            
                            if (!noteCancelButton.activeSelf)
                            {
                                noteCancelButton.SetActive(true);
                            }
                            
                            if (!noteWriteButton.activeSelf)
                            {
                                noteWriteButton.SetActive(true);
                            }
                            //Show button which start retelling this note
                            if (!noteRetellingButton.activeSelf)
                            {
                                noteRetellingButton.SetActive(true);
                            }
                            
                        
                            // reset indices
                            _head = -1;
                            _tail = -1;
                        }
                    }

                    i++;
                }
            }
        }
        
        private void AddWordInList()
        {
            textDisplay.words.Add((page.textInfo.characterInfo[_head].index, page.textInfo.characterInfo[_tail].index));
        }
        
        
        // ================ TECHNICAL FUNCTION =========================
        
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
        
        private void GetFirstAndLastVisibleChar()
        {
            var position = scrollView.transform.position;
            var rect = scrollView.viewport.rect;
                    
            float top = scrollView.viewport.TransformPoint(new Vector3(0,position.y + rect.yMax, 0)).y;
            float bottom = scrollView.viewport.TransformPoint(new Vector3(0,position.y + rect.yMin, 0)).y;
                    
                    
            int firstVisibleCharacterIndex = 0;
            int lastVisibleCharacterIndex = 0;

            bool findStart = false;
                    
            for (int i = 0; i < page.textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = page.textInfo.characterInfo[i];
                        
                if (charInfo.isVisible)
                {
                    Vector3 worldBottomLeft = content.transform.TransformPoint(charInfo.bottomLeft);
                    Vector3 worldTopLeft = content.transform.TransformPoint(charInfo.topLeft);
                            
                    if (!findStart && worldTopLeft.y < top)
                    {
                        findStart = true;
                        firstVisibleCharacterIndex = i;
                    }

                    if (worldBottomLeft.y > bottom)
                    {
                        lastVisibleCharacterIndex = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            _startIndex = firstVisibleCharacterIndex;
            _endIndex = lastVisibleCharacterIndex;
        }
        
        private void HighlightWord(TMP_Text pageRef, Color color)
        {
            TMP_CharacterInfo ch;
            for (var i = _head; i <= _tail; i++)
            {
                ch = pageRef.textInfo.characterInfo[i];
                pageRef.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 0] = color;
                pageRef.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = color;
                pageRef.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = color;
                pageRef.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = color;
            }
            pageRef.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
        
        /**
         * Find word boundaries from the highlighted character
         */
        private void FindWordBorders(int charIdx, int start, int end)
        {

            var ch = page.textInfo.characterInfo[charIdx];
            _head = charIdx;
            _tail = charIdx;

            if (!char.IsLetterOrDigit(ch.character))
            {
                _head = -1;
                _tail = -1;
                return;
            }
        
            // find head
            for (var i = charIdx; i >= start; i--)
            {
                if (!char.IsLetterOrDigit(page.textInfo.characterInfo[i].character)) break;
                _head = i;
            }
        
            // find tail 
            for (var i = charIdx; i <= end; i++)
            {
                if (!char.IsLetterOrDigit(page.textInfo.characterInfo[i].character)) break;
                _tail = i;
            }
        }
    }
}
