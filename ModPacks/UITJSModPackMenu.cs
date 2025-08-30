using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerraJS.Contents.Utils;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.Social.Base;
using Terraria.UI;

namespace TerraJS.ModPacks
{
    public class UITJSModPackMenu : UIState, IHaveBackButtonCommand
    {
        public override void OnInitialize()
        {
            var constructor = TypeUtils.UILoaderAnimatedImage.GetConstructor(BindingFlags.Public | BindingFlags.Instance, [typeof(float), typeof(float), typeof(float)]);

            _uiLoader = constructor.Invoke([0.5f, 0.5f, 1f]) as UIElement;

            _uiElement = new()
            {
                Width = { Precent = 0.8f },
                MaxWidth = UICommon.MaxPanelWidth,
                Top = { Pixels = 220 },
                Height = { Pixels = -220, Precent = 1f },
                HAlign = 0.5f
            };

            _scrollPanel = new UIPanel
            {
                Width = { Percent = 1f },
                Height = { Pixels = -65, Percent = 0.9f },
                BackgroundColor = UICommon.MainPanelBackground
            };
            _uiElement.Append(_scrollPanel);

            _modPacks = new UIList
            {
                Width = { Pixels = -25, Percent = 1f },
                Height = { Percent = 0.9f },
                ListPadding = 5f
            };
            _scrollPanel.Append(_modPacks);

            var uIScrollbar = new UIScrollbar
            {
                Height = { Percent = 1f },
                HAlign = 1f
            }.WithView(100f, 1000f);
            _scrollPanel.Append(uIScrollbar);
            _modPacks.SetScrollbar(uIScrollbar);

            var backButton = new UIAutoScaleTextTextPanel<LocalizedText>(Language.GetText("UI.Back"))
            {
                Width = new StyleDimension(-10f, 1f / 3f),
                Height = { Pixels = 40 },
                VAlign = 1f,
                HAlign = 0f,
                Top = { Pixels = -20 }
            }.WithFadedMouseOver();
            backButton.OnLeftClick += BackClick;
            _uiElement.Append(backButton);

            var openFolderButton = new UIAutoScaleTextTextPanel<LocalizedText>(Language.GetText("TerraJS.UI.OpenModPackDir"));
            openFolderButton.CopyStyle(backButton);
            openFolderButton.HAlign = 0.5f;
            openFolderButton.WithFadedMouseOver();
            openFolderButton.OnLeftClick += OpenModPackDir;
            _uiElement.Append(openFolderButton);

            var saveNewButton = new UIAutoScaleTextTextPanel<LocalizedText>(Language.GetText("TerraJS.UI.CreateNewModPack"));
            saveNewButton.CopyStyle(backButton);
            saveNewButton.HAlign = 1f;
            saveNewButton.WithFadedMouseOver();
            saveNewButton.OnLeftClick += SaveNewModList;
            _uiElement.Append(saveNewButton);

            Append(_uiElement);
        }

        public override void OnActivate()
        {
            _scrollPanel.Append(_uiLoader);

            _modPacks.Clear();

            Task.Run(() =>
            {
                var dirs = Directory.GetDirectories(Pathes.ModPacksPath, "*", SearchOption.TopDirectoryOnly);

                var modPacks = new List<UIElement>();

                var json = File.Exists(Path.Combine(Pathes.ModPacksPath, "favorites.json")) ?
                    File.ReadAllText(Path.Combine(Pathes.ModPacksPath, "favorites.json")) : "{}";

                var favorites = JObject.Parse(json);

                foreach (var dir in dirs)
                {
                    try
                    {
                        var tjsPath = Path.Combine(dir, "TerraJS");

                        if (!File.Exists(Path.Combine(tjsPath, "pkgConfig.json")))
                            continue;

                        var pkgCfgContent = File.ReadAllText(Path.Combine(tjsPath, "pkgConfig.json"));

                        var pkgConfig = JsonConvert.DeserializeObject<ModPackConfig>(pkgCfgContent);

                        if (pkgConfig == null)
                            continue;

                        if (favorites.TryGetValue(pkgConfig.ID, out var value) && value.Type == JTokenType.Boolean)
                            modPacks.Add(new UITJSModPackItem(dir, value.Value<bool>()) { _pkgConfig = pkgConfig });
                        else
                            modPacks.Add(new UITJSModPackItem(dir, false) { _pkgConfig = pkgConfig });
                    }
                    catch
                    {
                        var badModPackMessage = new UIAutoScaleTextTextPanel<string>(Language.GetTextValue("tModLoader.ModPackMalformed", Path.GetFileName(dir)))
                        {
                            Width = { Percent = 1 },
                            Height = { Pixels = 50, Percent = 0 }
                        };
                        modPacks.Add(badModPackMessage);
                    }
                }

                Main.QueueMainThreadAction(() => {
                    modPacks.ForEach(e => e.Initialize());
                    _modPacks.AddRange(modPacks);
                    _scrollPanel.RemoveChild(_uiLoader);
                });
            });
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_skipDraw)
            {
                _skipDraw = false;
                return;
            }

            if (UpdateFavorites())
            {
                _skipDraw = true;
                Main.MenuUI.Draw(spriteBatch, new GameTime());
            }

            base.Draw(spriteBatch);
        }

        private void BackClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var favorites = new JObject();

            foreach (var item in Items)
                favorites.Add(item._pkgConfig.ID, item.Favorite);

            File.WriteAllText(Path.Combine(Pathes.ModPacksPath, "favorites.json"), favorites.ToString());

            (this as IHaveBackButtonCommand).HandleBackButtonUsage();
        }

        private void OpenModPackDir(UIMouseEvent evt, UIElement listeningElement)
        {
            FileUtils.OpenDirectory(Pathes.ModPacksPath);
        }

        private void SaveNewModList(UIMouseEvent evt, UIElement listeningElement)
        {
            //SoundEngine.PlaySound(SoundID.MenuOpen);
            foreach (var file in Directory.GetFiles(Pathes.TerraJSPath))
            {
                var fileName = file.Split("\\")[^1];

                if (fileName == "pkgConfig.json")
                {
                    var content = File.ReadAllText(file);

                    var config = JsonConvert.DeserializeObject<ModPackConfig>(content);

                    if (config == null)
                    {
                        ShowError();

                        return;
                    }

                    var modPackPath = Path.Combine(Pathes.ModPacksPath, config.ID);

                    FileUtils.DeleteDirectory(modPackPath);

                    FileUtils.CreateDirectoryIfNotExist(ConfigManager.ModConfigPath);

                    var pkgCfgPath = Path.Combine(modPackPath, "ModConfigs");

                    var pkgModPath = Path.Combine(modPackPath, "Mods");

                    var pkgTjsPath = Path.Combine(modPackPath, "TerraJS");

                    FileUtils.CreateDirectoryIfNotExist(pkgCfgPath);

                    FileUtils.CreateDirectoryIfNotExist(pkgModPath);

                    SaveMods(pkgCfgPath, pkgModPath);

                    SaveTjs(pkgTjsPath);

                    return;
                }
            }

            ShowError();

            return;
        }

        private void SaveMods(string pkgCfgPath, string pkgModPath)
        {
            File.Copy(Path.Combine(Pathes.ModsPath, "enabled.json"), Path.Combine(pkgModPath, "enabled.json"));

            var workshopIds = new List<string>();

            var modFileInfo = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance);

            var modOrganizer = TypeUtils.ModOrganizer;

            var tryReadManifest = modOrganizer.GetMethod("TryReadManifest", BindingFlags.NonPublic | BindingFlags.Static);

            var getParentDir = modOrganizer.GetMethod("GetParentDir", BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var mod in ModLoader.Mods)
            {
                var modFile = modFileInfo.GetValue(mod) as TmodFile;

                if (modFile == null)
                    continue;

                var parentDir = getParentDir.Invoke(null, [modFile.path]) as string;

                object[] parameters = [parentDir, null];

                if ((bool)tryReadManifest.Invoke(null, parameters))
                    workshopIds.Add((parameters[1] as FoundWorkshopEntryInfo).workshopEntryId.ToString());

                if (modFile.path != Path.Combine(pkgModPath, mod.Name + ".tmod"))
                    File.Copy(modFile.path, Path.Combine(pkgModPath, mod.Name + ".tmod"), true);
            }

            File.WriteAllLines(Path.Combine(pkgModPath, "install.txt"), workshopIds);
        }

        private void SaveTjs(string pkgTjsPath)
        {
            SaveTjsDir("Scripts", pkgTjsPath);

            SaveTjsDir("Textures", pkgTjsPath);

            SaveTjsFile("pkgConfig.json", pkgTjsPath);
        }

        private void SaveTjsDir(string dirName, string pkgTjsPath)
        {
            FileUtils.CopyDirectory(Path.Combine(Pathes.TerraJSPath, dirName), Path.Combine(pkgTjsPath, dirName));
        }

        private void SaveTjsFile(string fileName, string pkgTjsPath)
        {
            File.Copy(Path.Combine(Pathes.TerraJSPath, fileName), Path.Combine(pkgTjsPath, fileName));
        }

        private void ShowError()
        {

        }

        internal bool UpdateFavorites()
        {
            try
            {
                var list = Items;

                list.Sort((x, y) =>
                {
                    if (x.Favorite && !y.Favorite)
                        return -1;

                    if (!x.Favorite && y.Favorite)
                        return 1;

                    return (x.Name.CompareTo(y.Name) != 0) ? x.Name.CompareTo(y.Name) : x.GetPackID().CompareTo(y.GetPackID());
                });

                _modPacks.Clear();

                _modPacks.AddRange(list);

                return true;
            }
            catch
            {
                return false;
            }
        }

        internal List<UITJSModPackItem> Items => _modPacks?.Children?.First()?.Children?.Select(e => e is UITJSModPackItem item ? item : null).Where(e => e != null).ToList();

        public UIState PreviousUIState { get; set; }

        public static UITJSModPackMenu Instance = new();

        private bool _skipDraw = false;

        private UIElement _uiElement = null;

        private UIPanel _scrollPanel = null;

        private UIList _modPacks = null;

        private UIElement _uiLoader = null;
    }
}
