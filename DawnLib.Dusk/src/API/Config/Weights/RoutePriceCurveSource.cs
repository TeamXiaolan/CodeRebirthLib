using System;
using System.Collections.Generic;
using Dawn;
using UnityEngine;

namespace Dusk.Weights;

public sealed class RoutePriceCurveWeightSource : WeightModifierSource<AnimationCurve?>
{
    private readonly Func<IEnumerable<IntComparisonCurveConfigWeight>> _getConfigs;

    public RoutePriceCurveWeightSource(Func<IEnumerable<IntComparisonCurveConfigWeight>> getConfigs)
    {
        _getConfigs = getConfigs;
    }

    public override void Build(WeightBuildContext context, List<IWeightModifier<AnimationCurve?>> modifiers)
    {
        foreach (IntComparisonCurveConfigWeight config in _getConfigs())
        {
            modifiers.Add(new RoutePriceCurveWeightModifier(config));
        }
    }
}