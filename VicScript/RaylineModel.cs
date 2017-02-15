using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VicScript
{
    public class RaylineModel : MonoBehaviour
    {
        public Vector3 fromPos;
        public Vector3 targetPos;
        public float speed;
        public Action<Vector3> OnDeadEvent;

        Vector3 currentPos;
        public Vector3 CurrentPos{
            get
            {
                return currentPos;
            }
            set
            {
                currentPos = value;
            }
        }
        float maxLength;
        float degree;
        public float Degree
        {
            get
            {
                return degree;
            }
        }

        float _scaleFac = 0;
        public float ScaleFac
        {
            get
            {
                return _scaleFac;
            }
        }

        void Start()
        {
            currentPos = fromPos;
            maxLength = (targetPos - fromPos).magnitude;
            degree = Vector3.Angle(Vector3.right, targetPos - fromPos);
        }

        void Update()
        {
            Vector3 dir = targetPos - currentPos;
            float moveDistance = (currentPos - fromPos).magnitude;
            float movePercent = moveDistance / maxLength;
            if (movePercent < 1)
            {
                currentPos += dir.normalized * speed;

                if (movePercent < .3f)
                {
                    _scaleFac = movePercent / .3f;
                }
                else if (movePercent > .7f)
                {
                    float sync = movePercent - .7f;
                    _scaleFac = 1 - sync / .3f;
                }
                else
                {
                    _scaleFac = 1;
                }
            }
            else
            {
                if (OnDeadEvent != null) OnDeadEvent.Invoke(currentPos);
            }
        }
    }

}
