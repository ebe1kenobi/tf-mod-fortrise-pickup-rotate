using System;
using FortRise;
using HarmonyLib;
using TowerFall;

namespace TFModFortRisePickupRotate
{
  /// <summary>
  /// L'ecriture directe dans les taux du TreasureSpawner, gardee POUR LES ESSAIS.
  ///
  /// Elle remplace l'ancienne methode, qui allait ecraser le contenu du premier
  /// coffre par DynamicData : celle-ci obtenait le meme resultat en touchant un champ
  /// prive, ne servait qu'un coffre, et ne pouvait pas cohabiter avec le tirage du
  /// jeu.
  ///
  /// Le defaut de ce qui reste : le postfix passe APRES le constructeur, donc apres
  /// les ITowerHook et apres les exclusions de variantes - quoi qu'il ait ete decide,
  /// c'est cette valeur-la qui reste. La qualite, qui est exactement ce qu'on veut
  /// pour essayer un pickup : il tombe a coup sur.
  ///
  /// C'est pourquoi ce n'est bon que pour un essai, et pourquoi ca ne tourne qu'en
  /// mode TEST. En mode normal c'est TreasureRates qui decide, par l'API de FortRise.
  /// </summary>
  public class MyTreasureSpawner : IHookable
  {
    /// <summary>
    /// Poids donne a la rotation en mode TEST.
    ///
    /// Vingt et non un : les taux vanilla d'une tour totalisent plusieurs dizaines,
    /// et un poids de 1 ne sortirait qu'un coffre sur trente. Il ne s'agit pas ici de
    /// doser mais de voir le pickup.
    /// </summary>
    private const float TestRate = 20f;

    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredConstructor(typeof(TreasureSpawner), [typeof(Session), typeof(VersusTowerData)]),
          prefix: new HarmonyMethod(TreasureSpawner_ctor_Prefix),
          postfix: new HarmonyMethod(TreasureSpawner_ctor_Postfix)
      );
    }

    /// <summary>
    /// Donne a la tour la table de taux qu'il faut, avant que le spawner ne la lise.
    ///
    /// Les tours des MODS se fabriquent une copie de DefaultTreasureChances au
    /// chargement... et ce chargement a lieu AVANT que FortRise n'y ajoute les
    /// pickups des mods (RiseCore.Initialize precede ExtendTreasures). Leur copie
    /// s'arrete donc avant notre case, et le taux de notre pickup y vaut zero : il ne
    /// tombait jamais sur une tour de mod, quel que soit le masque - le bug ne se
    /// voyait pas sur les tours du jeu, qui partagent la table commune.
    ///
    /// En prefix et non en postfix : le constructeur lit la table pour calculer les
    /// taux, il faut donc l'avoir corrigee avant qu'il ne commence.
    /// </summary>
    public static void TreasureSpawner_ctor_Prefix(VersusTowerData versusTowerData)
    {
      try
      {
        if (versusTowerData == null)
        {
          return;
        }

        float[] canonical = TreasureSpawner.DefaultTreasureChances;
        float[] chances = versusTowerData.TreasureChances;

        // Une tour du jeu n'a pas de table a elle : elle retombe sur la table commune,
        // que ExtendTreasures a deja renseignee. Rien a faire.
        if (chances == null || ReferenceEquals(chances, canonical))
        {
          return;
        }

        if (chances.Length < canonical.Length)
        {
          Array.Resize(ref chances, canonical.Length);
          versusTowerData.TreasureChances = chances;
        }

        chances[(int)RotatePickup.RotateMeta.Pickups] = Rarity.Unit;
      }
      catch (Exception e)
      {
        Logger.Error("MyTreasureSpawner.TreasureSpawner_ctor_Prefix: " + e);
      }
    }

    public static void TreasureSpawner_ctor_Postfix(TreasureSpawner __instance)
    {
      try
      {
        if (TFModFortRisePickupRotateModule.Settings.periodicity != "Test")
        {
          return;
        }

        Pickups pickup = RotatePickup.RotateMeta.Pickups;

        // Meme en mode TEST, la variante decide : sans quoi on ne pourrait plus
        // essayer une partie SANS le pickup.
        float rate = TFModFortRisePickupRotateModule.activated() ? TestRate : 0f;
        __instance.TreasureRates[(int)pickup] = rate;

        Logger.Info($"[Test] rotation forcee a {rate}");
      }
      catch (Exception e)
      {
        Logger.Error("MyTreasureSpawner.TreasureSpawner_ctor_Postfix: " + e);
      }
    }
  }
}
