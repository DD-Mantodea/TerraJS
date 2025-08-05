using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.JSEngine;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace TerraJS.Contents.UI
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

            TJSEngine.GlobalAPI.Event.UI.RegisterUIEvent?.Invoke();

            UserInput.Initialize();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);

            var target = layers.Where(layer => layer.Name == "Vanilla: Player Chat").First();

            var index = layers.IndexOf(target);

            layers.Insert(index + 1, Layer);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UserInput.Update();

            base.UpdateUI(gameTime);

            Layer.Update(gameTime);
        }
    }
}
