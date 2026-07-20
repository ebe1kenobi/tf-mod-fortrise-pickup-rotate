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
    ];

    internal Type[] Hookables = [
        typeof(MyTreasureSpawner),
        typeof(MySession),
        typeof(MyLevel),
    ];

    public static TFModFortRisePickupRotateSettings Settings => Instance.GetSettings<TFModFortRisePickupRotateSettings>()!;

    public TFModFortRisePickupRotateModule(IModContent content, IModuleContext context, ILogger logger) : base(content, context, logger)
    {
      if (!Debugger.IsAttached)
      {
        //Debugger.Launch(); // Proposera d’attacher Visual Studio
      }
      Instance = this;

      foreach (var registerable in Registerables)
      {
        registerable.GetMethod(nameof(IRegisterable.Register))!.Invoke(null, [content, context.Registry]);
      }

      foreach (var hookable in Hookables)
      {
        hookable.GetMethod(nameof(IHookable.Load))!.Invoke(null, [context.Harmony]);
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
