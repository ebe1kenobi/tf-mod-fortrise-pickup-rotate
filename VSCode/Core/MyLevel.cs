using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;
using TowerFall;
using Color = Microsoft.Xna.Framework.Color;

namespace TFModFortRisePickupRotate
{
  public class MyLevel {
    static public TypeEffect effect = TypeEffect.None;
    public static bool sandboxEntityCreated = false;
    private static float flipTime = 0f; 
    private static RenderTarget2D finalTarget;
    private static float rotateTime = 0f;  
    private static float rotateTime360 = 0f;
    private static float targetAngle = 0f;
    private static float effectTimer = 0f;
    private static bool reverseEffect = false;

    internal static void Load()
    {
      On.TowerFall.Level.Update += Update_patch;
      On.TowerFall.Level.PostScreen += PostScreen_patch;

    }

    internal static void Unload()
    {
      On.TowerFall.Level.Update -= Update_patch;
      On.TowerFall.Level.PostScreen -= PostScreen_patch;

    }

    public static void startRotateEffect(TypeEffect effect)
    {
      //Logger.Info("startRotateEffect " + effect.ToString());
      // active l’effet global
      MyLevel.effect = effect;
      flipTime = 0;
      rotateTime= 0;
      rotateTime360 = 0f;
      reverseEffect = false;
      if (effect == TypeEffect.Rotate360)
        effectTimer = TFModFortRisePickupRotateModule.Settings.Effect360Time;
      else
        effectTimer = TFModFortRisePickupRotateModule.Settings.EffectTime;
    }

    public static void stopRotateEffect()
    {
      //Logger.Info("stopRotateEffect" + effect.ToString());
      if (effect == TypeEffect.Rotate360) {
        effect = TypeEffect.None;
      } else {
        reverseEffect = true;
      }
      rotateTime360 = 0;
    }

    public static void PostScreen_patch(On.TowerFall.Level.orig_PostScreen orig, global::TowerFall.Level self)
    {
      if (effect == TypeEffect.None)
      {
        orig(self);
        return;
      }

      var screen = Engine.Instance.Screen;
      Engine.Instance.GraphicsDevice.Clear(Color.Black);

      // Centre de l’écran
      var center = new Vector2(screen.Width * 0.5f, screen.Height * 0.5f);

      Matrix transform = Matrix.Identity;

      if (effect == TypeEffect.RotateRight90 || effect == TypeEffect.RotateLeft90) {
        // Animation aller-retour
        if (!reverseEffect)
          rotateTime = MathHelper.Min(1f, rotateTime + Engine.DeltaTime * (1f / 2f));
        else
          rotateTime = MathHelper.Max(0f, rotateTime - Engine.DeltaTime * (1f / 2f));

        // Définir l’angle cible
        if (effect == TypeEffect.RotateRight90)
          targetAngle = MathHelper.PiOver2;          // +90°
        else if (effect == TypeEffect.RotateLeft90)
          targetAngle = -MathHelper.PiOver2;         // -90°

        float angle = MathHelper.Lerp(0f, targetAngle, Ease.CubeInOut(rotateTime));

        // Dimensions source
        float w = screen.Width;
        float h = screen.Height;

        // Calcul taille projetée pendant la rotation
        float cosA = Math.Abs((float)Math.Cos(angle));
        float sinA = Math.Abs((float)Math.Sin(angle));

        // bounding box dynamique
        float rotatedW = w * cosA + h * sinA;
        float rotatedH = w * sinA + h * cosA;

        // scale dynamique (évolue selon l’angle)
        float scale = Math.Min(w / rotatedW, h / rotatedH);

        // Matrice avec scale + rotation
        transform =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateScale(scale) *
            Matrix.CreateRotationZ(angle) *
            Matrix.CreateTranslation(center.X, center.Y, 0f) *
            screen.Matrix;

        // Si retour terminé → effet OFF
        if (reverseEffect && rotateTime <= 0f)
        {
          reverseEffect = false;
          effect = TypeEffect.None;
        }
      } 
      // ---- ROTATIONS ----
      if (effect == TypeEffect.Rotate180)
      {
        // Avancement de l’animation
        if (!reverseEffect)
          rotateTime = MathHelper.Min(1f, rotateTime + Engine.DeltaTime * (1f / 2f));
        else
          rotateTime = MathHelper.Max(0f, rotateTime - Engine.DeltaTime * (1f / 2f));

        targetAngle = MathHelper.Pi;               // 180°

        float angle = MathHelper.Lerp(0f, targetAngle, Ease.CubeInOut(rotateTime));

        transform =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateRotationZ(angle) *
            Matrix.CreateTranslation(center.X, center.Y, 0f) *
            screen.Matrix;

        if (reverseEffect && rotateTime <= 0f)
        {
          reverseEffect = false;
          effect = TypeEffect.None;
        }
      }

      // ---- FlipX ----
      if (effect == TypeEffect.FlipX)
      {
        if (!reverseEffect)
          flipTime = MathHelper.Min(1f, flipTime + Engine.DeltaTime * (1f / 2f));
        else
          flipTime = MathHelper.Max(0f, flipTime - Engine.DeltaTime * (1f / 2f));

        float scaleX = MathHelper.Lerp(1f, -1f, Ease.CubeInOut(flipTime));

        transform =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateScale(scaleX, 1f, 1f) *
            Matrix.CreateTranslation(center.X, center.Y, 0f) *
            screen.Matrix;

        if (reverseEffect && flipTime <= 0f)
        {
          reverseEffect = false;
          effect = TypeEffect.None;
        }
      }

      // ---- FlipY (déjà animé) ----
      if (effect == TypeEffect.FlipY)
      {
        if (!reverseEffect)
          flipTime = MathHelper.Min(1f, flipTime + Engine.DeltaTime * (1f / 2f));
        else
          flipTime = MathHelper.Max(0f, flipTime - Engine.DeltaTime * (1f / 2f));

        float scaleY = MathHelper.Lerp(1f, -1f, Ease.CubeInOut(flipTime));

        transform =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateScale(1f, scaleY, 1f) *
            Matrix.CreateTranslation(center.X, center.Y, 0f) *
            screen.Matrix;

        if (reverseEffect && flipTime <= 0f)
        {
          reverseEffect = false;
          effect = TypeEffect.None;
        }
      }

      // ---- ROTATE 360 ----
      if (effect == TypeEffect.Rotate360)
      {
        // Avancement du temps
        rotateTime360 += Engine.DeltaTime;

        // Durée de 20 sec pour un tour complet
        float duration = TFModFortRisePickupRotateModule.Settings.Effect360Time;
        //float duration = 20f;

        // Angle actuel = interpolation linéaire de 0 → 2π
        float angle = MathHelper.Lerp(0f, MathHelper.TwoPi, rotateTime360 / duration);

        // Clamp pour éviter dépassements
        if (rotateTime360 >= duration)  
        {
          angle = MathHelper.TwoPi;
          effect = TypeEffect.None;   // stop direct
          rotateTime360 = 0f;            // reset propre
        }

        // Dimensions source
        float w = screen.Width;
        float h = screen.Height;

        // ---- scale dynamique selon l'angle ----
        float cosA = Math.Abs((float)Math.Cos(angle));
        float sinA = Math.Abs((float)Math.Sin(angle));

        float rotatedW = w * cosA + h * sinA;
        float rotatedH = w * sinA + h * cosA;

        float scale = Math.Min(w / rotatedW, h / rotatedH);

        // Matrice finale
        transform =
            Matrix.CreateTranslation(-center.X, -center.Y, 0f) *
            Matrix.CreateScale(scale) *
            Matrix.CreateRotationZ(angle) *
            Matrix.CreateTranslation(center.X, center.Y, 0f) *
            screen.Matrix;
      }

      // ---- Rendu final ----
      Draw.SpriteBatch.Begin(
          SpriteSortMode.Deferred,
          BlendState.AlphaBlend,
          SamplerState.PointClamp,
          DepthStencilState.None,
          RasterizerState.CullNone,
          null,
          transform
      );

      Draw.SpriteBatch.Draw(screen.RenderTarget, Vector2.Zero, Color.White);

      if (self.ReplayViewer.Visible)
      {
        self.ReplayViewer.PostScreenRender(); 
      }
      else
      {
        var dyn = DynamicData.For(self);
        var hudLayer = (Layer)dyn.Get("hudLayer");
        var pauseLayer = (Layer)dyn.Get("pauseLayer");
        hudLayer.EntityRenderWalk();
        pauseLayer.EntityRenderWalk();
        dyn.Dispose();
      }

      Draw.SpriteBatch.End();
      if (!self.Paused && !self.Frozen && self.ReplayRecorder != null && SaveData.Instance.Options.ReplayMode == Options.ReplayModes.UseCPU)
      {
        self.ReplayRecorder.RecordRender();
      }
    }

    public static void Update_patch(On.TowerFall.Level.orig_Update orig, global::TowerFall.Level self)
    {
      /// 🔥 COMPTEUR D’EFFET
      if (effectTimer > 0)
      {
        effectTimer -= Engine.DeltaTime;

        // fin du timer → arrêt effet
        if (effectTimer <= 0f && effect != TypeEffect.Rotate360)
        {
          stopRotateEffect();
        }
      }
      orig(self);
    }
  }
}
