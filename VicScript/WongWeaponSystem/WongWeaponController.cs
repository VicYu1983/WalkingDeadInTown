using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WongWeaponController : MonoBehaviour {

    public bool IsAim = false;
    public List<IWeapon> weapons = new List<IWeapon>();
    /*
    public void AddWeapon( object[] config )
    {
            bool autoWeapon = (bool)config[10];
            if (autoWeapon)
            {
                weapons.Add(new AutoWeapon(config));
            }
            else
            {
                weapons.Add(new HalfAutoWeapon(config));
            }
    }
    */
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
