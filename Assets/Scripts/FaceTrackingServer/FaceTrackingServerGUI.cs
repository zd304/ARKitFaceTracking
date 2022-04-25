using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(ServerController))]
public class FaceTrackingServerGUI : MonoBehaviour
{
#if UNITY_EDITOR
    ServerController controller;

    public GameObject adjustsItemPrefab;

    private List<AdjustController> adjustControllers = new List<AdjustController>();

    private class AdjustController
    {
        public FaceTrackingServerAdjustItemGUI view;
        public BlendShapeAdjust model;
    }

    private void Awake()
    {
        controller = GetComponent<ServerController>();
    }

    void Update()
    {
        for (int i = 0; i < adjustControllers.Count; ++i)
        {
            AdjustController c = adjustControllers[i];
            c.model.offset = c.view.Offset;
            c.model.scale = c.view.Scale;
            c.model.location = c.view.Location;
        }
    }

    public void OnSelectFace()
    {
        string facePath = EditorUtility.OpenFilePanel("Select Face", "Assets/ServerAssets/Faces", "prefab");
        if (string.IsNullOrEmpty(facePath))
        {
            return;
        }
        facePath = $"Assets{facePath.Substring(Application.dataPath.Length)}";
        GameObject inst = AssetDatabase.LoadAssetAtPath<GameObject>(facePath);
        inst = GameObject.Instantiate(inst);
        RemoteFace face = inst.GetComponent<RemoteFace>();
        if (face == null)
        {
            EditorUtility.DisplayDialog("Error", "The GameObject has not RemoteFace component", "OK");
            GameObject.Destroy(inst);
            return;
        }
        if (controller.remoteFace != null)
        {
            GameObject.Destroy(controller.remoteFace.gameObject);
        }
        controller.remoteFace = face;

        for (int i = 0; i < controller.remoteFace.adjusts.Count; ++i)
        {
            BlendShapeAdjust adjust = controller.remoteFace.adjusts[i];
            BindAdjustGUI(adjust);
        }
    }

    public void OnAddAdjust()
    {
        if (controller.remoteFace == null)
        {
            return;
        }
        BlendShapeAdjust adjust = new BlendShapeAdjust()
        {
            location = UnityEngine.XR.ARKit.ARKitBlendShapeLocation.BrowDownLeft,
            offset = 0.0f,
            scale = 1.0f
        };

        controller.remoteFace.adjusts.Add(adjust);

        BindAdjustGUI(adjust);
    }

    private void BindAdjustGUI(BlendShapeAdjust adjust)
    {
        GameObject instAdjustItem = GameObject.Instantiate(adjustsItemPrefab);
        instAdjustItem.transform.SetParent(adjustsItemPrefab.transform.parent, false);
        instAdjustItem.name = adjust.location.ToString();
        instAdjustItem.SetActive(true);

        var view = instAdjustItem.GetComponent<FaceTrackingServerAdjustItemGUI>();
        view.Offset = adjust.offset;
        view.Scale = adjust.scale;
        view.Location = adjust.location;

        AdjustController c = new AdjustController();
        c.view = view;
        c.model = adjust;

        adjustControllers.Add(c);

        view.onDelete = () =>
        {
            if (controller.remoteFace == null)
            {
                return;
            }
            controller.remoteFace.adjusts.Remove(adjust);
            adjustControllers.Remove(c);
            GameObject.Destroy(instAdjustItem);
        };
    }
#endif
}
