using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VersOne.Epub;

namespace MenuScripts
{
    public class BookDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text bookName;
        [SerializeField] private TMP_Text bookDescription;


        [Header("Default book cover")]
        public Texture2D defaultTexture;
        public DecalProjector projector;
        private Texture2D bookCoverImage;
        



        public void DisplayBook(EpubBook book)
        {
            bookName.text = book.Title;
            bookDescription.text = book.Author;
            
            LoadBookCoverImage(book);
            
        }

        private void LoadBookCoverImage(EpubBook book)
        {
             bookCoverImage = new Texture2D(1,1);
             if (book.CoverImage == null || book.CoverImage.Length <= 3)
             {
                 projector.material.SetTexture("Base_Map", defaultTexture);
                 return;
             }
             
             //     decalMaterial.SetTexture("_BaseMap", tex);
            //     return;
            // }
            //
            bookCoverImage.LoadImage(book.CoverImage);
            projector.material.SetTexture("Base_Map", bookCoverImage);
            
            // tex.LoadImage(book.CoverImage);
            // //_bookImage = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width/2, tex.height/2));
            // _bookCover = tex;
        }
    
    }
}
