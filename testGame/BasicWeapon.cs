using UnityEngine;
using System.Collections;
using System;

public class BasicWeapon : IWeapon {

    protected ViewController vc;
    protected object[] config;

    public BasicWeapon(ViewController vc, object[] config )
    {
        this.vc = vc;
        this.config = config;
    }

    public virtual void StartAim(Vector3 pos)
    {
        throw new Exception("need to be override!");
    }
    public virtual void EndAim()
    {
        throw new Exception("need to be override!");
    }
    public virtual void MoveAim(Vector3 pos)
    {
        throw new Exception("need to be override!");
    }
    public virtual void KeepStartAim(Vector3 pos)
    {
        throw new Exception("need to be override!");
    }

    protected ViewController GetViewController()
    {
        return vc;
    }

    protected object[] GetConfig()
    {
        return this.config;
    }
}
