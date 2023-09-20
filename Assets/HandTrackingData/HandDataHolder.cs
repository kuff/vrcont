using System;
using System.IO;
using HandTrackingData.Data;
using UnityEngine;

namespace HandTrackingData
{
    
    [CreateAssetMenu(menuName = "Create HandDataHolder", fileName = "HandDataHolder", order = 0)]
    public class HandDataHolder : ScriptableObject
    {
        public static double InverseLerp(double a, double b, double value)
        {
            return (value - a) / (b - a);
        }
        public string participant;
        public GestureType currentType;
        public bool isRightHand;
        public float handLength;
        public float handWidth;
        public HandAspect[] HandAspects = new HandAspect[2];
        private StreamWriter sw;
        [SerializeField] private bool streamWriterInited = false;
        private Vector3[,] HandPositions = new Vector3[2,24];
        [SerializeField] private Vector3 headPosition;
        [SerializeField] private Vector3 headRotation;
        [SerializeField] public Vector3 headRight;
        [SerializeField] public Vector3 thumbDirection;
        
        public double[] handInputs;
        public int index;
        public int tally;
        // Values to normalize the HandAspects
        public double[] maxes = new double[21]{0.09021042,0.1111464,0.09921884,0.1323981,0.06017102,0.07532546,0.1339429,0.06814501,0.03681857,0.1253064,0.06339972,0.06077478,0.1132728,0.04930152,0.147451,0.9428114,0.9908907,0.9999788,0.30046,1.184354,0.4404907 };
        public double[] mines = new double[21]{0.06838486,0.08991423,0.01402679,0.1115697,0.03685664,0.01539352,0.1057315,0.04198776,0.01321025,0.09662139,0.03922486,0.01349914,0.09491879,0.03493643,0.04432947,-2.75817E-05,3.92234E-06,0.000278436,-0.4935929,0.7348156,0.008642531 };
        public bool isLogging;
        public Vector3 handUp;
        public Vector3 headForward;
        public bool rightHandInited;
        public event Action RightHandStarted; 
        #region FunctionsToForButtons

        public void SetToGesture(int index)
        {
            currentType = (GestureType)index;
        }
        
        public void SwapHands()
        {
            isRightHand = !isRightHand;
        }
        

        #endregion
        public void UpdateHandAspects(HandPostionsForCalc data,Vector3[] handPositions, int handedness)
        {
            for (var i = 0; i < handPositions.Length; i++)
            {
                HandPositions[handedness,i] =  handPositions[i];
            }

            if (HandAspects[handedness] == null)
            {
                HandAspects[handedness] =  new HandAspect(data);
                if (handedness ==1)
                {
                    if (RightHandStarted != null) RightHandStarted.Invoke();
                }
                return;
            }
            HandAspects[handedness].SetAllAspects(data);


            float[] inputs = new float[15] ;
            for (var finger = 0; finger < HandAspects[1].values.GetLength(0); finger++)
            for (var type = 0; type < HandAspects[1].values.GetLength(1); type++)
            {
                inputs[finger * 3 + type]= HandAspects[1].values[finger, type];
            }

            handInputs = ConvertHandAspectsToNNInputs(HandAspects[1]);
        }

        private double[] ConvertHandAspectsToNNInputs(HandAspect handAspect)
        {
            double[] handInputs = new double[18];
            int i = 0;
            handAspect = HandAspects[1];
            foreach (float aspect in handAspect.values)
            {
                handInputs[i] = InverseLerp(mines[i],maxes[i],aspect);
                i++;
            }
            Vector3 rote = handAspect.wristRotation.eulerAngles.normalized;
            handInputs[15] = InverseLerp(mines[i],maxes[i],rote.x);
            handInputs[16] = InverseLerp(mines[i],maxes[i],rote.y);
            handInputs[17] = InverseLerp(mines[i],maxes[i],rote.z);
            return handInputs;
        }


        public void LogHandPosition()
        {
            if (!streamWriterInited)
            {
                StartStreamWriter();
            }

            int hand = isRightHand ? 1 : 0;
            string tempWrite = "";

            double[] nnInputs = ConvertHandAspectsToNNInputs(HandAspects[hand]);
            
            //Log Neural Net Inputs
            //tempWrite += nnInputs[0];
            //for (var i = 1; i < nnInputs.Length; i++)
            //{
              //  tempWrite += "," + nnInputs[i];
            //}
            for (var i0 = 0; i0 < HandAspects[hand].values.GetLength(0); i0++)
            for (var i1 = 0; i1 < HandAspects[hand].values.GetLength(1); i1++)
            {
                if (i0+i1 ==0 )
                {
                    
                    tempWrite += HandAspects[hand].values[i0, i1];
                }
                tempWrite += ","+HandAspects[hand].values[i0, i1];
            }

            Vector3 rote = HandAspects[hand].wristRotation.eulerAngles.normalized;
            tempWrite += "," + rote.x;
            tempWrite += "," + rote.y;
            tempWrite += "," + rote.z;
            //Log Label
            tempWrite +=  ";"+ currentType +";";
            
            //Log Hand (0 = left,1 = right)
            tempWrite += hand+";";

            //Log hand sized defined in editor
            tempWrite += handLength + ",";
            tempWrite += handWidth + ",";
            
            //Log head Values
            tempWrite += headPosition.x+ ","+headPosition.y+ ","+headPosition.z+ ",";
            Vector3 headRote = headRotation.normalized;
            tempWrite +=  headRote.x+ ","+headRote.y+ ","+headRote.z;
            for (int i = 0; i < HandPositions.GetLength(1); i++)
            {
                tempWrite += "," + HandPositions[hand, i];
            }
            
            //Log logging values:
            //index = the datapoint number,
            //tally = logging session number(we ++ everytime we hold down "space",
            //so it represents which "space" press the datapoint is, so we can remove logging errors easily)
            //DataTime.now is the time
            tempWrite += ";"+ index.ToString() +",";
            tempWrite += tally.ToString() +",";
            tempWrite += DateTime.Now.ToString();
            
            sw.WriteLine(tempWrite);
            sw.Flush();
            index++;
        }

        private void StartStreamWriter()
        {
            string startTime = DateTime.Now.ToString();
            startTime = startTime.Replace(":", "-"); // Windows' filsystem kan ikke bruge kolon
            startTime = startTime.Replace("/", "-");
            startTime = startTime.Replace(@"\", "-");
            string path =
                @$"{Environment.CurrentDirectory}\Assets/HandTrackingData/Data\{startTime}{participant}.txt"; // Directory path + data m. tidspunkt
            Debug.Log("printing to :" + path);
            sw = File.CreateText(path);

            streamWriterInited = true;
            tally = 0;
            index = 0;
        }

        public void UpdateHeadValues(Vector3 headPosition, Vector3 headRotation, Vector3 right, Vector3 forward)
        {
            this.headPosition = headPosition;
            this.headRotation = headRotation;
            headRight = right;
            headForward = forward;
        }
        public void ResetLogger()
        {
            streamWriterInited = false;
        }
        
    }
}

