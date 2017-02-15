using UnityEngine;

namespace VicScript.WongWeaponSystem
{
    public interface IWeapon
    {
        AimViewController AimViewController
        {
            get; set;
        }
        WongWeaponController Owner
        {
            set; get;
        }
        object[] Config
        {
            set; get;
        }
        void StartAim(Vector3 pos);
        void EndAim();
        void MoveAim(Vector3 pos);
        void KeepStartAim(Vector3 pos);
        void AimOnce(Vector3 pos);
        void Update();
        bool IsBlade();
    }
}
