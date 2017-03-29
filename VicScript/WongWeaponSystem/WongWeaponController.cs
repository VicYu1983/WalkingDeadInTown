using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VicScript.WongGesture;

namespace VicScript.WongWeaponSystem
{
    public class WongWeaponController : MonoBehaviour
    {
        public AimViewController AimViewController;
       // public WongGestureController WongGestureController;
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
            print(config[0]);
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
        /*
        public void Start()
        {
            WongGestureController.OnOneFingerClicked += OnOneFingerClicked;
            WongGestureController.OnOneFingerDown += OnOneFingerDown;
            WongGestureController.OnOneFingerMove += OnOneFingerMove;
            WongGestureController.OnOneFingerMoveAfterHold += OnOneFingerMoveAfterHold;
            WongGestureController.OnEachFingerUp += OnEachFingerUp;
        }

        public void OnDestroy()
        {
            WongGestureController.OnOneFingerClicked -= OnOneFingerClicked;
            WongGestureController.OnOneFingerDown -= OnOneFingerDown;
            WongGestureController.OnOneFingerMove -= OnOneFingerMove;
            WongGestureController.OnOneFingerMoveAfterHold -= OnOneFingerMoveAfterHold;
            WongGestureController.OnEachFingerUp -= OnEachFingerUp;
        }
        */
        private void OnEachFingerUp(Vector3 obj)
        {
            EndAim();
        }

        private void OnOneFingerMoveAfterHold(Vector3 obj)
        {
            KeepStartAim(obj);
        }

        private void OnOneFingerMove(Vector3 obj)
        {
            MoveAim(obj);
        }

        private void OnOneFingerDown(Vector3 obj)
        {
            StartAim(obj);
        }

        private void OnOneFingerClicked(Vector3 obj)
        {
            AimOnce(obj);
        }

        void Update()
        {
           // foreach (IWeapon w in weapons) w.Update();
        }
    }
}
