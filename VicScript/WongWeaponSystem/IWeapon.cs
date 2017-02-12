using UnityEngine;

namespace VicScript.WongWeaponSystem
{
    public interface IWeapon
    {
        void StartAim(Vector3 pos);
        void EndAim();
        void MoveAim(Vector3 pos);
        void KeepStartAim(Vector3 pos);
        void AimOnce(Vector3 pos);
        void Update();
        bool IsBlade();
    }
}
