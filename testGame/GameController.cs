using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using VicScript;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public KeyboardEvent ke;
    public ViewController vc;
    public UIController uc;

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
        vc.Player.weapons.Clear();
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
                        vc.Player.weapons.Add(new AutoWeapon(vc.Player, vc, c));
                    }
                }
                else
                {
                    vc.Player.weapons.Add(new HalfAutoWeapon(vc.Player, vc, c));
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
        
        vc.Player.GetComponent<TapGesture>().Tapped += OnPlayerTapped;
        // vc.Ground.GetComponent<TapGesture>().Tapped += OnTapped;
        //vc.GamePage.GetComponent<PressGesture>().Pressed += OnGamePagePressed;
        vc.GamePage.GetComponent<LongPressGesture>().LongPressed += OnGamePageLongPressed;

        /* 有時會因為gesture彼此間的衝突而不會觸發此事件，因此改用click比較穩 */
        //vc.GamePage.GetComponent<ReleaseGesture>().Released += OnGamePageReleased;

        vc.GamePage.GetComponent<Button>().onClick.AddListener(OnGamePageClick);
        vc.GamePage.GetComponent<FlickGesture>().Flicked += OnGamePageFlicked;
        vc.GamePage.GetComponent<DoubleFlickedGesture>().OnDoubleFlickedEvent += OnGamePageDoubleFlicked;

        ReStart();

    }

    void CreateEnemy()
    {
        Vector3 pos = new Vector3();
        pos.x = UnityEngine.Random.value * 100;
        pos.y = UnityEngine.Random.value * -100;
        pos += vc.Player.Position;
        vc.CreateEnemy(pos);
    }

    private void OnGamePageLongPressed(object sender, EventArgs e)
    {
#if UNITY_EDITOR
#else
        if ( Input.touchCount == 1 )
        {
#endif
            Vector3 touchPos = vc.GamePage.GetComponent<LongPressGesture>().ScreenPosition;
            Vector3 firePos = Camera.main.ScreenToWorldPoint(touchPos);
            foreach (IWeapon w in vc.Player.weapons) w.StartAim(firePos);
#if UNITY_EDITOR
#else
        }
        else if ( Input.touchCount == 2 ){
            
            isDoubleHold = true;
           // uc.SetState("OnGamePageLongPressed: isDoubleHold: " + isDoubleHold);
        }
#endif
        uc.SetState("OnGamePageLongPressed");
    }

    private void OnGamePageClick()
    {
        isDoubleHold = false;

        //uc.SetState("OnGamePageReleased: isDoubleHold: " + isDoubleHold);
        foreach (IWeapon w in vc.Player.weapons) w.EndAim();

        if (StopPlayerWhenTowFingerRelease)
        {
            if (GetTouchCount() == 2)
            {
                vc.Player.StopMove();
                uc.SetState("StopMove");
            }
        }
    }

    private void OnGamePagePressed(object sender, EventArgs e)
    {
#if UNITY_EDITOR
#else
        if ( Input.touchCount == 1)
        {
#endif
        Vector3 touchPos = vc.GamePage.GetComponent<PressGesture>().ScreenPosition;
        Vector3 firePos = Camera.main.ScreenToWorldPoint(touchPos);
        foreach (IWeapon w in vc.Player.weapons) w.StartAim(firePos);
#if UNITY_EDITOR
#else
        }
#endif
        uc.SetState("OnGamePagePressed");
    }

    private void OnGamePageDoubleFlicked(FlickGesture obj)
    {
#if UNITY_EDITOR
        DodgePlayer(obj.ScreenFlickVector.normalized);
#else
        if ( GetTouchCount() == 2)
        {
            DodgePlayer( obj.ScreenFlickVector.normalized );
        }
#endif
    }

    void DodgePlayer(Vector3 dir)
    {
        uc.SetState("Dodge");
        vc.Player.DodgePlayer(dir, GameConfig.DodgeSpeed);

        /* 因為此操作會和單指操作衝突，因此加個flag來決定單指操作是否能觸發 */
        isDoubleFlicked = true;
        StartCoroutine(DelayCall(.3f, () =>
        {
            isDoubleFlicked = false;
        }));
    }

    private void OnGamePageFlicked(object sender, EventArgs e)
    {
#if UNITY_EDITOR
        if (!isDoubleFlicked)
        {
            Vector3 dir = vc.GamePage.GetComponent<FlickGesture>().ScreenFlickVector.normalized;
            Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
            vc.Player.SetPlayerPosition(fromVec + dir * GameConfig.LongMoveDistance);

            uc.SetState("Move Long Distance:" + dir * GameConfig.LongMoveDistance);

            /* 因為此操作會和雙指持續按壓地面的操作衝突，因此加個flag來決定雙指持續按壓地面的操作是否能觸發 */
            isFlicked = true;
            StartCoroutine(DelayCall(.6f, () =>
            {
                isFlicked = false;
            }));
        }

#else
        if ( GetTouchCount() == 2 && !isDoubleFlicked )
        {
            Vector3 dir = vc.GamePage.GetComponent<FlickGesture>().ScreenFlickVector.normalized;
            Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
            vc.Player.SetPlayerPosition(fromVec + dir * GameConfig.LongMoveDistance);

            uc.SetState("Move Long Distance:" + dir * GameConfig.LongMoveDistance);

            /* 因為此操作會和雙指持續按壓地面的操作衝突，因此加個flag來決定雙指持續按壓地面的操作是否能觸發 */
            isFlicked = true;
            StartCoroutine(DelayCall( .6f, () =>
            {
                isFlicked = false;
            }));
        }
#endif

    }

    private void OnEnemyTapped(object sender, EventArgs e)
    {
        string senderName = ((TapGesture)sender).gameObject.name;
        GameObject senderObject = vc.GetObjectFromObjectContainerByName(senderName);
        if (senderObject != null)
        {
            Vector3 firePos = senderObject.GetComponent<RectTransform>().position;
            foreach (IWeapon w in vc.Player.weapons) w.AimOnce(firePos);
        }
    }

    private void OnSpaceClick()
    {
        vc.Player.MakePlayerStop();
        uc.SetState("Stop Player");
    }

    private void OnPlayerTapped(object sender, EventArgs e)
    {
        if ( GetTouchCount() == 2 && GetIsClick() )
        {
            uc.SetState("Stop Player");
            vc.Player.MakePlayerStop();
        }
    }
    
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

    IEnumerator DelayCall( float time, Action doAction )
    {
        yield return new WaitForSeconds( time );
        doAction();
    }

    int GetTouchCount()
    {
        return GetComponent<DetectTouchCountByPassTime>().TouchCount;
    }

    bool GetIsClick()
    {
        return GetComponent<DetectTouchCountByPassTime>().IsClick;
    }

    Vector3 GetTouchPosition()
    {
        return GetComponent<DetectTouchCountByPassTime>().GetAverageTouchPosition();
    }

    Vector3 GetLastTouchPosition()
    {
        Vector2 pos = new Vector2();
        int touchCount = Input.touchCount;
        foreach( Touch t in Input.touches)
        {
            pos += t.position;
        }
        pos /= touchCount;
        return new Vector3(pos.x, pos.y, 0);
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

    int judgeIsHoldTime = 0;
    
    void Update () {

        /* 是點擊、非swipe、非連續swipe、兩指在屏上才會觸發 */
	    if( GetTouchCount() == 2 && GetIsClick() && !isFlicked && !isDoubleFlicked)
        {
            vc.Player.SetPlayerPositionByScreenPos(GetTouchPosition());
            uc.SetState( "Normal Move" );
        }

        /* 如果單指有劃過，就不觸動持續移動的功能 */
        if ( isDoubleHold && !isFlicked && !isDoubleFlicked)
        {
            vc.Player.SetPlayerPositionByScreenPos(GetLastTouchPosition());
            uc.SetState("Normal Move");
        }

        if( EnableShadow )
        {
            if(isDoubleFlicked) vc.CreateShadow(vc.Player.Position, vc.Player.Scale);
        }

        //uc.SetState("isDoubleHold: " + isDoubleHold);
        //uc.SetState(GetComponent<DetectTouchCountByPassTime>().GetTouchString());
        //uc.SetState("judgeIsHoldTime: " + judgeIsHoldTime.ToString());

#if UNITY_EDITOR
        foreach (IWeapon w in vc.Player.weapons) w.MoveAim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        foreach (IWeapon w in vc.Player.weapons) w.KeepStartAim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
#else
        if (GetTouchCount() == 1 )
        {
            foreach (IWeapon w in vc.Player.weapons) w.MoveAim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if( GetIsClick() )
            {
                if( !isClicked ){
                    isClicked = true;
                    judgeIsHoldTime = 0;
                    StartCoroutine(DelayCall(.3f, () =>
                    {
                        isClicked = false;
                    }));

                    Vector3 firePos = Camera.main.ScreenToWorldPoint(GetTouchPosition());
                    foreach (IWeapon w in vc.Player.weapons) w.AimOnce(firePos);
                }
            }
            else
            {
                judgeIsHoldTime++;
                if( judgeIsHoldTime >= 5 ){
                    Vector3 firePos = Camera.main.ScreenToWorldPoint(GetTouchPosition());
                    foreach (IWeapon w in vc.Player.weapons) w.KeepStartAim(firePos);
                }
            }
        }else{
            judgeIsHoldTime = 0;
        }
#endif
        foreach (IWeapon w in vc.Player.weapons) w.Update();
    }
    /*
    void OnGUI()
    {
        CreateConfigGUI();
    }*/

    void CreateConfigGUI()
    {
        if (showConfig)
        {
            GUIStyle style = new GUIStyle();
#if UNITY_EDITOR
            style.fontSize = 10;
            style.fixedWidth = 40;
#else
            style.fontSize = 30;
            style.fixedWidth = 140;
#endif
            style.margin = new RectOffset(10, 10, 5, 5);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.LowerRight;

            GUILayout.BeginHorizontal();
            GUILayout.TextField("移動速度", style);
            GUILayout.TextField("閃避速度", style);
            GUILayout.TextField("長距離移動", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GameConfig.MoveSpeed = float.Parse(GUILayout.TextField(GameConfig.MoveSpeed + "", style));
            GameConfig.DodgeSpeed = float.Parse(GUILayout.TextField(GameConfig.DodgeSpeed + "", style));
            GameConfig.LongMoveDistance = int.Parse(GUILayout.TextField(GameConfig.LongMoveDistance + "", style));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.TextField("武器名稱", style);
            GUILayout.TextField("準星生命", style);
            GUILayout.TextField("準星大小", style);
            GUILayout.TextField("可拖動", style);
            GUILayout.TextField("數量", style);
            GUILayout.TextField("偏移量", style);
            GUILayout.TextField("擴張速度", style);
            GUILayout.TextField("延遲射擊", style);
            GUILayout.TextField("起使大小", style);
            GUILayout.TextField("離開消失", style);
            GUILayout.TextField("全/半自動", style);
            GUILayout.TextField("時長", style);
            GUILayout.TextField("刀裝備", style);
            GUILayout.TextField("裝備", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            /* name, age, size, dragable, count, offset, expand_speed, delay, startSize, clearWhenRelease */

            foreach (object[] c in GameConfig.WeaponConfig)
            {
                GUILayout.BeginHorizontal();
                c[0] = GUILayout.TextField((string)c[0], style);
                //c[1] = (int)GUILayout.HorizontalSlider((int)c[1], 0, 300);
                c[1] = int.Parse(GUILayout.TextField(c[1] + "", style));
                c[2] = float.Parse(GUILayout.TextField(c[2] + "", style));
                c[3] = GUILayout.Toggle((bool)c[3], (bool)c[3] ? "可" : "不可", style);
                c[4] = int.Parse(GUILayout.TextField(c[4] + "", style));
                c[5] = float.Parse(GUILayout.TextField(c[5] + "", style));
                c[6] = float.Parse(GUILayout.TextField(c[6] + "", style));
                c[7] = GUILayout.Toggle((bool)c[7], (bool)c[7] ? "延遲" : "非延遲", style);
                c[8] = float.Parse(GUILayout.TextField(c[8] + "", style));
                c[9] = GUILayout.Toggle((bool)c[9], (bool)c[9] ? "消失" : "不消失", style);
                c[10] = GUILayout.Toggle((bool)c[10], (bool)c[10] ? "全自" : "半自", style);
                c[11] = int.Parse(GUILayout.TextField(c[11] + "", style));
                c[12] = GUILayout.Toggle((bool)c[12], (bool)c[12] ? "是" : "否", style);
                c[13] = GUILayout.Toggle((bool)c[13], (bool)c[13] ? "裝備" : "不裝備", style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();
            }
        }
    }
}
