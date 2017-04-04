using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : MonoBehaviour {

    public Action<BarrelController, GameObject> OnHitEvent;

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
