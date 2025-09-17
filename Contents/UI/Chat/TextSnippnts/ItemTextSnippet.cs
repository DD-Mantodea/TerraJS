using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Extensions;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace TerraJS.Contents.UI.Chat.TextSnippnts
{
    public class ItemTextSnippet(Item item, string originalText) : TextSnippet(originalText)
    {
        public ItemTextSnippet() : this(null, "") { }

        public override string[] Identifiers => ["i", "item"];

        private Item _item = item;

        public Regex Regex = new("(i|item)(/[^:\\],]+)?(,[^:\\],]+)*:(\\d+)");

        public Regex Option = new("([a-z]+)([0-9]+)");

        public override Vector2 GetSize(TerraJSFont font)
        {
            return new(24, 24);
        }

        public override List<TextSnippet> SplitByWidth(TerraJSFont font, int width)
        {
            return [this];
        }

        public override void Draw(SpriteBatch spriteBatch, TerraJSFont font, ref Vector2 position)
        {
            ItemSlot.Draw(spriteBatch, ref _item, 14, position - new Vector2(5f), Color.White);

            var rect = RectangleUtils.FromVector2(position, new(20, 20));

            if (UserInput.GetMouseRectangle().Intersects(rect))
            {
                Main.HoverItem = _item.Clone();
                Main.instance.MouseText(_item.Name, _item.rare, 0);
            }

            position = position.Add(24, 0);
        }

        public override TextSnippet Parse(string str)
        {
            var item = new Item();

            var match = Regex.Match(str);

            if (match.Success)
            {
                var idStr = match.Groups[^1].Value;

                if (int.TryParse(idStr, out var id) && id < ItemLoader.ItemCount)
                    item.netDefaults(id);

                if (item.type <= 0)
                    return null;

                item.stack = 1;

                var options = string.Join("", match.Groups.Values.ToList()[2..^1]).Replace("/", "").Split(',');

                foreach (var option in options)
                {
                    var optMatch = Option.Match(option);

                    if (!optMatch.Success)
                        continue;

                    var optID = optMatch.Groups[1].Value;

                    switch (optID)
                    {
                        case "d":
                            item = ItemIO.FromBase64(optMatch.Groups[2].Value);

                            break;

                        case "s":
                        case "x":
                        case "stack":
                            if (int.TryParse(optMatch.Groups[2].Value, out var stack))
                                item.stack = Math.Clamp(stack, 1, item.maxStack);

                            break;

                        case "p":
                        case "prefix":
                            if (int.TryParse(optMatch.Groups[2].Value, out var prefix))
                                item.Prefix(Math.Clamp(prefix, 0, PrefixLoader.PrefixCount));

                            break;
                    }
                }
            }

            return new ItemTextSnippet(item, $"[{str}]");
        }

        public override string ToString()
        {
            return _item.Name;
        }
    }
}
