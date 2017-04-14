using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using VicScript;
using UnityEngine.UI;
using System.Collections.Generic;
using VicScript.WongGesture;
using VicScript.WongWeaponSystem;
using Assets.testGame;

public class GameController : MonoBehaviour {

    public KeyboardEvent ke;
    public ViewController vc;
    public UIController uc;
    public WongGestureController wgc;

    public bool EnableShadow = true;
    
    /* 代表雙指劃過 */
    bool isDoubleFlicked = false;
    

    
    public void ReStart()
    {
        vc.ClearPlayer();
        vc.ClearEnemy();
        vc.ClearBarrel();
        for (int i = 0; i < 5; ++i)
        {
            CreateEnemy();
        }
        for (int i = 0; i < 50; ++i)
        {
            CreateBarrels();
        }
        CreatePlayer();
        SetPlayerWeapons(0);
    }

    public void SetEnableShadow( bool e)
    {
        EnableShadow = e;
    }

    /*
    public void ToggleConfig( bool toggle )
    {
        showConfig = toggle;
        vc.Player.weapons.Clear();
        if (!showConfig)
        {
            UsingConfig();
        }
    }
    */
    
    public void SetPlayerWeapons( int value )
    {
        if (vc.IsGameOver) return;
        string[] ews = uc.GetWeaponListFromUI();
        vc.Player.GetComponent<WongWeaponController>().ClearWeapons();
        foreach (object[] c in GameConfig.WeaponConfig)
        {
            string wsName = c[0].ToString();
            string halfAutoName = ews[0];
            string halfAutoDelayName = ews[2];
            string autoName = ews[1];
            
            if (wsName == halfAutoName || wsName == halfAutoDelayName || wsName == autoName)
            {
                bool autoWeapon = (bool)c[10];
                if (autoWeapon)
                {
                    if(halfAutoDelayName == "遲發動(不裝備)")
                    {
                        vc.Player.GetComponent<WongWeaponController>().AddWeapon(c);
                    }
                }
                else
                {
                    vc.Player.GetComponent<WongWeaponController>().AddWeapon(c);
                }
            }
        }
        vc.Player.UpdateBody();
    }

    // Use this for initialization
    void Start () {
        ke.OnFClick += OnFClick;
        ke.OnSpaceClick += OnSpaceClick;
        ke.OnFPress += OnFPress;
        ke.OnDClick += OnDClick;

        wgc.OnDoubleTwoFingerFlicked += OnDoubleTwoFingerFlicked;
        wgc.OnEachFingerUp += OnEachFingerUp;
        wgc.OnOneFingerClicked += OnOneFingerClicked;
        wgc.OnOneFingerDown += OnOneFingerDown;
        wgc.OnOneFingerMoveAfterHold += OnOneFingerMoveAfterHold;
        wgc.OnOneFingerMove += OnOneFingerMove;
        wgc.OnTwoFingerClicked += OnTwoFingerClicked;
        wgc.OnTwoFingerFlicked += OnTwoFingerFlicked;
        wgc.OnTwoFingerMove += OnTwoFingerMove;
        //wgc.OnDoubleTwoFingerClicked += OnDoubleTwoFingerClicked;

        vc.AimViewController.OnWeaponFireOnce += OnWeaponFireOnce;

        ReStart();
        //Time.timeScale = 0;
    }
    /*
    private void OnDoubleTwoFingerClicked(Vector3 obj)
    {
        DodgePlayer(( obj - vc.Player.Position ).normalized, GameConfig.DodgeSpeed);
    }
    */
    private void OnWeaponFireOnce(IWeapon weapon, Vector3 to)
    {
        PlayerController owner = weapon.Owner.GetComponent<PlayerController>();
        if( owner == vc.Player) {
            ProcessPlayerAttack(owner.gameObject, weapon, to);
        }else
        {
            StartCoroutine(DelayProcessPlayerAttack(owner, weapon, to));
        }
    }

    void ProcessPlayerAttack(GameObject owner, IWeapon weapon, Vector3 to)
    {
        ControllerRigidbody crb = owner.GetComponent<ControllerRigidbody>();
        

        if (crb == null) return;
        Vector3 fromPos = crb.Position;

        if (weapon.IsBlade())
        {
            PlayerController pc = owner.GetComponent<PlayerController>();
            Vector3 diff = to - crb.Position;
            pc.GetComponent<ControllerRigidbody>().Dodge(diff, GameConfig.DodgeSpeed * 1.2f);
            vc.CreateRayLine(weapon, fromPos, to, true);
        }
        else
        {
            vc.CreateRayLine(weapon, fromPos, to);
        }
    }

    IEnumerator DelayProcessPlayerAttack(PlayerController owner, IWeapon weapon, Vector3 to)
    {
        yield return new WaitForSeconds(1);
        try
        {
            ProcessPlayerAttack(owner.gameObject, weapon, to);
        }
        catch( MissingReferenceException e)
        {
            print(e);
        }
    }

    private void OnOneFingerMoveAfterHold(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<WongWeaponController>().KeepStartAim(obj);
    }

    private void OnTwoFingerMove(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<ControllerRigidbody>().SetPlayerPosition(obj);
    }

    private void OnTwoFingerFlicked(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        Vector3 dir = obj.normalized;
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        vc.Player.GetComponent<ControllerRigidbody>().SetPlayerPosition(fromVec + dir * GameConfig.LongMoveDistance);
    }

    bool twoFingerClicked = false;

    private void OnTwoFingerClicked(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        //vc.Player.SetPlayerPosition(obj);

        if (twoFingerClicked)
        {
            DodgePlayer((obj - vc.Player.GetComponent<ControllerRigidbody>().Position).normalized, GameConfig.DodgeSpeed);
            twoFingerClicked = false;
        }else
        {
            vc.Player.GetComponent<ControllerRigidbody>().SetPlayerPosition(obj);
        }
        twoFingerClicked = true;
        StartCoroutine(DelayCall(.5f, () =>
        {
            twoFingerClicked = false;
        }));
    }

    private void OnOneFingerMove(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<WongWeaponController>().MoveAim(obj);
    }

    private void OnOneFingerDown(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<WongWeaponController>().StartAim(obj);
        vc.PlayDongDongDong();
    }

    private void OnOneFingerClicked(Vector3 obj)
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<WongWeaponController>().AimOnce(obj);
        vc.PlayOneShot();
    }

    private void OnEachFingerUp(Vector3 pos)
    {
        if (vc.IsGameOver) return;
#if UNITY_EDITOR
        vc.Player.GetComponent<WongWeaponController>().AimOnce(pos);
#else
        vc.Player.GetComponent<WongWeaponController>().EndAim();
        vc.StopDongDongDong();
#endif
    }

    private void OnDoubleTwoFingerFlicked(Vector3 obj)
    {
        DodgePlayer( obj.normalized, GameConfig.DodgeSpeed );
    }

    void CreatePlayer()
    {
        GameObject player = vc.CreatePlayer();
        player.name = "Player";
        vc.Player.GetComponent<ControllerRigidbody>().OnHitEvent += OnEnemyHit;

        WongWeaponController wwc = player.GetComponent<WongWeaponController>();
        wwc.AimViewController = vc.AimViewController;
    }

    void CreateBarrels()
    {
        Vector3 pos = new Vector3();
        pos.x = UnityEngine.Random.value * 3000 - 1500;
        pos.y = UnityEngine.Random.value * 3000 - 1500;
        GameObject barrel = vc.CreateBarrel(pos);
        barrel.GetComponent<ControllerHP>().HP = 120;
        ControllerRigidbody bc = barrel.GetComponent<ControllerRigidbody>();
        bc.OnHitEvent += OnBarrelHit;
    }

    private void OnBarrelHit(ControllerRigidbody barrel, GameObject rayLine)
    {
        ControllerHP ch = barrel.GetComponent<ControllerHP>();
        ch.HP -= 10;
        if( ch.IsDead() )
        {
            barrel.OnHitEvent -= OnBarrelHit;
            barrel.gameObject.GetComponent<Animator>().SetBool("Die", true);
            vc.PlayBombBong();
            ExplodeWind(barrel.GetComponent<RectTransform>().position);
        }
        else
        {
            RaylineModel rm = rayLine.GetComponent<RaylineModel>();
            if(rm != null)
            {
                Vector3 from = rm.fromPos;
                Vector3 offset = barrel.GetComponent<RectTransform>().position - from;
                offset.Normalize();
                barrel.GetComponent<Rigidbody2D>().AddForce(offset * 30);
            }
        }
    }

    private void ExplodeWind( Vector3 pos )
    {
        List<GameObject> newlist = new List<GameObject>(vc.ForSortingZ);
        foreach( GameObject g in newlist)
        {
            if(g != null)
            {
                Vector3 gPos = g.GetComponent<RectTransform>().position;
                Vector3 dir = gPos - pos;
                if (dir.magnitude < 100)
                {
                    g.GetComponent<Rigidbody2D>().AddForce(dir.normalized * 60);
                    if (g.GetComponent<PlayerController>() != null)
                    {
                        ProcessHitEffect(g.gameObject, gPos, pos, 20, 50);
                    }
                }
            }
            
        }
    }
    

    void CreateEnemy()
    {
        Vector3 pos = new Vector3();
        pos.x = UnityEngine.Random.value * 100;
        pos.y = UnityEngine.Random.value * -100;
        GameObject enemy = vc.CreateEnemy(pos);
        PlayerController ep = enemy.GetComponent<PlayerController>();
        ep.GetComponent<ControllerHP>().HP = 100;
        ep.GetComponent<ControllerRigidbody>().OnHitEvent += OnEnemyHit;
        ep.GetComponent<AgeCalculator>().DeadAge = Mathf.FloorToInt(UnityEngine.Random.value * 1000) + 500;
        ep.GetComponent<AgeCalculator>().OnDeadEvent += OnEnemySpeakEvent;
        ep.gameObject.name = "enemy";

        AIMove moveAi = new AIMove();
        moveAi.PlayerController = ep;
        moveAi.ViewController = vc;
        ep.AIMove = moveAi;

        WongWeaponController wwc = enemy.GetComponent<WongWeaponController>();
        wwc.AimViewController = vc.AimViewController;
        wwc.AddWeapon(new object[] { "步槍(半自動)", 10, .6f, false, 5, 30.0f, 0.5f, false, 0.0f, false, false, 5, false, true });
        /*
        float wid = UnityEngine.Random.value;
        if( wid < .6f)
        {
            wwc.AddWeapon(GameConfig.WeaponConfig[1]);
        }
        else if( wid < .8f)
        {
            wwc.AddWeapon(GameConfig.WeaponConfig[5]);
        }
        else
        {
            wwc.AddWeapon(GameConfig.WeaponConfig[0]);
        }
        */
        AIWeapon weaponAI = new AIWeapon();
        weaponAI.PlayerController = ep;
        weaponAI.WongWeaponController = wwc;
        weaponAI.ViewController = vc;
        ep.AIWeapon = weaponAI;
    }

    private void OnEnemySpeakEvent(AgeCalculator obj)
    {
        if( obj != null)
        {
            obj.ResetAge();
            obj.DeadAge = Mathf.FloorToInt(UnityEngine.Random.value * 1000) + 500;
            
            StartCoroutine(vc.DisplayPlayerSpeak(obj.gameObject));
        }
    }

    void DodgePlayer(Vector3 dir, float force)
    {
        if (vc.IsGameOver) return;

        uc.SetState("Dodge");
        vc.Player.GetComponent<ControllerRigidbody>().Dodge(dir, force);
        vc.PlayDodgeSound();

        isDoubleFlicked = true;
        StartCoroutine(DelayCall(.3f, () =>
        {
            isDoubleFlicked = false;
        }));
    }

    private void OnEnemyHit(ControllerRigidbody beenHit, GameObject other)
    {
        if (other.name.IndexOf("RayLineObject") != -1)
        {
            RaylineModel rm = other.GetComponent<RaylineModel>();
            RayLineView rv = other.GetComponent<RayLineView>();
            try
            {
                if (rv.Weapon.Owner.GetComponent<PlayerController>() == beenHit) return;
            }
            catch
            {
                //maybe player die now!
            }

            ProcessHitEffect(beenHit.gameObject, rm.targetPos, rm.fromPos, rm.speed, 10);
            /*
            Vector3 dir = rm.targetPos - rm.fromPos;
            beenHit.Hit(dir, rm.speed, 10);
            
            if( beenHit.HP == 0)
            {
                beenHit.OnHitEvent = null;
                beenHit.GetComponent<AgeCalculator>().OnDeadEvent = null;
                vc.CreateExplodeEffect(beenHit.Position, beenHit.color);
                vc.DestoryEnemy(beenHit.gameObject);
                vc.PlayDeadSound();
            }
            else
            {
                if( beenHit == vc.Player)
                {
                    vc.MakeEnemyHitEffect(beenHit, Color.white );
                }
                else
                {
                    vc.MakeEnemyHitEffect(beenHit, vc.EnemyColor);
                }
                
            }
            */
            Destroy(other.gameObject);
        }
        
    }

    void ProcessHitEffect(GameObject beenHit, Vector3 hitPos, Vector3 fromPos, float power, int damage)
    {
        ControllerRigidbody crb = beenHit.GetComponent<ControllerRigidbody>();
        ControllerHP ch = beenHit.GetComponent<ControllerHP>();
        ControllerColor cc = beenHit.GetComponent<ControllerColor>();

        Vector3 dir = hitPos - fromPos;
        crb.Hit(dir, power);
        ch.Hit(damage);

        if (ch.IsDead() )
        {
            crb.OnHitEvent = null;
            beenHit.GetComponent<AgeCalculator>().OnDeadEvent = null;
            vc.CreateExplodeEffect(crb.Position, cc.color);
            vc.DestoryEnemy(beenHit.gameObject);
            vc.PlayDeadSound();
        }
        else
        {
            try
            {
                if (beenHit.gameObject == vc.Player.gameObject)
                {
                    vc.MakeEnemyHitEffect(beenHit.gameObject, Color.white);
                }
                else
                {
                    vc.MakeEnemyHitEffect(beenHit.gameObject, vc.EnemyColor);
                }
            }
            catch( NullReferenceException e)
            {
                print(e);
            }

        }
    }

    /*
    private void OnEnemyTapped(object sender, EventArgs e)
    {
        string senderName = ((TapGesture)sender).gameObject.name;
        GameObject senderObject = vc.GetObjectFromObjectContainerByName(senderName);
        if (senderObject != null)
        {
            Vector3 firePos = senderObject.GetComponent<RectTransform>().position;
            vc.Player.GetComponent<WongWeaponController>().AimOnce(firePos);
          //  foreach (IWeapon w in vc.Player.weapons) w.AimOnce(firePos);
        }
    }
    */
    private void OnSpaceClick()
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<ControllerRigidbody>().MakePlayerStop();
        uc.SetState("Stop Player");
    }
    /*
    private void OnPlayerTapped(object sender, EventArgs e)
    {
        if ( GetTouchCount() == 2 && GetIsClick() )
        {
            uc.SetState("Stop Player");
            vc.Player.MakePlayerStop();
        }
    }
    */
    private void OnTapped(object sender, EventArgs e)
    {
        uc.SetState("OnGroundTapped");
    }

    private void OnFPress()
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<ControllerRigidbody>().SetPlayerPositionByScreenPos( Input.mousePosition );
    }


    private void OnDClick()
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<ControllerRigidbody>().DodgePlayerByScreenPos(Input.mousePosition, GameConfig.DodgeSpeed);
    }

    private void OnFClick()
    {
        if (vc.IsGameOver) return;
        vc.Player.GetComponent<ControllerRigidbody>().SetPlayerPositionByScreenPos( Input.mousePosition );
    }
    /*
    void FireOnce( Vector3 firePos )
    {
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        Vector3 dir = (firePos - fromVec).normalized;
        vc.CreateBullet(fromVec + dir * 50, dir, 100);
    }
    */
    /*
    void FireSpecialOnce(Vector3 firePos)
    {
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        Vector3 dir = (firePos - fromVec).normalized;
        vc.CreateSpecialBullet(fromVec + dir * 50, dir, 100);
    }
    */
    void Update () {
        
        if (EnableShadow)
        {
            if (vc.Player == null) return;
            if (isDoubleFlicked) vc.CreateShadow(vc.Player.GetComponent<ControllerRigidbody>().Position, vc.Player.Scale);
        }
    }

    IEnumerator DelayCall(float time, Action doAction)
    {
        yield return new WaitForSeconds(time);
        doAction();
    }
}
