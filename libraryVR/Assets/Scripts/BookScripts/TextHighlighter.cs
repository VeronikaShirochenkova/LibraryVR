using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace BookScripts
{
    public class TextHighlighter : MonoBehaviour
    {
        [SerializeField] private XRRayInteractor rayUI;                     // used like marker
        [SerializeField] private InputActionReference actionReference;      // trigger button action
        [SerializeField] private TMP_Text page;                             // ref to page which display text
        [SerializeField] private GameObject marker;                         // marker mesh
    
        // Notes
        [SerializeField] private GameObject noteButton;

        private TextDisplay _textDisplay;
    
        // User data
        private UserData _userData;

        // indices of first and last char of word which will be highlighted
        private int _head;
        private int _tail;
    
        // highlighting color
        private Color _color;


        private void Start()
        {
            _head = -1;
            _tail = -1;
            _color = new Color32(255, 128, 0, 255);

            // cause in 1st frame it's null
            page.ForceMeshUpdate();
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
                var firstChar = page.textInfo.pageInfo[page.pageToDisplay - 1].firstCharacterIndex;
                var lastChar = page.textInfo.pageInfo[page.pageToDisplay - 1].lastCharacterIndex;
                
                for (var i = firstChar; i <= lastChar; i++)
                {
                    TMP_CharacterInfo cInfo = page.textInfo.characterInfo[i];
                
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
                        if ((int)page.text[i] >= 33 && page.textInfo.meshInfo[cInfo.materialReferenceIndex].colors32[cInfo.vertexIndex] != _color)
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

                            TMP_CharacterInfo ch;
                        
                            // highlight word
                            for (var j = _head; j <= _tail; j++)
                            {
                                if ((int)page.text[j] >= 33)
                                {
                                    ch = page.textInfo.characterInfo[j];
                                    page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 0] = _color;
                                    page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = _color;
                                    page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = _color;
                                    page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = _color;
                                    page.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                                }
                            }

                            // Show button which save new note
                            if (!noteButton.activeSelf) {noteButton.SetActive(true);}
                        
                            // reset indices
                            _head = -1;
                            _tail = -1;
                        }
                    }
                }
            }
        }

        /**
         * Find word boundaries from the highlighted character
         */
        private void FindWordBorders(int charIdx, int start, int end)
        {
            _head = charIdx;
            _tail = charIdx;
        
            // if char isn't a letter or punctuation mark
            if ((int)page.text[charIdx] >= 0 && (int)page.text[charIdx] <= 32)
            {
                _head = -1;
                _tail = -1;
                return;
            }
        
            // find head
            for (var i = charIdx; i >= start; i--)
            {
                if ((int)page.text[i] >= 0 && (int)page.text[i] <= 32) break;
                _head = i;
            }
        
            // find tail 
            for (var i = charIdx; i <= end; i++)
            {
                if ((int)page.text[i] >= 0 && (int)page.text[i] <= 32) break;
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
