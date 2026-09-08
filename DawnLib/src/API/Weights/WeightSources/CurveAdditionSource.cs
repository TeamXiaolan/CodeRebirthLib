using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dawn;

public sealed class CurveAdditionSource : WeightModifierSource<AnimationCurve?>
{
    private readonly Func<float> _getAddition;
    private readonly Func<WeightContext, bool>? _canApply;

    public CurveAdditionSource(Func<float> getAddition, Func<WeightContext, bool>? canApply = null)
    {
        _getAddition = getAddition;
        _canApply = canApply;
    }

    public override void Build(WeightBuildContext context, List<IWeightModifier<AnimationCurve?>> modifiers)
    {
        modifiers.Add(new CurveAdditionModifier(_getAddition, _canApply));
    }
}