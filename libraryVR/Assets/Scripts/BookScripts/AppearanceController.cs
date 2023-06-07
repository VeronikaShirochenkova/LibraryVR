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
        [SerializeField] private List<GameObject> meshes;
        [SerializeField] private List<GameObject> frames;


        private List<(Color, Color)> _assets;
        

        void Start()
        {
            frames[0].SetActive(true);
            
            Color pageColor1 = new Color(0.95f, 0.92f, 0.87f);
            Color pageColor2 = new Color(0.19f, 0.19f, 0.19f);
            Color pageColor3 = new Color(0.98f, 0.98f, 0.98f);
           
            Color textColor1 = new Color(0f, 0f, 0f, 0.98f);
            Color textColor2 = new Color(1.0f, 1.0f,1.0f,0.75f);
            Color textColor3 = new Color(0f, 0f, 0f, 0.98f);
            

            _assets = new List<(Color, Color)>
            {
                (pageColor1, textColor1),
                (pageColor2, textColor2),
                (pageColor3, textColor3)
            };
        }

        /**
         * Change page color / text color
         */
        public void ChangeAppearance(int index)
        {
            foreach (var mesh in meshes)
            {
                int idx = mesh.GetComponent<MeshRenderer>().materials.Length - 1;
                mesh.GetComponent<MeshRenderer>().materials[idx].color = _assets[index].Item1;
            }
            
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

            for (int i = 0; i < frames.Count; i++)
            {
                frames[i].SetActive(i == index);
            }
        }
    }
}

