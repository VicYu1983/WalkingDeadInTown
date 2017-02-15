using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

namespace VicScript.WongWeaponSystem
{
    public class AimViewController : MonoBehaviour
    {
        public GameObject ObjectContainer;
        public GameObject Prefab;

        public Action<Vector3, object[]> OnCreateAim;
        public Action<Vector3> OnDragAim;
        public Action<Vector3> OnDestroyAim;
        public Action OnAimEmpty;

        Dictionary<int, List<AimController>> Aims = new Dictionary<int, List<AimController>>();
        int aimsId = 0;

        public int CreateAim(Vector3 pos, object[] config)
        {
            int age = (int)config[1];
            float size = (float)config[2];
            bool dragable = (bool)config[3];
            int count = (int)config[4];
            float seperateRange = (float)config[5];
            float expandSpeed = (float)config[6];
            bool delay = (bool)config[7];
            float startSize = (float)config[8];
            bool isBlade = (bool)config[12];

            aimsId++;
            if (!Aims.ContainsKey(aimsId))
            {
                Aims.Add(aimsId, new List<AimController>());
            }

            for (int i = 0; i < count; ++i)
            {
                GameObject aim = Instantiate(Prefab);
                aim.SetActive(true);
                aim.transform.parent = ObjectContainer.transform;
                aim.GetComponent<AimController>().SetConfig(age, size, startSize, dragable, expandSpeed, delay);
                Vector3 offset = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, 0);
                aim.GetComponent<AimController>().Offset = offset;
                aim.GetComponent<AimController>().SetPosition(pos);
                aim.GetComponent<AimController>().GroupId = aimsId;
                aim.GetComponent<AgeCalculator>().OnDeadEvent += OnAimDeadEvent;
                Aims[aimsId].Add(aim.GetComponent<AimController>());

                if (OnCreateAim != null)
                {
                    OnCreateAim.Invoke(pos + offset, config);
                }
            }
            
            return aimsId;
        }

        private void OnAimDeadEvent(AgeCalculator obj)
        {
            obj.OnDeadEvent -= OnAimDeadEvent;
            GameObject aimSender = obj.gameObject;
            int groupId = aimSender.GetComponent<AimController>().GroupId;
            List<AimController> nowAims = GetAimById(groupId);
            if (nowAims != null)
            {
                nowAims.Remove(aimSender.GetComponent<AimController>());
                Destroy(aimSender);

                if (nowAims.Count == 0)
                {
                    if (Aims.ContainsKey(groupId))
                    {
                        Aims.Remove(groupId);
                    }
                }
            }
            CheckIsStopAim();
        }

        public void DragAimsByIds(int[] ids, Vector3 pos)
        {
            foreach (int id in ids)
            {
                List<AimController> nowAims = GetAimById(id);
                if (nowAims != null)
                {
                    foreach (AimController a in nowAims)
                    {
                        if (a.Dragable)
                        {
                            a.GetComponent<AimController>().SetPosition(pos);
                            if (OnDragAim != null) OnDragAim.Invoke(pos);
                        }
                    }
                }
            }
        }

        public List<AimController> GetAimById(int id)
        {
            if (Aims.ContainsKey(id))
            {
                return Aims[id];
            }
            return null;
        }

        public void ClearAimsByIds(int[] ids)
        {
            foreach (int id in ids)
            {
                List<AimController> nowAims = GetAimById(id);
                if (nowAims != null)
                {
                    foreach (AimController a in nowAims)
                    {
                        a.GetComponent<AgeCalculator>().OnDeadEvent -= OnAimDeadEvent;
                        Destroy(a.gameObject);
                    }
                    nowAims.Clear();
                    if (Aims.ContainsKey(id)) Aims.Remove(id);
                }
            }
            CheckIsStopAim();
        }

        void CheckIsStopAim()
        {
            if (Aims.Count == 0)
            {
                if (OnAimEmpty != null) OnAimEmpty.Invoke();
            }
        }

        void Start()
        {
            CheckIsStopAim();
        }


    }

}
