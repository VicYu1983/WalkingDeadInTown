using UnityEngine;
using System.Collections;
using VicScript.WongWeaponSystem;

namespace VicScript.WongWeaponSystem
{
    public class AutoWeapon : BasicWeapon, IWeapon
    {
        public AutoWeapon()
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

        public override void KeepStartAim(Vector3 pos)
        {
            DoKeepStartAim(pos);
        }

        public override void MoveAim(Vector3 pos)
        {
            /* 全自動武器不可以移動準星 */
        }
    }
}
