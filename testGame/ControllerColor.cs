using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.testGame
{
    class ControllerColor : MonoBehaviour
    {
        public Color color;

        public void SetColor(Color color)
        {
            try
            {
                Image[] cs = GetComponentsInChildren<Image>();
                foreach (Image c in cs)
                {
                    if (c.name != "Handlight")
                        c.color = color;
                }
                this.color = color;
            }
            catch (Exception e)
            {
                //maybe die now!
            }
        }

        void Start()
        {
            SetColor(color);
        }
    }
}
