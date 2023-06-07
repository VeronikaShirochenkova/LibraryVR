using UnityEngine;

namespace Other
{
    public class PlayerPosition : MonoBehaviour
    {
        public Vector3 startPos;
        // Start is called before the first frame update
        void Start()
        {
            transform.localPosition = startPos;
            Debug.Log("set");
        }

    }
}
