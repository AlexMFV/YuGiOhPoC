using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Scripts
{
    internal static class SpriteManager
    {
        public static void PreloadSprites(Player p1, Player p2)
        {
            //Go through all both players decks and add to a new list all the unique card sprite names
            List<string> uniqueSprites = new List<string>();
            foreach (Card card in p1.Deck)
                if (!uniqueSprites.Contains(card._imageName))
                    uniqueSprites.Add(card._imageName);

            foreach (Card card in p2.Deck)
                if (!uniqueSprites.Contains(card._imageName))
                    uniqueSprites.Add(card._imageName);

            Globals.Sprites = new Dictionary<string, Sprite>(); //Reset sprite list
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

            //Add the default card back sprite
            Texture2D tex = Resources.Load($"Cards/card_ura") as Texture2D;
            Sprite sprt = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sprites.Add("card_ura", sprt);

            //Loop uniqueSprites and create new sprites loaded from the string
            foreach (string spriteName in uniqueSprites)
            {
                Texture2D s = Resources.Load($"Cards/{spriteName}") as Texture2D;
                Sprite newSprite = Sprite.Create(s, new Rect(0, 0, s.width, s.height), new Vector2(0.5f, 0.5f));
                sprites.Add(spriteName, newSprite);
            }

            Globals.Sprites = sprites;
        }

        public static Sprite GetSpriteFromGlobal(string spriteName)
        {
            if (Globals.Sprites.ContainsKey(spriteName))
                return Globals.Sprites[spriteName];
            else
                return null;
        }
    }
}
