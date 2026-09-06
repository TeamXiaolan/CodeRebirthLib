using Dawn;
using UnityEngine;

namespace Dusk.Weights;

public sealed class RoutePriceCurveWeightModifier : IWeightModifier<AnimationCurve?>
{
    private readonly IntComparisonCurveConfigWeight _getConfig;

    public RoutePriceCurveWeightModifier(IntComparisonCurveConfigWeight getConfig)
    {
        _getConfig = getConfig;
    }

    public NamespacedKey Key => DuskKeys.RoutePriceCurveWeight;

    public WeightModifierPhase Phase => WeightModifierPhase.Override;

    public int Priority => 1000;

    public bool CanApply(WeightContext context)
    {
        if (!context.TryGet(DuskWeightContextKeys.RoutePrice, out int routePrice))
            return false;

        return _getConfig.Matches(routePrice);
    }

    public void Apply(ref AnimationCurve? value, WeightContext context)
    {
        if (!context.TryGet(DuskWeightContextKeys.RoutePrice, out int _))
            return;

        AnimationCurve overrideCurve = _getConfig.Curve;
        value = overrideCurve;
    }
}