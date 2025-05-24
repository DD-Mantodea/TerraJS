using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using TerraJS.Attributes;
using TerraJS.UI;
using TerraJS.UI.Components.Containers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace TerraJS.Systems
{
    public class TJSUISystem : ModSystem
    {
        public static TJSUILayer Layer = new("TJSUILayer", InterfaceScaleType.UI);

        public override void PostSetupContent()
        {
            Layer.ScreenContainer = new(Main.screenWidth, Main.screenHeight);

            foreach (var type in AssemblyManager.GetLoadableTypes(GetType().Assembly))
            {
                if(type.IsSubclassOf(typeof(Container)) && type.GetCustomAttribute<RegisterUIAttribute>() is { } attribute)
                {
                    var container = Activator.CreateInstance(type) as Container;

                    container.ID = attribute.ID;

                    Layer.Register(container);
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);

            layers.Add(Layer);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);

            Layer.Update(gameTime);
        }
    }
}
