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

        public DecalProjector projector;
        public TMP_Text roomFormatName;

        private int _roomCount;
        [HideInInspector] public int roomIndex;


        void Start()
        {
            _roomCount = roomFormats.Count;
            roomIndex = 0;

            ShowRoomFormat();
        }

        private void ShowRoomFormat()
        {
            projector.material.SetTexture("Base_Map", roomFormats[roomIndex]);
            roomFormatName.text = roomFormatsNames[roomIndex];
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
