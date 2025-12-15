using FortRise;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TowerFall;
using MonoMod.Utils;

namespace TFModFortRisePickupRotate
{
  public class MyTreasureSpawner
  {
    internal static void Load()
    {
      On.TowerFall.TreasureSpawner.GetChestSpawnsForLevel += GetChestSpawnsForLevel_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.TreasureSpawner.GetChestSpawnsForLevel -= GetChestSpawnsForLevel_patch;
    }

    public static List<TreasureChest> GetChestSpawnsForLevel_patch(
       On.TowerFall.TreasureSpawner.orig_GetChestSpawnsForLevel orig,
       TowerFall.TreasureSpawner self,
       List<Vector2> chestPositions,
       List<Vector2> bigChestPositions)
    { 
      List<TreasureChest> chestSpawnsForLevel = orig(self, chestPositions, bigChestPositions);
      if (chestSpawnsForLevel.Count == 0)
      {
        return chestSpawnsForLevel;
      }

      if (!TFModFortRisePickupRotateModule.activated()) return chestSpawnsForLevel;

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
          //Logger.Info("GetChestSpawnsForLevel_patch Rotate draw=" + draw);
          var dynData = DynamicData.For(chestSpawnsForLevel[0]);
          List<Pickups> pickups = (List<Pickups>)dynData.Get("pickups");
          pickups[0] = ModRegisters.PickupType<RotatePickup>();
          MySession.NbRotatePickupActivated++;
          dynData.Dispose();
        }
      }

      return chestSpawnsForLevel;
    }
  }
}