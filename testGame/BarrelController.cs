using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour {

    public Action<BarrelController, GameObject> OnHitEvent;

    int _hp = 120;
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            if (_hp < 0) _hp = 0;
        }
    }

    public bool isDead
    {
        get
        {
            return _hp <= 0;
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (OnHitEvent != null)
            OnHitEvent(this, other.gameObject);
    }
}
