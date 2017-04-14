using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour {
    
    public GameObject body;
    public GameObject foot;
    public GameObject handlight;
    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);
    Vector3 currentDir = new Vector3(0, -1, 0);
    
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
    public Sprite GetPlayerImage()
    {
        Texture2D texture = new Texture2D(32, 32);
        DrawSprite(texture, foot.GetComponent<Image>().sprite.texture);
        DrawSprite(texture, body.GetComponent<Image>().sprite.texture, true);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2()); ;
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
    public void UpdateBody()
    {
        SetBodyImage(currentDir);
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
        if( gameObject.name != "Player")
        {
            handlight.SetActive(false);
        }
    }

    void Update()
    {
        foot.GetComponent<Animator>().SetFloat("Speed", GetComponent<Rigidbody2D>().velocity.magnitude);
        if (AIMove != null) AIMove.Update();
        if( AIWeapon != null) AIWeapon.Update();
    }

    void OnDestroy()
    {
        AIMove = null;
        AIWeapon = null;
    }
}
