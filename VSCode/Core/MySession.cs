using FortRise;
using HarmonyLib;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  public class MySession : IHookable
  {
    public static int NbRotatePickupActivated { get; set; }

    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(Session), nameof(Session.EndRound)),
          prefix: new HarmonyMethod(EndRound_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(Session), nameof(Session.StartRound)),
          prefix: new HarmonyMethod(StartRound_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(Session), nameof(Session.StartGame)),
          prefix: new HarmonyMethod(StartGame_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(Session), nameof(Session.GotoNextRound)),
          prefix: new HarmonyMethod(GotoNextRound_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredConstructor(typeof(Session), [typeof(MatchSettings)]),
          prefix: new HarmonyMethod(ctor_patch)
      );
    }

    public static void StartRound_patch(Session __instance)
    {
      MyLevel.effect = TypeEffect.None;
    }

    public static void EndRound_patch(Session __instance)
    {
      if (MyLevel.effect != TypeEffect.None)
      {
        MyLevel.stopRotateEffect();
      }
    }

    public static void StartGame_patch(Session __instance)
    {
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.OncePerMatch)
      {
        NbRotatePickupActivated = 0;
      }
    }

    public static void GotoNextRound_patch(Session __instance)
    {
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.OncePerRound)
      {
        NbRotatePickupActivated = 0;
      }
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.Test)
      {
        NbRotatePickupActivated = 0;
      }
    }

    public static void ctor_patch(Session __instance, MatchSettings settings)
    {
      NbRotatePickupActivated = 0;
    }
  }
}
