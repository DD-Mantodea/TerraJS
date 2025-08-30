using System.Reflection;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using TerraJS.Contents.Extensions;
using Terraria.UI;
using Terraria.ModLoader.UI;
using Terraria.Localization;
using System;
using Terraria.Audio;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using TerraJS.Contents.Utils;

namespace TerraJS.ModPacks
{
    public class UITJSModPackItem(string path, bool favorite) : UIPanel
    {
        public override void OnInitialize()
        {
            BorderColor = new Color(89, 116, 213) * 0.7f;

            Width.Percent = 1f;

            Height.Pixels = 80;

            SetPadding(0);

            InitializeTextures();

            _packIcon = new UIImage(_iconTexture.Value)
            {
                Left = { Pixels = 8 },
                Top = { Pixels = 8 },
            };
            Append(_packIcon);

            _packName = new UIText(Name)
            {
                Left = new(80, 0),
                Top = { Pixels = 10 },
            };
            Append(_packName);

            var favoriteButton = new UIImageButton(_favorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture)
            {
                VAlign = 0f,
                Top = new(50, 0),
                Left = new(80, 0)
            };
            favoriteButton.OnLeftClick += FavoriteClick;
            favoriteButton.OnMouseOver += FavoriteMouseOver;
            favoriteButton.OnMouseOut += ButtonMouseOut;
            favoriteButton.SetVisibility(1f, _favorite ? 0.8f : 0.4f);
            Append(favoriteButton);

            var renameButton = new UIImageButton(_buttonRenameTexture)
            {
                VAlign = 0f,
                Top = new(50, 0),
                Left = new(104, 0)
            };
            renameButton.OnLeftClick += RenameClick;
            renameButton.OnMouseOver += RenameMouseOver;
            renameButton.OnMouseOut += ButtonMouseOut;
            Append(renameButton);

            _buttonLabel = new UIText("")
            {
                VAlign = 0f,
                Top = new(54, 0),
                Left = new(132, 0)
            };
            Append(_buttonLabel);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            BackgroundColor = UICommon.DefaultUIBlue;

            BorderColor = new Color(89, 116, 213);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);

            BackgroundColor = UICommon.DefaultUIBlueMouseOver;

            BorderColor = new Color(89, 116, 213) * 0.7f;
        }

        public string GetPackID()
        {
            return _pkgConfig?.ID;
        }

        private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_favorite)
                _buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
            else
                _buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));

        }

        private void FavoriteClick(UIMouseEvent evt, UIElement listeningElement)
        {
            _favorite = !_favorite;

            ((UIImageButton)evt.Target).SetImage(_favorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture);
            
            ((UIImageButton)evt.Target).SetVisibility(1f, _favorite ? 0.8f : 0.4f);

            if (_favorite)
            {
                _buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
            }
            else
            {
                _buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
            }

            if (Parent.Parent is UIList list)
                list.UpdateOrder();
        }

        private void RenameMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            _buttonLabel.SetText(Language.GetTextValue("UI.Rename"));
        }

        private void RenameClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            
            Main.clrInput();
            
            UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Language.GetTextValue("TerraJS.UI.EnterModPackName"), "", OnFinishedSettingName, GoBackHere, 0, allowEmpty: true);
            
            uIVirtualKeyboard.SetMaxInputLength(27);

            Main.MenuUI.SetState(uIVirtualKeyboard);

            if (Parent.Parent is UIList list)
                list.UpdateOrder();
        }

        private void OnFinishedSettingName(string name)
        {
            string newDisplayName = name.Trim();

            var renamePath = _packPath.Replace(Name, name);

            FileUtils.MoveDirectory(_packPath, renamePath);

            _packPath = renamePath;

            _packName.SetText(Name);

            Main.MenuUI.SetState(UITJSModPackMenu.Instance);

            Main.menuMode = MenuID.FancyUI;
        }

        private void GoBackHere()
        {
            Main.MenuUI.SetState(UITJSModPackMenu.Instance);

            Main.menuMode = MenuID.FancyUI;
        }

        private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            _buttonLabel.SetText("");
        }

        private void InitializeTextures()
        {
            var tjsPath = Path.Combine(_packPath, "TerraJS");

            _iconTexture = File.Exists(Path.Combine(tjsPath, "icon.png")) ?
                TerraJS.Instance.Assets.CreateUntracked<Texture2D>(File.OpenRead(Path.Combine(tjsPath, "icon.png")), "icon") :
                ModContent.Request<Texture2D>("TerraJS/Assets/Textures/UI/ModPacks/DefaultModPack", AssetRequestMode.ImmediateLoad);

            _buttonFavoriteActiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteActive", AssetRequestMode.ImmediateLoad);

            _buttonFavoriteInactiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteInactive", AssetRequestMode.ImmediateLoad);

            _buttonRenameTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonRename", AssetRequestMode.ImmediateLoad);
        }

        public bool Favorite => _favorite;

        public string Name => new DirectoryInfo(_packPath).Name;

        internal ModPackConfig _pkgConfig = null;

        private Asset<Texture2D> _iconTexture = null;

        private Asset<Texture2D> _buttonFavoriteActiveTexture = null;

        private Asset<Texture2D> _buttonFavoriteInactiveTexture = null;

        private Asset<Texture2D> _buttonRenameTexture = null;

        private string _packPath = path;

        private bool _favorite = favorite;

        private UIText _packName = null;

        private UIImage _packIcon = null;

        private UIText _buttonLabel = null;
    }
}
