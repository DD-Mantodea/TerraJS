[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/DD-Mantodea/TerraJS)

<h1 align="center">TerraJS</h1>

<div align="center">

English | [简体中文](README.md)
    
A mod allows you to make contents by JavaScript.

</div>

## 📖 Introduction
`TerraJS` is a mod like `KubeJS` in Minecraft. It allow modders to register item or modify and register recipe by JavaScript.

This mod is just begin so only has few functions and lots of bugs. Plz send issues if you meet questions to help the author to develop this mod better! Thx!

## ✨ Usage

After the first loading, the mod will create a directory named `TerraJS` under `Mods` directory, which contains two child directories named `Scripts` and `Textures`.

Create a  `.js` file in the `Scripts` directory, then you can start the first step!

#### Example: Register a basic item

Write the codes below in your `.js` file.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem").Register()
})
```

Save the file then reload the mod, you will find a new item!

#### Example: Use `SetDefaults` to set item infos

You can use `SetDefaults` to set item infos.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .SetDefaults((item) => {
            item.value = 1000                       
            item.damage = 10
            item.useAnimation = 20
            item.useTime = 20
            item.DamageType = DamageClass.Melee
            item.useStyle = ItemUseStyleID.Swing
            item.autoReuse = true
            item.rare = ItemRarityID.Green
        })
        .Register()
})
```

These settings are nearly the same as those in  `tModLoader` .

#### Example: Use `Name` and `Tooltip` to set item name and tooltip.

You can use `Name` and `Tooltip` to set item name and tooltip.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.Chinese, "基础物品")
        .Tooltip(Cultures.Chinese, "基础工具提示文本")
        .Register()
})
```

`Name` and `Tooltip` both use two parameters: `culture` and `text`.

`culture` is just like the `GameCulture.CultureName` in `tModLoader` and `text` is whatever you like.

#### Example: Use `UpdateAccessory` to set item as accessory's action

You can use `UpdateAccessory` to set item as accessory's action.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .UpdateAccessory((player, hideVisual) => {
            player.statLifeMax2 += 20 //when equip, the player's max life will improve by 20
        })
        .Register()
})
```

Nearly the same as it in `tModLoader`.