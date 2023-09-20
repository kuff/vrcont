using System;
using UnityEngine;

namespace Exhibition
{
    public class HandRecogniser : MonoBehaviour
    {
        public double cutoff =0.8;
        public int reconTime = 3;
        [SerializeField] private float triggerDistance =  0.2f;
        public float cancelAngle;
        public TextAsset networkFile;
        NeuralNetwork _neuralNetwork;
        public Material[] HandShaders;
        private static readonly int Property = Shader.PropertyToID("_ColorTop");
        public ExebitionManager exebitionManager;
        [SerializeField] private AudioClip gestureReconisedSound;
        [SerializeField] private AudioClip gestureActivatedSound;
        [SerializeField] private AudioClip gestureCanceledSound;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool useAngleCancel;
        [SerializeField] private Color notSelectedColour;
        [SerializeField] private Color selectedColour;
        [SerializeField] private Color chargingColour;
        [SerializeField] private Color recognisedColour;
        [SerializeField] private Color cancelingColour;
        [SerializeField] private Color confirmingColour;
        
        public GestureType[] predicted = new GestureType[2];
        private float[] timers = new float[2];
        public bool[] dynamicTracking = new bool[2]; 
        public Vector3[] wristPos = new Vector3[2];
        public Vector3[] startPos = new Vector3[2];
        public Vector3[] startRot = new Vector3[2];
        public bool[] activated = new bool[2];
        void Start()
        {
            if (networkFile)
            {
                _neuralNetwork = NetworkSaveData.LoadNetworkFromData(networkFile.text);
            }
        }

        private void ClassifyHand(int hand, double[] handInput,Vector3 handRight,Vector3 headRight)
        {
            GestureType predictedGesture;
            int pred;
            double[] output;
            (pred, output) = _neuralNetwork.Classify(handInput);

            if (output[pred] > cutoff)
            {
                predictedGesture = (GestureType)pred;
            }
            else
            {
                predictedGesture = GestureType.None;
            }

            if (predictedGesture is GestureType.None1 or GestureType.None2 )
            {
                predictedGesture = GestureType.None;
            }

            float dot = Vector3.Dot(handRight.normalized, headRight.normalized);
            print(dot);
            if (predictedGesture is GestureType.Activation or GestureType.Season or GestureType.TimeOfDay && dot <0 )
            {
                
                predictedGesture = GestureType.None;
            }
            if (predictedGesture is GestureType.Location && dot >0 )
            {
                predictedGesture = GestureType.None;
            }
            if (predictedGesture != predicted[hand] && predicted[hand] != GestureType.None)
            {
                predictedGesture = GestureType.None;
            }
            
            predicted[hand] = predictedGesture;
        }

        public void TriggerHand(int hand, Vector3 wristPosition, Vector3 headRight, double[] handInput,
            Vector3 handRight)
        {
            
            wristPos[hand] = wristPosition;
            if (dynamicTracking[hand])
            {
                DynamicTracking(hand);
                return;
            }
            ClassifyHand(hand,handInput,handRight,headRight);
            if (predicted[hand] != GestureType.None &&predicted[hand] != GestureType.Activation)
            {
                timers[hand] += Time.deltaTime;
                if (timers[hand] > reconTime)
                {
                    dynamicTracking[hand] = true;
                    startPos[hand] = wristPosition;
                    startRot[hand] = headRight;
                    audioSource.clip = gestureReconisedSound;
                    audioSource.Play();
                }
            }
            else
            {
                timers[hand] = Mathf.Max(0,timers[hand]-Time.deltaTime);
            }
            HandShaders[hand].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timers[hand]*timers[hand]/reconTime*reconTime));
        }
        public void TriggerZoneHand(int hand, Vector3 wristPosition, Vector3 headRight, double[] handInput,
            Vector3 handRight,Transform head)
        {
            
            wristPos[hand] = wristPosition;
            if (dynamicTracking[hand])
            {
                DynamicTracking(hand);
                return;
            }
            var forward = head.forward;
            float angle = Vector3.Angle (forward,  wristPosition-head.position);
            
            if (angle>30)
            {
                
                HandShaders[hand].SetColor(Property, notSelectedColour);
                return;
            }
            ClassifyHand(hand,handInput,handRight,headRight);
            if (predicted[hand] != GestureType.None &&predicted[hand] != GestureType.Activation)
            {
                timers[hand] += Time.deltaTime;
                if (timers[hand] > reconTime)
                {
                    dynamicTracking[hand] = true;
                    startPos[hand] = wristPosition;
                    startRot[hand] = headRight;
                    audioSource.clip = gestureReconisedSound;
                    audioSource.Play();
                }
            }
            else
            {
                timers[hand] = Mathf.Max(0,timers[hand]-Time.deltaTime);
            }
            HandShaders[hand].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timers[hand]*timers[hand]/reconTime*reconTime));
        }
        public void ActivateHand(int hand, Vector3 wristPosition, Vector3 headRight, double[] handInput,
            Vector3 handRight)
        { 
            wristPos[hand] = wristPosition;
            if (dynamicTracking[hand])
            {
                DynamicTracking(hand);
                return;
            }
            ClassifyHand(hand,handInput,handRight,headRight);
            if (predicted[hand] != GestureType.None)
            {
                if (!activated[hand] && predicted[hand] == GestureType.Activation)
                {
                    activated[hand] = true;
                    timers[hand] = 0;
                    
                    HandShaders[hand].SetColor(Property, chargingColour);
                    return;
                }

                if (!activated[hand]||predicted[hand] == GestureType.Activation)
                {
                    return;
                }
                
                timers[hand] += Time.deltaTime;
                if (timers[hand] > 0.5f)
                {
                    dynamicTracking[hand] = true;
                    activated[hand] = false;
                    startPos[hand] = wristPosition;
                    startRot[hand] = headRight;
                    audioSource.clip = gestureReconisedSound;
                    audioSource.Play();
                }
            }
            else
            {
                timers[hand] = Mathf.Max(0,timers[hand]-Time.deltaTime);
            }

            if (!activated[hand])
            {
                HandShaders[hand].SetColor(Property, selectedColour);
            }
            else
            {
                HandShaders[hand].SetColor(Property, Color.Lerp(chargingColour,recognisedColour,timers[hand]/reconTime));
            }
        }

        private void DynamicTracking(int hand)
        {
            float distance = Vector3.Distance(wristPos[hand], startPos[hand]);
            
            float dot = Vector3.Dot(startRot[hand], (startPos[hand] - wristPos[hand]).normalized);
            HandShaders[hand].SetColor(Property, Color.Lerp(recognisedColour,confirmingColour,distance/triggerDistance));
            if (dot < cancelAngle && dot > -cancelAngle && useAngleCancel && distance>0.05f) 
            {
                HandShaders[hand].SetColor(Property, Color.Lerp(recognisedColour,cancelingColour,distance/triggerDistance));
            }
            if (distance > triggerDistance)
            {
                dynamicTracking[hand] = false;
                timers[hand] = 0;
                if (dot < cancelAngle && dot > -cancelAngle && useAngleCancel)
                {
                    audioSource.clip = gestureCanceledSound;
                    audioSource.Play();
                    return;
                }
                exebitionManager.Activate(predicted[hand], dot > 0,hand);
                audioSource.clip = gestureActivatedSound;
                audioSource.Play();
            }
        }

        public void ResetHand(int hand)
        {
            timers[hand] = 0;
            dynamicTracking[hand] = false;
            HandShaders[hand].SetColor(Property, notSelectedColour);
        }

        public void DynamicCheck()
        {
            if (dynamicTracking[0])
            {
                DynamicTracking(0);
            }
            if (dynamicTracking[1])
            {
                DynamicTracking(1);
            }
        }
    }
}