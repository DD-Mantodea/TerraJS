using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Attributes;
using TerraJS.Contents.UI.Chat;
using TerraJS.Contents.UI.Components;
using TerraJS.Contents.UI.Components.Containers;
using TerraJS.Contents.Utils;
using TerraJS.Hooks;
using TerraJS.JSEngine;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace TerraJS.Contents.UI
{
    public class UISystem : ModSystem
    {
        public static Dictionary<string, UILayer> TJSUILayers = [];

        public static Dictionary<string, Container> UIInstances = [];

        public static T GetUIInstance<T>(string ID) where T : Container
        {
            return UIInstances.TryGetValue(ID, out var instance) ? instance as T : null;
        }

        public override void OnWorldUnload()
        {
            ChatBox.Instance.ClearMessage();

            MessageBox.Instance.ClearMessage();

            base.OnWorldUnload();
        }

        public override void PostSetupContent()
        {
            foreach (var field in typeof(TJSLayerID).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var id = field.GetValue(null) as string;

                var scaleType = TJSLayerID.GetScaleType(id);

                if (scaleType == null)
                    return;

                var uiLayer = new UILayer(id, scaleType.Value);

                TJSUILayers.Add(id, uiLayer);
            }

            UserInput.Initialize();

            foreach (var assembly in ModLoader.Mods.Select(m => m.Code))
            {
                foreach (var type in AssemblyManager.GetLoadableTypes(assembly))
                {
                    if (type.IsSubclassOf(typeof(Container)) && type.GetCustomAttribute<RegisterUIAttribute>() is { } attribute)
                    {
                        var container = Activator.CreateInstance(type) as Container;

                        container.ID = attribute.ID;

                        container.Priority = attribute.Priority;

                        if (UIInstances.ContainsKey(container.ID))
                            continue;

                        if (TJSUILayers.TryGetValue(attribute.Layer, out var layer))
                        {
                            layer.Register(container);

                            UIInstances.Add(container.ID, container);
                        }
                    }
                }
            }

            TJSEngine.GlobalAPI.Event.UI.RegisterUIEvent?.Invoke();

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            base.ModifyInterfaceLayers(layers);

            var newList = new List<GameInterfaceLayer>();

            foreach (var layer in layers)
            {
                newList.Add(layer);

                if (layer.Name.StartsWith("Vanilla"))
                {
                    var name = layer.Name.Replace("Vanilla:", "TerraJS:");

                    if (TJSUILayers.TryGetValue(name, out var uiLayer))
                        newList.Add(uiLayer);
                }
            }

            layers.Clear();

            layers.AddRange(newList);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UserInput.Update();

            base.UpdateUI(gameTime);

            MainHook.ShouldDisableViewZoom = false;

            PlayerHook.ShouldDisableScrollHotbar = false;

            PlayerHook.ShouldDisableOpenInventory = false;

            TextBox.HasActiveInstance = false;

            IMEUtils.DisableIME();

            foreach (var layer in TJSUILayers.Values)
                layer.Update(gameTime);
        }
    }

    public class TJSLayerID
    {
        public const string Interface_Logic_1 = "TerraJS: Interface Logic 1";

        public const string MP_Player_Names = "TerraJS: MP Player Names";

        public const string Emote_Bubbles = "TerraJS: Emote Bubbles";

        public const string Entity_Markers = "TerraJS: Entity Markers";

        public const string Smart_Cursor_Targets = "TerraJS: Smart Cursor Targets";

        public const string Laser_Ruler = "TerraJS: Laser Ruler";

        public const string Ruler = "TerraJS: Ruler";

        public const string Gamepad_Lock_On = "TerraJS: Gamepad Lock On";

        public const string Tile_Grid_Option = "TerraJS: Tile Grid Option";

        public const string Town_NPC_House_Banners = "TerraJS: Town NPC House Banners";

        public const string Hide_UI_Toggle = "TerraJS: Hide UI Toggle";

        public const string Wire_Selection = "TerraJS: Wire Selection";

        public const string Capture_Manager_Check = "TerraJS: Capture Manager Check";

        public const string Ingame_Options = "TerraJS: Ingame Options";

        public const string Fancy_UI = "TerraJS: Fancy UI";

        public const string Achievement_Complete_Popups = "TerraJS: Achievement Complete Popups";

        public const string Entity_Health_Bars = "TerraJS: Entity Health Bars";

        public const string Invasion_Progress_Bars = "TerraJS: Invasion Progress Bars";

        public const string Map_And_Minimap = "TerraJS: Map / Minimap";

        public const string Diagnose_Net = "TerraJS: Diagnose Net";

        public const string Diagnose_Video = "TerraJS: Diagnose Video";

        public const string Sign_Tile_Bubble = "TerraJS: Sign Tile Bubble";

        public const string Hair_Window = "TerraJS: Hair Window";

        public const string Dresser_Window = "TerraJS: Dresser Window";

        public const string NPC_And_Sign_Dialog = "TerraJS: NPC / Sign Dialog";

        public const string Interface_Logic_2 = "TerraJS: Interface Logic 2";

        public const string Resource_Bars = "TerraJS: Resource Bars";

        public const string Interface_Logic_3 = "TerraJS: Interface Logic 3";

        public const string Inventory = "TerraJS: Inventory";

        public const string Info_Accessories_Bar = "TerraJS: Info Accessories Bar";

        public const string Settings_Button = "TerraJS: Settings Button";

        public const string Hotbar = "TerraJS: Hotbar";

        public const string Builder_Accessories_Bar = "TerraJS: Builder Accessories Bar";

        public const string Radial_Hotbars = "TerraJS: Radial Hotbars";

        public const string Mouse_Text = "TerraJS: Mouse Text";

        public const string Player_Chat = "TerraJS: Player Chat";

        public const string Death_Text = "TerraJS: Death Text";

        public const string Cursor = "TerraJS: Cursor";

        public const string Debug_Stuff = "TerraJS: Debug Stuff";

        public const string Mouse_Item_And_NPC_Head = "TerraJS: Mouse Item / NPC Head";

        public const string Mouse_Over = "TerraJS: Mouse Over";

        public const string Interact_Item_Icon = "TerraJS: Interact Item Icon";

        public const string Interface_Logic_4 = "TerraJS: Interface Logic 4";

        public static InterfaceScaleType? GetScaleType(string ID)
        {
            return ID switch
            {
                "TerraJS: Interface Logic 1" => InterfaceScaleType.Game,
                "TerraJS: MP Player Names" => InterfaceScaleType.Game,
                "TerraJS: Emote Bubbles" => InterfaceScaleType.Game,
                "TerraJS: Entity Markers" => InterfaceScaleType.Game,
                "TerraJS: Smart Cursor Targets" => InterfaceScaleType.Game,
                "TerraJS: Laser Ruler" => InterfaceScaleType.Game,
                "TerraJS: Ruler" => InterfaceScaleType.Game,
                "TerraJS: Gamepad Lock On" => InterfaceScaleType.Game,
                "TerraJS: Tile Grid Option" => InterfaceScaleType.Game,
                "TerraJS: Town NPC House Banners" => InterfaceScaleType.Game,
                "TerraJS: Hide UI Toggle" => InterfaceScaleType.UI,
                "TerraJS: Wire Selection" => InterfaceScaleType.UI,
                "TerraJS: Capture Manager Check" => InterfaceScaleType.Game,
                "TerraJS: Ingame Options" => InterfaceScaleType.UI,
                "TerraJS: Fancy UI" => InterfaceScaleType.UI,
                "TerraJS: Achievement Complete Popups" => InterfaceScaleType.UI,
                "TerraJS: Entity Health Bars" => InterfaceScaleType.Game,
                "TerraJS: Invasion Progress Bars" => InterfaceScaleType.UI,
                "TerraJS: Map / Minimap" => InterfaceScaleType.UI,
                "TerraJS: Diagnose Net" => InterfaceScaleType.UI,
                "TerraJS: Diagnose Video" => InterfaceScaleType.UI,
                "TerraJS: Sign Tile Bubble" => InterfaceScaleType.Game,
                "TerraJS: Hair Window" => InterfaceScaleType.UI,
                "TerraJS: Dresser Window" => InterfaceScaleType.UI,
                "TerraJS: NPC / Sign Dialog" => InterfaceScaleType.UI,
                "TerraJS: Interface Logic 2" => InterfaceScaleType.Game,
                "TerraJS: Resource Bars" => InterfaceScaleType.UI,
                "TerraJS: Interface Logic 3" => InterfaceScaleType.Game,
                "TerraJS: Inventory" => InterfaceScaleType.UI,
                "TerraJS: Info Accessories Bar" => InterfaceScaleType.UI,
                "TerraJS: Settings Button" => InterfaceScaleType.UI,
                "TerraJS: Hotbar" => InterfaceScaleType.UI,
                "TerraJS: Builder Accessories Bar" => InterfaceScaleType.UI,
                "TerraJS: Radial Hotbars" => InterfaceScaleType.UI,
                "TerraJS: Mouse Text" => InterfaceScaleType.UI,
                "TerraJS: Player Chat" => InterfaceScaleType.UI,
                "TerraJS: Death Text" => InterfaceScaleType.UI,
                "TerraJS: Cursor" => InterfaceScaleType.UI,
                "TerraJS: Debug Stuff" => InterfaceScaleType.UI,
                "TerraJS: Mouse Item / NPC Head" => InterfaceScaleType.UI,
                "TerraJS: Mouse Over" => InterfaceScaleType.Game,
                "TerraJS: Interact Item Icon" => InterfaceScaleType.UI,
                "TerraJS: Interface Logic 4" => InterfaceScaleType.UI,
                _ => null
            };
        }
    }
}
