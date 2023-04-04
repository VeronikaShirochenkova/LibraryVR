using System;
using TMPro;
using UnityEngine;

namespace BookScripts
{
    public class TextSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Text leftPage;
        [SerializeField] private TMP_Text rightPage;

        [SerializeField] private GameObject maxButton;
        [SerializeField] private GameObject minButton;

        private const float Min = 0.01f;                // min font size
        private const float Max = 0.02f;                // max font size
        private const float Offset = 0.001f;            // font resize step
        private const float Tolerance = 0.001f;         // tolerance for comparing floating numbers

        private void Start()
        {
            if (Math.Abs(leftPage.fontSize - Min) <= Tolerance) minButton.SetActive(false);
            if (Math.Abs(leftPage.fontSize - Max) <= Tolerance) maxButton.SetActive(false);
        }
        
        /**
         * Increases font size of displayed text
         */
        public void IncreaseFont()
        {
            // if font size is increased from the smallest -> show "min" button
            if (Math.Abs(leftPage.fontSize - Min) <= Tolerance)
            {
                minButton.SetActive(true);
            }
        
            // increase font size
            leftPage.fontSize += Offset;
            rightPage.fontSize += Offset;
        
            // if it's the biggest font size -> hide "max" button
            if (Math.Abs(leftPage.fontSize - Max) <= Tolerance)
            {
                maxButton.SetActive(false);
                leftPage.fontSize = Max;
                rightPage.fontSize = Max;
            }
        
            Debug.Log(leftPage.fontSize);
        }

        /**
         * Reduces font size of displayed text
         */
        public void ReduceFont()
        {
            // if font size is reduced from the biggest -> show "max" button
            if (Math.Abs(leftPage.fontSize - Max) <= Tolerance)
            {
                maxButton.SetActive(true);
            }
        
            // reduce font size
            leftPage.fontSize -= Offset;
            rightPage.fontSize -= Offset;
        
            // if it's the smallest font size -> hide "min" button
            if (Math.Abs(leftPage.fontSize - Min) <= Tolerance)
            {
                minButton.SetActive(false);
                leftPage.fontSize = Min;
                rightPage.fontSize = Min;
            }
        
            Debug.Log(leftPage.fontSize);
        }
    
    }
}
