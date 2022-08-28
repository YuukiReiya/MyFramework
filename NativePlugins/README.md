# はじめに
C#UnityClientでC++を扱うためのWrapper用DLLプロジェクト
* ExternalDynamicLinkLibrary : DLL
* ExecutionProject : 動作確認用のCLIプロジェクト

# 内容

## Lua

2022/08/28
C#でLuaを扱いたい。
→ C#側のLuaプラグインがNLuaで若干古め(2014年以降更新が停止している？)のため、
C++公式プラグインを使ってネイティブ(アンマネージ)で実装してみる。
※Client側でパーサーを用意する。

### 導入

1. Source/DLLをダウンロード

http://luabinaries.sourceforge.net/download.html

~※今回落としたのは`lua-5.4.2_Win64_dllw6_lib.zip`~
※今回落としたのは`lua-5.4.2_Win64_bin.zip`

2. 解凍、配置

落としてきた圧縮ファイルを解凍。
`NativePlugins/dll/lua/`に配置。
* liblua54.a : static library
* lua54.dll : dynamic link library
※静的/動的ライブラリで呼び出し方(静的:パスを通す,動的:ランタイムロード)が変わる点に注意。


3. プロジェクトにパスを通す

