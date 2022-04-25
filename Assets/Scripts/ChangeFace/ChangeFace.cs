using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARFaceManager))]
public class ChangeFace : MonoBehaviour
{
    private Material faceMaterial;
    ARFaceManager arFaceManager;
    private int textureIndex = 0;

    public Texture2D[] texture2Ds = null;

    void Awake()
    {
        arFaceManager = GetComponent<ARFaceManager>();
        if (arFaceManager == null)
        {
            return;
        }
        GameObject facePrefab = arFaceManager.facePrefab;
        if (facePrefab == null)
        {
            return;
        }
        MeshRenderer renderer = facePrefab.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            return;
        }
        faceMaterial = renderer.material;
    }

    private void OnEnable()
    {
        if (arFaceManager == null)
        {
            return;
        }
        arFaceManager.facesChanged += OnFacesChanged;
    }

    private void OnDisable()
    {
        if (arFaceManager == null)
        {
            return;
        }
        arFaceManager.facesChanged -= OnFacesChanged;
    }

    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (var trackedFace in eventArgs.added)
        {
            OnFaceAdded(trackedFace);
        }
    }

    private void OnFaceAdded(ARFace refFace)
    {
        if (texture2Ds == null || texture2Ds.Length == 0)
        {
            return;
        }
        textureIndex = (++textureIndex) % texture2Ds.Length;
        faceMaterial.mainTexture = texture2Ds[textureIndex];
    }
}
