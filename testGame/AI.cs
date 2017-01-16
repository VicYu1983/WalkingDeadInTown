using UnityEngine;
using System.Collections;

public class AI {

    private ViewController _vc;
    public ViewController ViewController
    {
        set
        {
            _vc = value;
        }
    }

    private PlayerController _pc;
    public PlayerController PlayerController
    {
        set
        {
            _pc = value;
        }
    }

    public void Update()
    {
        PlayerController player = _vc.Player.GetComponent<PlayerController>();
        Vector3 diff = _pc.Position - player.Position;
        if( diff.magnitude > 300)
        {
           // _pc.
        }
    }
}
