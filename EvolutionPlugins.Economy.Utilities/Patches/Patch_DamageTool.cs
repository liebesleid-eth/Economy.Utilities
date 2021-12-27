using HarmonyLib;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Patches
{
    [HarmonyPatch]
    internal static class Patch_DamageTool
    {
        internal static DamageZombieParameters s_CurrentDamageZombieParameters;
        internal static DamageAnimalParameters s_CurrentDamageAnimalParameters;

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageZombie))]
        [HarmonyPrefix]
        private static void PreDamageZombie(ref DamageZombieParameters parameters)
        {
            s_CurrentDamageZombieParameters = parameters;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageZombie))]
        [HarmonyPostfix]
        private static void PostDamageZombie()
        {
            s_CurrentDamageZombieParameters = default;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageAnimal))]
        [HarmonyPrefix]
        private static void PreDamageAnimal(ref DamageAnimalParameters parameters)
        {
            s_CurrentDamageAnimalParameters = parameters;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageAnimal))]
        [HarmonyPostfix]
        private static void PostDamageAnimal()
        {
            s_CurrentDamageAnimalParameters = default;
        }
    }
}
