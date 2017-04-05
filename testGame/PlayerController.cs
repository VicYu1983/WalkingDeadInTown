using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour {
    
    public GameObject body;
    public GameObject foot;
    public GameObject handlight;
    public Text Bubble;
    public Color color;

    public bool IsWalk = false;

    public string SpeakContent
    {
        set
        {
            try
            {
                Bubble.text = value;
            }
            catch
            {
                //maybe die now!
            }
        }
        get
        {
            return Bubble.text;
        }
    }

    int _hp = 100;
    public int HP
    {
        get
        {
            return _hp;
        }
        set
        {
            if (value < 0) value = 0;
            _hp = value;
        }
    }
    
    public bool IsDead()
    {
        return HP <= 0;
    }
    public Action<PlayerController,GameObject> OnHitEvent;

    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);
    Vector3 currentDir = new Vector3(0, -1, 0);

    /* 持有武器 */
   // public List<IWeapon> weapons = new List<IWeapon>();

    Vector3? targetPos;
    
    public AIBasic AIMove
    {
        set;get;
    }
    public AIBasic AIWeapon
    {
        set;get;
    }
    
    public Vector3 Scale
    {
        set
        {
            this.body.transform.localScale = value;
        }
        get
        {
            return this.body.transform.localScale;

        }
    }
    public Vector3 Position
    {
        set
        {
            value.z = 4;
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

    bool _isBlade = false;
    public bool IsBlade
    {
        set
        {
            _isBlade = value;
        }
        get
        {
            return _isBlade;
        }
    }

    void DrawSprite(Texture2D canvas, Texture2D drawTarget, bool ignoreAlpha = false )
    {
        for (int y = 0; y < canvas.height; y++)
        {
            for (int x = 0; x < canvas.width; x++)
            {
                Color color = drawTarget.GetPixel(x * 4, y * 4);
                if(ignoreAlpha)
                {
                    if (color.a != 0) canvas.SetPixel(x, y, color);
                }
                else
                {
                    canvas.SetPixel(x, y, color);
                }
                
            }
        }
    }

    public void Hit( Vector3 dir, float force, int damage )
    {
        GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
        HP -= damage;
        if (HP < 0) HP = 0;
    }

    public Sprite GetPlayerImage()
    {
        Texture2D texture = new Texture2D(32, 32);
        DrawSprite(texture, foot.GetComponent<Image>().sprite.texture);
        DrawSprite(texture, body.GetComponent<Image>().sprite.texture, true);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2()); ;
    }

    public void SetPlayerPositionByScreenPos(Vector3 screenPos)
    {
        targetPos = GetMousePositionOnWorld(screenPos);
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public void DodgePlayerByScreenPos(Vector3 dir, float force)
    {
        SetPlayerForce((GetMousePositionOnWorld(dir) - Position).normalized, force);
        targetPos = null;
    }

    public void Dodge(Vector3 dir, float force)
    {
        SetPlayerForce(dir, force);
        targetPos = null;
    }

    public void MakePlayerStop()
    {
        Vector3 playerAcc = GetComponent<Rigidbody2D>().velocity;
        GetComponent<Rigidbody2D>().AddForce(-playerAcc * 40);
        targetPos = null;
    }

    public void StopMove()
    {
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
        try
        {
            Image[] cs = GetComponentsInChildren<Image>();
            foreach (Image c in cs)
            {
                if( c.name != "Handlight" )
                    c.color = color;
            }
            this.color = color;
        }
        catch( Exception e)
        {
            //maybe die now!
        }
    }
    /*
    public void SetAI( ViewController vc, AIBasic ai)
    {
        AIMove = ai;
        AIMove.ViewController = vc;
        AIMove.PlayerController = this;
    }
    */
    public void UpdateBody()
    {
        SetBodyImage(currentDir);
    }
    
    void OnTriggerEnter2D( Collider2D other )
    {
        if(OnHitEvent != null)
            OnHitEvent(this,other.gameObject);
    }

    void SetPlayerForce(Vector3 dir, float force)
    {
       // GetComponent<Rigidbody2D>().AddForce(dir.normalized * force);
        GetComponent<Rigidbody2D>().velocity = dir.normalized * force * 100;
        
        //使用數據移動，會喪失一些物理效果，例如會穿過物理物件
        /*
        Vector3 newpos = Position;
        newpos += dir.normalized * force * 2;
        newpos.z = 0;
        Position = newpos;
        */
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
        return _isBlade;
    }
    
    string GetAnimationStr( string dir )
    {
        string w = isHaveBlade() ? "_Blade" : "_Gun";
        string i = IsAim ? "" : "_Idle";
        /* 暫時做法，因為不想多拉拿槍動態的animation */
        if (w == "_Gun") i = "_Idle";

        return "Player" + w + i + "_" + dir;
    }

    void SetBodyImage(Vector3 dir )
    {
        if (dir.x > .2f)
        {
            Scale = normalScale;
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
            Scale = flipScale;
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
            Scale = normalScale;
            if (dir.y > .2f)
            {
                SetAnimation(GetAnimationStr("Up"));
            }
            else if (dir.y < -.2f)
            {
                SetAnimation(GetAnimationStr("Down"));
            }
        }
        SetHandlightDir(dir);
    }

    void SetHandlightDir( Vector3 dir )
    {
        double radian = Math.Atan2(dir.x, -dir.y);
        float angle = (float)radian / (float)Math.PI * 180;
        handlight.GetComponent<RectTransform>().rotation = Quaternion.Euler( 0, 0, angle);
        
        // 180, 0 wei shang xia
        float diffA = 180 - Math.Abs(angle);
        float diffB = Math.Abs(angle);
        handlight.GetComponent<RectTransform>().localScale = new Vector3(1, Math.Min(diffA, diffB) / 90 * .3f + .7f, 1);
    }

    void Start()
    {
        SetColor(color);

        if( gameObject.name != "Player")
        {
            handlight.SetActive(false);
        }
    }

    void Update()
    {
        foot.GetComponent<Animator>().SetFloat("Speed", GetComponent<Rigidbody2D>().velocity.magnitude);
        UpdatePosition();
        if (AIMove != null) AIMove.Update();
        if( AIWeapon != null) AIWeapon.Update();
    }

    void OnDestroy()
    {
        AIMove = null;
        AIWeapon = null;
    }

    void UpdatePosition()
    {
        if (targetPos != null)
        {
            Vector3 diffVec = (Vector3)targetPos - Position;
            if (diffVec.magnitude < 20)
            {
                targetPos = null;
                IsWalk = false;
            }
            else
            {
                SetPlayerForce(diffVec, GameConfig.MoveSpeed);
                IsWalk = true;
            }
        }else
        {
            IsWalk = false;
        }
    }

    Vector3 GetMousePositionOnWorld(Vector3 screenPos)
    {
        Vector3 clickPos = screenPos;
        clickPos.z = Camera.main.transform.localPosition.z;
        return Camera.main.ScreenToWorldPoint(clickPos);
    }
}
