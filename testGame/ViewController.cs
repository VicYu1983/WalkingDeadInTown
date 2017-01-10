﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

enum PrefabName{
    BULLET,
    SPECIAL,
    AIM
}

public class ViewController : MonoBehaviour {

    public GameObject GamePage;
    public GameObject ObjectContainer;
    public GameObject Player;
    public GameObject Ground;
    public GameObject[] Enemys;

    public GameObject[] Prefabs;

    [SerializeField]
    private List<GameObject> Bullets;

    [SerializeField]
    private Dictionary<int,List<AimController>> Aims = new Dictionary<int,List<AimController>>();

    int aimsId = 0;

    Vector3? targetPos;
    Vector3 additivePos = new Vector3();

    public void SetPlayerPositionByScreenPos( Vector3 screenPos )
    {
        targetPos = GetMousePositionOnWorld( screenPos );
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public void DodgePlayer( Vector3 dir, float force )
    {
        SetPlayerForce(dir, force);
    }

    public GameObject GetObjectFromObjectContainerByName( string name )
    {
        return ObjectContainer.transform.FindChild(name).gameObject;
    }

    public void MakePlayerStop()
    {
        Vector3 playerAcc = Player.GetComponent<Rigidbody2D>().velocity;
        Player.GetComponent<Rigidbody2D>().AddForce(-playerAcc * 40);
        targetPos = null;
    }

    public void CreateBullet( Vector3 from, Vector3 dir, float force )
    {
        CreateOneBullet(PrefabName.BULLET, from, dir, force);
    }

    public void CreateSpecialBullet(Vector3 from, Vector3 dir, float force)
    {
        CreateOneBullet(PrefabName.SPECIAL, from, dir, force);
    }

    public int CreateAim( Vector3 pos, object[] config )
    {
        int age = (int)config[0];
        float size = (float)config[1];
        bool dragable = (bool)config[2];
        int count = (int)config[3];
        float seperateRange = (float)config[4];
        float expandSpeed = (float) config[5];
        bool delay = (bool)config[6];
        float startSize = (float)config[7];

        aimsId++;
        if (!Aims.ContainsKey(aimsId))
        {
            Aims.Add(aimsId, new List<AimController>());
        }

        for (int i = 0; i < count; ++i)
        {
            GameObject aim = GameObjectFactory(PrefabName.AIM);
            aim.SetActive(true);
            aim.transform.parent = ObjectContainer.transform;
            aim.GetComponent<AimController>().SetConfig(age, size, startSize, dragable, expandSpeed, delay);
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, 0);
            aim.GetComponent<AimController>().Offset = offset;
            aim.GetComponent<AimController>().SetPosition(pos);
            Aims[aimsId].Add(aim.GetComponent<AimController>());
        }
        return aimsId;
    }

    public void DragAimsByIds(int[] ids, Vector3 pos)
    {
        foreach( int id in ids)
        {
            List<AimController> nowAims = GetAimById(id);
            if (nowAims != null)
            {
                foreach (AimController a in nowAims)
                {
                    if (a.Dragable)
                    {
                        try
                        {
                            a.GetComponent<AimController>().SetPosition(pos);
                        }
                        catch (Exception e)
                        {
                            /* AimController 在時間到時會自己刪掉自己。發生時。這邊就不用刪了 */
                        }
                    }
                }
            }
            
        }
    }

    public List<AimController> GetAimById(int id)
    {
        if(Aims.ContainsKey(id))
        {
            return Aims[id];
        }
        return null;
    }

    public void ClearAimsByIds( int[] ids )
    {
        foreach( int id in ids)
        {
            List<AimController> nowAims = GetAimById(id);
            if (nowAims != null)
            {
                foreach (AimController a in nowAims)
                {
                    try
                    {
                        if (a.Delay)
                        {
                            /* fire delay shoot! */
                        }
                        Destroy(a.gameObject);
                    }
                    catch (Exception e)
                    {
                        /* AimController 在時間到時會自己刪掉自己。發生時。這邊就不用刪了 */
                    }

                }
                Aims.Remove(aimsId - 1);
            }
        }
        
    }
    /*
    public void ClearLastestAims()
    {
        ClearAimsById(aimsId);
    }
    */
    private void CreateOneBullet(PrefabName bulletName, Vector3 from, Vector3 dir, float force)
    {
        GameObject bullet = GameObjectFactory(bulletName);
        bullet.SetActive(true);
        bullet.transform.parent = ObjectContainer.transform;
        bullet.GetComponent<RectTransform>().position = from;
        bullet.GetComponent<Rigidbody2D>().AddForce(dir * force);
        bullet.GetComponent<AgeCalculator>().OnDeadEvent += OnBulletDeadEvent;
        Bullets.Add(bullet);
    }

    private void OnBulletDeadEvent(AgeCalculator sender)
    {
        sender.gameObject.GetComponent<AgeCalculator>().OnDeadEvent -= OnBulletDeadEvent;
        Bullets.Remove(sender.gameObject);
        Destroy(sender.gameObject);
    }

    GameObject GameObjectFactory(PrefabName name )
    {
        switch( name)
        {
            case PrefabName.BULLET:
                return Instantiate(Prefabs[0]);
            case PrefabName.SPECIAL:
                return Instantiate(Prefabs[1]);
            case PrefabName.AIM:
                return Instantiate(Prefabs[2]);
        }
        return null;
    }

    Vector3 GetMousePositionOnWorld( Vector3 screenPos )
    {
        Vector3 clickPos = screenPos;
        clickPos.z = Camera.main.transform.localPosition.z;
        return Camera.main.ScreenToWorldPoint(clickPos);
    }

    void SetPlayerForce(Vector3 dir, float force)
    {
        Player.GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
    }
	
	void Update () {
        Vector3 cameraPos = Camera.main.transform.localPosition;
        cameraPos.x = Player.GetComponent<RectTransform>().position.x;
        cameraPos.y = Player.GetComponent<RectTransform>().position.y;
        Camera.main.transform.localPosition = cameraPos;
      
        if( targetPos != null)
        {
            Vector3 diffVec = (Vector3)targetPos - Player.GetComponent<RectTransform>().position;
            if (diffVec.magnitude < 40)
            {
                targetPos = null;
            }
            else
            {
                SetPlayerForce(diffVec, 400);
            }
        }
    }


}
