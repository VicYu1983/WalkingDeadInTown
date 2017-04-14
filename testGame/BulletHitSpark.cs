using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletHitSpark : MonoBehaviour {

    public List<Sprite> sprites;

	// Use this for initialization
	void Start () {

        int seed = Mathf.FloorToInt(UnityEngine.Random.value * sprites.Count);
        GetComponent<Image>().sprite = sprites[seed];

        GetComponent<AgeCalculator>().OnDeadEvent += OnDeadEvent;
    }

    private void OnDeadEvent(AgeCalculator obj)
    {
        GetComponent<AgeCalculator>().OnDeadEvent -= OnDeadEvent;
        Destroy(this.gameObject);
    }
    
}
