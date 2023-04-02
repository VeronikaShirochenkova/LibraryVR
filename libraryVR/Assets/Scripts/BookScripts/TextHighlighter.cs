using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TextHighlighter : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayUI;                     // used like marker
    [SerializeField] private InputActionReference actionReference;      // trigger button action
    [SerializeField] private TMP_Text page;                             // ref to page which display text
    [SerializeField] private GameObject marker;                         // marker mesh
    
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
        if (value >= 0.6f && marker.activeSelf) // if button almost pressed -> start highlighted text
        {
            HighlightText();
        }
    }
    
    private void HighlightText()
    {
        RaycastResult hitInfo;
        bool b = rayUI.TryGetCurrentUIRaycastResult(out hitInfo);
        
        // if ray hit UI element
        if (b)
        {
            // get indices of first and last char on current page 
            var firstChar = page.textInfo.pageInfo[page.pageToDisplay - 1].firstCharacterIndex;
            var lastChar = page.textInfo.pageInfo[page.pageToDisplay - 1].lastCharacterIndex;
                
            for (var i = firstChar; i < lastChar; i++)
            {
                TMP_CharacterInfo cInfo = page.textInfo.characterInfo[i];
                
                // get word position of char
                Vector3 worldBottomLeft = transform.TransformPoint(cInfo.bottomLeft);
                Vector3 worldBottomRight = transform.TransformPoint(cInfo.bottomRight);
                Vector3 worldTopLeft = transform.TransformPoint(cInfo.topLeft);
                Vector3 worldTopRight = transform.TransformPoint(cInfo.topRight);
                    
                // check if ray hit this char
                bool res = PointInsideRectangle(hitInfo.worldPosition, worldBottomLeft, worldTopLeft, worldTopRight, worldBottomRight);
                
                if (res)
                {
                    // if char isn't highlighted yet
                    if (page.textInfo.meshInfo[cInfo.materialReferenceIndex].colors32[cInfo.vertexIndex] != _color)
                    {
                        // find all chars in this word
                        FindWordBorders(i, firstChar, lastChar);
                        if (_tail == -1 || _head == -1) continue;
                        
                        // highlight word
                        for (var j = _head; j <= _tail; j++)
                        {
                            TMP_CharacterInfo ch = page.textInfo.characterInfo[j];
                            page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex] = _color;
                            page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = _color;
                            page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = _color;
                            page.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = _color;
                            page.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        }
                        
                        // reset indices
                        _head = -1;
                        _tail = -1;
                    }
                }
            }
        }
    }

    private void FindWordBorders(int charIdx, int firstChar, int lastChar)
    {
        _head = charIdx;
        _tail = charIdx;
        
        if (page.text[charIdx] == ' ' || page.text[charIdx] == '\n') return;

        // find head
        for (var i = charIdx-1; i >= firstChar; i--)
        {
            if (page.text[i] == ' ' || page.text[i] == '\n') break;
            _head = i;
        }
        
        // find tail 
        for (var i = charIdx+1; i <= lastChar; i++)
        {
            if (page.text[i] == ' ' || page.text[i] == '\n') break;
            _tail = i;
        }
    }


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
