using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARKit;

public class ServerController : MonoBehaviour
{
    public string host = "192.168.122.79";
    public int port = 5304;

    public FaceTrackingServer server;

    public Text ipAndPortInput;
    public Text fpsInput;

    public RemoteFace remoteFace;

    private float timeDiff = 0.0f;
    private List<CoefficientChg> changes = new List<CoefficientChg>();
    private List<BlendShapesChg> blendShapesChanges = new List<BlendShapesChg>();
    public float delayTime = 0.1f;

    private float fpsCalcTime = 1.0f;
    private int frames = 0;

    private void Awake()
    {
        server.StartServer(host, port);
        ipAndPortInput.text = host + ":" + port;

        server.RegistHandler(MsgCmd.HandShake, OnHandShake);
        server.RegistHandler(MsgCmd.CoefficientChg, OnCoefficientChg);
        server.RegistHandler(MsgCmd.BlendShapesChg, OnBlendShapesChg);

        Application.targetFrameRate = 120;
    }

    void OnHandShake(MsgCmd cmd, byte[] data, RemoteClient client)
    {
        Debug.Log("OnHandShake");
        HandShake msg = SerializeHelper.Deserialize<HandShake>(data);
        timeDiff = msg.clientTime - Time.realtimeSinceStartup;
    }

    void OnCoefficientChg(MsgCmd cmd, byte[] data, RemoteClient client)
    {
        CoefficientChg msg = SerializeHelper.Deserialize<CoefficientChg>(data);
        msg.time -= timeDiff;

        changes.Add(msg);
    }

    void OnBlendShapesChg(MsgCmd cmd, byte[] data, RemoteClient client)
    {
        BlendShapesChg msg = SerializeHelper.Deserialize<BlendShapesChg>(data);
        msg.time -= timeDiff;

        Debug.Log("OnBlendShapesChg + " + msg.blendShapes.Length);

        blendShapesChanges.Add(msg);
    }

    private List<ARKitBlendShapeLocation> playedLoacations = new List<ARKitBlendShapeLocation>();

    private void Update()
    {
        if (fpsCalcTime > 0.0f)
        {
            fpsCalcTime -= Time.deltaTime;
            ++frames;
            if (fpsCalcTime <= 0.0f)
            {
                fpsInput.text = "FPS:" + frames;
                frames = 0;
                fpsCalcTime = 1.0f;
            }
        }

        float curTime = Time.realtimeSinceStartup;
        playedLoacations.Clear();

        for (int i = blendShapesChanges.Count - 1; i >= 0; --i)
        {
            BlendShapesChg chg = blendShapesChanges[i];
            if (curTime - delayTime >= chg.time)
            {
                if (remoteFace != null)
                {
                    for (int b = 0; b < chg.blendShapes.Length; ++b)
                    {
                        float w = chg.blendShapes[b];
                        ARKitBlendShapeLocation location = (ARKitBlendShapeLocation)b;
                        remoteFace.OnFaceCoefficientChange(location, w);
                    }
                    blendShapesChanges.RemoveAt(i);
                }
            }
        }
    }
}