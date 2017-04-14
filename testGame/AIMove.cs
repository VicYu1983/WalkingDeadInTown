using Assets.testGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMove : AIBasic
{
    public override void Update()
    {
        ControllerRigidbody crb = PlayerController.GetComponent<ControllerRigidbody>();
        Vector3 targetPos = crb.Position;
        if(ViewController.Player != null)
        {
            targetPos += GetPlayerMoveTarget(PlayerController, ViewController.Player, 100, TrackAndKeepMethod);

            List<PlayerController> mates = ViewController.Enemys;
            foreach (PlayerController m in mates)
            {
                if (PlayerController != m)
                {
                    targetPos += GetPlayerMoveTarget(PlayerController, m, 60, KeepMethod);
                }
            }
            crb.SetPlayerPosition(targetPos);
        }
    }

    delegate bool TrackMethod(Vector3 diff, float distance);

    bool TrackAndKeepMethod(Vector3 diff, float distance)
    {
        return diff.magnitude > distance || diff.magnitude < distance - 10;
    }

    bool KeepMethod(Vector3 diff, float distance)
    {
        return diff.magnitude < distance - 10;
    }

    Vector3 GetPlayerMoveTarget(PlayerController follower, PlayerController hoster, float distance, TrackMethod method)
    {
        ControllerRigidbody crb_follower = follower.GetComponent<ControllerRigidbody>();
        ControllerRigidbody crb_hoster = hoster.GetComponent<ControllerRigidbody>();
        Vector3 diff = crb_hoster.Position - crb_follower.Position;
        Vector3 retOffset = new Vector3();
        if (method(diff, distance))
        {
            Vector3 dir = diff.normalized;
            Vector3 targetPos = crb_hoster.Position + -dir * distance;
            retOffset = targetPos - crb_follower.Position;
        }
        return retOffset;
    }
}
