using HarmonyLib;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Patches
{
    [HarmonyPatch]
    internal static class Patch_DamageAnimalParameters
    {
        internal static (Animal animal, ELimb limb)? s_CurrentDamageAnimalParameters;

        [HarmonyPatch(typeof(DamageAnimalParameters), nameof(DamageAnimalParameters.make))]
        [HarmonyPrefix]
        private static void PreMake(Animal animal, ELimb limb)
        {
            s_CurrentDamageAnimalParameters = (animal, limb);
        }

        [HarmonyPatch(typeof(DamageAnimalParameters), nameof(DamageAnimalParameters.make))]
        [HarmonyPostfix]
        private static void PostMake()
        {
            s_CurrentDamageAnimalParameters = null;
        }
    }
}
