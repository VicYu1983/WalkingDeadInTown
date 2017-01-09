using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VicScript
{
    public class DetectTouchCountByPassTime : MonoBehaviour
    {

        public int sample = 10;
        private Queue<int> touchCountOnPassTime = new Queue<int>();
        private Dictionary<int, int> forDetectCount = new Dictionary<int, int>();

        private Queue<Touch[]> touchOnPassTime = new Queue<Touch[]>();

        private bool _isClick = false;
        public bool IsClick
        {
            get
            {
                return _isClick;
            }
        }

        private int _touchCount = 0;
        public int TouchCount
        {
            get
            {
                return _touchCount;
            }
        }

        public string GetTouchString()
        {
            string retstr = "";
            foreach (int t in touchCountOnPassTime)
            {
                retstr += t + ",";
            }
            return retstr;
        }

        public Vector3 GetAverageTouchPosition()
        {
            Vector2 pos = new Vector2(0,0);
            int count = 0;
            foreach( Touch[] ts in touchOnPassTime )
            {
                foreach( Touch t in ts)
                {
                    pos += t.position;
                    count++;
                }
            }
            pos /= count;
            return new Vector3(pos.x, pos.y, 0);
        }

        void Update()
        {

            if (touchCountOnPassTime.Count >= sample)
            {
                touchCountOnPassTime.Dequeue();
                touchOnPassTime.Dequeue();
            }
            touchCountOnPassTime.Enqueue(Input.touchCount);
            touchOnPassTime.Enqueue( Input.touches );

            forDetectCount.Clear();
            foreach (int t in touchCountOnPassTime)
            {
                if (forDetectCount.ContainsKey(t))
                {
                    forDetectCount[t] += 1;
                }
                else
                {
                    forDetectCount.Add(t, 0);
                }
            }

            int maxCount = 0;
            _touchCount = 0;
            foreach (int v in forDetectCount.Keys)
            {
                if (v != 0)
                {
                    if (forDetectCount[v] > maxCount)
                    {
                        maxCount = forDetectCount[v];
                        _touchCount = v;
                    }
                }
            }

            _isClick = false;
            if ( TouchCount > 0)
            {
                if (touchCountOnPassTime.Peek() == 0 && Input.touchCount == 0)
                {
                    _isClick = true;
                }
            }
        }
    }
}

