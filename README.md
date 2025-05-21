[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/DD-Mantodea/TerraJS)

<h1 align="center">TerraJS</h1>

<div align="center">

[English](README-en.md) | 简体中文

一个允许你使用JavaScript制作mod内容的mod

</div>

## 📖 介绍
`TerraJS` 是一个类似Minecraft中 `KubeJS` 的mod，它允许创作者们使用JavaScript来注册物品或注册和更改合成表。

此mod处于刚刚开始开发的阶段，功能少，bug多都是正常现象，请积极上报来帮助作者开发。

#### ✔️ 已经完成的内容

`> 使用JS编写Mod内容`

`> 指令的参数补全及结构化的指令注册`

#### ➕ 正在开发的内容

`> 任务树功能及任务树编辑页面`

`> 指令支持的更多参数类型`

## ✨ 用法

在首次加载mod之后，会在你的 `Mods` 目录下生成一个名为 `TerraJS` 的文件夹，其中包含 `Scripts` 和 `Textures` 两个子文件夹。

在 `Scripts` 文件夹中创建一个 `.js` 文件，即可开始下一步。

#### 示例：注册一个最基础的物品

在 `.js` 文件中写入如下内容。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem").Register()
})
```

保存文件，重新加载mod，即可看到该物品。

#### 示例：使用 `SetDefaults` 函数设置物品的属性

你可以使用 `SetDefaults` 函数来设置物品的基础属性。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .SetDefaults((item) => {
            item.value = 1000                       //价值
            item.damage = 10                        //伤害
            item.useAnimation = 20                  //使用动画长度
            item.useTime = 20                       //使用时间长度
            item.DamageType = DamageClass.Melee     //伤害类型
            item.useStyle = ItemUseStyleID.Swing    //使用动画类型
            item.autoReuse = true                   //是否自动挥舞
            item.rare = ItemRarityID.Green          //稀有度
        })
        .Register()
})
```

这些设置与在 `tModLoader` 中的设置方法基本完全相同。

#### 示例：使用 `Name` 和 `Tooltip` 函数设置物品的名称和工具提示文本

你可以使用 `Name` 和 `Tooltip` 函数设置物品的名称和工具提示文本。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.Chinese, "基础物品")
        .Tooltip(Cultures.Chinese, "基础工具提示文本")
        .Register()
})
```

`Name` 和 `Tooltip` 函数均使用两个参数： `语言` 和 `文本` 。

其中 `语言` 可以参考 `tModLoader` 的 `GameCulture.CultureName`， `文本` 则只需填写你希望显示的文本。

#### 示例：使用 `UpdateAccessory` 函数设置作为饰品时的行为

你可以使用 `UpdateAccessory` 函数设置作为饰品时的行为。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .UpdateAccessory((player, hideVisual) => {
            player.statLifeMax2 += 20 //装备时，玩家最大生命值加20
        })
        .Register()
})
```

具体使用方法与 `tModLoader` 基本相同。