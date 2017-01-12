using UnityEngine;
using System.Collections;
using System;

public class EffectTimeEvent : MonoBehaviour {

    public Action<GameObject> OnEffectEndEvent;

	public void EffectEnd()
    {
        if (OnEffectEndEvent != null) OnEffectEndEvent.Invoke(this.gameObject);
    }
}
