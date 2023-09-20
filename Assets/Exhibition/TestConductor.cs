using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HandTrackingData.DataGetter;
using Tasks;
using TMPro;
using UnityEngine;

public enum TestType
{
    NoActivationMethod,
    ActivationGesture,
    ActivationZone
}
public class TestConductor : MonoBehaviour
{
    public HandDataGetter handDataGetter;
    public TaskManager taskManager;
    public ExebitionManager exebitionManager;

    public bool started;
    private StreamWriter sw;

    public int participant;
    public TestType testType = TestType.NoActivationMethod;
    public int fps;
    public bool block;
    public TextMeshProUGUI startedText;
    public TextMeshProUGUI testTypeText;
    public TextMeshProUGUI lastLoggedText;
    public TextMeshProUGUI fpsText;

    public GameObject vrRig;
    public GameObject vrEye;
    public GameObject positionInWorld;
    private void Update()
    {
        fps =(int) (1.0f / Time.deltaTime);
        fpsText.text = "FPS:"+fps;
        testTypeText.text = testType.ToString();
        handDataGetter.testType = testType;
        if (Input.GetKeyDown(KeyCode.M))
        {
            ResetPosition();
        }
        if (!started && Input.GetKeyDown(KeyCode.S))
        {
            startedText.text = "Started";
            started = true;
            exebitionManager.ResetSetting();
            taskManager.ResetTasks();
            string startTime = DateTime.Now.ToString();
            startTime = startTime.Replace(":", "-"); // Windows' filsystem kan ikke bruge kolon
            startTime = startTime.Replace("/", "-");
            startTime = startTime.Replace(@"\", "-");
            string path =
                @$"{Environment.CurrentDirectory}\Assets/TestData\{startTime} participant {participant} {testType}.txt"; // Directory path + data m. tidspunkt
            Debug.Log("printing to :" + path);
            sw = File.CreateText(path);
            taskManager.started = true;
        }

        if (!started)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sw.WriteLine(participant+","+testType+","+fps+","+taskManager.taskNumber+","+1+","+Time.time+",Location");
            sw.Flush();
            lastLoggedText.text = "Last logged was: Location";
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sw.WriteLine(participant+","+testType+","+fps+","+taskManager.taskNumber+","+1+","+Time.time+",Season");
            sw.Flush();
            lastLoggedText.text = "Last logged was: Season";
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            sw.WriteLine(participant+","+testType+","+fps+","+taskManager.taskNumber+","+1+","+Time.time+",TimeOfDay");
            sw.Flush();
            lastLoggedText.text = "Last logged was: TimeOfDay";
            return;
        }

        sw.WriteLine(participant+","+testType+","+fps+","+taskManager.taskNumber+","+0+","+Time.time);
        sw.Flush();
    }

    private void ResetPosition()
    {
        Vector3 transformPosition = positionInWorld.transform.position-vrEye.transform.position;
        transformPosition.y = 0;
        
        vrRig.transform.position += transformPosition;
        vrRig.transform.rotation = positionInWorld.transform.rotation;
    }

    public void LogChange(GestureType predictedGesture, int hand)
    {
        if (!started)
        {
            return;
        }
        sw.WriteLine(participant+","+testType+","+fps+","+taskManager.taskNumber+","+2+","+Time.time+","+predictedGesture+","+hand);
        sw.Flush();
    }
}
