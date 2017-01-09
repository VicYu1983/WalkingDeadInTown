using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public GameObject txt_state;

    public void SetState( string msg )
    {
        txt_state.GetComponent<Text>().text = msg;
        print(msg);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
