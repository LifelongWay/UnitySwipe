using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SwipeX : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private int MAX_SIZE = 100;
    float speedTreshold = 1.14f;
    Vector3 panelPosition;
    Vector3 initialPosition;
    bool isActive;
    float startTime;
    float deltaTime;
    [SerializeField]public float changingTime = 0.35f ; // can be changed in inspector
    [SerializeField]public bool initialized = false;
    [SerializeField]public bool isBounded = true;
    List<GameObject> navElements = new List<GameObject>();
    public List<GameObject> Buffer;
    [SerializeField] public short navId = 0;

    void Start()
    {
        initialize();
    }
    public void initialize()
    {
        if(initialized)return; // don't let someone reinitialize -> will destroy our panel
        else initialized = true;
        
        

        RectTransform rectTransform = this.GetComponent<RectTransform>();

        // set pivot to (0.5,1)
        rectTransform.pivot = new Vector2(0f, 0.5f);
        // set size of panel
        rectTransform.sizeDelta = new Vector2(Screen.width * MAX_SIZE, Screen.height);
        // set position
        this.transform.position = new Vector2(0, Screen.height/2);


        // var-s to return back (can be used later)
        panelPosition = transform.position;
        initialPosition = panelPosition;


        // if navElements list isn't empty add elements to initialized panel
        foreach(GameObject obj in Buffer)add(obj);
    }

    public void add(GameObject item)
    {
        // prevent overflow
        if(navElements.Count == MAX_SIZE)
        {
            Debug.LogError("PanelOverflow prevention! \n Adding quiz denied due to size limit!");
            return;
        }
        // set object pivot to (0.5,1)
        item.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        // set full-screen size
        item.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        // setting position 
        item.transform.position = new Vector2( ((navElements.Count == 0)? 0 : navElements[navElements.Count-1].transform.position.x + Screen.width), Screen.height/2);
        // setting it a child of panel
        item.transform.SetParent(transform, true);
        // add to list
        navElements.Add(item);
    }
    public void remove(short id)
    {
        if(id > navElements.Count - 1)return; // outOfRange
    
        // get reference of current 
        GameObject obj = navElements[id];
        // remove from list
        navElements.RemoveAt(id);
        // Destroy 
        Destroy(obj);
        
    }
    public void insert(GameObject obj, int pos)
    {
        // prevent overflow
        if(navElements.Count == MAX_SIZE)
        {
            Debug.LogError("QuizPanelOverflow prevention! \n Adding quiz denied due to size limit! \n Possible solution: change MAX_SIZE in script!");
            return;
        }

        // add to list
        navElements.Insert(pos-1, obj);
        // setting position 
        obj.transform.position = new Vector2( ((pos == 1)? obj.transform.position.x : navElements[pos-2].transform.position.x + Screen.width), obj.transform.position.y);
        // setting it a child of panel
        obj.transform.SetParent(transform, true);

        // shift all rightmost kits to the right
        for(int j = pos; j < navElements.Count; j++)
        {
            navElements[j].transform.position = new Vector2( (navElements[j].transform.position.x + Screen.width), obj.transform.position.y);
        }
    }
    public void close()
    {
        // 1. destroy instantiated objects
        foreach(GameObject obj in navElements)Destroy(obj);
        // 2. clear references
        navElements.Clear();
        navId = 0;
        // 3. delete leaked textures in heap
        Resources.UnloadUnusedAssets();
    }
    public void reinitialize()
    {
        transform.position = initialPosition;
        panelPosition = initialPosition; 

        
        // adapting tutorial to device screen sizes manually
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        // scroll panel
        //rectTransform.sizeDelta = new Vector2(rectTransform.rect.width * navElements.Count, rectTransform.sizeDelta.y);
        for(int i=0; i < navElements.Count; i++)
        {
            // setting position of each slide of tutorial
            navElements[i].transform.position = new Vector2(navElements[i].transform.position.x + Screen.width*(i), navElements[i].transform.position.y);
            // setting it a child of panel
            navElements[i].transform.SetParent(transform, true);
        }
    }







    // SCROLL PART



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
        float difference = (data.position.x - data.pressPosition.x); // always getting deltaY from Y0 to Yposition
        // only drag horizontally !
        if((navId == 0 && difference > 0) || (isBounded && navId == navElements.Count - 1 && difference < 0))
        {
            // Debug.Log("Control Denied!");
            return;
        }
        transform.position = panelPosition + new Vector3(difference, 0, 0); // setting it, with keeping in mind Y0 before drag
        
        this.deltaTime++;
    }
    public void OnEndDrag(PointerEventData data)
    {    
        if(isActive)return;
        // getting swipe speed parameter
        
        float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
        deltaTime = Time.time - startTime;
            
        float swipeSpeed = Math.Abs(percentage/deltaTime);
       // Debug.Log("SwipeSpeed = " + swipeSpeed);
        
        //Debug.Log("|deltaPosition.Y| = " + Math.Abs(transform.position.y - panelPosition.y) );
        
        float deltaPositionTreshold;
        deltaPositionTreshold = Screen.width/2;
        

        // done

        // getting deltaPosition of panel
        float deltaPosition = Math.Abs(transform.position.x - panelPosition.x);

        // making swipe based on parameters
        if(percentage > 0 && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold)) // instead of false put treshold code
        {
            if(navId == navElements.Count - 1 && isBounded) return;
            // Right Swipe
            navId++;
            StartCoroutine(SmoothMove(transform.position, panelPosition - new Vector3(Screen.width, 0, 0), changingTime));
        }
        else if(percentage < 0 && (swipeSpeed >= speedTreshold || deltaPosition >= deltaPositionTreshold))
        {
            if(navId == 0)return;
            // Left Swipe
            navId--;
            StartCoroutine(SmoothMove(transform.position, panelPosition + new Vector3(Screen.width, 0, 0), changingTime));
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

        // if we swiped it all, remove object
        if(navId == navElements.Count && !isBounded)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void moveRight()
    {
        if(isActive || navId == navElements.Count - 1 && isBounded)return;
        navId++;
        StartCoroutine(SmoothMove(transform.position, panelPosition - new Vector3(Screen.width, 0, 0), changingTime));
    }
    public void moveLeft()
    {
        if(isActive || navId == 0 && isBounded)return;
        navId--;
        StartCoroutine(SmoothMove(transform.position, panelPosition + new Vector3(Screen.width, 0, 0), changingTime));
    }
    void updateNavigation()
    {
        foreach(GameObject obj in navElements)obj.SetActive(false);
        navElements[navId].SetActive(true);
    }

    public bool isFull()
    {
        if(navElements.Count == MAX_SIZE)return true;
        return false;
    }
}

