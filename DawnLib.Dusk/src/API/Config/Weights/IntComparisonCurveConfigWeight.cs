using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Dawn;
using Dawn.Internal;
using UnityEngine;

namespace Dusk.Weights;

[Serializable]
public class IntComparisonCurveConfigWeight
{
    public IntComparison IntComparison;
    public AnimationCurve Curve = AnimationCurve.Constant(0, 1, 0);

    public IntComparisonCurveConfigWeight()
    {
    }

    public IntComparisonCurveConfigWeight(IntComparison intComparison, AnimationCurve curve)
    {
        IntComparison = intComparison;
        Curve = curve;
    }

    public bool Matches(int value)
    {
        return IntComparison.ComparisonOperation switch
        {
            ComparisonOperation.Equal => value == IntComparison.Value,
            ComparisonOperation.NotEqual => value != IntComparison.Value,
            ComparisonOperation.Greater => value > IntComparison.Value,
            ComparisonOperation.Less => value < IntComparison.Value,
            ComparisonOperation.GreaterOrEqual => value >= IntComparison.Value,
            ComparisonOperation.LessOrEqual => value <= IntComparison.Value,
            _ => false
        };
    }

    public static List<IntComparisonCurveConfigWeight> ConvertManyFromString(string input)
    {
        List<IntComparisonCurveConfigWeight> results = new();
        if (string.IsNullOrWhiteSpace(input))
            return results;

        string[] entries = input.Split('|');
        foreach (string rawEntry in entries)
        {
            string entry = rawEntry.Trim();
            if (string.IsNullOrWhiteSpace(entry))
                continue;

            int separatorIndex = entry.IndexOf(" - ", StringComparison.Ordinal);
            if (separatorIndex < 0)
            {
                DawnPlugin.Logger.LogWarning($"Invalid route price curve override entry '{entry}'. Expected format: '<Comparison><Value> - <Curve>'.");
                continue;
            }

            string comparisonInput = entry[..separatorIndex].Trim();
            string curveInput = entry[(separatorIndex + 3)..].Trim();
            if (!TryParseComparison(comparisonInput, out IntComparison? comparison))
            {
                DawnPlugin.Logger.LogWarning($"Invalid route price comparison '{comparisonInput}'. Expected examples: '>=100', '<300', '==0'.");
                continue;
            }

            AnimationCurve curve = ConfigManager.ParseCurve(curveInput);
            results.Add(new IntComparisonCurveConfigWeight(comparison, curve));
        }

        return results;
    }

    public static string ConvertManyToString(IEnumerable<IntComparisonCurveConfigWeight> configs)
    {
        List<string> parts = new();
        foreach (IntComparisonCurveConfigWeight config in configs)
        {
            string comparison = config.IntComparison.ComparisonOperation switch
            {
                ComparisonOperation.Equal => "==",
                ComparisonOperation.NotEqual => "!=",
                ComparisonOperation.Greater => ">",
                ComparisonOperation.Less => "<",
                ComparisonOperation.GreaterOrEqual => ">=",
                ComparisonOperation.LessOrEqual => "<=",
                _ => "==",
            };

            parts.Add($"{comparison}{config.IntComparison.Value} - {ConfigManager.ParseString(config.Curve)}");
        }

        return string.Join(" | ", parts);
    }

    private static bool TryParseComparison(string input, [NotNullWhen(true)] out IntComparison? comparison)
    {
        comparison = null;
        Debuggers.Weights?.Log($"Converting IntComparisonCurveConfigWeight from string: {input}");
        if (string.IsNullOrWhiteSpace(input))
        {
            DuskPlugin.Logger.LogWarning("Input string was null or empty.");
            return false;
        }

        Match match = Regex.Match(
            input.Trim(),
            @"^(==|!=|<=|>=|<|>)(-?\d+)$"
        );

        if (!match.Success)
        {
            DuskPlugin.Logger.LogWarning($"Invalid IntComparisonCurveConfigWeight format: {input}");
            return false;
        }

        string comparisonToken = match.Groups[1].Value;
        string valueToken = match.Groups[2].Value;

        ComparisonOperation comparisonOperation = comparisonToken switch
        {
            "==" => ComparisonOperation.Equal,
            "!=" => ComparisonOperation.NotEqual,
            "<" => ComparisonOperation.Less,
            ">" => ComparisonOperation.Greater,
            "<=" => ComparisonOperation.Less | ComparisonOperation.Equal,
            ">=" => ComparisonOperation.Greater | ComparisonOperation.Equal,
            _ => ComparisonOperation.Equal
        };

        if (!int.TryParse(valueToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            DuskPlugin.Logger.LogWarning($"Invalid comparison value in input: {input}");
            return false;
        }

        comparison = new IntComparison
        {
            Value = value,
            ComparisonOperation = comparisonOperation
        };

        return true;
    }
}