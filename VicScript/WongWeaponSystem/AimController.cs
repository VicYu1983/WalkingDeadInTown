using UnityEngine;
using System.Collections;
using System;

namespace VicScript.WongWeaponSystem
{
    public class AimController : MonoBehaviour
    {
        
        public int GroupId;
        
        public float ExpandSpeed = 1.0f;
        public float Size = 1.0f;
        public bool Dragable = true;
        public bool Delay = false;

        public IWeapon Weapon
        {
            set;get;
        }

        public Vector3 Position
        {
            set
            {
                value.z = 0;
                GetComponent<RectTransform>().position = value + _offset;
            }
            get
            {
                return GetComponent<RectTransform>().position;
            }
        }

        object[] config;
        public object[] Config
        {
            set{
                config = value;

                float startSize = (float)config[8];

                this.Size = (float)config[2];
                this.Dragable = (bool)config[3];
                this.ExpandSpeed = (float)config[6];
                this.Delay = (bool)config[7];
                this.currentSize = new Vector2
                (
                    ori_size.x * startSize, ori_size.y * startSize
                );
                
            }
            get{
                return config;
            }
            
        }

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
