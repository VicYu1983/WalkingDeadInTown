using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIWeapon : AIBasic
{
    public override void Update()
    {
        if (PlayerController != null) {
            Vector3 diff = ViewController.Player.Position - PlayerController.Position;
            if (UnityEngine.Random.value > .995f)
            {
                WongWeaponController.weapons[0].AimOnce(ViewController.Player.Position);
            }
        }
    }
}
