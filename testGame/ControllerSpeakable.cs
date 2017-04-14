using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.testGame
{
    class ControllerSpeakable : MonoBehaviour
    {
        public Text Bubble;

        public string SpeakContent
        {
            set
            {
                try
                {
                    Bubble.text = value;
                }
                catch
                {
                    //maybe die now!
                }
            }
            get
            {
                return Bubble.text;
            }
        }
    }
}
