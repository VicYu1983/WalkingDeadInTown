using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public Sprite[] bodySprites;
    public GameObject body;
    public GameObject foot;

    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 flipScale = new Vector3(-1, 1, 1);

    public void SetBodyImage( Vector3 dir )
    {
        if( dir.x > .2f)
        {
            body.transform.localScale = normalScale;
            if ( dir.y > .2f)
            {
                body.GetComponent<Image>().sprite = bodySprites[3];
            }
            else if( dir.y < -.2f )
            {
                body.GetComponent<Image>().sprite = bodySprites[1];
            }
            else
            {
                body.GetComponent<Image>().sprite = bodySprites[2];
            }
        }
        else if( dir.x < -.2f)
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
