using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Unity.Collections;

[RequireComponent(typeof(ARFace))]
public class BlendShapeVisualizer : MonoBehaviour
{
    [SerializeField]
    private BlendShapeMappings blendShapeMappings;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private ARKitFaceSubsystem aRKitFaceSubsystem;

    private Dictionary<ARKitBlendShapeLocation, int> faceArkitBlendShapeIndexMap = new Dictionary<ARKitBlendShapeLocation, int>();

    private ARFace face;

    private void Awake()
    {
        face = GetComponent<ARFace>();
        CreateFeatureBlendMapping();
    }

    void CreateFeatureBlendMapping()
    {
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
        {
            return;
        }
        if (blendShapeMappings == null || blendShapeMappings.mappings == null || blendShapeMappings.mappings.Count == 0)
        {
            Debug.LogError("Mappings must be configured before using BlendShapeVisualizer");
            return;
        }

        foreach (Mapping mapping in blendShapeMappings.mappings)
        {
            faceArkitBlendShapeIndexMap[mapping.location] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(mapping.name);
        }

        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; ++i)
        {
            string name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            Debug.LogError("-------- " + i + " : " + name);
        }
    }

    void SetVisible(bool visible)
    {
        if (skinnedMeshRenderer == null)
        {
            return;
        }
        skinnedMeshRenderer.enabled = visible;
    }

    void UpdateVisibility()
    {
        var visible = enabled && (face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);
        SetVisible(visible);
    }

    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null)
        {
            aRKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
        }

        UpdateVisibility();

        face.updated += OnUpdate;
        ARSession.stateChanged += OnSystemStateChanged;
    }

    private void OnDisable()
    {
        face.updated -= OnUpdate;
        ARSession.stateChanged -= OnSystemStateChanged;
    }

    void OnUpdate(ARFaceUpdatedEventArgs args)
    {
        UpdateVisibility();
        UpdateFaceFeatures();
    }

    void OnSystemStateChanged(ARSessionStateChangedEventArgs eventArgs)
    {
        UpdateVisibility();
    }

    void UpdateFaceFeatures()
    {
        if (skinnedMeshRenderer == null || !skinnedMeshRenderer.enabled || skinnedMeshRenderer.sharedMesh == null)
        {
            return;
        }

        using (var blendShapes = aRKitFaceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp))
        {
            foreach (var featureCoefficient in blendShapes)
            {
                int mappedBlendShapeIndex;
                if (faceArkitBlendShapeIndexMap.TryGetValue(featureCoefficient.blendShapeLocation, out mappedBlendShapeIndex))
                {
                    if (mappedBlendShapeIndex >= 0)
                    {
                        skinnedMeshRenderer.SetBlendShapeWeight(mappedBlendShapeIndex, featureCoefficient.coefficient * blendShapeMappings.coefficientScale);
                    }
                }
            }
        }
    }
}
