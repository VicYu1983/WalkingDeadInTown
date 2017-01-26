using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    public abstract void Update();

    
}
