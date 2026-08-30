using System;
using System.Collections.Generic;
using FortRise;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  /// <summary>
  /// Fait entrer la rotation dans le tirage des coffres, par la voie que FortRise
  /// prevoit pour cela - et qui ne sert qu'au mode NORMAL.
  ///
  /// Enregistrer un pickup ne fait que l'EXISTER : le tableau des taux du
  /// TreasureSpawner est agrandi, mais la case du nouveau venu reste a zero, et il
  /// ne tombe jamais. Ce qui l'y met, c'est un ITowerHook : son
  /// VersusTowerTreasurePatch tourne DANS le constructeur du TreasureSpawner, donc
  /// une fois par match, une fois les variantes posees.
  ///
  /// Le poids final que le jeu tire vaut <c>taux x Chance</c> - le taux etant
  /// l'entier qu'on ajoute ici, et Chance le flottant declare a l'enregistrement du
  /// pickup. C'est ce produit qui donne le "degre d'apparition" par rapport aux
  /// autres objets de la tour.
  ///
  /// Ce que cette voie NE SAIT PAS faire : un tirage pondere n'a pas de plafond,
  /// donc "une fois par manche" ne s'y exprime pas. Les crans OncePerMatch et
  /// OncePerRound ont disparu pour cette raison. Reste le mode TEST, traite par
  /// MyTreasureSpawner, qui force l'apparition pour essayer.
  /// </summary>
  public class TreasureRates : IRegisterable, ITowerHook
  {
    public static void Register(IModContent content, IModRegistry registry)
    {
      registry.TowerHooks.RegisterTowerHook("RotateRates", new TreasureRates());
    }

    /// <summary>
    /// Toutes les tours chargees, celles du jeu comme celles des mods.
    ///
    /// <c>ITowerHook.IsGlobal</c> existe et n'est lu NULLE PART dans FortRise 5.3 :
    /// le chemin versus ne teste que cette liste. Il faut donc l'enumerer soi-meme.
    ///
    /// On la lit dans GameData plutot que dans TowerRegistry, et c'est la seule
    /// facon juste : TowerRegistry ne contient QUE les tours des mods, alors que
    /// celles du jeu recoivent elles aussi un identifiant - patch_GameData.Load leur
    /// donne celui de leur theme, "FrostfangKeep", "SacredGround". GameData les tient
    /// toutes, dans les deux cas, sous la forme exacte que le TreasureSpawner
    /// comparera.
    ///
    /// Relue a chaque fois plutot que figee : un mod de tours charge apres nous
    /// manquerait a l'appel. Elle n'est lue qu'une fois par match.
    /// </summary>
    public HashSet<string> TargetTowers
    {
      get
      {
        var towers = new HashSet<string>();

        foreach (VersusTowerData tower in GameData.VersusTowers)
        {
          towers.Add(tower.LevelID);
        }

        return towers;
      }
    }

    /// <summary>
    /// Faux, et c'est l'inverse de ce que le nom laisse croire : faux veut dire "ce
    /// patch n'est pas saute quand IGNORE TOWER ITEM SET est coche".
    ///
    /// Il FAUT y tourner. Cette variante remplit le masque de 1 partout, y compris
    /// pour les pickups des mods : sans repasser derriere, la rotation tomberait
    /// alors que sa variante n'est pas cochee.
    /// </summary>
    public bool AffectedByIgnoreTowerItemSetVariant => false;

    public void VersusTowerTreasurePatch(IVersusTowerTreasurePatchContext ctx)
    {
      try
      {
        Pickups pickup = RotatePickup.RotateMeta.Pickups;

        // Toujours repartir de zero : c'est le seul moyen d'IMPOSER un taux, l'API
        // ne sachant qu'ajouter ou retrancher. Sans cela on s'ajouterait a ce que la
        // tour - ou IGNORE TOWER ITEM SET - a deja mis dans notre case.
        ctx.RemoveTreasureRates(pickup);

        int rate = WantedRate();
        Logger.Info($"[Rates] rotation : taux {rate}");

        if (rate > 0)
        {
          ctx.IncreaseTreasureRates(pickup, rate);
        }
      }
      catch (Exception e)
      {
        Logger.Error("TreasureRates.VersusTowerTreasurePatch: " + e);
      }
    }

    /// <summary>
    /// Le nombre d'UNITES de masque voulu, ou zero pour ne pas apparaitre du tout.
    ///
    /// Une unite vaut un milliieme (voir Rarity) : c'est ce qui permet de descendre
    /// jusqu'au taux de l'orbe du chaos, alors que l'API ne sait ajouter que des
    /// entiers. Le poids que le jeu tire vaut unites x Chance.
    /// </summary>
    private static int WantedRate()
    {
      // **Un taux nul DIT pourquoi.** Il y a trois raisons de ne pas apparaitre et elles
      // se ressemblent toutes a l'ecran : rien ne sort du coffre. Sans la raison, on
      // passe la soiree a chercher un defaut la ou il n'y a qu'une case decochee - ce
      // qui est exactement arrive, deux fois.
      if (TFModFortRisePickupRotateModule.Settings.periodicity == "Test")
      {
        Logger.Info("[Rates] la rotation : taux 0 car periodicite TEST "
            + "(le taux est alors pose de force apres le tirage)");
        return 0;
      }

      if (!TFModFortRisePickupRotateModule.activated())
      {
        Logger.Info("[Rates] la rotation : taux 0 car la variante n'est pas cochee - "
            + "la cocher, ou activer le reglage 'Pickup activated' du mod");
        return 0;
      }

      int units = Rarity.UnitsOf(TFModFortRisePickupRotateModule.Settings.treasureRarity);

      if (units <= 0)
      {
        Logger.Info("[Rates] la rotation : taux 0 car le cran d'apparition est au minimum");
      }

      return units;
    }
  }
}
