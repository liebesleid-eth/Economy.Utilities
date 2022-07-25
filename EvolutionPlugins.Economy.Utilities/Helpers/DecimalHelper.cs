using System;

namespace EvolutionPlugins.Economy.Utilities.Helpers;

internal static class DecimalHelper
{
    public static bool IsNearlyZero(this decimal value, decimal tolerance = 0.01m) => Math.Abs(value) < tolerance;
}
