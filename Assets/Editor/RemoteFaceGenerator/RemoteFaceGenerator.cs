using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkinnedMeshAndBlendShape
{
    public SkinnedMeshRenderer renderer;
    public string path;
}

public class RemoteFaceGenerator : EditorWindow
{
    static RemoteFaceGenerator instance = null;

    private GameObject prefab;
    private List<SkinnedMeshAndBlendShape> blendshapes = new List<SkinnedMeshAndBlendShape>();
    private int blendshapeSelIndex = -1;
    private BlendShapeMappings blendShapeMapping = null;

    [MenuItem("Tools/Remote Face Generator")]
    static void Init()
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }
        instance = GetWindow<RemoteFaceGenerator>();
        instance.titleContent = new GUIContent("Remote Face Generator");
        instance.Show();
    }

    private void OnEnable()
    {
        blendShapeMapping = AssetDatabase.LoadAssetAtPath<BlendShapeMappings>("Assets/Resources/Models/StandardBlendShapeMappingAsset.asset");
    }

    private void OnGUI()
    {
        GameObject oldPrefab = prefab;

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Blend Shape Mapping Asset", blendShapeMapping, typeof(BlendShapeMappings), false);
        GUI.enabled = true;

        EditorGUILayout.HelpBox("将角色模型的FBX文件拖入下面的框里", MessageType.Info);
        prefab = (GameObject)EditorGUILayout.ObjectField("FBX", prefab, typeof(GameObject), false);
        if (oldPrefab != prefab && prefab != null)
        {
            SkinnedMeshRenderer[] renderers = prefab.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var r in renderers)
            {
                if (r.sharedMesh.blendShapeCount == 0)
                {
                    continue;
                }
                SkinnedMeshAndBlendShape smabs = new SkinnedMeshAndBlendShape()
                {
                    renderer = r,
                    path = GetPath(r.transform)
                };
                blendshapes.Add(smabs);
            }
        }
        else if (prefab == null)
        {
            blendshapeSelIndex = -1;
            blendshapes.Clear();
        }

        if (blendshapes.Count != 0)
        {
            if (blendshapeSelIndex < 0)
            {
                for (int i = 0; i < blendshapes.Count; ++i)
                {
                    SkinnedMeshAndBlendShape smabs = blendshapes[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("    " + smabs.renderer.name, smabs.renderer, typeof(SkinnedMeshRenderer), false);
                    if (GUILayout.Button("选择"))
                    {
                        blendshapeSelIndex = i;
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                SkinnedMeshAndBlendShape smabs = blendshapes[blendshapeSelIndex];
                EditorGUILayout.ObjectField("    " + smabs.renderer.name, smabs.renderer, typeof(SkinnedMeshRenderer), false);
            }
        }

        if (prefab != null && blendshapeSelIndex >= 0 && GUILayout.Button("保存"))
        {
            SkinnedMeshAndBlendShape smabs = blendshapes[blendshapeSelIndex];
            GameObject inst = GameObject.Instantiate(prefab);
            inst.name = prefab.name;
            RemoteFace remoteFace = inst.AddComponent<RemoteFace>();

            Transform trans = inst.transform.Find(smabs.path);
            if (trans != null)
            {
                remoteFace.skinnedMeshRenderer = trans.GetComponent<SkinnedMeshRenderer>();
            }
            remoteFace.blendShapeMappings = blendShapeMapping;

            string prefabPath = "Assets/ServerAssets/Faces/" + inst.name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(inst, prefabPath);
            GameObject.DestroyImmediate(inst);

            EditorUtility.DisplayDialog("保存成功", "保存路径：" + prefabPath, "确定");
        }
    }

    private string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            if (t.parent != null && t.parent.parent != null)
            {
                path = $"{t.parent}/{path}";
            }
            t = t.parent;
        }
        return path;
    }
}
