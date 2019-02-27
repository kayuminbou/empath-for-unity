using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class EmpathManager : MonoBehaviour
{
    [SerializeField]
    private string m_subscriptionKey = string.Empty;

    [SerializeField]
    private Text empathResult;

    private AudioClip micClip;
    private float[] microphoneBuffer;

    private int head;
    private int position;
    private bool isRecording;

    public int maxRecordingTime;
    private const int samplingFrequency = 11025;
    private string micDeviceName;

    const int HEADER_SIZE = 44;
    const float rescaleFactor = 32767; //to convert float to Int16

    public void Start()
    {
        micDeviceName = Microphone.devices[0];
    }

    public void ButtonClicked()
    {
        if (!isRecording)
        {
            StartCoroutine(RecordingForEmpathAPI());
        }
    }

    public IEnumerator RecordingForEmpathAPI()
    {
        RecordingStart();

        yield return new WaitForSeconds(maxRecordingTime);

        RecordingStop();

        yield return null;
    }

    public void RecordingStart()
    {
        StartCoroutine(WavRecording(micDeviceName, maxRecordingTime, samplingFrequency));
    }

    public void RecordingStop()
    {
        isRecording = false;
        position = Microphone.GetPosition(null);
        Microphone.End(micDeviceName);
        Debug.Log("Recording end");
        byte[] empathByte = WavUtility.FromAudioClip(micClip);
        StartCoroutine(Upload(empathByte));
    }

    IEnumerator Upload(byte[] wavbyte)
    {
        WWWForm form = new WWWForm();
        form.AddField("apikey", m_subscriptionKey);
        form.AddBinaryData("wav", wavbyte);
        string receivedJson = null;

        using (WWW www = new WWW("https://api.webempath.net/v2/analyzeWav", form))
        {
            yield return www;
            Debug.Log(www.text);
            receivedJson = www.text;
        }

        EmpathData empathData = ConvertEmpathToJson(receivedJson);
        empathResult.text = ConvertEmpathDataToString(empathData);
    }

    public IEnumerator WavRecording(string micDeviceName, int maxRecordingTime, int samplingFrequency)
    {
        Debug.Log("Recording start");
        //Recording開始
        isRecording = true;

        //Buffer
        microphoneBuffer = new float[maxRecordingTime * samplingFrequency];
        //録音開始
        micClip = Microphone.Start(deviceName: micDeviceName, loop: false,
                                   lengthSec: maxRecordingTime, frequency: samplingFrequency);
        yield return null;
    }

    public EmpathData ConvertEmpathToJson(string json)
    {
        Debug.AssertFormat(!string.IsNullOrEmpty(json), "Jsonの取得に失敗しています。");

        EmpathData empathData = null;

        try
        {
            empathData = JsonUtility.FromJson<EmpathData>(json);
        }
        catch (System.Exception i_exception)
        {
            Debug.LogWarningFormat("Jsonをクラスへ変換することに失敗しました。exception={0}", i_exception);
            empathData = null;
        }
        return empathData;
    }

    public string ConvertEmpathDataToString(EmpathData empathData)
    {
        string result;
        if(empathData.error == 0)
        {
            int calm = empathData.calm;
            int anger = empathData.anger;
            int joy = empathData.joy;
            int sorrow = empathData.sorrow;
            int energy = empathData.energy;
            result = "calm : " + calm +
                     "\nanger : " + anger +
                     "\njoy : " + joy + 
                     "\nsorrow : " + sorrow + 
                     "\nenergy : " + energy;
        }
        else
        {
            int error = empathData.error;
            string msg = empathData.msg;
            result = "error : " + error +
                     "\nmsg : " + msg;
        }
        return result;
    }
}
