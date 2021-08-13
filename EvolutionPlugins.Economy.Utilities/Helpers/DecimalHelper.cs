using System;

namespace EvolutionPlugins.Economy.Utilities.Helpers
{
    public static class DecimalHelper
    {
        public static bool IsNearlyZero(this decimal value, decimal tolerance = 0.01m)
        {
            return Math.Abs(value) < tolerance;
        }
    }
}
