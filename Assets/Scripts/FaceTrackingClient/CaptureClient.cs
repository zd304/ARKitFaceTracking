using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;

[RequireComponent(typeof(ARFace))]
public class CaptureClient : MonoBehaviour
{
    private FaceTrackingClinet client;
    private ARFaceManager faceManager;
    
    public static float coefficientThreshold = 0.01f;

    private ARKitFaceSubsystem aRKitFaceSubsystem;

    private Dictionary<ARKitBlendShapeLocation, float> coefficients = new Dictionary<ARKitBlendShapeLocation, float>();

    private ARFace face;

    private bool start = false;

    private void Awake()
    {
        face = GetComponent<ARFace>();
        client = FindObjectOfType<FaceTrackingClinet>();
        faceManager = FindObjectOfType<ARFaceManager>();

        if (faceManager != null)
        {
            aRKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
        }

        start = false;

        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        if (face == null || aRKitFaceSubsystem == null || client == null)
        {
            return;
        }

        if (!start && client.IsConnect)
        {
            HandShake handShake = new HandShake()
            {
                clientTime = UnityEngine.Time.realtimeSinceStartup
            };
            client.Send(MsgCmd.HandShake, handShake);

            start = true;
        }
        if (!start)
        {
            return;
        }

        BlendShapesChg msg = new BlendShapesChg();

        using (var blendShapes = aRKitFaceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp))
        {
            foreach (var featureCoefficient in blendShapes)
            {
                msg.blendShapes[(int)featureCoefficient.blendShapeLocation] = featureCoefficient.coefficient * 10000.0f;
            }
        }

        client.Send(MsgCmd.BlendShapesChg, msg);
    }
}