    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine;
    using TMPro;
    
    public class SwipeY : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public TextMeshProUGUI tmp_speed;
        // connect to settings 
        int ScreenHeight = Screen.height;
        Dropdown settings_treshold;

        //

        public enum tresholdType
        {
            zero = 0,
            whole = 1,
            half = 2,
            forth = 4,
            fifth = 5,
            _60Percents_ = 6
        }
        [SerializeField]public tresholdType treshold;
        [SerializeField]public float speedTreshold = 0f;
        [SerializeField]public float changingTime = 0.35f ; // can be changed in inspector
        [SerializeField]int numberOfObjects;
        int currentObject = 1;

        bool isActive;
        float startTime;
        float deltaTime;
        Vector3 panelPosition;

        [SerializeField]List<GameObject> child = new List<GameObject>();
        void Start()
        {
            isActive = false;
            RectTransform rect = this.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, child.Count*ScreenHeight);
            //set objects as children
            for(int i=0; i<child.Count; i++)child[i].transform.SetParent(this.transform, true);
            Debug.Log(transform.position);
            panelPosition = transform.position; // getting Y0 of panel
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(isActive)return;
            startTime = Time.time;
            Debug.Log(panelPosition);
        }

        public void OnDrag(PointerEventData data)
        {
            if(isActive)return;
            float difference = (data.position.y - data.pressPosition.y); // always getting deltaY from Y0 to Yposition
            // only drag vertically !
            transform.position = panelPosition + new Vector3(0, difference, 0); // setting it, with keeping in mind Y0 before drag
    
            this.deltaTime++;

    
        }

        public void OnEndDrag(PointerEventData data)
        {
             if(isActive)return;
            // getting swipe speed parameter
            float percentage = (data.pressPosition.y - data.position.y) / ScreenHeight;
            deltaTime = Time.time - startTime;
            float swipeSpeed = Math.Abs(percentage/deltaTime);
            Debug.Log("SwipeSpeed = " + swipeSpeed);
            tmp_speed.text = Convert.ToString(swipeSpeed);
            Debug.Log("|deltaPosition.Y| = " + Math.Abs(transform.position.y - panelPosition.y) );

            // done

            // getting treshold given in settings
            float deltaPositionTreshold;
            if(this.treshold == tresholdType.zero)deltaPositionTreshold = 0f;
            else if(this.treshold == tresholdType._60Percents_)deltaPositionTreshold = 0.6f*ScreenHeight;
            else if(this.treshold == tresholdType.whole)deltaPositionTreshold = 0.66f*ScreenHeight;
            else
            {
                deltaPositionTreshold = ScreenHeight/(((int)treshold)*1f);
            }
            Debug.Log("Current treshold is: " + deltaPositionTreshold);

            // done

            // getting deltaPosition of panel
            float deltaPosition = Math.Abs(transform.position.y - panelPosition.y);

            // making swipe based on parameters
            if(percentage > 0 && currentObject > 1 && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold) ) // instead of false put treshold code
            {
                // UP Swipe
                Debug.Log("ScreenHeight = " + ScreenHeight);
                currentObject--;
                StartCoroutine(SmoothMove(transform.position, panelPosition - new Vector3(0,ScreenHeight,0), changingTime));
            }
	    else if(percentage < 0 && currentObject < numberOfObjects && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold))
            {
                // Down Swipe
                currentObject++;
                StartCoroutine(SmoothMove(transform.position, panelPosition + new Vector3(0,ScreenHeight,0), changingTime));
            }
            else
            {
                StartCoroutine(SmoothMove(transform.position, panelPosition, changingTime));
            }

            // swipe
        }

        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float changingTime){
            isActive = true;
            float t = 0f;
            while(t <= 1.0){
                t += Time.deltaTime / (changingTime);
                transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            panelPosition = transform.position;
            isActive = false;

        }

    }
