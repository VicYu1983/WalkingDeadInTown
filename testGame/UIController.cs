using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

    public GameObject txt_state;
    public Dropdown dpd_halfAutoWeapons;
    public Dropdown dpd_autoWeapons;

    public void SetState( string msg )
    {
        txt_state.GetComponent<Text>().text = msg;
    }

    public string[] GetWeaponListFromUI()
    {
        string equipAName = dpd_halfAutoWeapons.options[dpd_halfAutoWeapons.value].text;
        string equipBName = dpd_autoWeapons.options[dpd_autoWeapons.value].text;
        return new string[] { equipAName, equipBName };
    }

	// Use this for initialization
	void Start () {
        dpd_halfAutoWeapons.options.Add(new Dropdown.OptionData(""));
        dpd_autoWeapons.options.Add(new Dropdown.OptionData(""));
        foreach (object[] c in GameConfig.WeaponConfig)
        {
            bool autoWeapon = (bool)c[10];
            if (autoWeapon)
            {
                dpd_autoWeapons.options.Add(new Dropdown.OptionData(c[0].ToString()));
            }
            else
            {
                dpd_halfAutoWeapons.options.Add(new Dropdown.OptionData(c[0].ToString()));
            }

            /*
            bool usingWeapon = (bool)c[13];
            if (usingWeapon)
            {
                bool autoWeapon = (bool)c[10];
                if (autoWeapon)
                {
                   // vc.Player.weapons.Add(new AutoWeapon(vc, c));
                }
                else
                {
                   // vc.Player.weapons.Add(new HalfAutoWeapon(vc, c));
                }
            }
            */
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
