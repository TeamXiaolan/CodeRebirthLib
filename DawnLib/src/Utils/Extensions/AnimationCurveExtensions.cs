using UnityEngine;

namespace Dawn;

public static class AnimationCurveExtensions
{
    public static void Multiply(this AnimationCurve curve, float multiplier)
    {
        Keyframe[] keys = curve.keys;
        for (int i = 0; i < keys.Length; i++)
        {
            Keyframe key = keys[i];
            key.value *= multiplier;
            key.inTangent *= multiplier;
            key.outTangent *= multiplier;
            keys[i] = key;
        }
    }
}