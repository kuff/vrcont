using System;
using System.Collections.Generic;
using Exhibition;
using HandTrackingData.Data;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Tasks;
using UnityEditor;
using UnityEngine;

namespace HandTrackingData.DataGetter
{
    public class HandDataGetter: MonoBehaviour
    {
        [SerializeField] private Transform head;
        
        [SerializeField] private Hand rightHand;
        
        [SerializeField] private Hand leftHand;
        [SerializeField] private HandVisual rightHandVisual;
        
        
        [SerializeField] private HandVisual leftHandVisual;
        private HandVisual currentHandVisual;
        private HandAspect[] handAspects = new HandAspect[2];
        [SerializeField] private bool inited;
        [SerializeField] private TrainedClassifier _classifier;

        [SerializeField] private float cutoffAngle;
        

        [SerializeField] private float rightHandAngle;
        [SerializeField] private float leftHandAngle;
        // Nothing, Activation Gesture , Activation Zone
        [SerializeField] public TestType testType;

        [SerializeField] private HandRecogniser _handRecogniser;
        [SerializeField] private bool[] activated = new bool[2];
        [SerializeField] private GameObject activationGestureFrame;

        
        public void Update()
        {
            
            activationGestureFrame.SetActive(testType==TestType.ActivationGesture);
            if (!rightHand.IsConnected&&!leftHand.IsConnected) return;
            switch (testType)
            {
                case TestType.NoActivationMethod:
                    if (rightHand.IsConnected)
                    {
                        _handRecogniser.TriggerHand(1,rightHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(rightHandVisual)),rightHandVisual.Joints[0].forward);
                    }
                    if (leftHand.IsConnected)
                    {
                        _handRecogniser.TriggerHand(0,leftHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(leftHandVisual)),leftHandVisual.Joints[0].forward);
                    }
                    break;
                case TestType.ActivationGesture:
                    if (rightHand.IsConnected)
                    {
                        _handRecogniser.ActivateHand(1,rightHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(rightHandVisual)),rightHandVisual.Joints[0].forward);
                    }
                    if (leftHand.IsConnected)
                    {
                        _handRecogniser.ActivateHand(0,leftHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(leftHandVisual)),leftHandVisual.Joints[0].forward);
                    }
                    break;
                case TestType.ActivationZone:
                    if (rightHand.IsConnected)
                    {
                        _handRecogniser.TriggerZoneHand(1,rightHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(rightHandVisual)),rightHandVisual.Joints[0].forward,head);
                    }
                    if (leftHand.IsConnected)
                    {
                        _handRecogniser.TriggerZoneHand(0,leftHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(leftHandVisual)),leftHandVisual.Joints[0].forward,head);
                    }
                    return;
                    var forward = head.forward;
                    var position = head.position;
                    rightHandAngle = Vector3.Angle (forward,  rightHandVisual.Joints[0].position-position);
                    leftHandAngle = Vector3.Angle (forward,  leftHandVisual.Joints[0].position-position);
                    
                    //_handRecogniser.DynamicCheck();
                    if (rightHandAngle>cutoffAngle&&leftHandAngle>cutoffAngle )
                    {
                        return;
                    }
                    int hand = rightHandAngle<leftHandAngle? 1:0;
                    currentHandVisual = hand ==1?rightHandVisual:leftHandVisual;
                    _handRecogniser.TriggerHand(hand,currentHandVisual.Joints[0].position,head.right,HandAspect.ConvertHandInputs(Convert(currentHandVisual)),currentHandVisual.Joints[0].forward);
                    _handRecogniser.ResetHand(1-hand);
                    break;
            }
            
        }
        private HandPostionsForCalc Convert(HandVisual hand)
        {
            Vector3[] tips =
            {
                hand.Joints[5].position,
                hand.Joints[8].position,
                hand.Joints[11].position,
                hand.Joints[14].position,
                hand.Joints[18].position
            };
            Vector3[] corners =
            {
                hand.Joints[4].position,
                hand.Joints[7].position,
                hand.Joints[10].position,
                hand.Joints[13].position,
                hand.Joints[17].position
            };Vector3[] knuckles =
            {
                hand.Joints[4].position,
                hand.Joints[6].position,
                hand.Joints[9].position,
                hand.Joints[12].position,
                hand.Joints[16].position
            };
            return new HandPostionsForCalc(hand.Joints[0].position,hand.Joints[0].rotation,tips,knuckles,corners);
        }
        private HandPostionsForCalc ConvertHandVisualToHandPositions(HandVisual hand)
        {
            Vector3[] tips =
            {
                hand.Joints[5].position,
                hand.Joints[8].position,
                hand.Joints[11].position,
                hand.Joints[14].position,
                hand.Joints[18].position
            };
            Vector3[] corners =
            {
                hand.Joints[4].position,
                hand.Joints[7].position,
                hand.Joints[10].position,
                hand.Joints[13].position,
                hand.Joints[17].position
            };Vector3[] knuckles =
            {
                hand.Joints[4].position,
                hand.Joints[6].position,
                hand.Joints[9].position,
                hand.Joints[12].position,
                hand.Joints[16].position
            };
            return new HandPostionsForCalc(hand.Joints[0].position,hand.Joints[0].rotation,tips,knuckles,corners);
        }
    }
}