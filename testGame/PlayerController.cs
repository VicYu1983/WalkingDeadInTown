﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

   // public Sprite[] bodySprites;
    public GameObject body;
    public GameObject foot;
    public Color color;
    public Action<GameObject,GameObject> OnHitEvent;

    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);
    Vector3 currentDir = new Vector3(0, -1, 0);

    /* 持有武器 */
    public List<IWeapon> weapons = new List<IWeapon>();

    Vector3? targetPos;

    public AIBasic AIMove;
    public List<AIBasic> AIWeapons = new List<AIBasic>();
    
    public Vector3 Position
    {
        set
        {
            this.GetComponent<RectTransform>().position = value;
        }
        get
        {
            return this.GetComponent<RectTransform>().position;
        }
    }

    bool _isAim = false;
    public bool IsAim
    {
        set
        {
            _isAim = value;
        }
        get
        {
            return _isAim;
        }
    }

    public void SetPlayerPositionByScreenPos(Vector3 screenPos)
    {
        targetPos = GetMousePositionOnWorld(screenPos);
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public void DodgePlayer(Vector3 dir, float force)
    {
        SetPlayerForce(dir, force);
    }

    public void MakePlayerStop()
    {
        Vector3 playerAcc = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().AddForce(-playerAcc * 40);
        targetPos = null;
    }

    public void BodyRotateByAimDir ( Vector3 dir ){
        currentDir = dir;
        _isAim = true;
        SetBodyImage(dir);
    }

    public void BodyRotateByMoveDir( Vector3 dir )
    {
        currentDir = dir;
        if (_isAim) return;
        SetBodyImage(dir);
    }

    public void SetColor( Color color )
    {
        Image[] cs = GetComponentsInChildren<Image>();
        foreach (Image c in cs) c.color = color;
        this.color = color;
    }

    public void SetAI( ViewController vc, AIBasic ai)
    {
        AIMove = ai;
        AIMove.ViewController = vc;
        AIMove.PlayerController = this;
    }

    public void UpdateBody()
    {
       // SetBodyImage(currentDir);
    }
    
    void OnTriggerEnter2D( Collider2D other )
    {
        if(OnHitEvent != null)
            OnHitEvent(this.gameObject,other.gameObject);
    }

    void SetPlayerForce(Vector3 dir, float force)
    {
        GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
        GetComponent<PlayerController>().BodyRotateByMoveDir(dir.normalized);
    }

    void SetAnimation( string aniName )
    {
        body.GetComponent<Animator>().enabled = false;
        body.GetComponent<Animator>().enabled = true;
        body.GetComponent<Animator>().Play(aniName);
    }

    bool isHaveBlade()
    {
        foreach( IWeapon w in weapons)
        {
            if (w.IsBlade()) return true;
        }
        return false;
    }

    string GetAnimationStr( string dir )
    {
        string w = isHaveBlade() ? "_Blade" : "_Gun";
        string i = IsAim ? "" : "_Idle";
        return "Player" + w + i + "_" + dir;
    }

    void SetBodyImage(Vector3 dir )
    {
        if (dir.x > .2f)
        {
            body.transform.localScale = normalScale;
            if (dir.y > .2f)
            {
                SetAnimation(GetAnimationStr("Right_Up"));
            }
            else if (dir.y < -.2f)
            {
                SetAnimation(GetAnimationStr("Right_Down"));
            }
            else
            {
                SetAnimation(GetAnimationStr("Right"));
            }
        }
        else if (dir.x < -.2f)
        {
            body.transform.localScale = flipScale;
            if (dir.y > .2f)
            {
                SetAnimation(GetAnimationStr("Right_Up"));
            }
            else if (dir.y < -.2f)
            {
                SetAnimation(GetAnimationStr("Right_Down"));
            }
            else
            {
                SetAnimation(GetAnimationStr("Right"));
            }
        }
        else
        {
            body.transform.localScale = normalScale;
            if (dir.y > .2f)
            {
                SetAnimation(GetAnimationStr("Up"));
            }
            else if (dir.y < -.2f)
            {
                //print("DD");
                SetAnimation(GetAnimationStr("Down"));
            }
        }
    }

    void Start()
    {
        SetColor(color);
    }

    void Update()
    {
        foot.GetComponent<Animator>().SetFloat("Speed", GetComponent<Rigidbody2D>().velocity.magnitude);
        UpdatePosition();
        if (AIMove != null) AIMove.Update();
        if(AIWeapons!= null) foreach (AIBasic aiw in AIWeapons) aiw.Update();
    }

    void UpdatePosition()
    {
        if (targetPos != null)
        {
            Vector3 diffVec = (Vector3)targetPos - Position;
            if (diffVec.magnitude < 40)
            {
                targetPos = null;
            }
            else
            {
                SetPlayerForce(diffVec, GameConfig.MoveSpeed);
            }
        }
    }

    Vector3 GetMousePositionOnWorld(Vector3 screenPos)
    {
        Vector3 clickPos = screenPos;
        clickPos.z = Camera.main.transform.localPosition.z;
        return Camera.main.ScreenToWorldPoint(clickPos);
    }
}
