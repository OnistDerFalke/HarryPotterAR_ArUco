using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayersSwipe : MonoBehaviour
    {
        public Transform[] optionsTransforms;
        public GameObject optionsBox;
        public int currentOptionID = 0;
        public float optionsOffset = 130f;
        public float swipeSpeed = 3f;
        public float snapSpeed = 5f;

        private Vector2 fingerDown;
        private Vector2 fingerUp;
        //private float swipeThreshold = 50f; 
        //private int currentPosition = 0;
        private float midDistance;

        private bool snapLock = false;

        void Start()
        {
            SetValues();
        }

        void Update()
        {
            if (Input.touchCount != 1) return;
            
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    snapLock = true;
                    fingerDown = touch.position;
                    fingerUp = touch.position;
                    break;
                case TouchPhase.Moved:
                {
                    fingerUp = touch.position;
                    var distance = fingerDown.x - fingerUp.x;
                    
                    if (optionsBox.transform.localPosition.x > 0)
                        optionsBox.transform.localPosition = new Vector3(
                            0f, optionsBox.transform.localPosition.y, optionsBox.transform.localPosition.z);
                    if (optionsBox.transform.localPosition.x < -(optionsTransforms.Length - 1) * optionsOffset)
                        optionsBox.transform.localPosition = new Vector3(
                            -(optionsTransforms.Length - 1) * optionsOffset, optionsBox.transform.localPosition.y, optionsBox.transform.localPosition.z);

                    if (optionsBox.transform.localPosition.x <= 0 && optionsBox.transform.localPosition.x >=
                        -(optionsTransforms.Length - 1) * optionsOffset)
                    {
                        optionsBox.transform.localPosition +=
                            new Vector3(-distance * swipeSpeed * Time.deltaTime, 0f, 0f);
                    }

                    break;
                }
                case TouchPhase.Ended:
                {
                    snapLock = false;
                    fingerUp = touch.position;
                    var currentPos = optionsBox.transform.localPosition.x;
                    var id = (int)Math.Abs(Math.Round(currentPos / optionsOffset));
                    StartCoroutine(SnapToPosition(id));
                    break;
                }
            }
        }

        IEnumerator SnapToPosition(int id)
        {
            var optionsBoxPos = optionsBox.transform.localPosition;
            var targetPosition = new Vector3(-id * optionsOffset, optionsBoxPos.y, optionsBoxPos.z);
            
            while (transform.position.x + id * optionsOffset != 0)
            {
                if (snapLock) break;
                optionsBox.transform.localPosition = Vector3.MoveTowards(
                    optionsBox.transform.localPosition, targetPosition, snapSpeed * Time.deltaTime);
                yield return null;
            }
            
            currentOptionID = id;
        }

        void SetValues()
        {
            for (var i = 0; i < optionsTransforms.Length; i++)
                optionsTransforms[i].localPosition = new Vector3(i * optionsOffset, 
                    optionsTransforms[i].localPosition.y, optionsTransforms[i].localPosition.z);

            optionsBox.transform.localPosition = Vector3.zero + 
                                                 new Vector3(
                                                     -currentOptionID * optionsOffset,
                                                     optionsBox.transform.localPosition.y,
                                                     optionsBox.transform.localPosition.z);
        }
    }
}



