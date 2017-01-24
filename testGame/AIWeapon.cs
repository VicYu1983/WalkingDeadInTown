using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIWeapon : AIBasic
{
    public override void Update()
    {
        if (_pc != null) {
            Vector3 diff = _vc.Player.Position - _pc.Position;
            if (diff.magnitude < 100)
            {
                if (UnityEngine.Random.value > .99f)
                {
                    _pc.weapons[0].AimOnce(_vc.Player.Position);
                }
            }
        }
    }
}
