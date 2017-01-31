using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedShadowController : MonoBehaviour {

    public bool EnableAlpha = false;
    
	void Start () {
        GetComponent<AgeCalculator>().OnDeadEvent += OnDeadEvent;
    }

    public void SetImage( Sprite image )
    {
        GetComponent<Image>().sprite = image;
    }

    private void OnDeadEvent(AgeCalculator obj)
    {
        GetComponent<AgeCalculator>().OnDeadEvent -= OnDeadEvent;
        Destroy(this.gameObject);
    }

    void Update()
    {
        if(EnableAlpha) GetComponent<Image>().color = new Color(1, 1, 1, 1.0f - GetComponent<AgeCalculator>().GetPercent());
    }
}
