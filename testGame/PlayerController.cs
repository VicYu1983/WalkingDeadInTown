using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour {

    public Sprite[] bodySprites;
    public GameObject body;
    public GameObject foot;
    public Action<GameObject,GameObject> OnHitEvent;

    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);

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


    public void BodyRotateByAimDir ( Vector3 dir ){
        SetBodyImage(dir);
        _isAim = true;
    }

    public void BodyRotateByMoveDir( Vector3 dir )
    {
        if (_isAim) return;
        SetBodyImage(dir);
    }

    void OnTriggerEnter2D( Collider2D other )
    {
        if(OnHitEvent != null)
            OnHitEvent(this.gameObject,other.gameObject);
    }

    void SetBodyImage(Vector3 dir )
    {
        if (dir.x > .2f)
        {
            body.transform.localScale = normalScale;
            if (dir.y > .2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[3];
            }
            else if (dir.y < -.2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[1];
            }
            else
            {
                body.GetComponent<Image>().sprite = bodySprites[2];
            }
        }
        else if (dir.x < -.2f)
        {
            body.transform.localScale = flipScale;
            if (dir.y > .2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[3];
            }
            else if (dir.y < -.2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[1];
            }
            else
            {
                body.GetComponent<Image>().sprite = bodySprites[2];
            }
        }
        else
        {
            body.transform.localScale = normalScale;
            if (dir.y > .2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[4];
            }
            else if (dir.y < -.2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[0];
            }
        }
    }

    void Update()
    {
        foot.GetComponent<Animator>().SetFloat("Speed", GetComponent<Rigidbody2D>().velocity.magnitude);
    }
}
