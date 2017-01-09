using UnityEngine;
using System.Collections;

public class HalfAutoWeapon : BasicWeapon, IWeapon {
    public HalfAutoWeapon(ViewController vc, object[] config) : base(vc, config)
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

    public override void MoveAim(Vector3 pos)
    {
        GetViewController().DragAims(pos);
    }

    public override void KeepStartAim(Vector3 pos)
    {
        /* 半自動步槍不能持續開火 */
    }
}
