using FortRise;
using Microsoft.Xna.Framework;
using System;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  public class MyPickup
  {
    internal static void Load()
    {
      On.TowerFall.Pickup.CreatePickup += CreatePickup_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Pickup.CreatePickup -= CreatePickup_patch;
    }

    public static Pickup CreatePickup_patch(
          On.TowerFall.Pickup.orig_CreatePickup orig, Vector2 position, Vector2 targetPosition, Pickups type, int playerIndex)
    {
      Pickup pickup;
      try
      {
        pickup = orig(position, targetPosition, type, playerIndex);
      }
      catch (Exception e)
      {
        try
        {
          if (type == ModRegisters.PickupType<RotatePickup>())
          {
            pickup = new RotatePickup(position, targetPosition, type);
          }
          else
          {
            throw new Exception();
          }
        }
        catch (Exception ExceptionRotate)
        {
          throw ExceptionRotate;
        }
      }

      return pickup;

    }
  }
}