using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.testGame
{
    class ControllerRigidbody : MonoBehaviour
    {
        Vector3? targetPos;
        public bool IsWalk = false;

        public Vector3 Position
        {
            set
            {
                value.z = 4;
                this.GetComponent<RectTransform>().position = value;
            }
            get
            {
                return this.GetComponent<RectTransform>().position;
            }
        }

        public Action<ControllerRigidbody, GameObject> OnHitEvent;

        public void Hit(Vector3 dir, float force)
        {
            GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
        }

        public void SetPlayerPositionByScreenPos(Vector3 screenPos)
        {
            targetPos = GetMousePositionOnWorld(screenPos);
        }

        public void SetPlayerPosition(Vector3 pos)
        {
            targetPos = pos;
        }

        public void DodgePlayerByScreenPos(Vector3 dir, float force)
        {
            SetPlayerForce((GetMousePositionOnWorld(dir) - Position).normalized, force);
            targetPos = null;
        }

        public void Dodge(Vector3 dir, float force)
        {
            SetPlayerForce(dir, force);
            targetPos = null;
        }

        public void MakePlayerStop()
        {
            Vector3 playerAcc = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().AddForce(-playerAcc * 40);
            targetPos = null;
        }

        public void StopMove()
        {
            targetPos = null;
        }

        void Update()
        {
            UpdatePosition();
        }

        void SetPlayerForce(Vector3 dir, float force)
        {
            // GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
            GetComponent<Rigidbody2D>().velocity = dir.normalized * force * 100;

            //使用數據移動，會喪失一些物理效果，例如會穿過物理物件
            /*
            Vector3 newpos = Position;
            newpos += dir.normalized * force * 2;
            newpos.z = 0;
            Position = newpos;
            */
            GetComponent<PlayerController>().BodyRotateByMoveDir(dir.normalized);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (OnHitEvent != null)
                OnHitEvent(this, other.gameObject);
        }

        void UpdatePosition()
        {
            if (targetPos != null)
            {
                Vector3 diffVec = (Vector3)targetPos - Position;
                if (diffVec.magnitude < 20)
                {
                    targetPos = null;
                    IsWalk = false;
                }
                else
                {
                    SetPlayerForce(diffVec, GameConfig.MoveSpeed);
                    IsWalk = true;
                }
            }
            else
            {
                IsWalk = false;
            }
        }

        Vector3 GetMousePositionOnWorld(Vector3 screenPos)
        {
            Vector3 clickPos = screenPos;
            clickPos.z = Camera.main.transform.localPosition.z;
            return Camera.main.ScreenToWorldPoint(clickPos);
        }
    }
}
