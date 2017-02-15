using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VicScript;
using VicScript.WongWeaponSystem;

public class RayLineView : MonoBehaviour {

    public IWeapon Weapon
    {
        set;get;
    }

    RaylineModel rm;
    RectTransform trans;

	// Use this for initialization
	void Start () {
        rm = GetComponent<RaylineModel>();
        rm.OnDeadEvent += OnDeadEvent;
        trans = GetComponent<RectTransform>();

        trans.localPosition = rm.CurrentPos;
        if ( rm.targetPos.y - rm.fromPos.y < 0)
        {
            trans.rotation = Quaternion.Euler(new Vector3(0, 0, -rm.Degree));
        }else
        {
            trans.rotation = Quaternion.Euler(new Vector3(0, 0, rm.Degree));
        }
	}

    private void OnDeadEvent(Vector3 obj)
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        rm.OnDeadEvent -= OnDeadEvent;
    }

    // Update is called once per frame
    void Update () {
        trans.localScale = new Vector3(rm.ScaleFac, 1, 1);
        trans.localPosition = rm.CurrentPos;
    }
}
