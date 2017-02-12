using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;
using VicScript;

public class WongGestureController : MonoBehaviour {

   // public GameObject Player;
    public GameObject TouchScreen;

   // public Action OnStopPlayer;
    public Action<Vector3> OnOneFingerDown;
    public Action OnEachFingerUp;
    public Action<Vector3> OnOneFingerClicked;
    public Action<Vector3> OnOneFingerMove;
    public Action<Vector3> OnOneFingerMoveAfterHold;
    public Action<Vector3> OnTwoFingerClicked;
    public Action<Vector3> OnTwoFingerMove;
    public Action<Vector3> OnTwoFingerFlicked;
    public Action<Vector3> OnDoubleTwoFingerFlicked;
    
    bool isDoubleHold = false;
    bool isDoubleFlicked = false;
    bool isFlicked = false;
    bool isClicked = false;
    int judgeIsHoldTime = 0;

    // Use this for initialization
    void Start () {
       // Player.GetComponent<TapGesture>().Tapped += OnPlayerTapped;
        TouchScreen.GetComponent<LongPressGesture>().LongPressed += OnGamePageLongPressed;
        TouchScreen.GetComponent<Button>().onClick.AddListener(OnGamePageClick);
        TouchScreen.GetComponent<FlickGesture>().Flicked += OnGamePageFlicked;
        TouchScreen.GetComponent<DoubleFlickedGesture>().OnDoubleFlickedEvent += OnGamePageDoubleFlicked;
    }
	
	// Update is called once per frame
	void Update () {
        /* 是點擊、非swipe、非連續swipe、兩指在屏上才會觸發 */
        if (GetTouchCount() == 2 && GetIsClick() && !isFlicked && !isDoubleFlicked)
        {
           // Player.SetPlayerPositionByScreenPos(GetTouchPosition());
            // uc.SetState("Normal Move");

            if (OnTwoFingerClicked != null) OnTwoFingerClicked.Invoke( Camera.main.ScreenToWorldPoint(GetTouchPosition()));
        }

        /* 如果單指有劃過，就不觸動持續移動的功能 */
        if (isDoubleHold && !isFlicked && !isDoubleFlicked)
        {
            //Player.SetPlayerPositionByScreenPos(GetLastTouchPosition());
            if (OnTwoFingerMove != null) OnTwoFingerMove.Invoke(Camera.main.ScreenToWorldPoint(GetTouchPosition()));
            //  uc.SetState("Normal Move");
        }
        /*
        if (EnableShadow)
        {
            if (isDoubleFlicked) vc.CreateShadow(vc.Player.Position, vc.Player.Scale);
        }
        */
        
        if (GetTouchCount() == 1 )
        {
            //foreach (IWeapon w in vc.Player.weapons) w.MoveAim(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (OnOneFingerMove != null) OnOneFingerMove.Invoke(Camera.main.ScreenToWorldPoint(GetTouchPosition()));
            if ( GetIsClick() )
            {
                if( !isClicked ){
                    isClicked = true;
                    judgeIsHoldTime = 0;
                    StartCoroutine(DelayCall(.3f, () =>
                    {
                        isClicked = false;
                    }));
                    if (OnOneFingerClicked != null) OnOneFingerClicked.Invoke(Camera.main.ScreenToWorldPoint(GetTouchPosition()));
                   // Vector3 firePos = Camera.main.ScreenToWorldPoint(GetTouchPosition());
                   // foreach (IWeapon w in vc.Player.weapons) w.AimOnce(firePos);
                }
            }
            else
            {
                judgeIsHoldTime++;
                if( judgeIsHoldTime >= 5 ){
                  //  Vector3 firePos = Camera.main.ScreenToWorldPoint(GetTouchPosition());
                  //  foreach (IWeapon w in vc.Player.weapons) w.KeepStartAim(firePos);

                    if (OnOneFingerMoveAfterHold != null) OnOneFingerMoveAfterHold.Invoke(Camera.main.ScreenToWorldPoint(GetTouchPosition()));
                }
            }
        }else{
            judgeIsHoldTime = 0;
        }
       // foreach (IWeapon w in vc.Player.weapons) w.Update();
    }


    private void OnPlayerTapped(object sender, EventArgs e)
    {
        if (GetTouchCount() == 2 && GetIsClick())
        {
           // if (OnStopPlayer != null) OnStopPlayer.Invoke();
        }
    }


    private void OnGamePageLongPressed(object sender, EventArgs e)
    {
        if ( Input.touchCount == 1 )
        {
            /*
            Vector3 touchPos = vc.GamePage.GetComponent<LongPressGesture>().ScreenPosition;
            Vector3 firePos = Camera.main.ScreenToWorldPoint(touchPos);
            foreach (IWeapon w in vc.Player.weapons) w.StartAim(firePos);
            */
            Vector3 touchPos = TouchScreen.GetComponent<LongPressGesture>().ScreenPosition;
            if (OnOneFingerDown != null) OnOneFingerDown(Camera.main.ScreenToWorldPoint(touchPos));
        }
        else if ( Input.touchCount == 2 ){
            
            isDoubleHold = true;
           // uc.SetState("OnGamePageLongPressed: isDoubleHold: " + isDoubleHold);
        }
      //  uc.SetState("OnGamePageLongPressed");
    }

    private void OnGamePageClick()
    {
        isDoubleHold = false;
        if (OnEachFingerUp != null) OnEachFingerUp.Invoke();
        //uc.SetState("OnGamePageReleased: isDoubleHold: " + isDoubleHold);
        /*
        foreach (IWeapon w in vc.Player.weapons) w.EndAim();

        if (StopPlayerWhenTowFingerRelease)
        {
            if (GetTouchCount() == 2)
            {
                vc.Player.StopMove();
                uc.SetState("StopMove");
            }
        }
        */
    }

    private void OnGamePageFlicked(object sender, EventArgs e)
    {
#if UNITY_EDITOR

        if (!isDoubleFlicked)
        {
            Vector3 dir = TouchScreen.GetComponent<FlickGesture>().ScreenFlickVector.normalized;
            if (OnTwoFingerFlicked != null) OnTwoFingerFlicked.Invoke(dir);

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
            Vector3 dir = TouchScreen.GetComponent<FlickGesture>().ScreenFlickVector.normalized;
            if (OnTwoFingerFlicked != null) OnTwoFingerFlicked.Invoke(dir);

            /* 因為此操作會和雙指持續按壓地面的操作衝突，因此加個flag來決定雙指持續按壓地面的操作是否能觸發 */
            isFlicked = true;
            StartCoroutine(DelayCall( .6f, () =>
            {
                isFlicked = false;
            }));
        }
#endif
    }


    private void OnGamePageDoubleFlicked(FlickGesture obj)
    {
#if UNITY_EDITOR
        if (OnDoubleTwoFingerFlicked != null)
        {
            OnDoubleTwoFingerFlicked.Invoke(obj.ScreenFlickVector);
            /* 因為此操作會和單指操作衝突，因此加個flag來決定單指操作是否能觸發 */
            isDoubleFlicked = true;
            StartCoroutine(DelayCall(.3f, () =>
            {
                isDoubleFlicked = false;
            }));
        }
#else
        if ( GetTouchCount() == 2)
        {
            if (OnDoubleTwoFingerFlicked != null) OnDoubleTwoFingerFlicked.Invoke(obj.ScreenFlickVector);
            /* 因為此操作會和單指操作衝突，因此加個flag來決定單指操作是否能觸發 */
            isDoubleFlicked = true;
            StartCoroutine(DelayCall(.3f, () =>
            {
                isDoubleFlicked = false;
            }));
        }
#endif
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
        foreach (Touch t in Input.touches)
        {
            pos += t.position;
        }
        pos /= touchCount;
        return new Vector3(pos.x, pos.y, 0);
    }

    IEnumerator DelayCall(float time, Action doAction)
    {
        yield return new WaitForSeconds(time);
        doAction();
    }
}
