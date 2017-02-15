using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using VicScript.WongWeaponSystem;

public abstract class AIBasic
{
    public ViewController ViewController
    {
        get;set;
    }
    
    public PlayerController PlayerController
    {
        get;set;
    }

    public WongWeaponController WongWeaponController
    {
        get;set;
    }

    public abstract void Update();

    
}
