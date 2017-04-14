using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHP : MonoBehaviour {

    int _hp = 100;
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            if (value < 0) value = 0;
            _hp = value;
        }
    }

    public bool IsDead()
    {
        return HP <= 0;
    }

    public void Hit(int damage)
    {
        HP -= damage;
        if (HP < 0) HP = 0;
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
