namespace TFModFortRisePickupRotate
{
  public class MySession
  {
    public static int NbRotatePickupActivated { get; set; }

    internal static void Load()
    {
      On.TowerFall.Session.EndRound += EndRound_patch;
      On.TowerFall.Session.StartRound += StartRound_patch;
      On.TowerFall.Session.StartGame += StartGame_patch;
      On.TowerFall.Session.GotoNextRound += GotoNextRound_patch;
      On.TowerFall.Session.ctor += ctor_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Session.EndRound -= EndRound_patch;
      On.TowerFall.Session.StartRound -= StartRound_patch;
      On.TowerFall.Session.StartGame -= StartGame_patch;
      On.TowerFall.Session.GotoNextRound -= GotoNextRound_patch;
      On.TowerFall.Session.ctor -= ctor_patch;
    }
    public MySession()
    {
    }

    public static void StartRound_patch(On.TowerFall.Session.orig_StartRound orig, global::TowerFall.Session self) {
      MyLevel.effect = TypeEffect.None;
      orig(self);
    }

    public static void EndRound_patch(On.TowerFall.Session.orig_EndRound orig, global::TowerFall.Session self)
    {
      //Logger.Info("EndRound_patch");
      if (MyLevel.effect != TypeEffect.None)
      {
      //Logger.Info("EndRound_patch2");
        MyLevel.stopRotateEffect();
      }
      orig(self);
    }

    public static void StartGame_patch(On.TowerFall.Session.orig_StartGame orig, global::TowerFall.Session self)
    {
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.OncePerMatch)
      {
        NbRotatePickupActivated = 0;
      }
      orig(self);
    }

    public static void GotoNextRound_patch(On.TowerFall.Session.orig_GotoNextRound orig, global::TowerFall.Session self)
    {
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.OncePerRound)
      {
        NbRotatePickupActivated = 0;
      }
      if (TFModFortRisePickupRotateModule.Settings.periodicity == TFModFortRisePickupRotateSettings.Test)
      {
        NbRotatePickupActivated = 0;
      }
      orig(self);
    }

    public static void ctor_patch(On.TowerFall.Session.orig_ctor orig, global::TowerFall.Session self, global::TowerFall.MatchSettings settings)
    {
      NbRotatePickupActivated = 0;
      orig(self, settings);
    }

  }
}
