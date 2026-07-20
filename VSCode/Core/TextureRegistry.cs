using System;
using System.Xml;
using FortRise;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace TFModFortRisePickupRotate
{
  // Enregistre l'icone du variant "Rotate".
  //
  // FortRise 4 : Content.LoadAtlas("Atlas/atlas.xml", "Atlas/atlas.png") puis
  // VariantManager.GetVariantIconFromName("Rotate", atlas), qui recuperait la
  // sous-texture "variants/rotate" de l'atlas.
  //
  // FortRise 5 : registry.Subtextures.RegisterTexture rend un ISubtextureEntry.
  // L'image atlas.png fait 75x14 mais l'icone n'est que la region 14x14 nommee
  // "variants/rotate" (x=0,y=0) ; enregistrer l'image entiere donnerait une icone
  // deformee. On enregistre donc, via le callback, la sous-texture DECOUPEE a la
  // region lue dans atlas.xml.
  public class TextureRegistry : IRegisterable
  {
    public static ISubtextureEntry RotateIcon { get; private set; } = null!;

    private const string AtlasXmlPath = "Content/Atlas/atlas.xml";
    private const string AtlasPngPath = "Content/Atlas/atlas.png";
    private const string IconName = "variants/rotate";

    public static void Register(IModContent content, IModRegistry registry)
    {
      RotateIcon = registry.Subtextures.RegisterTexture(
          IconName,
          () => LoadIconSubtexture(content),
          SubtextureAtlasDestination.Atlas
      );
    }

    // Callback resolu paresseusement (une fois le GraphicsDevice pret) : lit la
    // region dans atlas.xml, charge atlas.png et rend la sous-texture decoupee.
    private static Subtexture LoadIconSubtexture(IModContent content)
    {
      IResourceInfo xmlResource = content.GetResource(AtlasXmlPath);
      IResourceInfo pngResource = content.GetResource(AtlasPngPath);

      // Region par defaut = image entiere, ecrasee si trouvee dans le xml.
      int x = 0, y = 0, w = 0, h = 0;
      XmlDocument doc = xmlResource != null ? xmlResource.Xml : null;
      if (doc != null && doc["TextureAtlas"] != null)
      {
        foreach (XmlNode node in doc["TextureAtlas"].GetElementsByTagName("SubTexture"))
        {
          var el = node as XmlElement;
          if (el == null || el.GetAttribute("name") != IconName)
            continue;
          x = int.Parse(el.GetAttribute("x"));
          y = int.Parse(el.GetAttribute("y"));
          w = int.Parse(el.GetAttribute("width"));
          h = int.Parse(el.GetAttribute("height"));
          break;
        }
      }

      Texture2D tex2d;
      using (var stream = pngResource.Stream)
      {
        tex2d = Texture2D.FromStream(Engine.Instance.GraphicsDevice, stream);
      }

      var texture = new Monocle.Texture(tex2d);
      if (w <= 0 || h <= 0)
        return new Subtexture(texture);

      return new Subtexture(texture, x, y, w, h);
    }
  }
}
