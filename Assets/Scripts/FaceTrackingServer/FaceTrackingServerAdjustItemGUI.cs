using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceTrackingServerAdjustItemGUI : MonoBehaviour
{
    public Dropdown locationDropDown;
    public Slider offsetSlider;
    public InputField offsetValInput;
    public Slider scaleSlider;
    public InputField scaleValInput;

    public float Offset
    {
        set
        {
            offsetSlider.value = value;
            OnOffsetSliderChg();
        }
        get
        {
            return offsetSlider.value;
        }
    }

    public float Scale
    {
        set
        {
            scaleSlider.value = value;
            OnScaleSliderChg();
        }
        get
        {
            return scaleSlider.value;
        }
    }

    private int[] arKitBlendShapeLocationValues = null;
    private UnityEngine.XR.ARKit.ARKitBlendShapeLocation location;
    public UnityEngine.XR.ARKit.ARKitBlendShapeLocation Location
    {
        set
        {
            if (arKitBlendShapeLocationValues == null)
            {
                return;
            }
            int index = 0;
            for (int i = 0; i < arKitBlendShapeLocationValues.Length; ++i)
            {
                if ((int)value == arKitBlendShapeLocationValues[i])
                {
                    index = i;
                    break;
                }
            }
            locationDropDown.value = index;
            
            location = (UnityEngine.XR.ARKit.ARKitBlendShapeLocation)value;
        }
        get
        {
            return location;
        }
    }

    public System.Action onDelete;

    private void Awake()
    {
        arKitBlendShapeLocationValues = System.Enum.GetValues(typeof(UnityEngine.XR.ARKit.ARKitBlendShapeLocation)) as int[];
        string[] names = System.Enum.GetNames(typeof(UnityEngine.XR.ARKit.ARKitBlendShapeLocation)) as string[];
        locationDropDown.ClearOptions();
        locationDropDown.AddOptions(new List<string>(names));
    }

    public void OnOffsetSliderChg()
    {
        offsetValInput.text = ((int)offsetSlider.value).ToString();
    }

    public void OnScaleSliderChg()
    {
        scaleValInput.text = scaleSlider.value.ToString("F2");
    }

    public void OnOffsetValInputChg()
    {
        offsetSlider.value = float.Parse(offsetValInput.text);
    }

    public void OnScaleValInputChg()
    {
        scaleSlider.value = float.Parse(scaleValInput.text);
    }

    public void OnLoacationChg()
    {
        if (arKitBlendShapeLocationValues == null)
        {
            return;
        }
        int value = arKitBlendShapeLocationValues[locationDropDown.value];
        location = (UnityEngine.XR.ARKit.ARKitBlendShapeLocation)value;
        gameObject.name = location.ToString();
    }

    public void OnClickDelete()
    {
        onDelete?.Invoke();
    }
}
