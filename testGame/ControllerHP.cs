using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerHP : MonoBehaviour {

    public Text Txt_hp;

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
            Txt_hp.text = "HP:" + _hp;
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
    
}
