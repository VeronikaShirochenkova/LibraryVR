using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class RoomController : MonoBehaviour
    {
        public List<GameObject> rooms;
        void Start()
        {
            int roomID = PlayerPrefs.GetInt("selectedRoom");
            rooms[roomID].SetActive(true);
        }
    }
}
