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

`Execute` runs exactly where you attach it: attached to the whole command (right after `CreateCommandRegistry`) it runs for the **command itself with no arguments**; attached to a node it runs when the parse **reaches that node**.
An end point without any `Execute` is not executable and reports an error.

The code below is an example to use `StringArgument`.

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .Next(CommandNode.Argument(new StringArgument("text"))
            .Execute((group, caller) => {
                let text = group.GetString("text") //use the name to get arguments' value

                Main.NewText(text)
            }))
        .Register()
})
```

Arguments are described with **nodes**: `CommandNode.Argument(...)` is an argument node, `CommandNode.Literal("set")` is a literal node.
Sequences are written by **nesting** (a child node follows its parent), branches by **calling `Next` several times on the same node**, and actions are attached to the node itself.

The code below is a branch example (`/testcmd set <value>` and `/testcmd list`, two different shapes):

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .Next(CommandNode.Literal("set")
            .Next(CommandNode.Argument(new IntArgument("value"))
                .Execute((group, caller) => {
                    Main.NewText("set " + group.GetInt("value"))
                }))
            .Next(CommandNode.Literal("list")
                .Execute((group, caller) => {
                    Main.NewText("list")
                })))
        .Register()
})
```

Optional arguments use `CommandNode.Optional(new IntArgument("stack"))` (or `new IntArgument("stack", isOptional: true)`).

#### Example: Target selectors

`PlayersArgument` accepts Minecraft-like target selectors: `@p` (nearest player), `@r` (random player), `@a` (all players), `@s` (the executor), and plain player names.

Inside `[]` you can add conditions separated by commas, written as `member operator value`, with `=`, `!=`, `>`, `>=`, `<`, `<=`, and ranges for numbers:

```
/give @a[statLife>100] TerraJS:dirt
/give @p[distance<30,name=Steve] TerraJS:dirt
/give @a[team=1,sort=nearest,limit=3] TerraJS:dirt
```

`distance`, `r` and `rm` limit the distance from the executor, measured in tiles, so `distance<30` means within 30 tiles, `r=30` is the maximum distance and `rm=5` is the minimum one.

`x` and `y` together with `dx` and `dy` limit a rectangular area, and `dx` and `dy` must be given together. Without `dx` and `dy` you can still use `x` and `y` alone to move the origin used for distance.

`limit` caps how many targets are taken, `sort` picks the order and accepts `nearest`, `furthest`, `random` and `arbitrary`.

Every other condition name is a public field or property of `Player` (`int`, `long`, `float`, `bool`, `string` and enums), such as `statLife`, `dead`, `team`, `name` and `difficulty`.

`@p` takes the nearest one by default, `@r` a random one and `@a` all of them; an explicit `sort` or `limit` wins.

When a condition is written wrong, or nothing matches, the command reports an error together with its usage.

#### Example: Plugin

You can use the plugin system.

```csharp
using TerraJS.JSEngine.Plugins;

namespace MyMod.Plugins
{
    public class MyPlugin : TJSPlugin
    {
        public override void Load()
        {
            Mod.Logger.Info("my plugin is loaded");
        }

        public override void AddBindings()
        {
            BindInstance("MyAPI", new MyAPI());

            BindMethod("MyFunc", (int value) => value * 2);
        }
    }
}
```

`BindInstance` and `BindMethod` put objects and functions into JS, so JS can use `MyAPI` and `MyFunc(2)` directly.

Type `/terrajs plugins` in game to see which plugins are loaded.

#### Example: Write a selector for your own type

Terramon is used as an example here.

```csharp
public class TerramonArgument(string name, bool isOptional = false) : SelectorArgument<TerramonData>(name, isOptional)
{
    public override SelectorRegistry<TerramonData> Registry => SelectorRegistry<TerramonData>.Instance;

    public override string NameHint => "pokemon";

    public override bool TryResolveName(string text, out List<TerramonData> targets)
    {
        //the form without @, for example a dex number or a nickname
    }

    public override Vector2? PositionOf(TerramonData target)
    {
        //give a position if you need distance and sort=nearest, otherwise return null
    }

    protected override IEnumerable<CompletionEntry> NameCompletions()
    {
        //name candidates
    }
}
```

`Registry` decides which selectors this argument can use. Returning a new `SelectorRegistry<T>` of your own means only your own mod can add selectors to it.

`TryResolveName` handles the form without `@`, `NameCompletions` provides name candidates, `PositionOf` decides whether this target type can measure distance, and `NameHint` is the hint shown while typing a name.

Then register the selector together with its target type in the plugin.

```csharp
TJSPlugin.RegisterSelector<TerramonData>("p", new SelectorEntry<TerramonData>(candidates, SelectorSort.Nearest, 1));
```

`distance`, `x`, `y`, `dx`, `dy` and `sort=nearest` need a position, so they cannot be used on a type whose `PositionOf` returns `null`.
