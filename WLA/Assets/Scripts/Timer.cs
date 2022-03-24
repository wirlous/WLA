using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimerMode
{
    UP,
    DOWN
};

public class Timer : MonoBehaviour
{
    // public delegate void TimerDelegate();

    [SerializeField]
    private float targetTime;
    [SerializeField]
    private float internalTime;
    [SerializeField]
    private TimerMode mode;

    // Internal
    // private TimerDelegate callback;

    public void Init(float time = 0)
    {
        targetTime = (time <= 0) ? 0 : time;
        internalTime = 0;
        mode = TimerMode.UP;
    }

    public TimerMode GetMode()
    {
        return mode;
    }

    public float GetTime()
    {
        return internalTime;
    }

    public float GetTimeNormalize()
    {
        return internalTime / targetTime;
    }

    public void SetUp(float target = 0)
    {
        targetTime = (target <= 0) ? targetTime : target;
        mode = TimerMode.UP;
    }

    public void SetDown(float target = 0)
    {
        targetTime = (target <= 0) ? targetTime : target;
        mode = TimerMode.DOWN;
    }

    public void FixedUpdate()
    {
        int factor = (mode == TimerMode.UP) ? 1 : -1;
        internalTime += Time.fixedDeltaTime * factor;
        internalTime = Mathf.Max(Mathf.Min(targetTime, internalTime), 0);
    }

}
