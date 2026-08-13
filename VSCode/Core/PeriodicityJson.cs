using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TFModFortRisePickupRotate
{
  /// <summary>
  /// Relit une periodicite ecrite par une version anterieure du mod.
  ///
  /// Elle etait un ENTIER - un index dans la liste OncePerMatch, OncePerRound, Test -
  /// et elle est devenue une chaine quand les deux premiers crans ont disparu. Sans
  /// ce convertisseur, System.Text.Json refuse le fichier deja enregistre et le JEU
  /// NE DEMARRE PLUS : la lecture des reglages se fait dans SaveData.Load, bien avant
  /// qu'un mod puisse rattraper quoi que ce soit.
  ///
  /// C'est le prix d'avoir change le type d'un reglage deja ecrit sur disque. Le
  /// convertisseur peut disparaitre le jour ou plus personne n'a d'ancien fichier -
  /// autant dire jamais, pour quelques lignes.
  /// </summary>
  public class PeriodicityJsonConverter : JsonConverter<string>
  {
    /// <summary>Valeur de l'ancien index qui designait le mode d'essai.</summary>
    private const int OldTest = 2;

    public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
      if (reader.TokenType == JsonTokenType.Number)
      {
        // Les anciens crans "une fois par match" et "une fois par manche" n'existent
        // plus : ils deviennent Normal, qui est le mode de jeu ordinaire.
        return reader.GetInt32() == OldTest ? "Test" : "Normal";
      }

      return reader.GetString() ?? "Normal";
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value);
    }
  }
}
