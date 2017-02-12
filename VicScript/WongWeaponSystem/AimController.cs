using UnityEngine;
using System.Collections;
using System;

namespace VicScript.WongWeaponSystem
{
    public class AimController : MonoBehaviour
    {

        public int GroupId;

        public PlayerController owner;
        public float ExpandSpeed = 1.0f;
        public float Size = 1.0f;
        public bool Dragable = true;
        public bool Delay = false;

        Vector3 _offset = new Vector3();
        public Vector3 Offset
        {
            set
            {
                _offset = value;
            }
            get
            {
                return _offset;
            }
        }

        Vector2 currentSize;

        public void SetConfig(int maxAge, float size, float startSize, bool dragable, float speed, bool delay)
        {
            GetComponent<AgeCalculator>().DeadAge = maxAge;
            this.Size = size;
            this.Dragable = dragable;
            this.ExpandSpeed = speed;
            this.Delay = delay;
            this.currentSize = new Vector2
            (
                ori_size.x * startSize, ori_size.y * startSize
            );
        }

        public void SetPosition(Vector3 pos)
        {
            pos.z = 0;
            pos.y += GameConfig.AimOffsetY;
            GetComponent<RectTransform>().position = pos + _offset;
        }

        Vector2 ori_size;

        void Start()
        {
            ori_size = GetComponent<RectTransform>().sizeDelta;
            GetComponent<RectTransform>().sizeDelta = new Vector2();
        }

        void Update()
        {
            SetSize();
        }

        void SetSize()
        {
            int currentAge = GetComponent<AgeCalculator>().CurrentAge;
            int maxAge = GetComponent<AgeCalculator>().DeadAge;

            currentSize.x += ExpandSpeed * ori_size.x;
            currentSize.y += ExpandSpeed * ori_size.y;

            if (currentSize.x >= Size * ori_size.x) currentSize.x = Size * ori_size.x;
            if (currentSize.y >= Size * ori_size.y) currentSize.y = Size * ori_size.y;

            GetComponent<RectTransform>().sizeDelta = currentSize;
        }
    }
}
