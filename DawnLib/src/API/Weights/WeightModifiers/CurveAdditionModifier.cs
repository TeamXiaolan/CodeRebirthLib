using System;
using UnityEngine;

namespace Dawn;

public sealed class CurveAdditionModifier : IWeightModifier<AnimationCurve?>
{
    private readonly Func<float> _getAddition;
    private readonly Func<WeightContext, bool>? _canApply;

    public CurveAdditionModifier(Func<float> getAddition, Func<WeightContext, bool>? canApply = null)
    {
        _getAddition = getAddition;
        _canApply = canApply;
    }

    public NamespacedKey Key => DawnKeys.CurveAddition;

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

        float addition = _getAddition();
        value = value.Multiplied(addition);
    }
}