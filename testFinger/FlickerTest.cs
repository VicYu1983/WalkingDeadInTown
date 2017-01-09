using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using UnityEngine.UI;
using System.Collections.Generic;
using VicScript;

public class FlickerTest : MonoBehaviour {

    public FlickGesture tg;
    public Text txt_state;
    public Text txt_state2;

    void OnEnable()
    {
        tg.Flicked += OnFlicked;
    }

    private void OnChange(object sender, GestureStateChangeEventArgs e)
    {
    }

    private void OnFlicked(object sender, System.EventArgs e)
    {
        txt_state.text = "OnFlicked" + tg.ScreenFlickVector.ToString() + ", atcount:" + tg.ActiveTouches.Count + ", numTouchs:" + tg.NumTouches.ToString() + ", inputCount: " + Input.touchCount + ", calcount:" + GetComponent<DetectTouchCountByPassTime>().TouchCount;
    }

    void OnDisable()
    {
        
    }
    


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    }
}
