[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/DD-Mantodea/TerraJS)

<h1 align="center">TerraJS</h1>

<div align="center">

[English](README-en.md) | 简体中文

一个允许你使用JavaScript制作mod内容的mod

</div>

## 📖 介绍
`TerraJS` 是一个类似Minecraft中 `KubeJS` 的mod，它允许创作者们使用JavaScript来注册物品或注册和更改合成表等等。

此mod处于刚刚开始开发的阶段，功能少，bug多都是正常现象，请积极上报来帮助作者开发。

#### ✔️ 已经完成的内容

`> 使用JS编写Mod内容`

`> 指令的参数补全及结构化的指令注册`

`> 类似ProbeJS的收集类型信息与函数信息并生成 .d.ts 文件供 VSCode 实现补全的功能`

`> 游戏内重载JS函数`

#### ➕ 正在开发的内容

`> 任务树功能及任务树编辑页面`

`> 指令支持的更多参数类型`

## ✨ 用法

在首次加载mod之后，会在你的 `Mods` 目录下生成一个名为 `TerraJS` 的文件夹，其中包含 `Scripts` 和 `Textures` 两个子文件夹。

你可以在开始你的第一步之前，在游戏内的聊天框中输入 `/terrajs detect` 来启动 `Detector`，当你看到 `[Detector] 收集完成` 时，说明 `.d.ts` 文件已经生成完毕。

在 `Scripts` 文件夹中创建一个 `.js` 文件，即可开始下一步。

[在这里查看示例](Examples.md)