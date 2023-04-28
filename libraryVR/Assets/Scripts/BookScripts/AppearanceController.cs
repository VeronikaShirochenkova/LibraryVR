using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BookScripts
{
    public class AppearanceController : MonoBehaviour
    {
        [Header("Pages")] 
        [SerializeField] private TMP_Text leftPage;
        [SerializeField] private TMP_Text rightPage;
        [SerializeField] private TMP_Text leftPageNumber;
        [SerializeField] private TMP_Text rightPageNumber;
        [SerializeField] private Material material;

        
        private List<(Color, Color)> _assets;

        void Start()
        {

            //Color pageColor1 = new Color(0.243f, 0.234f, 0.223f);
            Color pageColor1 = new Color(0.95f, 0.92f, 0.87f);
            //Color pageColor2 = new Color(0.49f, 0.49f, 0.49f);
            Color pageColor2 = new Color(0.19f, 0.19f, 0.19f);
            //Color pageColor3 = new Color(0.250f, 0.250f, 0.250f);
            Color pageColor3 = new Color(0.98f, 0.98f, 0.98f);
           
            //Color textColor1 = new Color(63, 63, 63, 255);
            Color textColor1 = new Color(0f, 0f, 0f, 0.98f);
            //Color textColor2 = new Color(255, 255,255,190);
            Color textColor2 = new Color(1.0f, 1.0f,1.0f,0.75f);
            //Color textColor3 = new Color(0, 0, 0, 250);
            Color textColor3 = new Color(0f, 0f, 0f, 0.98f);
            

            _assets = new List<(Color, Color)>
            {
                (pageColor1, textColor1),
                (pageColor2, textColor2),
                (pageColor3, textColor3)
            };
        }

        public void ChangeAppearance(int index)
        {
            material.color = _assets[index].Item1;

            if (leftPage)
            {
                leftPage.color = _assets[index].Item2;
            }

            if (rightPage)
            {
                rightPage.color = _assets[index].Item2;
            }

            if (leftPageNumber)
            {
               leftPageNumber.color = _assets[index].Item2; 
            }

            if (rightPageNumber)
            {
                rightPageNumber.color = _assets[index].Item2;
            }
            
        }
    }
}
