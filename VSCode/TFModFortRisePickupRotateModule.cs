using FortRise;
using System.Diagnostics;
using Monocle;
using TowerFall;
using Microsoft.Xna.Framework;
using MonoMod.ModInterop;
using System;

namespace TFModFortRisePickupRotate
{
  [Fort("com.ebe1.kenobi.TFModFortRisePickupRotate", "TFModFortRisePickupRotate")]
  public class TFModFortRisePickupRotateModule : FortModule
  {
    public static TFModFortRisePickupRotateModule Instance;
    public static Atlas RotateAtlas;
    public override Type SettingsType => typeof(TFModFortRisePickupRotateSettings);
    public static TFModFortRisePickupRotateSettings Settings => (TFModFortRisePickupRotateSettings)Instance.InternalSettings;

    public TFModFortRisePickupRotateModule()
    {
      if (!Debugger.IsAttached)
      {
        //Debugger.Launch(); // Proposera d’attacher Visual Studio
      }
      Instance = this;
      Logger.Init("PickupRotate");
    }

    public override void LoadContent()
    {
      RotateAtlas = Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png");
    }

    public override void Load()
    {
      MyTreasureSpawner.Load();
      MySession.Load();
      MyLevel.Load();
      MyPickup.Load();
      //MyRoundLogic.Load();
      
    }

    public override void Unload()
    {
      MyTreasureSpawner.Unload();
      MySession.Unload();
      MyLevel.Unload();
      MyPickup.Unload();
      //MyRoundLogic.Unload();
    }

    public static bool activated()
    {
      return VariantManager.GetCustomVariant("Rotate") || Settings.activated;
    }

    public override void OnVariantsRegister(VariantManager manager, bool noPerPlayer = false)
    {
      var rotate = new CustomVariantInfo(
          "Rotate", VariantManager.GetVariantIconFromName("Rotate", RotateAtlas),
          CustomVariantFlags.None
          );
      manager.AddVariant(rotate);
    }
  }
}
