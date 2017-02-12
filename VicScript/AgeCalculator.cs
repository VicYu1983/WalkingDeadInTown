using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class AgeCalculator : MonoBehaviour {

    public int DeadAge = 300;
    public Action<AgeCalculator> OnDeadEvent;

    [SerializeField]
    private bool _isDead = false;
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
    }

    [SerializeField]
    private int _currentAge = 0;
    public int CurrentAge
    {
        get
        {
            return _currentAge;
        }
    }

    public float GetPercent()
    {
        return (_currentAge + 0.0f) / (DeadAge + 0.0f);
    }

    public void ResetAge()
    {
        _currentAge = 0;
        _isDead = false;
    }

	void Update () {
        if( !IsDead ) {
            if (_currentAge++ > DeadAge)
            {
                _isDead = true;
                if (OnDeadEvent != null) OnDeadEvent.Invoke(this);
            }
        }
    }
}
