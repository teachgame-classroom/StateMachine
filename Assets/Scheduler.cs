using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    public static Scheduler instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameController").GetComponent<Scheduler>();
            }

            return _instance;
        }
    }
    private static Scheduler _instance;
    private Dictionary<int, Stopwatch> stopwatches = new Dictionary<int, Stopwatch>();
    private int totalWatchCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Schedule(1f, true, Hello);
            Schedule(5f, true, ReportTime);
            Schedule(3f, true, SpawnCube);
        }

        foreach (Stopwatch sw in stopwatches.Values)
        {
            sw.Update();
        }

        for(int i = 0; i < totalWatchCount; i++)
        {
            if(stopwatches.ContainsKey(i))
            {
                if(stopwatches[i].canRemove)
                {
                    stopwatches.Remove(i);
                }
            }
        }
    }

    public int Schedule(float interval, bool repeat, System.Action callback)
    {
        int id = totalWatchCount;
        Stopwatch newWatch = new Stopwatch(interval, repeat, callback);
        stopwatches.Add(id, newWatch);
        newWatch.Start();

        totalWatchCount++;

        return id;
    }

    public void Deschedule(int handler)
    {
        if(stopwatches.ContainsKey(handler))
        {
            stopwatches[handler].Stop();
            stopwatches[handler].canRemove = true;
            //stopwatches.Remove(handler);
        }
    }

    private void PendingRemove(int handler)
    {

    }

    public void Hello()
    {
        Debug.Log("Hello");
    }

    public void ReportTime()
    {
        Debug.Log("Time is now:" + Time.realtimeSinceStartup);
    }

    public void SpawnCube()
    {
        Debug.Log("Spawn");
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>().velocity = Vector3.up * 20f;
    }
}
