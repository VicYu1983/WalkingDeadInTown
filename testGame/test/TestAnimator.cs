using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyUp( KeyCode.Space))
        {
            // GetComponent<Animator>().Stop();
            GetComponent<Animation>().Play("Blade");
            print(GetComponent<Animation>().IsPlaying("Blade"));
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            /*
            GetComponent<Animator>().enabled = false;
            GetComponent<Animator>().enabled = true;
            */
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            /*
            GetComponent<Animator>().enabled = false;
            GetComponent<Animator>().enabled = true;
            GetComponent<Animator>().Play("Blade_right");
            */
        }
    }
}
