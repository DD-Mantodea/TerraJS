#### 示例：注册一个最基础的物品

在 `.js` 文件中写入如下内容。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Item.CreateItemRegistry("basicItem").Register()
})
```

保存文件，重新加载mod，即可看到该物品。

#### 示例：使用 `Name` 和 `Tooltip` 函数设置物品的名称和工具提示文本

你可以使用 `Name` 和 `Tooltip` 函数设置物品的名称和工具提示文本。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.Chinese, "基础物品")
        .Tooltip(Cultures.Chinese, "基础工具提示文本")
        .Register()
})
```

`Name` 和 `Tooltip` 函数均使用两个参数： `语言` 和 `文本` 。

其中 `语言` 可以参考 `tModLoader` 的 `GameCulture.CultureName`， `文本` 则只需填写你希望显示的文本。

#### 示例：使用 `Texture` 函数设置物品的贴图

你可以使用 `Texture` 函数设置物品的贴图。

```javascript
TJS.Event.ModLoad(() => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.Chinese, "基础物品")
        .Tooltip(Cultures.Chinese, "基础工具提示文本")
        .Texture(TextureType.Items, ItemID.WoodenSword)
        .Texture("MyMod/BasicItem")
        .Register()
})
```

`Texture` 函数有两种用法，第一种是根据TextureType枚举和对应种类的ID来获取游戏内容中的贴图，如第一行所示的使用原版木剑的贴图；第二种则是使用与 `Scripts` 目录在同一位置下的 `Textures` 目录中的相对位置来获取贴图，如第二行的 `"MyMod/BasicItem"` 就代表会读取 `Textures/MyMod/BasicItem.png` 作为贴图。 

#### 示例：使用 `TJS.Event.Item.SetDefaults` 事件设置物品的属性

你可以使用 `TJS.Event.Item.SetDefaults` 事件设置物品的属性。

```javascript
TJS.Event.Item.SetDefaults((item) => {
    if(item.type == TJS.Item.GetModItem("TerraJS", "basicItem"))
    {
        item.value = 1000
        item.damage = 10
        item.useAnimation = 20
        item.useTime = 20
        item.DamageType = DamageClass.Melee
        item.useStyle = ItemUseStyleID.Swing
        item.autoReuse = true
        item.rare = ItemRarityID.Green
        item.accessory = true
    }
})
```

`TJS.Item.GetModItem("TerraJS", "basicItem")` 的作用是：获取 `TerraJS` mod中名字为 `basicItem` 物品的ID

通过判断 `item.type`，可以设置对应物品的属性。

#### 示例：注册一个指令

在 `.js` 文件中写入如下内容。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .Execute((group, caller) => {
            Main.NewText("Hello World!")
        })
        .Register()
})
```

在游戏内聊天框内输入 `/` 即可看到注册的指令。

#### 示例：使用基础的参数

参数可以通过 `Execute` 中传入函数的 `group` 参数来获取。

如下为一个使用 `StringArgument` 的示例。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(new StringArgument("str"))
        .Execute((group, caller) => {
            let str = group.GetString("str") //通过名字即可获取对应参数

            Main.NewText(str)
        })
        .Register()
})
```

如下为一个使用 `ComboArgument` 的示例。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(new ComboArgument("combo", ["enable1", "enable2"]))
        .Execute((group, caller) => {
            var combo = group.GetString("combo") //combo实际上也是字符串

            Main.NewText(combo)
        })
        .Register()
})
```