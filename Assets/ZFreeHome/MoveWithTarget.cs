using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 lastTargetPosition;
    public bool parentWhenAttach;

    void Start()
    {
        lastTargetPosition = target.position;
    }
    
    void Update()
    {
        if (parentWhenAttach) return;
        transform.position += target.position - lastTargetPosition;
        lastTargetPosition = target.position;
    }
}
