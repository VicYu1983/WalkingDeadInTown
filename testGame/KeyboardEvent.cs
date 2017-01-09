using UnityEngine;
using System.Collections;
using System;

public class KeyboardEvent : MonoBehaviour {

    public Action OnFClick;
    // public Action OnDClick;
    public Action OnSpaceClick;
    public Action OnFPress;

    // Update is called once per frame
    void Update () {
	    if(Input.GetKeyUp(KeyCode.F))
        {
            OnFClick.Invoke();
        }
        /*
        if (Input.GetKeyUp(KeyCode.D))
        {
            OnDClick.Invoke();
        }*/
        if (Input.GetKey(KeyCode.F))
        {
            OnFPress.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnSpaceClick.Invoke();
        }
	}
}
