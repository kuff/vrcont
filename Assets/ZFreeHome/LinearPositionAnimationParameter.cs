using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearPositionAnimationParameter : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private float  t;
    [SerializeField] private Animator anim;
    private static readonly int Mapping = Animator.StringToHash("Mapping");
    private PresetAnimator _animator;

    private void Start()
    {
        _animator = FindObjectOfType<PresetAnimator>().GetComponent<PresetAnimator>();
    }

    private void Update()
    {
        Vector3 endRel = end.position - start.position;
        Vector3 tRel = transform.position - start.position;
        t = Mathf.InverseLerp(0,endRel.magnitude,tRel.magnitude);
        if (t<0f)
        {
            t = 0f;
        }else if (t >0.99f)
        {
            t = 0.99f;
        }
        
    }

    
}
