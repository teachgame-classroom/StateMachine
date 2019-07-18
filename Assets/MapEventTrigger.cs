using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventTrigger : MonoBehaviour
{
    public string eventName;
    public int id;

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "Player")
        {
            GameController.instance.OnMapEvent(eventName, id, transform.position);
        }
    }

}
