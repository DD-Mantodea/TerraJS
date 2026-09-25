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

`Execute` 挂在哪里就在哪里执行：挂在整个指令上（跟在 `CreateCommandRegistry` 后面）表示**命令本身、不带任何参数**时执行什么；挂在节点上表示**走到该节点**时执行什么。
没有挂 `Execute` 的终点不可执行，走到那里会报错。

如下为一个使用 `StringArgument` 的示例。

```javascript
TJS.Event.ModLoad(() => {
    TJS.Command.CreateCommandRegistry("testcmd")
        .Next(CommandNode.Argument(new StringArgument("str"))
            .Execute((group, caller) => {
                let str = group.GetString("str") //通过名字即可获取对应参数

                Main.NewText(str)
            }))
        .Register()
})
```

指令的参数用**节点**描述：`CommandNode.Argument(...)` 是参数节点，`CommandNode.Literal("set")` 是字面量节点。
顺序靠**嵌套**（子节点接在父节点后面），分支靠**在同一个节点上连续 `Next`**，动作挂在节点自身上。

如下为一个带分支的示例（`/testcmd set <value>` 与 `/testcmd list` 两种不同形状）：

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

可选参数用 `CommandNode.Optional(new IntArgument("stack"))`（或者 `new IntArgument("stack", isOptional: true)`）。

#### 示例：选择器

`PlayersArgument` 支持类似 Minecraft 的目标选择器：`@p`（最近的玩家）、`@r`（随机玩家）、`@a`（所有玩家）、`@s`（执行者自己），也可以直接写玩家名。

`[]` 里用逗号追加条件，格式是 `属性 运算符 值`，运算符支持 `=`、`!=`、`>`、`>=`、`<`、`<=`，数字还支持区间写法：

```
/give @a[statLife>100] TerraJS:dirt
/give @p[distance<30,name=Steve] TerraJS:dirt
/give @a[team=1,sort=nearest,limit=3] TerraJS:dirt
```

`distance`、`r`、`rm` 用来限制与执行者的距离，单位是格子，所以 `distance<30` 表示30格以内，`r=30` 是最大距离，`rm=5` 是最小距离。

`x`、`y` 配 `dx`、`dy` 用来限制一个矩形范围，`dx` 和 `dy` 必须同时给出；不写 `dx`、`dy` 时也可以用 `x`、`y` 单独改变距离计算的起点。

`limit` 用来限制取几个目标，`sort` 用来指定排序方式，可以填 `nearest`、`furthest`、`random`、`arbitrary`。

其余条件名对应 `Player` 的公开字段或属性（`int`、`long`、`float`、`bool`、`string` 和枚举），比如 `statLife`、`dead`、`team`、`name`、`difficulty`。

`@p` 默认取最近的1个，`@r` 默认随机取1个，`@a` 默认取全部，显式写了 `sort` 或 `limit` 就以写的为准。

条件写错、或者匹配不到玩家时，指令会报错并给出用法。

#### 示例：插件

你可以使用插件系统来添加一些新的API。

```csharp
using TerraJS.JSEngine.Plugins;

namespace MyMod.Plugins
{
    public class MyPlugin : TJSPlugin
    {
        public override void Load()
        {
            Mod.Logger.Info("我的插件被加载了");
        }

        public override void AddBindings()
        {
            BindInstance("MyAPI", new MyAPI());

            BindMethod("MyFunc", (int value) => value * 2);
        }
    }
}
```

`BindInstance` 和 `BindMethod` 用来把对象和函数放进JS，JS侧直接用 `MyAPI` 和 `MyFunc(2)` 即可。

在游戏内输入 `/terrajs plugins` 可以看到当前加载了哪些插件。

#### 示例：给自己的类型写选择器

此处使用Terramon举例。

```csharp
public class TerramonArgument(string name, bool isOptional = false) : SelectorArgument<TerramonData>(name, isOptional)
{
    public override SelectorRegistry<TerramonData> Registry => SelectorRegistry<TerramonData>.Instance;

    public override string NameHint => "宝可梦";

    public override bool TryResolveName(string text, out List<TerramonData> targets)
    {
        //不用 @ 时的写法，比如图鉴号或者昵称
    }

    public override Vector2? PositionOf(TerramonData target)
    {
        //需要 distance 和 sort=nearest 时给出位置，不需要就返回 null
    }

    protected override IEnumerable<CompletionEntry> NameCompletions()
    {
        //名字候选
    }
}
```

`Registry` 指定这个参数可以使用什么选择器。如果返回自己新建的 `SelectorRegistry<T>` ，则只有自己的 mod 能加选择器。

`TryResolveName` 处理不用 `@` 的写法，`NameCompletions` 提供补全候选，`PositionOf` 决定这个目标类型能不能算距离，`NameHint` 是写名字时的提示文本。

然后在插件里把选择器和目标类型一起注册。

```csharp
TJSPlugin.RegisterSelector<TerramonData>("p", new SelectorEntry<TerramonData>(candidates, SelectorSort.Nearest, 1));
```

`distance`、`x`、`y`、`dx`、`dy` 和 `sort=nearest` 需要位置，`PositionOf` 返回 `null` 的类型上这几个参数无法使用。
