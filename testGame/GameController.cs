using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using VicScript;
using UnityEngine.UI;
using System.Collections.Generic;
using VicScript.WongGesture;
using VicScript.WongWeaponSystem;

public class GameController : MonoBehaviour {

    public KeyboardEvent ke;
    public ViewController vc;
    public UIController uc;
    public WongGestureController wgc;

    public bool EnableShadow = true;
    public bool StopPlayerWhenTowFingerRelease = false;

    bool isDoubleHold = false;

    bool isClicked = false;

    /* 代表雙指劃過 */
    bool isDoubleFlicked = false;

    /* 代表單指劃過 */
    bool isFlicked = false;

    bool showConfig = false;

    
    public void ReStart()
    {
        vc.Player.Position = new Vector3();
        vc.ClearEnemy();
        for (int i = 0; i < 5; ++i)
        {
            CreateEnemy();
        }
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
        string[] ews = uc.GetWeaponListFromUI();
        vc.Player.GetComponent<WongWeaponController>().ClearWeapons();
        //vc.Player.weapons.Clear();
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
    /*
    void UsingConfig()
    {
        foreach (object[] c in GameConfig.WeaponConfig)
        {
            bool usingWeapon = (bool)c[13];
            if (usingWeapon)
            {
                bool autoWeapon = (bool)c[10];
                if (autoWeapon)
                {
                    vc.Player.weapons.Add(new AutoWeapon(vc.Player, vc, c));
                }
                else
                {
                    vc.Player.weapons.Add(new HalfAutoWeapon(vc.Player, vc, c));
                }
            }
        }
    }
    */
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
        
        ReStart();

    }

    private void OnOneFingerMoveAfterHold(Vector3 obj)
    {
        vc.Player.GetComponent<WongWeaponController>().KeepStartAim(obj);
      //  foreach (IWeapon w in vc.Player.weapons) w.KeepStartAim(obj);
    }

    private void OnTwoFingerMove(Vector3 obj)
    {
        vc.Player.SetPlayerPosition(obj);
    }

    private void OnTwoFingerFlicked(Vector3 obj)
    {
        Vector3 dir = obj.normalized;
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        vc.Player.SetPlayerPosition(fromVec + dir * GameConfig.LongMoveDistance);
    }

    private void OnTwoFingerClicked(Vector3 obj)
    {
        vc.Player.SetPlayerPosition(obj);
    }

    private void OnOneFingerMove(Vector3 obj)
    {
       // foreach (IWeapon w in vc.Player.weapons) w.MoveAim(obj);
        vc.Player.GetComponent<WongWeaponController>().MoveAim(obj);
    }

    private void OnOneFingerDown(Vector3 obj)
    {
      //  foreach (IWeapon w in vc.Player.weapons) w.StartAim(obj);
        vc.Player.GetComponent<WongWeaponController>().StartAim(obj);
    }

    private void OnOneFingerClicked(Vector3 obj)
    {
      //  foreach (IWeapon w in vc.Player.weapons) w.AimOnce(obj);
        vc.Player.GetComponent<WongWeaponController>().AimOnce(obj);
    }

    private void OnEachFingerUp()
    {
      //  foreach (IWeapon w in vc.Player.weapons) w.EndAim();
        vc.Player.GetComponent<WongWeaponController>().EndAim();
    }

    private void OnDoubleTwoFingerFlicked(Vector3 obj)
    {
        DodgePlayer( obj.normalized );
    }

    void CreateEnemy()
    {
        Vector3 pos = new Vector3();
        pos.x = UnityEngine.Random.value * 100;
        pos.y = UnityEngine.Random.value * -100;
        pos += vc.Player.Position;
        vc.CreateEnemy(pos);
    }

    void DodgePlayer(Vector3 dir)
    {
        uc.SetState("Dodge");
        vc.Player.DodgePlayer(dir, GameConfig.DodgeSpeed);
    }

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

    private void OnSpaceClick()
    {
        vc.Player.MakePlayerStop();
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
        vc.Player.SetPlayerPositionByScreenPos( Input.mousePosition );
    }


    private void OnDClick()
    {
        vc.Player.DodgePlayerByScreenPos(Input.mousePosition, GameConfig.DodgeSpeed);
    }

    private void OnFClick()
    {
        vc.Player.SetPlayerPositionByScreenPos( Input.mousePosition );
    }

    void FireOnce( Vector3 firePos )
    {
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        Vector3 dir = (firePos - fromVec).normalized;
        vc.CreateBullet(fromVec + dir * 50, dir, 100);
    }

    void FireSpecialOnce(Vector3 firePos)
    {
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        Vector3 dir = (firePos - fromVec).normalized;
        vc.CreateSpecialBullet(fromVec + dir * 50, dir, 100);
    }
    
    void Update () {
        /*
        if (EnableShadow)
        {
            if (isDoubleFlicked) vc.CreateShadow(vc.Player.Position, vc.Player.Scale);
        }
        */
       // vc.Player.GetComponent<WongWeaponController>().AimOnce(firePos);
      //  foreach (IWeapon w in vc.Player.weapons) w.Update();
        
    }
}
