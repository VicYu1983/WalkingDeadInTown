using UnityEngine;
using System.Collections;
using System;

public class AimController : MonoBehaviour {

    public int GroupId;

    public float ExpandSpeed = 1.0f;
    public float Size = 1.0f;
    public bool Dragable = true;
    public bool Delay = false;

    Vector3 _offset = new Vector3();
    public Vector3 Offset
    {
        set
        {
            _offset = value;
        }
        get
        {
            return _offset;
        }
    }

    Vector2 currentSize;

    public void SetConfig( int maxAge, float size, float startSize, bool dragable, float speed, bool delay )
    {
        GetComponent<AgeCalculator>().DeadAge = maxAge;
        this.Size = size;
        this.Dragable = dragable;
        this.ExpandSpeed = speed;
        this.Delay = delay;
        // this.currentSize = startSize;
        this.currentSize = new Vector2
        (
            ori_size.x * startSize, ori_size.y * startSize
        );

        print(ori_size.ToString());
    }

    public void SetPosition( Vector3 pos )
    {
        pos.z = 0;
        pos.y += GameConfig.AimOffsetY;
        GetComponent<RectTransform>().position = pos + _offset;
    }
    
    Vector2 ori_size;

    void Start () {
       // GetComponent<RectTransform>().localScale = new Vector3();
       
        ori_size = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = new Vector2();
        print(ori_size.x);
    }
    /*
    private void OnDeadEvent(AgeCalculator obj)
    {
        GetComponent<AgeCalculator>().OnDeadEvent -= OnDeadEvent;
        Destroy(this.gameObject);
    }
    */
    void Update () {
        SetSize();
    }

    void SetSize()
    {
        int currentAge = GetComponent<AgeCalculator>().CurrentAge;
        int maxAge = GetComponent<AgeCalculator>().DeadAge;
        //  currentSize += ExpandSpeed;

        currentSize.x += ExpandSpeed * ori_size.x;
        currentSize.y += ExpandSpeed * ori_size.y;

        if (currentSize.x >= Size * ori_size.x ) currentSize.x = Size * ori_size.x;
        if (currentSize.y >= Size * ori_size.y ) currentSize.y = Size * ori_size.y;

        GetComponent<RectTransform>().sizeDelta = currentSize;
        /*
        int currentAge = GetComponent<AgeCalculator>().CurrentAge;
        int maxAge = GetComponent<AgeCalculator>().DeadAge;
        currentSize += ExpandSpeed;
        if (currentSize >= Size) currentSize = Size;
        Vector3 s = GetComponent<RectTransform>().localScale;
        s.x = s.y = currentSize;
        GetComponent<RectTransform>().localScale = s;
        */
    }
}
