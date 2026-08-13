using FortRise;

namespace TFModFortRisePickupRotate
{
  public class TFModFortRisePickupRotateSettings : ModuleSettings
  {
    public override void Create(ISettingsCreate settings)
    {
      settings.CreateOnOff("Pickup activated even \n\nwhen variant is not selected", activated, (x) => activated = x);
      settings.CreateOptions("Periodicity", periodicity, ["Normal", "Test"], (x) => periodicity = x.Item1);
      settings.CreateOptions("Treasure rate", Rarity.LabelOf(treasureRarity), Rarity.Labels,
          (x) => treasureRarity = x.Item2);
      settings.CreateOnOff("RotateRight90", RotateRight90, (x) => RotateRight90 = x);
      settings.CreateOnOff("RotateLeft90", RotateLeft90, (x) => RotateLeft90 = x);
      settings.CreateOnOff("Rotate180", Rotate180, (x) => Rotate180 = x);
      settings.CreateOnOff("Rotate360", Rotate360, (x) => Rotate360 = x);
      settings.CreateOnOff("FlipY", FlipY, (x) => FlipY = x);
      settings.CreateOnOff("FlipX", FlipX, (x) => FlipX = x);
      settings.CreateNumber("Effect Time", EffectTime, (x) => EffectTime = x, 5, 100);
      settings.CreateNumber("Effect 360 Time", Effect360Time, (x) => Effect360Time = x, 5, 100);
    }

    // Eteint par defaut : c'est la VARIANTE qui doit decider. Allume, l'objet
    // apparaissait dans des manches ou personne ne l'avait demande, simplement parce
    // que le mod etait installe - installer un mod ne doit pas changer le jeu tant
    // qu'on ne l'a pas choisi.
    //[SettingsName("Pickup activated even \n\nwhen variant is not selected")]
    public bool activated { get; set; } = false;

    /// <summary>
    /// Le cran d'apparition, index dans Rarity.Steps.
    ///
    /// Un index et non le taux lui-meme : c'est ce que rend la liste de l'ecran des
    /// options, et cela evite d'avoir a relire une valeur qui ne serait plus dans
    /// l'echelle. Le defaut est celui de la bombe, l'objet rare du jeu - l'ancien
    /// reglage partait de l'equivalent d'une fleche, ce qui etait beaucoup.
    ///
    /// Sans effet en mode TEST, qui impose son propre taux.
    /// </summary>
    public int treasureRarity { get; set; } = Rarity.Default;

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

    /// <summary>
    /// "Normal" ou "Test".
    ///
    /// Les anciens crans OncePerMatch et OncePerRound ont disparu avec la methode qui
    /// les portait : ils comptaient les apparitions pour en imposer une par match ou
    /// par manche, ce qu'un tirage pondere ne sait pas exprimer. Normal laisse le jeu
    /// tirer - donc obeir aux variantes et au jeu d'objets de la tour - et Test force
    /// l'apparition pour essayer.
    ///
    /// Le convertisseur est obligatoire : ce reglage etait un ENTIER, et un fichier de
    /// sauvegarde deja ecrit empeche le jeu de demarrer sans lui.
    /// </summary>
    [System.Text.Json.Serialization.JsonConverter(typeof(PeriodicityJsonConverter))]
    public string periodicity { get; set; } = "Normal";
  }
}
