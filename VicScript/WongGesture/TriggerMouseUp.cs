using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerMouseUp : MonoBehaviour {

    public Action<Vector3> OnTriggerMouseUp;

    private void OnMouseUp()
    {
        if (OnTriggerMouseUp != null) OnTriggerMouseUp.Invoke(Input.mousePosition);
    }
}
