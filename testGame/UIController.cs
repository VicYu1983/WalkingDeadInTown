using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

    public GameObject txt_state;
    public GameObject RightPanel;
    public Dropdown dpd_halfAutoWeapons;
    public Dropdown dpd_halfAutoDelayWeapons;
    public Dropdown dpd_autoWeapons;

    public void SetState( string msg )
    {
        print(msg);
        txt_state.GetComponent<Text>().text = msg;
    }

    public string[] GetWeaponListFromUI()
    {
        string equipAName = dpd_halfAutoWeapons.options[dpd_halfAutoWeapons.value].text;
        string equipBName = dpd_autoWeapons.options[dpd_autoWeapons.value].text;
        string equipCName = dpd_halfAutoDelayWeapons.options[dpd_halfAutoDelayWeapons.value].text;
        return new string[] { equipAName, equipBName, equipCName };
    }

    public void ShowRightPanel( bool show )
    {
        /* 這邊本來是用 setActive(show)，但是不知道為什麼會影響到點擊ui的事件，導致有些ui不能點，因此改為用scale來決定要不要顯示 */
        if( show)
        {
            RightPanel.GetComponent<RectTransform>().localScale = new Vector3(3, 3, 1);
        }else
        {
            RightPanel.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1);
        }
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
	
}
