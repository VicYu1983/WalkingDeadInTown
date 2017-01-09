using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;
using UnityEngine.UI;

public class TapTest : MonoBehaviour {

    public ReleaseGesture rg;
    public TapGesture tg;
    public FlickGesture fg;
    public PressGesture pg;
    public LongPressGesture lpg;
    public Text txt_state;
    
	void OnEnable() {
        //tg.Tapped += Tg_Tapped;
        // rg.Released += Rg_Released;
          fg.Flicked += Fg_Flicked;
        // pg.Pressed += Pg_Pressed;
        lpg.LongPressed += Lpg_LongPressed;
    }

    private void Lpg_LongPressed(object sender, System.EventArgs e)
    {
        LogString("Lpg_LongPressed");
    }

    private void Pg_Pressed(object sender, System.EventArgs e)
    {
        LogString("Pg_Pressed");
    }

    private void Fg_Flicked(object sender, System.EventArgs e)
    {
        LogString("Fg_Flicked");
    }

    private void Rg_Released(object sender, System.EventArgs e)
    {
        LogString("Rg_Released");
    }

    private void Tg_Tapped(object sender, System.EventArgs e)
    {
        LogString("Tg_Tapped");
    }

    void LogString( string msg )
    {
        txt_state.text = msg + ", Input.touchCount: " + Input.touchCount + ": tg.ActiveTouches.Count: " + tg.ActiveTouches.Count;
        print(txt_state.text);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
