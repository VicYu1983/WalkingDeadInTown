using UnityEngine;
using System.Collections;

public class AutoWeapon : BasicWeapon, IWeapon {
    public AutoWeapon(ViewController vc, object[] config) : base(vc, config)
    {

    }

    public override void StartAim(Vector3 pos)
    {
        GetViewController().CreateAim(pos, GetConfig());
    }

    public override void EndAim()
    {
        GetViewController().ClearLastestAims();
    }

    public override void KeepStartAim(Vector3 pos)
    {
        GetViewController().CreateAim(pos, GetConfig());
    }

    public override void MoveAim(Vector3 pos)
    {
        /* 全自動武器不可以移動準星 */
    }
}
