using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;
using System;

[Serializable]
public class BlendShapeAdjust
{
    public ARKitBlendShapeLocation location;
    public float offset = 0.0f;
    public float scale = 1.0f;
}

public class RemoteFace : MonoBehaviour
{
    public BlendShapeMappings blendShapeMappings;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    private Dictionary<ARKitBlendShapeLocation, int> faceArkitBlendShapeIndexMap = new Dictionary<ARKitBlendShapeLocation, int>();

    public List<BlendShapeAdjust> adjusts = new List<BlendShapeAdjust>();

    private void Awake()
    {
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
        }
    }

    public void OnFaceCoefficientChange(ARKitBlendShapeLocation location, float coefficient)
    {
        int mappedBlendShapeIndex;
        if (faceArkitBlendShapeIndexMap.TryGetValue(location, out mappedBlendShapeIndex))
        {
            if (mappedBlendShapeIndex >= 0)
            {
                BlendShapeAdjust adjust = adjusts.Find((BlendShapeAdjust a) => { return a.location == location; });
                float c = coefficient * 0.01f;
                if (adjust != null)
                {
                    c = c * adjust.scale + adjust.offset;
                }
                skinnedMeshRenderer.SetBlendShapeWeight(mappedBlendShapeIndex, c);
            }
        }
    }
}