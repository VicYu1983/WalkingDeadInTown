using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VicScript.WongWeaponSystem
{
    public class WongWeaponController : MonoBehaviour
    {
        public AimViewController AimViewController;
        public List<IWeapon> weapons = new List<IWeapon>();
        
        public int AddWeapon( object[] config )
        {
            bool autoWeapon = (bool)config[10];
            IWeapon weapon;
            if (autoWeapon)
            {
                weapon = new AutoWeapon();
            }
            else
            {
                weapon = new HalfAutoWeapon();
            }
            weapon.Owner = this;
            weapon.Config = config;
            weapon.AimViewController = AimViewController;
            weapons.Add(weapon);
            return weapons.Count - 1;
        }

        public void RemoveWeaponById( int id )
        {
            weapons.RemoveAt(id);
        }

        public void ClearWeapons()
        {
            weapons.Clear();
        }

        public void MoveAim(Vector3 obj)
        {
            foreach (IWeapon w in weapons) w.MoveAim(obj);
        }

        public void StartAim(Vector3 obj)
        {
            foreach (IWeapon w in weapons) w.StartAim(obj);
        }

        public void AimOnce(Vector3 obj)
        {
            foreach (IWeapon w in weapons) w.AimOnce(obj);
        }

        public void EndAim()
        {
            foreach (IWeapon w in weapons) w.EndAim();
        }

        public void KeepStartAim(Vector3 obj)
        {
            foreach (IWeapon w in weapons) w.KeepStartAim(obj);
        }

        void Update()
        {
            foreach (IWeapon w in weapons) w.Update();
        }
    }
}
