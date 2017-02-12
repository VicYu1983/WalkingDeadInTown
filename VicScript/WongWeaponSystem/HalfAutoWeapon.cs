using UnityEngine;
using System.Collections;
using VicScript.WongWeaponSystem;

namespace VicScript.WongWeaponSystem
{
    public class HalfAutoWeapon : BasicWeapon, IWeapon
    {
        public HalfAutoWeapon(AimViewController vc, object[] config) : base(vc, config)
        {

        }

        public override void StartAim(Vector3 pos)
        {
            DoStartAim(pos);
        }

        public override void EndAim()
        {
            DoEndAim();
        }

        public override void MoveAim(Vector3 pos)
        {
            DoMoveAim(pos);
        }

        public override void KeepStartAim(Vector3 pos)
        {
            /* 半自動步槍不能持續開火 */
        }

        public override void AimOnce(Vector3 pos)
        {
            DoAimOnce(pos);
        }
    }
}
