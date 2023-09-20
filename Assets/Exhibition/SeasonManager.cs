using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public Material[] seasonTreeMaterials ;
    public ParticleSystem[] snowMakers;
    private static readonly int SnowPower = Shader.PropertyToID("_SnowPower");
    public float timer;
    private static readonly int SnowLine = Shader.PropertyToID("_SnowLine");
    public int sign = -1;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >5)
        {
            return;   
        }
        float line = Mathf.Lerp(-100*sign, 100*sign, timer / 5);
        foreach (var mat in seasonTreeMaterials)
        {
            mat.SetFloat(SnowLine,line);
        }
    }

    public void ChangeToSeason(Season season)
    {
        //foreach (var mat in seasonTreeMaterials)
        //{
            //mat.SetFloat(SnowPower,(int)season*10);
       // }
        timer = 0;
        sign = season == Season.Winter ? -1 : 1;
        foreach (ParticleSystem snowMaker in snowMakers)
        {
            if (season==Season.Winter)
            {
                snowMaker.Play();
            }
            else
            {
               snowMaker.Stop(true,ParticleSystemStopBehavior.StopEmitting);
            }
        }
        
    }
}
