using System;
using System.Diagnostics;
using FortRise;
using Microsoft.Extensions.Logging;

namespace TFModFortRisePickupRotate
{
  public class TFModFortRisePickupRotateModule : Mod
  {
    public static TFModFortRisePickupRotateModule Instance;

    private static Type[] Registerables = [
        typeof(TextureRegistry),
        typeof(RotatePickup),
        typeof(Variants),
        // Apres le pickup : le hook de tour cite sa valeur Pickups, qui n'existe
        // qu'une fois l'enregistrement fait.
        typeof(TreasureRates),
    ];

    internal Type[] Hookables = [
        typeof(MyTreasureSpawner),
        typeof(MySession),
        typeof(MyLevel),
        typeof(MyVariantToggle),
    ];

    public static TFModFortRisePickupRotateSettings Settings => Instance.GetSettings<TFModFortRisePickupRotateSettings>()!;

    public TFModFortRisePickupRotateModule(IModContent content, IModuleContext context, ILogger logger) : base(content, context, logger)
    {
      if (!Debugger.IsAttached)
      {
        //Debugger.Launch(); // Proposera d’attacher Visual Studio
      }
      Instance = this;

      // Le journal du mod passe par celui de FortRise : rien a ouvrir, rien a
      // fermer, et les lignes se melent a celles des autres mods dans l'ordre reel.
      TFModFortRisePickupRotate.Logger.Init(logger);

      foreach (var registerable in Registerables)
      {
        registerable.GetMethod(nameof(IRegisterable.Register))!.Invoke(null, [content, context.Registry]);
      }

      foreach (var hookable in Hookables)
      {
        hookable.GetMethod(nameof(IHookable.Load))!.Invoke(null, [context.Harmony]);
      }
    }

    /// <summary>
    /// Ecrit les reglages sur le disque tout de suite.
    ///
    /// FortRise ne les enregistre qu'en sortant de SON ecran d'options : un reglage
    /// change depuis la fenetre de la variante ne vivrait qu'en memoire et serait
    /// perdu en quittant. SaveSettings est internal cote FortRise, d'ou la reflexion.
    /// </summary>
    public static void SaveSettingsNow()
    {
      if (Instance == null)
      {
        return;
      }

      try
      {
        var method = typeof(Mod).GetMethod("SaveSettings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(Instance, null);
      }
      catch (Exception e)
      {
        TFModFortRisePickupRotate.Logger.Info($"[Settings] sauvegarde immediate impossible : {e.Message}");
      }
    }

    public override ModuleSettings CreateSettings()
    {
      return new TFModFortRisePickupRotateSettings();
    }

    // Le pickup est actif si le variant "Rotate" est selectionne OU si le reglage
    // "activated" force son apparition. FortRise 4 utilisait
    // VariantManager.GetCustomVariant("Rotate") ; FortRise 5 : l'entree du registre.
    public static bool activated()
    {
      return Variants.Rotate.IsActive() || Settings.activated;
    }
  }
}
