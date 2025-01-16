    using System;
using System.IO;
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
    int ScreenHeight;  
    float childPosition; // keeps Y position for next kit that will be added
    Dropdown settings_treshold;
    [SerializeField] int Max_Items_Number = 100; // by default 100
    
    
    

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
    // [SerializeField]int numberOfObjects;
    [SerializeField]int currentObject = 1;
    [SerializeField]int lastVisitedObject;
    [SerializeField]int kitLimiter = 20;
    [SerializeField]public int loadNumber = 5; // how many kits will be loaded ahead

    
    bool isActive;
    float startTime;
    float deltaTime;
    Vector3 panelPosition;
    Vector3 initialPosition;

    [NonSerialized]public List<GameObject> child = new List<GameObject>();
    public List<GameObject> buffer = new List<GameObject>();
    public event EventHandler OnKitChanged;
    void Awake()
    {
        // NOTE: swipePanel MUST BE INITIALIZED!
        currentObject = 1;
        lastVisitedObject = 1;
        
        Resources.UnloadUnusedAssets();
        initialize();
        showAll();
    }



    public void initialize()
    {
        ScreenHeight = Screen.height;
        // set scrolling parameters
        isActive = false;
        RectTransform rect = this.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Max_Items_Number * ScreenHeight);
         
        panelPosition = transform.position; // getting Y0 of panel
        initialPosition = panelPosition;
    }


    /*
        ▗▄▄▖ ▗▄▄▖▗▄▄▖  ▗▄▖ ▗▖   ▗▖   
       ▐▌   ▐▌   ▐▌ ▐▌▐▌ ▐▌▐▌   ▐▌   
        ▝▀▚▖▐▌   ▐▛▀▚▖▐▌ ▐▌▐▌   ▐▌   
       ▗▄▄▞▘▝▚▄▄▖▐▌ ▐▌▝▚▄▞▘▐▙▄▄▖▐▙▄▄▖

    */
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(isActive)return;
        startTime = Time.time;
       // Debug.Log("Start time = " + startTime);
       // Debug.Log(panelPosition);
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
       // Debug.Log("SwipeSpeed = " + swipeSpeed);
        //Debug.Log("|deltaPosition.Y| = " + Math.Abs(transform.position.y - panelPosition.y) );

        // getting treshold given in settings
        float deltaPositionTreshold;
        if(this.treshold == tresholdType.zero)deltaPositionTreshold = 0f;
        else if(this.treshold == tresholdType._60Percents_)deltaPositionTreshold = 0.6f*ScreenHeight;
        else if(this.treshold == tresholdType.whole)deltaPositionTreshold = 0.66f*ScreenHeight;
        else
        {
            deltaPositionTreshold = ScreenHeight/(((int)treshold)*1f);
        }

        // getting deltaPosition of panel
        float deltaPosition = Math.Abs(transform.position.y - panelPosition.y);

        // making swipe based on parameters
        if(percentage > 0 && currentObject > 1 && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold) ) // instead of false put treshold code
        {
            // UP Swipe
            currentObject--; 
            OnKitChanged?.Invoke(this, EventArgs.Empty);
            StartCoroutine(SmoothMove(transform.position, panelPosition - new Vector3(0,ScreenHeight,0), changingTime));
        }
        else if(percentage < 0 && currentObject < child.Count && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold))
        {
            // Down Swipe
            currentObject++;
            OnKitChanged?.Invoke(this, EventArgs.Empty);
            StartCoroutine(SmoothMove(transform.position, panelPosition + new Vector3(0,ScreenHeight,0), changingTime));
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelPosition, changingTime));
        }
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

/*
        ▗▖  ▗▖ ▗▄▖ ▗▄▄▄▖▗▖  ▗▖
        ▐▛▚▞▜▌▐▌ ▐▌  █  ▐▛▚▖▐▌
        ▐▌  ▐▌▐▛▀▜▌  █  ▐▌ ▝▜▌
        ▐▌  ▐▌▐▌ ▐▌▗▄█▄▖▐▌  ▐▌
*/
    public void showAll()
    {
        // clean panel if something left
        //clear();
        
        // set current object to lastVisited
        currentObject = 1;
        
        // add to panel objects attached

        dropBuffer();
    }

 
  
/*
         ▗▄▄▖ ▗▄▖ ▗▖  ▗▖▗▖  ▗▖ ▗▄▖ ▗▖  ▗▖
        ▐▌   ▐▌ ▐▌▐▛▚▞▜▌▐▛▚▞▜▌▐▌ ▐▌▐▛▚▖▐▌
        ▐▌   ▐▌ ▐▌▐▌  ▐▌▐▌  ▐▌▐▌ ▐▌▐▌ ▝▜▌
        ▝▚▄▄▖▝▚▄▞▘▐▌  ▐▌▐▌  ▐▌▝▚▄▞▘▐▌  ▐▌
*/
    public void addKit(GameObject kit)
    {
        if(kit == null)return;
        // set transform of new adding kit before it becomes child of panel        
        kit.transform.position = new Vector2(Screen.width/2, (child.Count == 0)? (2778f/2778f*Screen.height) : (child[child.Count - 1].transform.position.y - Screen.height) );
        // set object as child of swipable panel
        kit.transform.SetParent(this.transform, true);
        // add it immediately to main list of child kits
        child.Add(kit);
    }


    public void bufferAll()
    {
        foreach(GameObject kit in child)
        {
            kit.SetActive(false);
            buffer.Add(kit);
        }
        child.Clear();
        Debug.Log("BUFFERED ALL!");
    }

    public void dropBuffer()
    {
        foreach(GameObject bufferedItem in buffer) addKit(bufferedItem);
        buffer.Clear();
        Debug.Log("DROPPED BUFFER");
    }



    public void clear()
    {
        // delete all children
        foreach(GameObject obj in child)
        {
            Destroy(obj);
        }
        child.Clear();
        Debug.Log("Cleaning panel");

        Resources.UnloadUnusedAssets();
        // set panel to its initial position
        transform.position = initialPosition;
        panelPosition = transform.position;

        // set current object to 1
        currentObject = 1;
    }

    public void Cleaner()
    {
        // cleaner will clean all from [0; N-Limiter]
        
        if(child.Count <= kitLimiter)return;
        
        int initialNumber = child.Count;

        // destroy part of top to satisfy limiter
        for(int i=0; i < initialNumber - kitLimiter; i++)
        {
            Destroy(child[0]);
            child[0] = null;
            child.RemoveAt(0);
        }
        
        // unloadUnusedAssets
        Resources.UnloadUnusedAssets();
        
        // set panel and remaining objects to initialPosition as if we changed filter
        transform.position =  initialPosition;
        panelPosition = transform.position;

        for(int i=0; i < child.Count; i++)
        {
            Debug.Log("moving " + i + "th kit with position: "  + child[i].transform.position);
            //set transform of new adding kit before it becomes child of panel
            Debug.Log("ScreeenHeight new: " + Screen.height);
            child[i].transform.position = new Vector2(Screen.width/2, (i == 0)? (2778f/2778f*Screen.height ) : (child[i - 1].transform.position.y - Screen.height) );
        }
        // Unload texture remainders from memory leak
        Resources.UnloadUnusedAssets();


        // newCurrentObject = currentObject - (initialNumber - kitLimiter) 
        currentObject = currentObject - (initialNumber - kitLimiter);
        

        // adjust panel position to show newCurrentObject
        transform.position = new Vector2 (transform.position.x, transform.position.y +  Screen.height * (currentObject-1));       
        panelPosition = transform.position;
    }
}

