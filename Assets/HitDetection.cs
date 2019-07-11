﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        ThirdPersonCharacter target = other.GetComponent<ThirdPersonCharacter>();

        if(target && !transform.IsChildOf(target.transform))
        {
            target.hp -= 10;
            Debug.Log("对" + target.gameObject.name + "造成了" + 10 + "点伤害,剩余生命" + target.hp);
        }
    }
}