using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetAnimator : MonoBehaviour
{
    public float timer;
    public float animTime = 1;
    public bool animate;
    public bool open;
    public Preset animatingFrom;
    public Preset animatingTo;
    public Transform[] cupboards;
    public float[] cupboardClosedAngles;
    public float[] cupboardOpenAngles;
    public Transform garden;
    private Vector3 gardenStartPos;
    private Vector3 gardenEndPos;
    private Vector3 gardenClosedPos;
    public Vector3 gardenOpenPos;

    public Vector3[] leftDoorLocations;
    public Vector3[] leftDoorRotations;
    public Vector3[] rightDoorLocations;
    public Vector3[] rightDoorRotations;
    public Transform doorCollider;
    public Transform doorColliderStart;

    public Transform doorColliderEnd;
    
    public float doorT;

    public Animator doorAnimator;

    public Animator tableAnimator;

    private static readonly int Mapping = Animator.StringToHash("Mapping");

    public Vector3 doorStartTemp;

    public Vector3 doorEndTemp;
    //CAB1,CAB2,CAB3,BED
    private void Start()
    {
        gardenClosedPos = garden.localPosition;
        cupboardClosedAngles = new float[cupboards.Length];
        cupboardOpenAngles = new float[cupboards.Length];
    }

    private void Update()
    {
        AnimateDoor();
        if (!animate&&animatingFrom != animatingTo)
        {
            animate = true;
            open = false;
            cupboardClosedAngles = new float[cupboards.Length];
            cupboardOpenAngles = new float[]{90,-90,-90,90,90,-90,-90,90};
            gardenStartPos = gardenClosedPos;
            gardenEndPos = gardenOpenPos;
            doorStartTemp = doorColliderStart.position;
            doorEndTemp = doorColliderEnd.position;
            switch (animatingTo)
            {
                case Preset.Sleeping:
                    cupboardClosedAngles[0] = cupboards[0].rotation.eulerAngles.y;
                    cupboardClosedAngles[1] = cupboards[1].rotation.eulerAngles.y;
                    cupboardClosedAngles[2] = cupboards[2].rotation.eulerAngles.y;
                    cupboardClosedAngles[3] = cupboards[3].rotation.eulerAngles.x;
                    break;
                case Preset.Cooking:
                    cupboardClosedAngles[4] = cupboards[4].rotation.eulerAngles.y;
                    cupboardClosedAngles[5] = cupboards[5].rotation.eulerAngles.y;
                    cupboardClosedAngles[6] = cupboards[6].rotation.eulerAngles.y;
                    break;
                case Preset.Garden:
                    cupboardClosedAngles[7] = cupboards[7].rotation.eulerAngles.y;
                    gardenStartPos = garden.transform.localPosition;
                    doorStartTemp = doorCollider.position;
                    break;
            }

            switch (animatingFrom)
            {
                case Preset.Sleeping:
                    cupboardOpenAngles[0] = cupboards[0].rotation.eulerAngles.y;
                    cupboardOpenAngles[1] = cupboards[1].rotation.eulerAngles.y-360;
                    cupboardOpenAngles[2] = cupboards[2].rotation.eulerAngles.y-360;
                    cupboardOpenAngles[3] = cupboards[3].rotation.eulerAngles.x;
                    break;
                case Preset.Cooking:
                    cupboardOpenAngles[4] = cupboards[4].rotation.eulerAngles.y;
                    cupboardOpenAngles[5] = cupboards[5].rotation.eulerAngles.y-360;
                    cupboardOpenAngles[6] = cupboards[6].rotation.eulerAngles.y-360;
                    break;
                case Preset.Garden:
                    cupboardOpenAngles[7] = cupboards[7].rotation.eulerAngles.y;
                    gardenEndPos = garden.transform.localPosition;
                    doorEndTemp= doorCollider.position;
                    break;
            }
        }
        if (!animate)
        {
            return;
        }
        timer += open ?  Time.deltaTime: -Time.deltaTime ;
        float timeScaled = timer / animTime;
        switch (animatingFrom)
        {
            case Preset.Sleeping:
                cupboards[0].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[0], cupboardOpenAngles[0], timeScaled),0);
                cupboards[1].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[1], cupboardOpenAngles[1], timeScaled),0);
                cupboards[2].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[2], cupboardOpenAngles[2], timeScaled),0);
                cupboards[3].rotation = Quaternion.Euler(Mathf.Lerp(cupboardClosedAngles[3], cupboardOpenAngles[3], timeScaled),0,0);
                break;
            case Preset.Cooking:
                cupboards[4].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[4], cupboardOpenAngles[4], timeScaled),0);
                cupboards[5].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[5], cupboardOpenAngles[5], timeScaled),0);
                cupboards[6].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[6], cupboardOpenAngles[6], timeScaled),0);
                if (timeScaled<0f)
                {
                    timeScaled = 0.01f;
                }else if (timeScaled >0.99f)
                {
                    timeScaled = 0.99f;
                }
                tableAnimator.SetFloat(Mapping,(timeScaled-1)*-1);
                break;
            case Preset.Garden:
                cupboards[7].rotation = Quaternion.Euler(0,Mathf.Lerp(cupboardClosedAngles[7], cupboardOpenAngles[7], timeScaled),0);
                garden.localPosition = Vector3.Lerp(gardenStartPos,gardenEndPos,timeScaled);
                doorCollider.position = Vector3.Lerp(doorStartTemp, doorEndTemp, timeScaled);
                break;
        }

        if (!open && 0 >timer)
        {
            open = true;
            animatingFrom = animatingTo;
        }
        if (open && timer >animTime)
        {
            animate = false;
        }
    }

    public void AnimateDoor()
    {
        Vector3 endRel = doorColliderEnd.position - doorColliderStart.position;
        Vector3 tRel = doorCollider.position - doorColliderStart.position;
        doorT = Mathf.InverseLerp(0,endRel.magnitude,tRel.magnitude);
        if (doorT<0f)
        {
            doorT = 0f;
        }else if (doorT >0.99f)
        {
            doorT = 0.99f;
        }
        doorAnimator.SetFloat(Mapping,doorT);
        
    }
}
