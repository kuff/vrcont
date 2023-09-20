using System;
using System.Collections;
using System.Collections.Generic;
using Runtime;
using UnityEngine;

public class StaticUntilGrabbed : MonoBehaviour
{
    private Rigidbody r;

    private void Start()
    {
        r = GetComponent<Rigidbody>();
    }
    public void Grabbed()
    {
        r.constraints = RigidbodyConstraints.None;
    }
}
