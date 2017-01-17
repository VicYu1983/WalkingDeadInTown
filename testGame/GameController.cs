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

    bool isDoubleHold = false;

    bool isClicked = false;

    /* 代表雙指劃過 */
    bool isDoubleFlicked = false;

    /* 代表單指劃過 */
    bool isFlicked = false;

    bool showConfig = false;
    public void ToggleConfig( bool toggle )
    {
        showConfig = toggle;
        vc.Player.weapons.Clear();
        if (!showConfig)
        {
            UsingConfig();
        }
    }

    void UsingConfig()
    {
        foreach (object[] c in GameConfig.WeaponConfig)
        {
            bool usingWeapon = (bool)c[12];
            if (usingWeapon)
            {
                bool autoWeapon = (bool)c[10];
                if (autoWeapon)
                {
                    vc.Player.weapons.Add(new AutoWeapon(vc, c));
                }
                else
                {
                    vc.Player.weapons.Add(new HalfAutoWeapon(vc, c));
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
        ke.OnFClick += OnFClick;
        ke.OnSpaceClick += OnSpaceClick;
        ke.OnFPress += OnFPress;
        
        vc.Player.GetComponent<TapGesture>().Tapped += OnPlayerTapped;
        // vc.Ground.GetComponent<TapGesture>().Tapped += OnTapped;
        //vc.GamePage.GetComponent<PressGesture>().Pressed += OnGamePagePressed;
        vc.GamePage.GetComponent<LongPressGesture>().LongPressed += OnGamePageLongPressed;
        vc.GamePage.GetComponent<ReleaseGesture>().Released += OnGamePageReleased;
        vc.GamePage.GetComponent<FlickGesture>().Flicked += OnGamePageFlicked;
        vc.GamePage.GetComponent<DoubleFlickedGesture>().OnDoubleFlickedEvent += OnGamePageDoubleFlicked;
        // foreach (GameObject e in vc.Enemys) e.GetComponent<TapGesture>().Tapped += OnEnemyTapped;

        UsingConfig();
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
        }
#endif
        uc.SetState("OnGamePageLongPressed");
    }

    private void OnGamePageReleased(object sender, EventArgs e)
    {
        isDoubleHold = false;
        foreach (IWeapon w in vc.Player.weapons) w.EndAim();
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
        if ( GetTouchCount() == 2)
        {
            uc.SetState("Dodge");
            vc.Player.DodgePlayer(obj.ScreenFlickVector.normalized, GameConfig.DodgeSpeed);

            /* 因為此操作會和單指操作衝突，因此加個flag來決定單指操作是否能觸發 */
            isDoubleFlicked = true;
            StartCoroutine(DelayCall(.6f, () =>
            {
                isDoubleFlicked = false;
            }));
        }
    }

    private void OnGamePageFlicked(object sender, EventArgs e)
    {
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
    }
    
    private void OnEnemyTapped(object sender, EventArgs e)
    {
        string senderName = ((TapGesture)sender).gameObject.name;
        GameObject senderObject = vc.GetObjectFromObjectContainerByName(senderName);
        if (senderObject != null)
        {
            FireSpecialOnce(senderObject.GetComponent<RectTransform>().position);
        }
    }

    private void OnSpaceClick()
    {
        vc.Player.MakePlayerStop();
    }

    private void OnPlayerTapped(object sender, EventArgs e)
    {
        if ( GetTouchCount() == 2)
        {
            uc.SetState("Stop Player");
            vc.Player.MakePlayerStop();
        }
    }
    
    private void OnTapped(object sender, EventArgs e)
    {
        //uc.SetState("OnGroundTapped");
    }

    private void OnFPress()
    {
        vc.Player.SetPlayerPositionByScreenPos( Input.mousePosition );
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

        /* 如果單指有劃過，就不觸動持續移動的功能 */
	    if( GetTouchCount() == 2 && GetIsClick() && !isFlicked )
        {
            vc.Player.SetPlayerPositionByScreenPos(GetTouchPosition());
            uc.SetState( "Normal Move" );
        }
        
        if ( isDoubleHold && !isFlicked )
        {
            vc.Player.SetPlayerPositionByScreenPos(GetTouchPosition());
            uc.SetState("Normal Move");
        }

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

    void OnGUI()
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
                c[10] = GUILayout.Toggle((bool)c[10], (bool)c[10] ? "全自" : "半自" , style);
                c[11] = int.Parse(GUILayout.TextField(c[11] + "", style));
                c[12] = GUILayout.Toggle((bool)c[12], (bool)c[12] ? "裝備" : "不裝備", style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.EndHorizontal();
            }
        }
        
    }
    
}
