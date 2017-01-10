using UnityEngine;
using System.Collections;
using System;

public class AimController : MonoBehaviour {

    public int id;

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

    float currentSize = 0.0f;

    public void SetConfig( int maxAge, float size, float startSize, bool dragable, float speed, bool delay )
    {
        GetComponent<AgeCalculator>().DeadAge = maxAge;
        this.Size = size;
        this.Dragable = dragable;
        this.ExpandSpeed = speed;
        this.Delay = delay;
        this.currentSize = startSize;
    }

    public void SetPosition( Vector3 pos )
    {
        pos.z = 0;
        pos.y += 50;
        GetComponent<RectTransform>().position = pos + _offset;
    }
    
    void Start () {
        GetComponent<RectTransform>().localScale = new Vector3();
        GetComponent<AgeCalculator>().OnDeadEvent += OnDeadEvent;
    }
    
    private void OnDeadEvent(AgeCalculator obj)
    {
        GetComponent<AgeCalculator>().OnDeadEvent -= OnDeadEvent;
        Destroy(this.gameObject);
    }
    
    void Update () {
        SetSize();
    }

    void SetSize()
    {
        int currentAge = GetComponent<AgeCalculator>().CurrentAge;
        int maxAge = GetComponent<AgeCalculator>().DeadAge;
        currentSize += ExpandSpeed;
        if (currentSize >= Size) currentSize = Size;
        Vector3 s = GetComponent<RectTransform>().localScale;
        s.x = s.y = currentSize;
        GetComponent<RectTransform>().localScale = s;
    }
}
