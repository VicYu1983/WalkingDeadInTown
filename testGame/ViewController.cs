using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

enum PrefabName{
    BULLET,
    SPECIAL,
    AIM,
    BODY_EXPLODE
}

public class ViewController : MonoBehaviour {

    public GameObject GamePage;
    public GameObject ObjectContainer;
    public GameObject Player;
    public GameObject Ground;
    public List<GameObject> Enemys;

    public GameObject[] Prefabs;

    [SerializeField]
    private List<GameObject> Bullets;
    private List<GameObject> ForSortingZ = new List<GameObject>();

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
        int age = (int)config[1];
        float size = (float)config[2];
        bool dragable = (bool)config[3];
        int count = (int)config[4];
        float seperateRange = (float)config[5];
        float expandSpeed = (float) config[6];
        bool delay = (bool)config[7];
        float startSize = (float)config[8];

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
            aim.GetComponent<AimController>().GroupId = aimsId;
            aim.GetComponent<AgeCalculator>().OnDeadEvent += OnAimDeadEvent;
            Aims[aimsId].Add(aim.GetComponent<AimController>());
        }
        BodyRotateByAimDir(pos);
        return aimsId;
    }

    private void BodyRotateByAimDir( Vector3 aimPos )
    {
        Vector3 aimDir = (aimPos - Player.GetComponent<RectTransform>().position).normalized;
        Player.GetComponent<PlayerController>().BodyRotateByAimDir(aimDir);
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
        foreach( int id in ids)
        {
            List<AimController> nowAims = GetAimById(id);
            if (nowAims != null)
            {
                foreach (AimController a in nowAims)
                {
                    if (a.Dragable)
                    {
                        a.GetComponent<AimController>().SetPosition(pos);
                        BodyRotateByAimDir(pos);
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
        if( Aims.Count == 0)
        {
            Player.GetComponent<PlayerController>().IsAim = false;
        }
    }
    
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

    private void CreateExplodeEffect( Vector3 pos )
    {
        GameObject explode = GameObjectFactory(PrefabName.BODY_EXPLODE);
        explode.SetActive(true);
        explode.transform.parent = ObjectContainer.transform;
        explode.GetComponent<RectTransform>().position = pos;
        explode.GetComponent<EffectTimeEvent>().OnEffectEndEvent += OnEffectEndEvent;
    }

    private void OnEffectEndEvent(GameObject obj)
    {
        obj.GetComponent<EffectTimeEvent>().OnEffectEndEvent -= OnEffectEndEvent;
        Destroy(obj);
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
            case PrefabName.BODY_EXPLODE:
                return Instantiate(Prefabs[3]);
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
        Player.GetComponent<PlayerController>().BodyRotateByMoveDir(dir.normalized);
    }

    void Start()
    {
        ForSortingZ.Add(Player);
        foreach (GameObject e in Enemys) ForSortingZ.Add(e);
        foreach( GameObject e in Enemys ) e.GetComponent<PlayerController>().OnHitEvent += OnEnmeyHit;
    }

    private void OnEnmeyHit(GameObject enemy, GameObject other )
    {
        if (other.name.IndexOf("Aim") != -1)
        {
            CreateExplodeEffect(enemy.GetComponent<RectTransform>().position);
            Destroy(enemy);
            Enemys.Remove(enemy);
            ForSortingZ.Remove(enemy);
        }
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
                SetPlayerForce(diffVec, GameConfig.MoveSpeed);
            }
        }
        SortingZ();
    }

    void SortingZ()
    {
        ForSortingZ.Sort((GameObject a, GameObject b) =>
        {
            if (a.GetComponent<RectTransform>().position.y > b.GetComponent<RectTransform>().position.y) return -1;
            return 1;
        });

        for (int i = ForSortingZ.Count - 1; i >= 0; --i)
        {
            ForSortingZ[i].transform.SetAsFirstSibling();
        }
    }
}
