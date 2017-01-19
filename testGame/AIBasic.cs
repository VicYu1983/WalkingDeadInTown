using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class AIBasic
{

    protected ViewController _vc;
    public ViewController ViewController
    {
        set
        {
            _vc = value;
        }
    }

    protected PlayerController _pc;
    public PlayerController PlayerController
    {
        set
        {
            _pc = value;
        }
    }

    public abstract void Update();

    
}
