using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dawn;

public sealed class CurveMultiplierSource : WeightModifierSource<AnimationCurve?>
{
    private readonly Func<float> _getMultiplier;
    private readonly Func<WeightContext, bool>? _canApply;

    public CurveMultiplierSource(Func<float> getMultiplier, Func<WeightContext, bool>? canApply = null)
    {
        _getMultiplier = getMultiplier;
        _canApply = canApply;
    }

    public override void Build(WeightBuildContext context, List<IWeightModifier<AnimationCurve?>> modifiers)
    {
        modifiers.Add(new CurveMultiplierModifier(_getMultiplier, _canApply));
    }
}