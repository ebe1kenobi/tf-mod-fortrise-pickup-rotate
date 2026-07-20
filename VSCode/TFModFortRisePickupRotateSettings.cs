using FortRise;

namespace TFModFortRisePickupRotate
{
  public class TFModFortRisePickupRotateSettings : ModuleSettings
  {
    // CreateOptions travaille sur des libelles et renvoie (libelle, index) :
    // l'index correspond aux constantes ci-dessous, donc periodicity reste un int.
    private static readonly string[] PeriodicityNames = ["OncePerMatch", "OncePerRound", "Test"];

    private static string PeriodicityName(int value)
    {
      if (value < 0 || value >= PeriodicityNames.Length)
        return PeriodicityNames[0];
      return PeriodicityNames[value];
    }

    public override void Create(ISettingsCreate settings)
    {
      settings.CreateOnOff("Pickup activated even \n\nwhen variant is not selected", activated, (x) => activated = x);
      settings.CreateNumber("Treasure Rate 1 chance on N, choose N", treasureRate, (x) => treasureRate = x, 10, 100);
      settings.CreateOnOff("RotateRight90", RotateRight90, (x) => RotateRight90 = x);
      settings.CreateOnOff("RotateLeft90", RotateLeft90, (x) => RotateLeft90 = x);
      settings.CreateOnOff("Rotate180", Rotate180, (x) => Rotate180 = x);
      settings.CreateOnOff("Rotate360", Rotate360, (x) => Rotate360 = x);
      settings.CreateOnOff("FlipY", FlipY, (x) => FlipY = x);
      settings.CreateOnOff("FlipX", FlipX, (x) => FlipX = x);
      settings.CreateNumber("Effect Time", EffectTime, (x) => EffectTime = x, 5, 100);
      settings.CreateNumber("Effect 360 Time", Effect360Time, (x) => Effect360Time = x, 5, 100);
      settings.CreateOptions("Periodicity", PeriodicityName(periodicity), PeriodicityNames, (x) => periodicity = x.Item2);
    }

    //[SettingsName("Pickup activated even \n\nwhen variant is not selected")]
    public bool activated { get; set; } = true;

    //[SettingsName("Treasure Rate 1 chance on N, choose N")]
    //[SettingsNumber(10, 100)]
    public int treasureRate { get; set; } = 100;

    public bool RotateRight90 { get; set; } = true;
    public bool RotateLeft90 { get; set; } = true;
    public bool Rotate180 { get; set; } = true;
    public bool Rotate360 { get; set; } = true;
    public bool FlipY { get; set; } = true;
    public bool FlipX { get; set; } = true;

    //[SettingsNumber(5, 100)]
    public int EffectTime { get; set; } = 20;

    //[SettingsNumber(5, 100)]
    public int Effect360Time { get; set; } = 20;

    public const int OncePerMatch = 0;
    public const int OncePerRound = 1;
    public const int Test = 2;

    //[SettingsOptions("OncePerMatch", "OncePerRound", "Test")]
    public int periodicity { get; set; } = 2;
  }
}
