using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CounterMode
{
    UP,
    DOWN
};

public class TimeCounter
{
    private float targetCounter;
    private float internalCounter;
    private CounterMode mode;

    public TimeCounter(float target = 0)
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

    public void Tick(float deltaTime)
    {
        int factor = (mode == CounterMode.UP) ? 1 : -1;
        internalCounter += deltaTime * factor;
        internalCounter = Mathf.Max(Mathf.Min(targetCounter, internalCounter), 0);
    }

}
