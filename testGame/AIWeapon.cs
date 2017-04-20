using Assets.testGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIWeapon : AIBasic
{
    public override void Update()
    {
        if (ViewController.Player == null) return;
        if (PlayerController != null) {
            ControllerRigidbody crb_player = ViewController.Player.GetComponent<ControllerRigidbody>();
            ControllerRigidbody crb_current = PlayerController.GetComponent<ControllerRigidbody>();
            Vector3 diff = crb_player.Position - crb_current.Position;
            if (UnityEngine.Random.value > .995f)
            {
                Vector3 aimpos = crb_player.Position + crb_player.Velocity;
                ViewController.CreateBigAim(aimpos);
                WongWeaponController.weapons[0].AimOnce(aimpos);
            }
        }
    }
}
