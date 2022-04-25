using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARKit;

[Serializable]
public class Mapping
{
    public ARKitBlendShapeLocation location;
    public string name;
}

[CreateAssetMenu(fileName = "BlendShapeMapping", menuName = "BlendShapeMappings/Mappings", order = 1)]
public class BlendShapeMappings : ScriptableObject
{
    public const string BLENDSHAPE_EXPRESSION = "";//"blendShape2.";

    public float coefficientScale = 100.0f;

    [SerializeField]
    public List<Mapping> mappings = new List<Mapping>
    {
        new Mapping{ location = ARKitBlendShapeLocation.BrowDownLeft, name = $"{BLENDSHAPE_EXPRESSION}browDownLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.BrowDownRight, name = $"{BLENDSHAPE_EXPRESSION}browDownRight" },
        new Mapping{ location = ARKitBlendShapeLocation.BrowInnerUp, name = $"{BLENDSHAPE_EXPRESSION}browInnerUp" },
        new Mapping{ location = ARKitBlendShapeLocation.BrowOuterUpLeft, name = $"{BLENDSHAPE_EXPRESSION}browOuterUpLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.BrowOuterUpRight, name = $"{BLENDSHAPE_EXPRESSION}browOuterUpRight" },
        new Mapping{ location = ARKitBlendShapeLocation.CheekPuff, name = $"{BLENDSHAPE_EXPRESSION}cheekPuff" },
        new Mapping{ location = ARKitBlendShapeLocation.CheekSquintLeft, name = $"{BLENDSHAPE_EXPRESSION}cheekSquintLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.CheekSquintRight, name = $"{BLENDSHAPE_EXPRESSION}cheekSquintRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeBlinkLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeBlinkLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeBlinkRight, name = $"{BLENDSHAPE_EXPRESSION}eyeBlinkRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookDownLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeLookDownLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookDownRight, name = $"{BLENDSHAPE_EXPRESSION}eyeLookDownRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookInLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeLookInLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookInRight, name = $"{BLENDSHAPE_EXPRESSION}eyeLookInRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookOutLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeLookOutLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookOutRight, name = $"{BLENDSHAPE_EXPRESSION}eyeLookOutRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookUpLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeLookUpLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeLookUpRight, name = $"{BLENDSHAPE_EXPRESSION}eyeLookUpRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeSquintLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeSquintLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeSquintRight, name = $"{BLENDSHAPE_EXPRESSION}eyeSquintRight" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeWideLeft, name = $"{BLENDSHAPE_EXPRESSION}eyeWideLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.EyeWideRight, name = $"{BLENDSHAPE_EXPRESSION}eyeWideRight" },
        new Mapping{ location = ARKitBlendShapeLocation.JawForward, name = $"{BLENDSHAPE_EXPRESSION}jawForward" },
        new Mapping{ location = ARKitBlendShapeLocation.JawLeft, name = $"{BLENDSHAPE_EXPRESSION}jawLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.JawOpen, name = $"{BLENDSHAPE_EXPRESSION}jawOpen" },
        new Mapping{ location = ARKitBlendShapeLocation.JawRight, name = $"{BLENDSHAPE_EXPRESSION}jawRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthClose, name = $"{BLENDSHAPE_EXPRESSION}mouthClose" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthDimpleLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthDimpleLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthDimpleRight, name = $"{BLENDSHAPE_EXPRESSION}mouthDimpleRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthFrownLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthFrownLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthFrownRight, name = $"{BLENDSHAPE_EXPRESSION}mouthFrownRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthFunnel, name = $"{BLENDSHAPE_EXPRESSION}mouthFunnel" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthLowerDownLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthLowerDownLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthLowerDownRight, name = $"{BLENDSHAPE_EXPRESSION}mouthLowerDownRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthPressLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthPressLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthPressRight, name = $"{BLENDSHAPE_EXPRESSION}mouthPressRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthPucker, name = $"{BLENDSHAPE_EXPRESSION}mouthPucker" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthRight, name = $"{BLENDSHAPE_EXPRESSION}mouthRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthRollLower, name = $"{BLENDSHAPE_EXPRESSION}mouthRollLower" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthRollUpper, name = $"{BLENDSHAPE_EXPRESSION}mouthRollUpper" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthShrugLower, name = $"{BLENDSHAPE_EXPRESSION}mouthShrugLower" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthShrugUpper, name = $"{BLENDSHAPE_EXPRESSION}mouthShrugUpper" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthSmileLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthSmileLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthSmileRight, name = $"{BLENDSHAPE_EXPRESSION}mouthSmileRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthStretchLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthStretchLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthStretchRight, name = $"{BLENDSHAPE_EXPRESSION}mouthStretchRight" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthUpperUpLeft, name = $"{BLENDSHAPE_EXPRESSION}mouthUpperUpLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.MouthUpperUpRight, name = $"{BLENDSHAPE_EXPRESSION}mouthUpperUpRight" },
        new Mapping{ location = ARKitBlendShapeLocation.NoseSneerLeft, name = $"{BLENDSHAPE_EXPRESSION}noseSneerLeft" },
        new Mapping{ location = ARKitBlendShapeLocation.NoseSneerRight, name = $"{BLENDSHAPE_EXPRESSION}noseSneerRight" },
        new Mapping{ location = ARKitBlendShapeLocation.TongueOut, name = $"{BLENDSHAPE_EXPRESSION}noseSneerLeft" },
    };
}