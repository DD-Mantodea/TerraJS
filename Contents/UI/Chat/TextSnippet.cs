using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraJS.Contents.Utils;
using Terraria.ModLoader;

namespace TerraJS.Contents.UI.Chat
{
    public abstract class TextSnippet(string originalText) : ModType
    {
        protected sealed override void Register()
        {
            foreach (var i in Identifiers)
                SnippetUtils.Parsers.TryAdd(i, Parse);
        }

        public abstract Vector2 GetSize(TerraJSFont font);

        public abstract override string ToString();

        public abstract List<TextSnippet> SplitByWidth(TerraJSFont font, int width);

        public abstract void Draw(SpriteBatch spriteBatch, TerraJSFont font, ref Vector2 position);

        public abstract TextSnippet Parse(string str);

        public abstract string[] Identifiers { get; }

        public string OriginalText { get; set; } = originalText;
    }
}
