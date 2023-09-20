using UnityEngine;

namespace HandTrackingData.Data
{
    public struct HandPostionsForCalc
    {
        public Vector3 wristPosition;
        public Quaternion wristRotation;
        public Vector3[] tips;
        public Vector3[] knuckles;
        public Vector3[] corners;
        public HandPostionsForCalc(Vector3 wristPosition,Quaternion wristRotation,Vector3[] tips,Vector3[] knuckles,Vector3[] corners)
        {
            this.wristRotation = wristRotation;
            this.wristPosition = wristPosition;
            this.tips = tips;
            this.corners = corners;
            this.knuckles = knuckles;
        }
    }
}