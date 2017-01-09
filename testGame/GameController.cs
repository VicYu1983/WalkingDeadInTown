using UnityEngine;
using System.Collections;
using System;
using TouchScript.Gestures;
using VicScript;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public KeyboardEvent ke;
    public ViewController vc;
    public UIController uc;

    /* 代表雙指劃過 */
    bool isDoubleFlicked = false;

    /* 代表單指劃過 */
    bool isFlicked = false;

	// Use this for initialization
	void Start () {
        ke.OnFClick += OnFClick;
        ke.OnSpaceClick += OnSpaceClick;
        ke.OnFPress += OnFPress;
        
        vc.Player.GetComponent<TapGesture>().Tapped += OnPlayerTapped;
        // vc.Ground.GetComponent<TapGesture>().Tapped += OnTapped;
        vc.GamePage.GetComponent<PressGesture>().Pressed += OnGamePagePressed;
        vc.GamePage.GetComponent<ReleaseGesture>().Released += OnGamePageReleased;
        vc.GamePage.GetComponent<FlickGesture>().Flicked += OnGamePageFlicked;
        vc.GamePage.GetComponent<DoubleFlickedGesture>().OnDoubleFlickedEvent += OnGamePageDoubleFlicked;
       // foreach (GameObject e in vc.Enemys) e.GetComponent<TapGesture>().Tapped += OnEnemyTapped;
    }

    private void OnGamePageReleased(object sender, EventArgs e)
    {
        vc.ClearLastestAims();
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

        /* age, size, dragable, count, offset, expand_speed, delay */
        /* 步槍(半自動) */
        //vc.CreateAim(firePos, new object[] { 6, .3f, false, 3, 30.0f, 1.0f, false });

        /* 高性能狙擊槍 */
        //vc.CreateAim(firePos, new object[] { 60, 2.0f, false, 1, 0.0f, 0.1f, false });

        /* 雙管散彈槍 */
        //vc.CreateAim(firePos, new object[] { 6, .2f, false, 10, 20.0f, 1.0f, false });

        /* 智慧型狙擊槍 */
        //vc.CreateAim(firePos, new object[] { 300, 1.0f, true, 1, 0.0f, 0.02f, true });

        /* 雷射加農砲 */
        //vc.CreateAim(firePos, new object[] { 300, 1.0f, true, 1, 0.0f, 0.1f, true });

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
            vc.DodgePlayer(obj.ScreenFlickVector.normalized, 14000);

            /* 因為此操作會和單指操作衝突，因此加個flag來決定單指操作是否能觸發 */
            isDoubleFlicked = true;
            StartCoroutine(DelayCall(1.0f, () =>
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
            vc.SetPlayerPosition(fromVec + dir * 200);

            uc.SetState("Move Long Distance:" + dir * 200);

            /* 因為此操作會和雙指持續按壓地面的操作衝突，因此加個flag來決定雙指持續按壓地面的操作是否能觸發 */
            isFlicked = true;
            StartCoroutine(DelayCall( 1.0f, () =>
            {
                isFlicked = false;
            }));
        }
    }

    private void OnGoundClick()
    {
        print("DDD");
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
        vc.MakePlayerStop();
    }

    private void OnPlayerTapped(object sender, EventArgs e)
    {
        if ( GetTouchCount() == 2)
        {
            uc.SetState("Stop Player");
            vc.MakePlayerStop();
        }
    }
    
    private void OnTapped(object sender, EventArgs e)
    {
        //uc.SetState("OnGroundTapped");
    }

    private void OnFPress()
    {
        vc.SetPlayerPositionByScreenPos( Input.mousePosition );
    }

    private void OnFClick()
    {
        vc.SetPlayerPositionByScreenPos( Input.mousePosition );
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

    void FireSpecialOnce( Vector3 firePos )
    {
        Vector3 fromVec = vc.Player.GetComponent<RectTransform>().position;
        Vector3 dir = (firePos - fromVec).normalized;
        vc.CreateSpecialBullet(fromVec + dir * 50, dir, 100);
    }
    
    void Update () {

        /* 如果單指有劃過，就不觸動持續移動的功能 */
	    if( GetTouchCount() == 2 && !isFlicked )
        {
            vc.SetPlayerPositionByScreenPos(GetTouchPosition());

            uc.SetState( "Normal Move" );
        }
        
#if UNITY_EDITOR
        vc.DragAims(Camera.main.ScreenToWorldPoint( Input.mousePosition ));
#else
        if (GetTouchCount() == 1 && !isFlicked )
        {
            //vc.DragAims(Camera.main.ScreenToWorldPoint(GetTouchPosition()));
            
            Vector3 firePos = Camera.main.ScreenToWorldPoint(GetTouchPosition());
            
            /* 步槍:全自動 */
            vc.CreateAim(firePos, new object[] { 6, .1f, false, 1, 30.0f, 0.02f, false });
        }
#endif
    }
}
