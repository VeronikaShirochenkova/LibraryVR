using System.Collections.Generic;
using BookScripts;
using ScrollTextScripts;
using TabletScripts;
using UnityEngine;

namespace Other
{
    public class RoomController : MonoBehaviour
    {
        public List<GameObject> books;

        private int _bookID;
        void Start()
        {
            _bookID = PlayerPrefs.GetInt("selectedBookFormat");
            books[_bookID].SetActive(true);
        }

        public void CallSaveUserData()
        {
            switch (_bookID)
            {
                case 0:
                    books[_bookID].GetComponent<TextDisplay>().SaveUserData();
                    break;
                case 1:
                    books[_bookID].GetComponent<TabletTextDisplay>().SaveUserData();
                    break;
                case 2:
                    books[_bookID].GetComponent<ScrollTextDisplay>().SaveUserData();
                    break;
            }
        }
    }
}
