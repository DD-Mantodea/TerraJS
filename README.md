[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/DD-Mantodea/TerraJS)

<h1 align="center">TerraJS</h1>

<div align="center">
    一个允许你使用JavaScript制作mod内容的mod
</div>

## 📖 介绍
`TerraJS` 是一个类似Minecraft中 `KubeJS` 的mod，它允许创作者们使用JavaScript来注册物品、更改合成表。

此mod处于刚刚开始开发的阶段，功能少，bug多都是正常现象，请积极上报来帮助作者开发。

## ✨ 用法

在首次加载mod之后，会在你的 `Steam tModLoader` 根目录下生成一个名为 `TerraJS` 的文件夹，其中包含 `Scripts` 和 `Textures` 两个子文件夹

在 `Scripts` 文件夹中创建一个 `.js` 文件，即可开始下一步

#### 示例：注册一个最基础的物品

在 `.js` 文件中写入如下内容

```javascript
TerraJS.Event.OnEvent("ModLoad", () => {
    TerraJS.Item.CreateItemRegistry("basicItem").Register()
})
```

保存文件，重新加载mod，即可看到该物品。
