using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch
{
    public bool isRepeat;
    public bool canRemove;
    public bool isCounting;
    public float startTime;
    public float endTime;
    public float interval;

    public float normalizedTime { get { return Mathf.Clamp01(time / interval); } }
    public float time;

    public System.Action onTimesUp;

    public Stopwatch(float interval, bool repeat, System.Action callback)
    {
        this.interval = interval;
        this.isRepeat = repeat;
        this.onTimesUp = callback;
    }

    public void Start()
    {
        startTime = Time.realtimeSinceStartup;
        endTime = startTime;
        isCounting = true;
    }

    public float Stop()
    {
        endTime = Time.realtimeSinceStartup;
        time = endTime - startTime;
        isCounting = false;
        Debug.Log("Normalized:" + normalizedTime);
        return time;
    }

    public float TimeSinceStart()
    {
        if(isCounting)
        {
            return Time.realtimeSinceStartup - startTime;
        }
        else
        {
            return 0;
        }
    }

    public void Update()
    {
        if(Time.realtimeSinceStartup - startTime >= interval)
        {
            if(onTimesUp != null)
            {
                onTimesUp();
                if(isRepeat)
                {
                    startTime = Time.realtimeSinceStartup;
                }
                else
                {
                    onTimesUp = null;
                }
            }
        }
    }
}
