using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CounterMode
{
    UP,
    DOWN
};

public class TimeCounter : MonoBehaviour
{
    [SerializeField]
    private float targetCounter;
    [SerializeField]
    private float internalCounter;
    [SerializeField]
    private CounterMode mode;

    public void Init(float target = 0)
    {
        targetCounter = (target <= 0) ? 0 : target;
        internalCounter = 0;
        mode = CounterMode.UP;
    }

    public CounterMode GetMode()
    {
        return mode;
    }

    public float GetT()
    {
        return internalCounter / targetCounter;
    }

    public void SetUp(float target = 0)
    {
        targetCounter = (target <= 0) ? targetCounter : target;
        mode = CounterMode.UP;
    }

    public void SetDown(float target = 0)
    {
        targetCounter = (target <= 0) ? targetCounter : target;
        mode = CounterMode.DOWN;
    }

    public void FixedUpdate()
    {
        int factor = (mode == CounterMode.UP) ? 1 : -1;
        internalCounter += Time.fixedDeltaTime * factor;
        internalCounter = Mathf.Max(Mathf.Min(targetCounter, internalCounter), 0);
    }

}
