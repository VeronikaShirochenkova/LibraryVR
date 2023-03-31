using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class TextHighlighter : MonoBehaviour
{
    [SerializeField] private XRRayInteractor rayUI;
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TMP_Text leftPage;
    //[SerializeField] private TMP_Text rightPage;
    [SerializeField] private GameObject book;


    private int head;
    private int tail;
    

    void Start()
    {
        // cause in 1st frame it's null
        leftPage.ForceMeshUpdate();
        head = -1;
        tail = -1;
    }
    
        // Update is called once per frame
    void Update()
    {
        float value = actionReference.action.ReadValue<float>();
        if (value >= 0.6f)
        {
            HighlightText();
        }


        //bool b = rayInteractor.TryGetCurrentUIRaycastResult(out hitInfo);

        //if (b)
        //{
        //    Debug.Log("get");
        //    for (int i = 0; i < text.text.Length; i++)
        //    {
        //        TMP_CharacterInfo cInfo = text.textInfo.characterInfo[i];
        //        Vector3 bottomLeft = cInfo.bottomLeft;
        //        Vector3 bottomRight = cInfo.bottomRight;
        //        Vector3 topLeft = cInfo.topLeft;
        //        Vector3 topRight = cInfo.topRight;
        //                    
        //        Vector3 worldBottomLeft = transform.TransformPoint(bottomLeft);
        //        Vector3 worldBottomRight = transform.TransformPoint(bottomRight);
        //        Vector3 worldTopLeft = transform.TransformPoint(topLeft);
        //        Vector3 worldTopRight = transform.TransformPoint(topRight);
        //        
        //        bool res = PointInsideRectangle(hitInfo.worldPosition, worldBottomLeft, worldTopLeft, worldTopRight, worldBottomRight);
        //        if (res)
        //        {
        //            Debug.Log("Change color");
        //            Color color = new Color32(255, 128, 0, 255);
        //            TMP_CharacterInfo firstChar = text.textInfo.characterInfo[i];
        //            text.textInfo.meshInfo[firstChar.materialReferenceIndex].colors32[firstChar.vertexIndex] = color;
        //            text.textInfo.meshInfo[firstChar.materialReferenceIndex].colors32[firstChar.vertexIndex + 1] = color;
        //            text.textInfo.meshInfo[firstChar.materialReferenceIndex].colors32[firstChar.vertexIndex + 2] = color;
        //            text.textInfo.meshInfo[firstChar.materialReferenceIndex].colors32[firstChar.vertexIndex + 3] = color;
        //            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        //            
        //            
        //        }
        //    }
        //}


    }
    
    private void HighlightText()
    {
        RaycastResult hitInfo;
        bool b = rayUI.TryGetCurrentUIRaycastResult(out hitInfo);

        if (b)
        {
            int frstChar = leftPage.textInfo.pageInfo[leftPage.pageToDisplay - 1].firstCharacterIndex;
            int lastChar = leftPage.textInfo.pageInfo[leftPage.pageToDisplay - 1].lastCharacterIndex;
                
            for (int i = frstChar; i < lastChar; i++)
            {
                TMP_CharacterInfo cInfo = leftPage.textInfo.characterInfo[i];

                Vector3 worldBottomLeft = transform.TransformPoint(cInfo.bottomLeft);
                Vector3 worldBottomRight = transform.TransformPoint(cInfo.bottomRight);
                Vector3 worldTopLeft = transform.TransformPoint(cInfo.topLeft);
                Vector3 worldTopRight = transform.TransformPoint(cInfo.topRight);
                    
                bool res = PointInsideRectangle(hitInfo.worldPosition, worldBottomLeft, worldTopLeft, worldTopRight, worldBottomRight);
                if (res)
                {
                    Color color = new Color32(255, 128, 0, 255);

                    if (leftPage.textInfo.meshInfo[cInfo.materialReferenceIndex].colors32[cInfo.vertexIndex] != color)
                    {
                        FindWordBorders(i, frstChar, lastChar);
                        if (tail == -1 || head == -1) continue;
                        for (int j = head; j <= tail; j++)
                        {
                            //Color color = new Color32(255, 128, 0, 255);
                            TMP_CharacterInfo ch = leftPage.textInfo.characterInfo[j];
                            leftPage.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex] = color;
                            leftPage.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 1] = color;
                            leftPage.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 2] = color;
                            leftPage.textInfo.meshInfo[ch.materialReferenceIndex].colors32[ch.vertexIndex + 3] = color;
                            leftPage.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                        }
                    }
                }
            }
        }
    }

    private void FindWordBorders(int charIdx, int frstChar, int lastChar)
    {
        head = charIdx;
        tail = charIdx;
        if (leftPage.text[charIdx] == ' ' || leftPage.text[charIdx] == '\n')
        {
            return;
        }

        for (int i = charIdx-1; i >= frstChar; i--)
        {
            if (leftPage.text[i] == ' ' || leftPage.text[i] == '\n')
            {
                break;
            }

            head = i;
        }
        // find tail 
        for (int i = charIdx+1; i <= lastChar; i++)
        {
            if (leftPage.text[i] == ' ' || leftPage.text[i] == '\n')
            {
                break;
            }

            tail = i;
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
