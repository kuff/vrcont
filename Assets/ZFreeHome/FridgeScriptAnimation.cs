using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeScriptAnimation : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public float max;


    private void Update()
    {
       // transform.position = Vector3.Lerp(start.position,end.position,LinearMappingToAnimation.count/max);
    }
}
