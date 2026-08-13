using FortRise;
using HarmonyLib;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  /// <summary>
  /// Remet l'ecran droit entre les manches.
  ///
  /// Cette classe tenait aussi le compteur "combien de fois le pickup est deja
  /// apparu", remis a zero par match ou par manche selon la periodicite. Il a disparu
  /// avec elle : l'apparition passe maintenant par le tirage pondere du jeu (voir
  /// TreasureRates), qui n'a pas de plafond a compter.
  /// </summary>
  public class MySession : IHookable
  {
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
  }
}
