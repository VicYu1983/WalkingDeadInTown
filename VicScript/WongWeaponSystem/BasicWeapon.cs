using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using VicScript.WongWeaponSystem;

namespace VicScript.WongWeaponSystem
{
    public class BasicWeapon : IWeapon
    {
        public AimViewController AimViewController
        {
            get;set;
        }
        public WongWeaponController Owner
        {
            set;get;
        }
        public object[] Config
        {
            set;get;
        }

        List<int> _ids = new List<int>();
        
        public BasicWeapon()
        {
            
        }

        public virtual void StartAim(Vector3 pos)
        {
            throw new Exception("need to be override!");
        }
        public virtual void EndAim()
        {
            throw new Exception("need to be override!");
        }
        public virtual void MoveAim(Vector3 pos)
        {
            throw new Exception("need to be override!");
        }
        public virtual void KeepStartAim(Vector3 pos)
        {
            throw new Exception("need to be override!");
        }

        public virtual void AimOnce(Vector3 pos)
        {
            throw new Exception("need to be override!");
        }

        public bool IsBlade()
        {
            return (bool)Config[12];
        }

        public virtual void Update()
        {
            keepShootTime++;
            if (IsKeepAim())
            {
                DoKeepStartAim(startShootingPos);
            }
        }

        Vector3 startShootingPos;

        protected void DoStartAim(Vector3 pos)
        {
            if (IsDragable())
            {
                Shooting(pos);
            }
        }

        protected void DoMoveAim(Vector3 pos)
        {
            AimViewController.DragAimsByIds(_ids.ToArray(), this, pos);
        }

        protected void DoKeepStartAim(Vector3 pos)
        {
            _ids.Add( AimViewController.CreateAim( this, pos ));
        }

        protected void DoEndAim()
        {
            if (IsClearWhenRelease())
                AimViewController.ClearAimsByIds(_ids.ToArray());
        }

        protected void DoAimOnce(Vector3 pos)
        {
            if (!IsDragable())
            {
                Shooting(pos);
            }
        }

        void Shooting(Vector3 pos)
        {
            _ids.Clear();
            _ids.Add( AimViewController.CreateAim(this, pos));

            startShootingPos = pos;
            keepShootTime = 0;
        }

        int keepShootTime = 0;
        bool IsKeepAim()
        {
            if (keepShootTime < GetShootingTime())
            {
                return true;
            }
            return false;
        }

        int GetShootingTime()
        {
            return (int)Config[11];
        }

        bool IsClearWhenRelease()
        {
            return (bool)Config[9];
        }

        bool IsDragable()
        {
            return (bool)Config[3];
        }

    }
}
