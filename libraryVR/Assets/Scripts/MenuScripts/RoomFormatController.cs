using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MenuScripts
{
    public class RoomFormatController : MonoBehaviour
    {
        public List<Texture2D> roomFormats;
        public List<string> roomFormatsNames;
        public List<GameObject> hints;

        public DecalProjector projector;
        public TMP_Text roomFormatName;
        public TMP_Text roomFormatNumber;

        private int _roomCount;
        [HideInInspector] public int roomIndex;


        void Start()
        {
            _roomCount = roomFormats.Count;
            roomIndex = 0;

            ShowRoomFormat();
        }
        
        private void ShowHint()
        {
            for (int i = 0; i < hints.Count; i++)
            {
                hints[i].SetActive(i == roomIndex);
            }
        }
        
        private void ShowRoomFormatNumber()
        {
            string text = (roomIndex+1) + " / " + _roomCount;
            roomFormatNumber.text = text;
        }

        private void ShowRoomFormat()
        {
            projector.material.SetTexture("Base_Map", roomFormats[roomIndex]);
            roomFormatName.text = roomFormatsNames[roomIndex];

            ShowRoomFormatNumber();
            ShowHint();
        }

        public void ShowNextFormat()
        {
            if (roomIndex == _roomCount - 1)
            {
                roomIndex = 0;
            }
            else
            {
                roomIndex++;
            }
            ShowRoomFormat();
        }
        
    }
}
