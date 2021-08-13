using System;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Helpers
{
    public static class LimbHelper
    {
        public static string Parse(this ELimb limb) => limb switch
        {
            ELimb.LEFT_FOOT or ELimb.LEFT_LEG or ELimb.RIGHT_FOOT or ELimb.RIGHT_LEG => "leg",
            ELimb.LEFT_HAND or ELimb.LEFT_ARM or ELimb.RIGHT_HAND or ELimb.RIGHT_ARM => "arm",
            ELimb.LEFT_BACK or ELimb.RIGHT_BACK or ELimb.LEFT_FRONT or ELimb.RIGHT_FRONT or ELimb.SPINE => "torso",
            ELimb.SKULL => "head",
            _ => throw new ArgumentOutOfRangeException(nameof(limb), "Limb is out of range")
        };
    }
}
