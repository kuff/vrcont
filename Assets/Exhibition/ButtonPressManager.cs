using System.Collections;
using System.Collections.Generic;
using HandTrackingData;
using UnityEngine;

public class ButtonPressManager : MonoBehaviour
{
    public HandDataHolder holder;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            holder.LogHandPosition();
            holder.isLogging = true;
        }
        else
        {
            holder.isLogging = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            holder.tally++;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
            holder.SetToGesture(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            holder.SetToGesture(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            holder.SetToGesture(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            holder.SetToGesture(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            holder.SetToGesture(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            holder.SetToGesture(5);
        if (Input.GetKeyDown(KeyCode.H))
            holder.SwapHands();
    }
}
