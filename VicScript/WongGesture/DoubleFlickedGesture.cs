using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using System.Collections.Generic;
using System;

namespace VicScript.WongWeaponSystem
{
    public class DoubleFlickedGesture : MonoBehaviour
    {

        public FlickGesture fg;
        public Action<FlickGesture> OnDoubleFlickedEvent;

        bool flicked = false;

        void Start()
        {
            fg.Flicked += OnFlicked;
        }

        private void OnFlicked(object sender, System.EventArgs e)
        {
            if (flicked)
            {
                if (OnDoubleFlickedEvent != null) OnDoubleFlickedEvent.Invoke(fg);
                flicked = false;
            }
            flicked = true;
            StartCoroutine(DelayCall(.5f, () =>
            {
                flicked = false;
            }));
        }

        IEnumerator DelayCall(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

    }

}
