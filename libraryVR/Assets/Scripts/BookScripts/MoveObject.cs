using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;

namespace BookScripts
{
    public class MoveObject : MonoBehaviour
    {
        public float moveDistance;              // Расстояние для перемещения объекта
        public float moveSpeed;                 // Скорость перемещения объекта
        public string axis;
        
        public bool needDeactivate;
        public GameObject objectToDeactivate;
        
        private Vector3 _startPosition;
        private Vector3 offset;
        public bool isMovingFromStart = false;      // Флаг для определения направления перемещения
        private bool isMoving = false;          // Флаг для определения, находится ли объект в движении
    

        public void MoveObjectOnClick()
        {
            offset = new Vector3();
            
            switch (axis)
            {
                case "x":
                    offset.x = 1;
                    break;
                case "y":
                    offset.y = 1;
                    break;
                case "z":
                    offset.z = 1;
                    break;
            }

            
            if (!isMoving)
            {
                isMovingFromStart = !isMovingFromStart;
                StartCoroutine(MoveObjectCoroutine());      
            }
        }

        IEnumerator MoveObjectCoroutine()
        {
            isMoving = true;                                        

            Vector3 destination;                                    

            if (isMovingFromStart)                                       
            {
                _startPosition = transform.localPosition;
                //destination = new Vector3(_startPosition.x, _startPosition.y, _startPosition.z - moveDistance); 
                destination = new Vector3(_startPosition.x, _startPosition.y, _startPosition.z) - offset * moveDistance; 
            }
            else                                                   
            {
                destination = _startPosition;
            }

            while (transform.localPosition != destination)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, moveSpeed * Time.deltaTime);
                yield return null;
            }

            isMoving = false;
            if (!isMovingFromStart && needDeactivate)
            {
                objectToDeactivate.SetActive(false);
            }
        }   
    }
}
