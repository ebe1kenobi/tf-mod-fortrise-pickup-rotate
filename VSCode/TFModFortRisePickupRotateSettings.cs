using FortRise;

namespace TFModFortRisePickupRotate
{
  public class TFModFortRisePickupRotateSettings : ModuleSettings
  {
    [SettingsName("Pickup activated even \n\nwhen variant is not selected")]
    public bool activated = true;

    [SettingsName("Treasure Rate 1 chance on N, choose N")]
    [SettingsNumber(10, 100)]
    public int treasureRate = 100;

    [SettingsName("RotateRight90")]
    public bool RotateRight90 = true;

    [SettingsName("RotateLeft90")]
    public bool RotateLeft90 = true;

    [SettingsName("Rotate180")]
    public bool Rotate180 = true;

    [SettingsName("Rotate360")]
    public bool Rotate360 = true;

    [SettingsName("FlipY")]
    public bool FlipY = true;

    [SettingsName("FlipX")]
    public bool FlipX = true;

    [SettingsName("Effect Time")]
    [SettingsNumber(5, 100)]
    public int EffectTime = 20;

    [SettingsName("Effect 360 Time")]
    [SettingsNumber(5, 100)]
    public int Effect360Time = 20;


    public const int OncePerMatch = 0;
    public const int OncePerRound = 1;
    public const int Test = 2;
    [SettingsOptions("OncePerMatch", "OncePerRound", "Test")]
    public int periodicity = 2;
  }
}
