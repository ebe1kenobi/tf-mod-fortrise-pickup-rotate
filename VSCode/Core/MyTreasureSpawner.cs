using FortRise;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRisePickupRotate
{
  public class MyTreasureSpawner : IHookable
  {
    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(TreasureSpawner), nameof(TreasureSpawner.GetChestSpawnsForLevel)),
          postfix: new HarmonyMethod(GetChestSpawnsForLevel_patch)
      );
    }

    public static void GetChestSpawnsForLevel_patch(
       TreasureSpawner __instance,
       List<Vector2> chestPositions,
       List<Vector2> bigChestPositions,
       List<TreasureChest> __result)
    {
      List<TreasureChest> chestSpawnsForLevel = __result;
      if (chestSpawnsForLevel == null || chestSpawnsForLevel.Count == 0)
      {
        return;
      }

      if (!TFModFortRisePickupRotateModule.activated()) return;

      if (MySession.NbRotatePickupActivated == 0)
      {
        Random rnd = new Random();
        int draw;
        if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.Test)
        {
          draw = 1;
        }
        else
        {
          draw = rnd.Next(0, TFModFortRisePickupRotateModule.Settings.treasureRate);
        }
        //Logger.Info("GetChestSpawnsForLevel_patch Rotate draw=" + draw);
        if (draw == 1)
        {
          var dynData = DynamicData.For(chestSpawnsForLevel[0]);
          List<Pickups> pickups = (List<Pickups>)dynData.Get("pickups");
          // FortRise 4 : ModRegisters.PickupType<RotatePickup>()
          // FortRise 5 : la valeur Pickups est portee par l'entree du registre.
          pickups[0] = RotatePickup.RotateMeta.Pickups;
          MySession.NbRotatePickupActivated++;
          dynData.Dispose();
        }
      }
    }
  }
}
