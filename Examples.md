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

#### 示例：注册一个指令

在 `.js` 文件中写入如下内容。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .Execute((group, caller) => {
            Main.NewText("Hello World!")
        })
        .Register()
})
```

在游戏内聊天框内输入 `/` 即可看到注册的指令。

#### 示例：使用基础的参数

`TerraJS.Command` 实例中包含了一些基础的参数类型：

- `TerraJS.Command.IntArgument(name, options)`
- `TerraJS.Command.BoolArgument(name, options)` 
- `TerraJS.Command.StringArgument(name, options)` 
- `TerraJS.Command.ComboArgument(name, enableValues, options)` 

其中 `options` 均为可选参数。

- `IntArgument` 的 `options` 包括 `bool isOptional, int maxValue, int minValue` 
- `BoolArgument` 的 `options` 包括 `bool isOptional` 
- `StringArgument` 的 `options` 包括 `bool isOptional` 
- `ComboArgument` 的 `options` 包括 `bool isOptional`

`ComboArgument` 的 `enableValues` 代表他可以接受的参数（string）。

参数可以通过 `Execute` 中传入函数的 `group` 参数来获取。

如下为一个使用 `StringArgument` 的示例。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(TerraJS.Command.StringArgument("text"))
        .Execute((group, caller) => {
            var text = group.GetString("text") //通过名字即可获取对应参数

            Main.NewText(text)
        })
        .Register()
})
```

如下为一个使用 `ComboArgument` 的示例。

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(TerraJS.Command.ComboArgument("combo", ["enable1", "enable2"]))
        .Execute((group, caller) => {
            var combo = group.GetString("combo") //combo实际上也是字符串

            Main.NewText(combo)
        })
        .Register()
})
```