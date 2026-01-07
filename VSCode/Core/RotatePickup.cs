using System.Collections.Generic;
using FortRise;
using Microsoft.Xna.Framework;
using Monocle;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  public enum TypeEffect
  {
    None,
    RotateRight90,
    RotateLeft90,
    Rotate180,
    Rotate360,
    FlipY,
    FlipX,
  }

  [CustomPickup("RotatePickup", "0.0")]
  public class RotatePickup : Pickup
  {
    // Effet appliqué par ce pickup
    TypeEffect effect = TypeEffect.None;

    public GraphicsComponent graphic;

    public RotatePickup(Vector2 position, Vector2 targetPosition, Pickups pickupType)
        : base(position, targetPosition)
    {
      // Pour du random, réactive ceci :
      List<TypeEffect> listEffect = new List<TypeEffect>();
      if (TFModFortRisePickupRotateModule.Settings.RotateLeft90) {
        listEffect.Add(TypeEffect.RotateLeft90);
      }
      if (TFModFortRisePickupRotateModule.Settings.RotateRight90) {
        listEffect.Add(TypeEffect.RotateRight90);
      }
      if (TFModFortRisePickupRotateModule.Settings.Rotate180) {
        listEffect.Add(TypeEffect.Rotate180);
      } 
      if (TFModFortRisePickupRotateModule.Settings.Rotate360) {
        listEffect.Add(TypeEffect.Rotate360);
      }
      if (TFModFortRisePickupRotateModule.Settings.FlipX) {
        listEffect.Add(TypeEffect.FlipX);
      }
      if (TFModFortRisePickupRotateModule.Settings.FlipY) {
        listEffect.Add(TypeEffect.FlipY);
      }

      if (listEffect.Count > 0) {
        effect = listEffect[Calc.Random.Next(0, listEffect.Count)];
      } else {
        effect = TypeEffect.None;
      }

      this.Collider = new Hitbox(16f, 16f, -8f, -8f);
      this.Tag(GameTags.PlayerCollectible);

      //this.graphic = new Image(TFGame.Atlas["pickups/arrowPickup"]); //todo changer le sprite
      this.graphic = new Image(TFModFortRisePickupRotateModule.RotateAtlas["variants/rotate"]); //todo changer le sprite
      this.graphic.CenterOrigin();
      this.Add(this.graphic);
    }

    public override void OnPlayerCollide(Player player)
    {
      // Active l’effet dans MyLevel
      MyLevel.startRotateEffect(effect);

      this.RemoveSelf();
    }

    public override void DoPlayerCollect(Player player)
    {
      MyLevel.startRotateEffect(effect);
    }

    public override void Render()
    {
      this.DrawGlow();
      this.graphic.DrawOutline();
      base.Render();
    }

    public override void TweenUpdate(float t)
    {
      base.TweenUpdate(t);
      this.graphic.Scale = Vector2.One * t;
    }
  }
}
