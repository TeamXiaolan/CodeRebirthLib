using System;
using UnityEngine;

namespace Dawn;

public sealed class CurveMultiplierModifier : IWeightModifier<AnimationCurve?>
{
    private readonly Func<float> _getMultiplier;
    private readonly Func<WeightContext, bool>? _canApply;

    public CurveMultiplierModifier(Func<float> getMultiplier, Func<WeightContext, bool>? canApply = null)
    {
        _getMultiplier = getMultiplier;
        _canApply = canApply;
    }

    public NamespacedKey Key => DawnKeys.CurveMultiplier;

    public WeightModifierPhase Phase => WeightModifierPhase.Final;

    public int Priority => 0;

    public bool CanApply(WeightContext context)
    {
        return _canApply?.Invoke(context) ?? true;
    }

    public void Apply(ref AnimationCurve? value, WeightContext context)
    {
        if (value == null)
            return;

        float multiplier = _getMultiplier();
        value.Multiply(multiplier);
    }
}