using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AI {

    private ViewController _vc;
    public ViewController ViewController
    {
        set
        {
            _vc = value;
        }
    }

    private PlayerController _pc;
    public PlayerController PlayerController
    {
        set
        {
            _pc = value;
        }
    }

    public void Update()
    {
        Vector3 targetPos = _pc.Position;
        targetPos += GetPlayerMoveTarget(_pc, _vc.Player, 100, TrackAndKeepMethod);

        List<PlayerController> mates = _vc.Enemys;
        foreach(PlayerController m in mates)
        {
            if( _pc != m)
            {
                targetPos += GetPlayerMoveTarget(_pc, m, 60, KeepMethod);
            }
        }
        _pc.SetPlayerPosition(targetPos);
    }

    delegate bool TrackMethod(Vector3 diff, float distance);

    bool TrackAndKeepMethod( Vector3 diff, float distance )
    {
        return diff.magnitude > distance || diff.magnitude < distance - 10;
    }

    bool KeepMethod(Vector3 diff, float distance)
    {
        return diff.magnitude < distance - 10;
    }

    Vector3 GetPlayerMoveTarget( PlayerController follower, PlayerController hoster, float distance, TrackMethod method   )
    {
        Vector3 diff = hoster.Position - follower.Position;
        Vector3 retOffset = new Vector3();
        if (method(diff, distance))
        {
            Vector3 dir = diff.normalized;
            Vector3 targetPos = hoster.Position + -dir * distance;
            retOffset = targetPos - follower.Position;
        }
        return retOffset;
    }
}
