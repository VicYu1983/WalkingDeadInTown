using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using VicScript.WongWeaponSystem;
using VicScript;
using Assets.testGame;

enum PrefabName{
    BULLET,
    SPECIAL,
    RAYLINE,
    BODY_EXPLODE,
    AIM_POINT_0,
    ENEMY,
    SPEED_SHADOW,
    EXPLODE,
    BARREL,
    FIRSTAID
}

public class ViewController : MonoBehaviour {

    public GameObject GamePage;
    public GameObject ObjectContainer;
    public AimViewController AimViewController;
    public PlayerController Player;
    public GameObject Ground;
    public Color EnemyColor;
    public List<GameObject> Stuffs;
    public GameObject[] Prefabs;
    public List<PlayerController> Enemys;
    public List<GameObject> Barrels;
    public List<GameObject> FirstAids;
    public AudioClip[] Audios;
    AudioSource AudioDongDongDong;
    AudioSource AudioDaDaDa;
    AudioSource AudioOneShot;

    public string[] HitSpeaks_100;
    public string[] HitSpeaks_75;
    public string[] HitSpeaks_50;
    public string[] HitSpeaks_25;
    public string[] HitSpeaks_10;

    public bool IsGameOver
    {
        get
        {
            return Player == null;
        }
    }

    [SerializeField]
    private List<GameObject> Bullets;
    public List<GameObject> ForSortingZ = new List<GameObject>();

    private Dictionary<int,List<AimController>> Aims = new Dictionary<int,List<AimController>>();

    int aimsId = 0;

    public GameObject GetObjectFromObjectContainerByName( string name )
    {
        return ObjectContainer.transform.FindChild(name).gameObject;
    }

    public void ClearPlayer()
    {
        if (Player != null) DestoryEnemy(Player.gameObject);
    }

    public void ClearBarrel()
    {
        for (int i = Barrels.Count - 1; i > 0; --i)
        {
            if (Barrels[i] != null)
                DestoryEnemy(Barrels[i].gameObject);
        }
        Barrels.Clear();
    }

    public void ClearEnemy()
    {
        for( int i = Enemys.Count - 1; i > 0; --i)
        {
            DestoryEnemy(Enemys[i].gameObject);
        }
        Enemys.Clear();
    }

    public GameObject CreatePlayer()
    {
        GameObject e = GameObjectFactory(PrefabName.ENEMY);
        e.transform.SetParent(ObjectContainer.transform);
        e.transform.position = new Vector3();

        PlayerController ep = e.GetComponent<PlayerController>();
        ep.GetComponent<ControllerHP>().HP = 200;
        ep.GetComponent<ControllerColor>().SetColor(Color.white);
        ep.UpdateBody();
        ForSortingZ.Add(e);

        Player = ep;
        return e;
    }
    public GameObject CreateEnemy( Vector3 pos )
    {
        GameObject e = GameObjectFactory(PrefabName.ENEMY);
        e.transform.SetParent(ObjectContainer.transform);
        e.transform.position = pos;

        PlayerController ep = e.GetComponent<PlayerController>();
        Enemys.Add(ep);

        ep.GetComponent<ControllerColor>().SetColor(EnemyColor);
        ep.UpdateBody();
        ForSortingZ.Add(e);

        return e;
    }

    public GameObject CreateFirstAid(Vector3 pos)
    {
        GameObject e = GameObjectFactory(PrefabName.FIRSTAID);
        e.transform.SetParent(ObjectContainer.transform);
        e.transform.position = pos;
        FirstAids.Add(e);
        return e;
    }

    public GameObject CreateBarrel( Vector3 pos)
    {
        GameObject e = GameObjectFactory(PrefabName.BARREL);
        e.transform.SetParent(ObjectContainer.transform);
        e.transform.position = pos;

        Barrels.Add(e);
        ForSortingZ.Add(e);
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
    public void CreateRayLine( IWeapon weapon, Vector3 from, Vector3 target, bool symbol = false )
    {
        GameObject s = GameObjectFactory(PrefabName.RAYLINE);
        s.transform.SetParent(ObjectContainer.transform);
        s.GetComponent<RayLineView>().Weapon = weapon;
        s.GetComponent<RaylineModel>().fromPos = from;
        s.GetComponent<RaylineModel>().targetPos = target;
        s.GetComponent<RaylineModel>().speed = 12;
        s.GetComponent<RaylineModel>().CurrentPos = from;


        if (symbol) s.GetComponent<Image>().color = new Color(0, 0, 0, 0);
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
        Vector3 aimDir = (aimPos - attacker.GetComponent<ControllerRigidbody>().Position).normalized;
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

    public void CreateExplodeEffect(Vector3 pos, Color color )
    {
        GameObject explode = GameObjectFactory(PrefabName.BODY_EXPLODE);
        explode.SetActive(true);
        explode.transform.parent = ObjectContainer.transform;
        explode.GetComponent<RectTransform>().position = pos;
        explode.GetComponent<EffectTimeEvent>().OnEffectEndEvent += OnEffectEndEvent;
        
        explode.GetComponent<Image>().color = color;
    }

    public void CreateBigExplodeEffect( Vector3 pos)
    {
        GameObject explode = GameObjectFactory(PrefabName.EXPLODE);
        explode.SetActive(true);
        explode.transform.parent = ObjectContainer.transform;
        explode.GetComponent<RectTransform>().position = pos;
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
            case PrefabName.BARREL:
                return Instantiate(Prefabs[7]);
            case PrefabName.FIRSTAID:
                return Instantiate(Prefabs[8]);
        }
        return null;
    }

   // Dictionary<PlayerController, bool> playersAimCount = new Dictionary<PlayerController, bool>();
    void Start()
    {
        /*
        ForSortingZ.Add(Player.gameObject);
        
        foreach (GameObject e in Stuffs) ForSortingZ.Add(e);
        Player.UpdateBody();
        */
        AimViewController.OnDragAim += OnDragAim;
        AimViewController.OnCreateAim += OnCreateAim;
        AimViewController.OnAimEmpty += OnAimEmpty;

        AudioDongDongDong = GetComponents<AudioSource>()[0];
        AudioDaDaDa = GetComponents<AudioSource>()[1];
        AudioOneShot = GetComponents<AudioSource>()[2];
    }

    private void OnAimEmpty()
    {
        if (Player != null)
        {
            Player.IsAim = false;
        }
        
        foreach( PlayerController e in Enemys)
        {
            if( e != null)
            {
                e.IsAim = false;
            }
        }
    }

    /*
private void OnAimEmpty()
{
   Player.IsAim = false;
   Player.UpdateBody();
}
*/
    private void OnCreateAim(IWeapon weapon, Vector3 to)
    {
        bool isBlade = (bool)weapon.Config[12];

        PlayerController p = weapon.Owner.GetComponent<PlayerController>();
        p.IsBlade = isBlade;
        BodyRotateByAimDir(p, to);
    }

    private void OnDragAim(IWeapon weapon, Vector3 pos)
    {
        PlayerController p = weapon.Owner.GetComponent<PlayerController>();
        BodyRotateByAimDir(p, pos);
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

    public void MakeEnemyHitEffect(GameObject p, Color color )
    {
        try
        {
            StartCoroutine(DisplayHitEffect(p, color));
        }
        catch( Exception e)
        {
            //maybe die now!
        }
    }

    IEnumerator DisplayHitEffect(GameObject p, Color color)
    {
        ControllerColor cc = p.GetComponent<ControllerColor>();
        cc.SetColor(Color.red);
        yield return new WaitForSeconds(.05f);
        cc.SetColor(color);
    }

    public IEnumerator DisplayPlayerSpeak(GameObject p)
    {
        ControllerHP ch = p.GetComponent<ControllerHP>();
        string[] targetSpeaks = null;
        if( ch.HP <= 10)
        {
            targetSpeaks = HitSpeaks_10;
        }
        else if( ch.HP <= 25)
        {
            targetSpeaks = HitSpeaks_25;
        }
        else if (ch.HP <= 50)
        {
            targetSpeaks = HitSpeaks_50;
        }
        else if (ch.HP <= 75)
        {
            targetSpeaks = HitSpeaks_75;
        }
        else if (ch.HP <= 100)
        {
            targetSpeaks = HitSpeaks_100;
        }
        int choose = Mathf.FloorToInt(UnityEngine.Random.value * targetSpeaks.Length);

        p.GetComponent<ControllerSpeakable>().SpeakContent = targetSpeaks[choose];
        yield return new WaitForSeconds(3f);
        p.GetComponent<ControllerSpeakable>().SpeakContent = "";
    }

    public void DestoryEnemy( GameObject enemy )
    {
        PlayerController p = enemy.GetComponent<PlayerController>();
        if( p == Player)
        {
            Player = null;
        }
        else
        {
            Enemys.Remove(p);
        }

        ForSortingZ.Remove(enemy);
        Destroy(enemy);
    }

    public void DestoryFirstAid(GameObject firstAid)
    {
        FirstAids.Remove(firstAid);
        Destroy(firstAid);
    }

    public void PlayDeadSound()
    {
        PlaySound(UnityEngine.Random.value > .5f ? Audios[2] : Audios[3]);
    }

    public void PlayDongDongDong()
    {
        AudioDongDongDong.Play();
    }

    public void PlayBombBong()
    {
        PlaySound(Audios[4]);
    }

    public void StopDongDongDong()
    {
        AudioDongDongDong.Stop();
    }

    public void PlayOneShot()
    {
        PlaySound(Audios[0]);
    }

    public void PlayDodgeSound()
    {
        PlaySound(Audios[1]);
    }

    public void PlayWalkSound()
    {
        AudioDaDaDa.Play();
    }

    public void StopWalkSound()
    {
        AudioDaDaDa.Stop();
    }

    bool isPlayWalkSound = false;

    void Update () {
        if (IsGameOver) return;
        Vector3 cameraPos = Camera.main.transform.localPosition;
        cameraPos.x = Player.GetComponent<ControllerRigidbody>().Position.x;
        cameraPos.y = Player.GetComponent<ControllerRigidbody>().Position.y;
        Camera.main.transform.localPosition = cameraPos;
        SortingZ();

        if( Player != null )
        {
            if (Player.GetComponent<ControllerRigidbody>().IsWalk)
            {
                if (!isPlayWalkSound)
                {
                    isPlayWalkSound = true;
                    PlayWalkSound();
                }
            }else
            {
                isPlayWalkSound = false;
                StopWalkSound();
            }
        }else
        {
            StopWalkSound();
        }
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

    void PlaySound( AudioClip ac )
    {
        AudioOneShot.clip = ac;
        AudioOneShot.PlayOneShot(ac);
    }
    
}
