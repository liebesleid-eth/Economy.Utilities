using HarmonyLib;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Patches
{
    [HarmonyPatch]
    internal static class Patch_DamageTool
    {
        internal static DamageZombieParameters s_CurrentDamageZombieParameters;
        internal static DamageAnimalParameters s_CurrentDamageAnimalParameters;
        internal static DamagePlayerParameters s_CurrentDamagePlayerParameters;

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageZombie))]
        [HarmonyPrefix]
        private static void PreDamageZombie(DamageZombieParameters parameters)
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
        private static void PreDamageAnimal(DamageAnimalParameters parameters)
        {
            s_CurrentDamageAnimalParameters = parameters;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damageAnimal))]
        [HarmonyPostfix]
        private static void PostDamageAnimal()
        {
            s_CurrentDamageAnimalParameters = default;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damagePlayer))]
        [HarmonyPrefix]
        private static void PreDamagePlayer(DamagePlayerParameters parameters)
        {
            s_CurrentDamagePlayerParameters = parameters;
        }

        [HarmonyPatch(typeof(DamageTool), nameof(DamageTool.damagePlayer))]
        [HarmonyPostfix]
        private static void PostDamagePlayer()
        {
            s_CurrentDamagePlayerParameters = default;
        }
    }
}
