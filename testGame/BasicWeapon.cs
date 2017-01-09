using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BasicWeapon : IWeapon {

    protected ViewController vc;
    protected object[] config;

    List<int> _ids = new List<int>();

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

    protected void DoStartAim( Vector3 pos )
    {
        _ids.Clear();
        _ids.Add(GetViewController().CreateAim(pos, GetConfig()));
    }

    protected void DoMoveAim(Vector3 pos)
    {
        GetViewController().DragAimsByIds( _ids.ToArray(), pos );
    }

    protected void DoKeepStartAim(Vector3 pos)
    {
        _ids.Add(GetViewController().CreateAim(pos, GetConfig()));
    }

    protected void DoEndAim()
    {
        GetViewController().ClearAimsByIds(_ids.ToArray());
    }

    object[] GetConfig()
    {
        return this.config;
    }
}
