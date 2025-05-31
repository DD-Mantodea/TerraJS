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
        .Name(Cultures.English, "BasicItem")
        .Tooltip(Cultures.English, "Basic tooltip")
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

#### Example: Register a command

Write the codes below in your `.js` file.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .Execute((group, caller) => {
            Main.NewText("Hello World!")
        })
        .Register()
})
```

Type `/` in the game's chatbox then you can see the command.

#### Example: Use basic arguments

`TerraJS.Command` contains some basic arguments:

- `TerraJS.Command.IntArgument(name, options)`
- `TerraJS.Command.BoolArgument(name, options)` 
- `TerraJS.Command.StringArgument(name, options)` 
- `TerraJS.Command.ComboArgument(name, enableValues, options)` 

the `options` are all optional parameters.

- `options` of `IntArgument` contains `bool isOptional, int maxValue, int minValue` 
- `options` of `BoolArgument` contains `bool isOptional` 
- `options` of `StringArgument` contains `bool isOptional` 
- `options` of `ComboArgument` contains `bool isOptional`

`enableValues` of `ComboArgument` means the enable values of the combo argument.

You can get arguments' value by the `group` parameter of the function you send in `Execute`.

The code below is an example to use `StringArgument`.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(TerraJS.Command.StringArgument("text"))
        .Execute((group, caller) => {
            var text = group.GetString("text") //use the name to get arguments' value

            Main.NewText(text)
        })
        .Register()
})
```

The code below is an example to use `ComboArgument`.

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Command.CreateCommandRegistry("testcmd")
        .NextArgument(TerraJS.Command.ComboArgument("combo", ["enable1", "enable2"]))
        .Execute((group, caller) => {
            var combo = group.GetString("combo") //combo is actually a string

            Main.NewText(combo)
        })
        .Register()
})
```