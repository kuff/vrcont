using UnityEngine;

public class TrainedClassifier : MonoBehaviour
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
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private bool useAngleCancel;
	[SerializeField] private Color notSelectedColour;
	[SerializeField] private Color selectedColour;
	[SerializeField] private Color chargingColour;
	[SerializeField] private Color recognisedColour;
	public GameObject arrows;
	public bool useEndOfGestureRecognition;

	public GestureType[] predictedPerHand;
	
	public GestureType predicted;
	public double[] output;
	public float timer;
	[SerializeField] private bool dynamicTracking;
	public Vector3 wristPos;
	public Vector3 startPos;
	public Vector3 startRote;
	[SerializeField] private float dot;
	public float distance;
	public int classifyingRightHand;
	private float[] timerPerHand;
	private bool activated;

	void Start()
	{
		if (networkFile)
		{
			_neuralNetwork = NetworkSaveData.LoadNetworkFromData(networkFile.text);
		}
	}

	public void PredictBothHands(double[] leftHandInputs,double[] rightHandInputs)
	{
		
		
		GestureType leftPredictedGesture = PredictedGesture(leftHandInputs);
		if (leftPredictedGesture!= predictedPerHand[0] )
		{
			timerPerHand[0] =0;
		}
		GestureType rightPredictedGesture = PredictedGesture(rightHandInputs);
		if (rightPredictedGesture!= predictedPerHand[1] )
		{
			timerPerHand[1] =0;
		}

		predictedPerHand[0] = leftPredictedGesture;
		predictedPerHand[1] = rightPredictedGesture;
		if (predicted != GestureType.None && 
		    predicted != GestureType.None1 &&
		    predicted != GestureType.None2 )
		{
			timer += Time.deltaTime;
			HandShaders[classifyingRightHand].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timer/reconTime));
		}
	}
	public void RecognizeHand(bool isRightHand, double[] handInputs, Vector3 wristPosition, Vector3 headRight)
	{
		wristPos = wristPosition;
		int b = isRightHand ? 1 :0;
		if (b!=classifyingRightHand )
		{
			HandShaders[classifyingRightHand].SetColor(Property, notSelectedColour);
			ResetRecognition();
			dynamicTracking = false;
		}
		classifyingRightHand = b;
		
		HandShaders[classifyingRightHand].SetColor(Property, selectedColour);
		if (dynamicTracking)
		{
			HandShaders[classifyingRightHand].SetColor(Property, recognisedColour);
			//HandShaders[1-classifyingRightHand].SetColor(Property, Color.red);
			DynamicTrackHand(handInputs);
			return;
		}

		GestureType predictedGesture = PredictedGesture(handInputs);
		
		if (predictedGesture!= predicted )
		{
			ResetRecognition();
		}
		predicted = predictedGesture;
		if (predicted != GestureType.None && 
		    predicted != GestureType.None1 &&
		    predicted != GestureType.None2 )
		{
			timer += Time.deltaTime;
			HandShaders[classifyingRightHand].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timer/reconTime));
		}

		if (timer > reconTime)
		{
			dynamicTracking = true;
			startPos = wristPosition;
			arrows.transform.position = wristPosition;
			arrows.transform.right = headRight;
			startRote = headRight;
			audioSource.clip = gestureReconisedSound;
			audioSource.Play();
			arrows.SetActive(true);
		}
	}
	public void RecognizeBoth(bool isRightHand, double[] handInputs, Vector3 wristPosition, Vector3 headRight)
	{
		wristPos = wristPosition;
		
		HandShaders[1].SetColor(Property, selectedColour);
		if (dynamicTracking)
		{
			HandShaders[1].SetColor(Property, recognisedColour);
			//HandShaders[1-classifyingRightHand].SetColor(Property, Color.red);
			DynamicTrackHand(handInputs);
			return;
		}

		GestureType predictedGesture = PredictedGesture(handInputs);
		
		if (predictedGesture!= predicted )
		{
			ResetRecognition();
		}
		predicted = predictedGesture;
		if (predicted != GestureType.None&& 
		    predicted != GestureType.None1&&
		    predicted != GestureType.None2&&
		    predicted != GestureType.Activation)
		{
			timer += Time.deltaTime;
			HandShaders[1].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timer/reconTime));
		}

		if (timer > reconTime)
		{
			dynamicTracking = true;
			startPos = wristPosition;
			arrows.transform.position = wristPosition;
			arrows.transform.right = headRight;
			startRote = headRight;
			audioSource.clip = gestureReconisedSound;
			audioSource.Play();
			//arrows.SetActive(true);
		}
	}

	private GestureType PredictedGesture(double[] handInputs)
	{
		GestureType predictedGesture;
		int pred;
		(pred, output) = _neuralNetwork.Classify(handInputs);

		if (output[pred] > cutoff)
		{
			predictedGesture = (GestureType)pred;
		}
		else
		{
			predictedGesture = GestureType.None;
		}

		return predictedGesture;
	}

	public void DynamicTrackHand(double[] handInputs)
	{
		distance = Vector3.Distance(wristPos,startPos);
		if (distance>triggerDistance)
		{
			//arrows.SetActive(false);
			dot = Vector3.Dot(startRote.normalized, wristPos.normalized);
			dynamicTracking = false;
			timer = 0;
			if (useEndOfGestureRecognition)
			{
				if (predicted != PredictedGesture(handInputs))
				{
					return;
				}
			}
			if (dot < cancelAngle && dot > -cancelAngle&&useAngleCancel)
			{
				return;
			}
			exebitionManager.Activate(predicted,dot>0,1);
			audioSource.clip = gestureActivatedSound;
			audioSource.Play();
			//arrows.SetActive(false);
		}
	}

	public void ResetRecognition()
	{
		timer =0;
	}
	public void NoHandsInArea()
	{
		ResetRecognition();
		HandShaders[0].SetColor(Property, notSelectedColour);
		HandShaders[1].SetColor(Property, notSelectedColour);
	}

	public void RecognizeHand(double[]  handInputs, Vector3 wristPosition, Vector3 headRight)
	{
		wristPos = wristPosition;
		HandShaders[1].SetColor(Property, selectedColour);
		if (dynamicTracking)
		{
			HandShaders[1].SetColor(Property, recognisedColour);
			DynamicTrackHand(handInputs);
			return;
		}
		GestureType predictedGesture = PredictedGesture(handInputs);
		if (!activated&&predictedGesture == GestureType.Activation)
		{
			activated = true;
			HandShaders[1].SetColor(Property, chargingColour);
		}

		if (activated && 
		    predicted != GestureType.None&& 
		    predicted != GestureType.None1&&
		    predicted != GestureType.None2&&
		    predicted != GestureType.Activation)
		{
			dynamicTracking = true;
			startPos = wristPosition;
			arrows.transform.position = wristPosition;
			arrows.transform.right = headRight;
			startRote = headRight;
			audioSource.clip = gestureReconisedSound;
			audioSource.Play();
		}
		predicted = predictedGesture;
		if (predicted != GestureType.None&& 
		    predicted != GestureType.None1&&
		    predicted != GestureType.None2)
		{
			timer += Time.deltaTime;
			HandShaders[1].SetColor(Property, Color.Lerp(selectedColour,chargingColour,timer/reconTime));
		}
	}
}
