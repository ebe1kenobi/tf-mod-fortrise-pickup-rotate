using FortRise;

namespace TFModFortRisePickupRotate
{
  // Variant custom "Rotate".
  //
  // FortRise 4 : OnVariantsRegister + manager.AddVariant(new CustomVariantInfo(...)).
  // FortRise 5 : enregistrement explicite via registry.Variants.RegisterVariant,
  // qui rend une IVariantEntry exposant IsActive().
  public class Variants : IRegisterable
  {
    public static IVariantEntry Rotate = null!;

    public static void Register(IModContent content, IModRegistry registry)
    {
      Rotate = registry.Variants.RegisterVariant("Rotate", new()
      {
        Title = "Rotate",
        Flags = CustomVariantFlags.None,
        Icon = TextureRegistry.RotateIcon
      });
    }
  }
}
