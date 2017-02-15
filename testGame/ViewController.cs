using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using VicScript.WongWeaponSystem;
using VicScript;

enum PrefabName{
    BULLET,
    SPECIAL,
    RAYLINE,
    BODY_EXPLODE,
    AIM_POINT_0,
    ENEMY,
    SPEED_SHADOW
}

public class ViewController : MonoBehaviour {

    public GameObject GamePage;
    public GameObject ObjectContainer;
    public AimViewController AimController;
    public PlayerController Player;
    public GameObject Ground;
    public Color EnemyColor;
    public List<GameObject> Stuffs;
    public GameObject[] Prefabs;
    public List<PlayerController> Enemys;
    
    public string[] HitSpeaks_100;
    public string[] HitSpeaks_75;
    public string[] HitSpeaks_50;
    public string[] HitSpeaks_25;
    public string[] HitSpeaks_10;

    [SerializeField]
    private List<GameObject> Bullets;
    private List<GameObject> ForSortingZ = new List<GameObject>();

    private Dictionary<int,List<AimController>> Aims = new Dictionary<int,List<AimController>>();

    int aimsId = 0;

    public GameObject GetObjectFromObjectContainerByName( string name )
    {
        return ObjectContainer.transform.FindChild(name).gameObject;
    }

    public void ClearEnemy()
    {
        for( int i = Enemys.Count - 1; i > 0; --i)
        {
            DestoryEnemy(Enemys[i].gameObject);
        }
    }

    public GameObject CreateEnemy( Vector3 pos )
    {
        GameObject e = GameObjectFactory(PrefabName.ENEMY);
        e.transform.SetParent(ObjectContainer.transform);
        e.transform.position = pos;

        PlayerController ep = e.GetComponent<PlayerController>();
        Enemys.Add(ep);

        ep.SetColor(EnemyColor);
        ForSortingZ.Add(e);
        ep.UpdateBody();
        /*
        ep.HP = 100;
        ep.OnHitEvent += OnEnmeyHit;
        ep.GetComponent<AgeCalculator>().DeadAge = Mathf.FloorToInt(UnityEngine.Random.value * 1000) + 500;
        ep.GetComponent<AgeCalculator>().OnDeadEvent += OnEnemySpeakEvent;
        ep.SetAI(this, new AIMove());
        */
        return e;
    }
    /*
    private void OnEnemySpeakEvent(AgeCalculator obj)
    {
        obj.ResetAge();
        obj.DeadAge = Mathf.FloorToInt(UnityEngine.Random.value * 1000) + 500;
        StartCoroutine(DisplayPlayerSpeak(obj.GetComponent<PlayerController>()));
    }
    */
    public void CreateRayLine( Vector3 from, Vector3 target )
    {
        GameObject s = GameObjectFactory(PrefabName.RAYLINE);
        s.GetComponent<RaylineModel>().fromPos = from;
        s.GetComponent<RaylineModel>().targetPos = target;
        s.GetComponent<RaylineModel>().speed = 12;
        s.transform.SetParent(ObjectContainer.transform);
    }

    public void CreateShadow(Vector3 pos, Vector3 scale)
    {
        GameObject s = GameObjectFactory(PrefabName.SPEED_SHADOW);
        s.transform.parent = ObjectContainer.transform;
        s.GetComponent<RectTransform>().position = pos;
        s.GetComponent<RectTransform>().localScale = scale;
        s.GetComponent<SpeedShadowController>().SetImage(Player.GetPlayerImage());
    }

    public void CreateBullet( Vector3 from, Vector3 dir, float force )
    {
        CreateOneBullet(PrefabName.BULLET, from, dir, force);
    }

    public void CreateSpecialBullet(Vector3 from, Vector3 dir, float force)
    {
        CreateOneBullet(PrefabName.SPECIAL, from, dir, force);
    }
    /*
    public void CreateAimPoint( int type, Vector3 pos)
    {
        GameObject ap;
        switch( type)
        {
            case 0: ap = GameObjectFactory(PrefabName.AIM_POINT_0);break;
            case 1: ap = GameObjectFactory(PrefabName.AIM_POINT_0); break;
            case 2: ap = GameObjectFactory(PrefabName.AIM_POINT_0); break;
            default:ap = GameObjectFactory(PrefabName.AIM_POINT_0); break;
        }
        ap.transform.parent = AimPointContainer.transform;
        ap.GetComponent<RectTransform>().position = pos;
    }

    public int CreateAim(PlayerController attacker, Vector3 pos, object[] config )
    {
        int age = (int)config[1];
        float size = (float)config[2];
        bool dragable = (bool)config[3];
        int count = (int)config[4];
        float seperateRange = (float)config[5];
        float expandSpeed = (float) config[6];
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
            GameObject aim = GameObjectFactory(PrefabName.AIM);
            aim.SetActive(true);
            aim.transform.parent = ObjectContainer.transform;
            aim.GetComponent<AimController>().SetConfig(age, size, startSize, dragable, expandSpeed, delay);
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, UnityEngine.Random.Range(-1.0f, 1.0f) * seperateRange, 0);
            aim.GetComponent<AimController>().owner = attacker;
            aim.GetComponent<AimController>().Offset = offset;
            aim.GetComponent<AimController>().SetPosition(pos);
            aim.GetComponent<AimController>().GroupId = aimsId;
            aim.GetComponent<AgeCalculator>().OnDeadEvent += OnAimDeadEvent;
            Aims[aimsId].Add(aim.GetComponent<AimController>());
        }
        BodyRotateByAimDir(attacker, pos);
        attacker.IsBlade = isBlade;
        return aimsId;
    }
    */
    private void BodyRotateByAimDir( PlayerController attacker, Vector3 aimPos )
    {
        Vector3 aimDir = (aimPos - attacker.Position).normalized;
        attacker.BodyRotateByAimDir(aimDir);
    }
    /*
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
                        BodyRotateByAimDir(Player, pos);
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

        List<PlayerController> buffer = new List<PlayerController>(playersAimCount.Keys);
        foreach (PlayerController key in buffer)
        {
            playersAimCount[key] = false;
        }
        
        foreach ( List<AimController> acs in Aims.Values )
        {
            foreach( AimController ac in acs)
            {
                playersAimCount[ac.owner] = true;
            }
        }

        foreach( PlayerController owner in playersAimCount.Keys)
        {
            if( !playersAimCount[owner] )
            {
                owner.IsAim = false;
                owner.UpdateBody();
            }
        }
        
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

    public void CreateExplodeEffect(Vector3 pos, Color color)
    {
        GameObject explode = GameObjectFactory(PrefabName.BODY_EXPLODE);
        explode.SetActive(true);
        explode.transform.parent = ObjectContainer.transform;
        explode.GetComponent<RectTransform>().position = pos;
        explode.GetComponent<EffectTimeEvent>().OnEffectEndEvent += OnEffectEndEvent;
        explode.GetComponent<Image>().color = color;
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
            case PrefabName.RAYLINE:
                return Instantiate(Prefabs[2]);
            case PrefabName.BODY_EXPLODE:
                return Instantiate(Prefabs[3]);
            case PrefabName.AIM_POINT_0:
                return Instantiate(Prefabs[4]);
            case PrefabName.ENEMY:
                return Instantiate(Prefabs[5]);
            case PrefabName.SPEED_SHADOW:
                return Instantiate(Prefabs[6]);
        }
        return null;
    }

   // Dictionary<PlayerController, bool> playersAimCount = new Dictionary<PlayerController, bool>();
    void Start()
    {
    //    playersAimCount.Add(Player, false);
        ForSortingZ.Add(Player.gameObject);

        /*
        foreach (PlayerController e in Enemys)
        {
            e.SetColor(EnemyColor);
            e.OnHitEvent += OnEnmeyHit;
            e.SetAI(this, new AIMove());
            ForSortingZ.Add(e.gameObject);
            playersAimCount.Add(e, false);
            e.UpdateBody();
        }
        
        */

        foreach (GameObject e in Stuffs) ForSortingZ.Add(e);
        Player.UpdateBody();

        GetComponent<AimViewController>().OnDragAim += OnDragAim;
        GetComponent<AimViewController>().OnCreateAim += OnCreateAim;
        GetComponent<AimViewController>().OnAimEmpty += OnAimEmpty;
   //     CheckIsStopAim();
    }

    private void OnAimEmpty()
    {
        Player.IsAim = false;
        Player.UpdateBody();
    }

    private void OnCreateAim(Vector3 arg1, object[] arg2)
    {
        bool isBlade = (bool)arg2[12];
        Player.IsBlade = isBlade;
        BodyRotateByAimDir(Player, arg1);
    }

    private void OnDragAim(Vector3 obj)
    {
        BodyRotateByAimDir(Player, obj);
    }
    /*
    private void OnEnmeyHit(GameObject enemy, GameObject other )
    {
        if (other.name.IndexOf("Aim") != -1)
        {
            PlayerController e = enemy.GetComponent<PlayerController>();
            e.HP -= 10;

            Vector3 aimPos = other.GetComponent<RectTransform>().localPosition;
            
            if( Player.IsBlade )
            {
                Player.DodgePlayer(e.Position - Player.Position, GameConfig.DodgeSpeed * 2);
            }
            else
            {
                CreateRayLine(Player.Position, aimPos);
            }

            if (e.IsDead())
            {
                CreateExplodeEffect(enemy.GetComponent<PlayerController>().Position, EnemyColor);
                DestoryEnemy(enemy);
            }
            else
            {
                MakeEnemyHitEffect(e, Player.Position, aimPos);
            }
            
        }
    }
    */

    public void MakeEnemyHitEffect(PlayerController p, Vector3 hitPos, Vector3 aimPos, float delay = 0)
    {
        StartCoroutine(DisplayHitEffect(p, hitPos, aimPos, delay));
    }

    IEnumerator DisplayHitEffect(PlayerController p, Vector3 hitPos, Vector3 aimPos, float delay = 0)
    {
        Vector3 tempP = new Vector3(p.Position.x, p.Position.y, 0);
        yield return new WaitForSeconds(delay);
        p.SetColor(Color.red);
        Vector3 hitforce = tempP - hitPos;
        p.GetComponent<Rigidbody2D>().AddForce(hitforce.normalized * 30);
        yield return new WaitForSeconds(.05f);
        p.SetColor(EnemyColor);
    }

    public IEnumerator DisplayPlayerSpeak(PlayerController p)
    {
        string[] targetSpeaks = null;
        if( p.HP <= 10)
        {
            targetSpeaks = HitSpeaks_10;
        }
        else if( p.HP <= 25)
        {
            targetSpeaks = HitSpeaks_25;
        }
        else if (p.HP <= 50)
        {
            targetSpeaks = HitSpeaks_50;
        }
        else if (p.HP <= 75)
        {
            targetSpeaks = HitSpeaks_75;
        }
        else if (p.HP <= 100)
        {
            targetSpeaks = HitSpeaks_100;
        }
        int choose = Mathf.FloorToInt(UnityEngine.Random.value * targetSpeaks.Length);
        p.SpeakContent = targetSpeaks[choose];
        yield return new WaitForSeconds(3f);
        p.SpeakContent = "";
    }

    public void DestoryEnemy( GameObject enemy )
    {
        Destroy(enemy);
        Enemys.Remove(enemy.GetComponent<PlayerController>());
   //     playersAimCount.Remove(enemy.GetComponent<PlayerController>());
        ForSortingZ.Remove(enemy);
    }

    void Update () {
        Vector3 cameraPos = Camera.main.transform.localPosition;
        cameraPos.x = Player.Position.x;
        cameraPos.y = Player.Position.y;
        Camera.main.transform.localPosition = cameraPos;
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
