using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CodeRebirthLib.Extensions;
using UnityEngine;

namespace CodeRebirthLib.ContentManagement.MapObjects;
public static class CRModMapObjectExtensions
{
    public static bool TryGetFromMapObjectName(this IEnumerable<CRMapObjectDefinition> registry, string mapObjectName, [NotNullWhen(true)] out CRMapObjectDefinition? value)
    {
        return registry.TryGetFirstBySomeName(it => it.MapObjectName,
            mapObjectName,
            out value,
            $"TryGetFromMapObjectName failed with mapObjectName: {mapObjectName}"
        );
    }

    public static bool TryGetDefinition(this GameObject type, [NotNullWhen(true)] out CRMapObjectDefinition? definition)
    {
        definition = LethalContent.MapObjects.CRLib.FirstOrDefault(it => it.GameObject == type);
        if (!definition) CodeRebirthLibPlugin.ExtendedLogging($"TryGetDefinition for MapObjectDefinition failed with {type.name}");
        return definition; // implict cast
    } // todo
}