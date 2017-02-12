using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using VicScript.WongWeaponSystem;

public class BasicWeapon : IWeapon {

    protected AimViewController vc;
    protected object[] config;
    public PlayerController owner;
    public List<PlayerController> enemys = null;

    List<int> _ids = new List<int>();

    public BasicWeapon(PlayerController owner, AimViewController vc, object[] config )
    {
        this.vc = vc;
        this.owner = owner;
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

    public virtual void AimOnce(Vector3 pos)
    {
        throw new Exception("need to be override!");
    }

    public bool IsBlade()
    {
        return (bool)GetConfig()[12];
    }

    public virtual void Update()
    {
        keepShootTime++;
        if( IsKeepAim())
        {
            DoKeepStartAim(startShootingPos);
        }
    }

    protected AimViewController GetViewController()
    {
        return vc;
    }

    Vector3 startShootingPos;

    protected void DoStartAim( Vector3 pos )
    {
        if ( IsDragable())
        {
            Shooting(pos);
        }
    }

    protected void DoMoveAim(Vector3 pos)
    {
        GetViewController().DragAimsByIds( _ids.ToArray(), pos );
    }

    protected void DoKeepStartAim(Vector3 pos)
    {
        _ids.Add(GetViewController().CreateAim(owner, pos, GetConfig()));
    }

    protected void DoEndAim()
    {
        if(IsClearWhenRelease())
            GetViewController().ClearAimsByIds(_ids.ToArray());
    }

    protected void DoAimOnce(Vector3 pos)
    {
        if( !IsDragable() )
        {
            Shooting( pos );
        }
    }

    void Shooting( Vector3 pos )
    {
        _ids.Clear();
        _ids.Add(GetViewController().CreateAim(owner, pos, GetConfig()));

        startShootingPos = pos;
        keepShootTime = 0;
    }

    int keepShootTime = 0;
    bool IsKeepAim()
    {
        if( keepShootTime < GetShootingTime())
        {
            return true;
        }
        return false;
    }

    int GetShootingTime()
    {
        return (int)GetConfig()[11];
    }

    bool IsClearWhenRelease()
    {
        return (bool)GetConfig()[9];
    }

    bool IsDragable()
    {
        return (bool)GetConfig()[3];
    }

    object[] GetConfig()
    {
        return this.config;
    }
}
