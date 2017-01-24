using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

    public GameObject txt_state;
    public Dropdown dpd_halfAutoWeapons;
    public Dropdown dpd_halfAutoDelayWeapons;
    public Dropdown dpd_autoWeapons;

    public void SetState( string msg )
    {
        txt_state.GetComponent<Text>().text = msg;
    }

    public string[] GetWeaponListFromUI()
    {
        string equipAName = dpd_halfAutoWeapons.options[dpd_halfAutoWeapons.value].text;
        string equipBName = dpd_autoWeapons.options[dpd_autoWeapons.value].text;
        string equipCName = dpd_halfAutoDelayWeapons.options[dpd_halfAutoDelayWeapons.value].text;
        return new string[] { equipAName, equipBName, equipCName };
    }

	// Use this for initialization
	void Start () {
        foreach (object[] c in GameConfig.WeaponConfig)
        {
            bool autoWeapon = (bool)c[10];
            if (autoWeapon)
            {
                dpd_autoWeapons.options.Add(new Dropdown.OptionData(c[0].ToString()));
            }
            else
            {
                bool delay = (bool)c[7];
                if(delay)
                {
                    dpd_halfAutoDelayWeapons.options.Add(new Dropdown.OptionData(c[0].ToString()));
                }
                else
                {
                    dpd_halfAutoWeapons.options.Add(new Dropdown.OptionData(c[0].ToString()));
                }
                
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
