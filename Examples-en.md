#### Example: Register a basic item

Write the codes below in your `.js` file.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Item.CreateItemRegistry("basicItem").Register()
})
```

Save the file then reload the mod, you will find a new item!

#### Example: Use `Name` and `Tooltip` to set item name and tooltip.

You can use `Name` and `Tooltip` to set item name and tooltip.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.English, "BasicItem")
        .Tooltip(Cultures.English, "Basic tooltip")
        .Register()
})
```

`Name` and `Tooltip` both use two parameters: `culture` and `text`.

`culture` is just like the `GameCulture.CultureName` in `tModLoader` and `text` is whatever you like.

#### Example: Use `Texture` method to set item's texture.

You can use `Texture` method to set item's texture.

```javascript
TJS.Event.ModLoad(() => {
    TerraJS.Item.CreateItemRegistry("basicItem")
        .Name(Cultures.English, "BasicItem")
        .Tooltip(Cultures.English, "Basic tooltip")
        .Texture(TextureType.Items, ItemID.WoodenSword)
        .Texture("MyMod/BasicItem")
        .Register()
})
```

There are two usages of `Texture` method: the first one is using TextureType enum and the corresponding ID to get texture in game content, just like the first line which use the vanilla wooden sword's texture; the second one is use relative path in `Textures` directory to get texture, just like the second line which use `"MyMod/BasicItem"` means it will load `Textures/MyMod/BasicItem.png` as the texture.

#### Example: Use `TJS.Event.Item.SetDefaults` event to set item attributes.

You can use `TJS.Event.Item.SetDefaults` event to set item attributes.

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

The effect of `TJS.Item.GetModItem("TerraJS", "basicItem")` is get item's ID with the name of `basicItem` in `TerraJS` mod.

By judging the item.type, you can set item's attributes with the corresponding ID.

#### Example: Register a command

Write the codes below in your `.js` file.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .Execute((group, caller) => {
            Main.NewText("Hello World!")
        })
        .Register()
})
```

Type `/` in the game's chatbox then you can see the command.

#### Example: Use basic arguments

You can get arguments' value by the `group` parameter of the function you send in `Execute`.

The code below is an example to use `StringArgument`.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(new StringArgument("text"))
        .Execute((group, caller) => {
            let text = group.GetString("text") //use the name to get arguments' value

            Main.NewText(text)
        })
        .Register()
})
```

The code below is an example to use `ComboArgument`.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(new ComboArgument("combo", ["enable1", "enable2"]))
        .Execute((group, caller) => {
            var combo = group.GetString("combo") //combo is actually a string

            Main.NewText(combo)
        })
        .Register()
})
```