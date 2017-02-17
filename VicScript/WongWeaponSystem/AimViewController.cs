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
        public GameObject Prefab;

        public Action<IWeapon, Vector3> OnCreateAim;
        public Action<IWeapon, Vector3> OnDragAim;
        public Action<IWeapon, Vector3> OnDestroyAim;
        public Action<IWeapon, Vector3> OnWeaponFireOnce;

        public Action OnAimEmpty;

        Dictionary<int, List<AimController>> Aims = new Dictionary<int, List<AimController>>();
        int aimsId = 0;

        public int CreateAim(IWeapon weapon, Vector3 pos)
        {
            object[] config = weapon.Config;
            WongWeaponController owner = weapon.Owner;

            int age = (int)config[1];
            int count = (int)config[4];
            float seperateRange = (float)config[5];

            aimsId++;
            if (!Aims.ContainsKey(aimsId))
            {
                Aims.Add(aimsId, new List<AimController>());
            }

            for (int i = 0; i < count; ++i)
            {
                GameObject aim = Instantiate(Prefab);
                aim.SetActive(true);
                aim.transform.parent = gameObject.transform;

                Vector3 offset = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, 0);

                AimController aimController = aim.GetComponent<AimController>();
                aimController.Weapon = weapon;
                aimController.Offset = offset;
                aimController.Position = pos;
                aimController.GroupId = aimsId;

                AgeCalculator ageCalculator = aim.GetComponent<AgeCalculator>();
                ageCalculator.DeadAge = age;
                ageCalculator.OnDeadEvent += OnAimDeadEvent;
                Aims[aimsId].Add(aimController);

                if (OnCreateAim != null)
                {
                    OnCreateAim.Invoke(weapon, pos + offset);
                }

                if(!aimController.Delay)
                {
                    if (OnWeaponFireOnce != null)
                    {
                        OnWeaponFireOnce.Invoke(weapon, pos + offset);
                    }
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
                AimController aim = aimSender.GetComponent<AimController>();
                if (aim.Delay)
                {
                    if (OnWeaponFireOnce != null) OnWeaponFireOnce.Invoke(aim.Weapon, aim.Position);
                }

                nowAims.Remove( aim );
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

        public void DragAimsByIds(int[] ids, IWeapon weapon, Vector3 pos )
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
                            a.GetComponent<AimController>().Position = pos;
                            if (OnDragAim != null) OnDragAim.Invoke(weapon, pos);
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
                        AgeCalculator agec = a.GetComponent<AgeCalculator>();
                        if (agec.OnDeadEvent != null) agec.OnDeadEvent.Invoke(agec);
                        agec.OnDeadEvent -= OnAimDeadEvent;
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
