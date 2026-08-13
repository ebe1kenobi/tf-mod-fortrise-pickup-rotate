using System;

namespace TFModFortRisePickupRotate
{
  /// <summary>
  /// Les taux d'apparition proposes, du plus rare au plus courant.
  ///
  /// Une echelle de valeurs choisies et non un curseur de 1 a 20000 : entre 0,001 et
  /// 1, les crans qui comptent sont ceux du jeu, et faire defiler vingt mille
  /// positions pour les trouver n'est pas un reglage.
  ///
  /// Les reperes entre parenthese sont les taux REELS des objets du jeu, tels qu'ils
  /// figurent dans TreasureSpawner.DefaultTreasureChances. Ils disent ce que le
  /// nombre veut dire mieux qu'un pourcentage, qui dependrait de la tour.
  ///
  /// Tout est compte en MILLIEMES : le poids que le jeu tire vaut le nombre d'unites
  /// (ce que l'API sait ajouter, un entier) multiplie par la valeur d'une unite (le
  /// Chance declare a l'enregistrement, un flottant). Une unite vaut un milliieme,
  /// soit exactement le taux de l'orbe du chaos - le plus faible du jeu.
  /// </summary>
  public static class Rarity
  {
    /// <summary>La valeur d'une unite de masque. C'est le taux le plus bas du jeu.</summary>
    public const float Unit = 0.001f;

    /// <summary>Les crans, en milliemes.</summary>
    public static readonly int[] Steps =
    {
      1, 2, 5, 10, 20, 50, 100, 150, 250, 500, 750, 1000, 1500, 2000, 3000, 5000, 10000, 20000
    };

    public static readonly string[] Labels =
    {
      "0.001 (CHAOS ORB)",
      "0.002",
      "0.005",
      "0.010",
      "0.020",
      "0.050",
      "0.100 (BOMB)",
      "0.150 (ORBS)",
      "0.250 (MIRROR)",
      "0.500 (SHIELD)",
      "0.750",
      "1.000 (ARROWS)",
      "1.500",
      "2.000",
      "3.000",
      "5.000",
      "10.000",
      "20.000"
    };

    /// <summary>Le cran par defaut : celui de la bombe, l'objet rare du jeu.</summary>
    public const int Default = 6;

    public static int Clamp(int index)
    {
      return index < 0 ? 0 : (index >= Steps.Length ? Steps.Length - 1 : index);
    }

    /// <summary>Le nombre d'unites de ce cran, a passer a IncreaseTreasureRates.</summary>
    public static int UnitsOf(int index)
    {
      return Steps[Clamp(index)];
    }

    public static string LabelOf(int index)
    {
      return Labels[Clamp(index)];
    }

    public static int IndexOf(string label)
    {
      int at = Array.IndexOf(Labels, label);
      return at < 0 ? Default : at;
    }
  }
}
