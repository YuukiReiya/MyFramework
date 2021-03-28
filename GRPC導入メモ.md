環境
Unity2019.4.17f1
gRPC2.37.0

# gRPC導入手順メモ

## 1. gRPCの用意

1\) リンク先の[gRPC Packages](https://packages.grpc.io/)の「**`Daily Builds of master Branch`**」で`Timestamp`が最新(推奨)の`Build ID`のリンクに飛ぶ。<br>2\) C#から下記2点をDLする。
* grpc_unity_package.x.xx.x. ... (zip)
* Grpc.Tools.x.xx.x (nupkg)

3\) DLしたものを解凍する。<br>(※`nupkg`の拡張子は`zip`にリネームすることで解凍可能)


## 2. Clientのプロジェクト設定

1\) Unityプロジェクト側のマネージプラグイン(`Aseets/Plugins`)に`grpc_unity_package`を解凍して出てきた`Plugins`を配置する。<br>2\) Unityの`.Net Framework`のバージョンを上げる
```
Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level* : .Net 4.x
もしくは
Build Settings → Player Settings... → Player → Other Settings → Configuration → Api Compatibility Level* : .Net 4.x
```

## 3. Serverのプロジェクト設定

1\) プロジェクトに下記ライブラリのパスを通す。

* Google.Protobuf
* Grpc.Core
* Grpc.Core.Api

2\) このままだとまだ下記のようなエラーが出てしまう。

```c
Error loading native library. 
Not found in any of the possible locations:
<参照しているパス>/grpc_csharp_ext.x64.dll
```               
これは参照先のパスに`grpc_csharp_ext.x64.dll`が存在しないエラーなので、`grpc_csharp_ext.dll`をリネームして用意してあげる。

`grpc_csharp_ext.dll`の存在するパスは以下。
```lua
grpc_unity_package.x.xx.x./Plugins/Grpc.Core/runtimes/<OS>/grpc_csharp_ext.dll
例:
grpc_unity_package.x.xx.x./Plugins/Grpc.Core/runtimes/win/x64/grpc_csharp_ext.dll
```

※今回はServerのプロジェクトにビルド前イベントで`system.ini`に書かれたファイルのパスを出力先にコピーするようにしてある。
```
ファイルコピーするためのsystem.iniの設定
[COPY_FILE_PATH]セクション([]で区切られた箇所のこと)に'変数=値'で登録する。
COPY_FILE_2＝<コピー先ファイルパス> みたいな感じで追加してくれればOK
```

## 4. .protoファイルのコンパイル

1\) `protoc.exe`があるディレクトリで`CLI(CMD)`を起動する。(パスを通すのでもOK)<br>2\) コマンドを打ってコンパイルする。
```
protoc -I . --csharp_out=<書き出し先のディレクトリ> --grpc_out=<書き出し先のディレクトリ> <コンパイル元のファイル.proto> --plugin=protoc-gen-grpc="<grpc_csharp_plugin.exeまでのフルパス>"

例:
protoc -I . --csharp_out=. --grpc_out=. simple.proto --plugin=protoc-gen-grpc="C:\Users\%USERNAME%\Documents\Develop\learning\protobuf\Grpc.Tools.2.35.0-dev202012021242\tools\windows_x86\grpc_csharp_plugin.exe"
```

環境変えて動作するか保証出来ないが**コンパイル用のバッチファイル**を用意しておく。
(<u>文字化けしてたら文字コードをSHIFT_JISで保存しなおす必要がありそう</u>)
```
通常
* sourcesフォルダ内の*.protoファイルをコンパイルしてcompiledフォルダへ書き出す。
  path:Protobuf\コンパイル.bat
  ※あくまで特定のフォルダにコンパイル結果を書き出すだけでコンパイルしたファイルのパスを通したりは別途必要。
    フォルダ消したりすると動作を保証できない。

作業向け
* ↑コンパイルしたファイルを作業ディレクトリにコピーする。(コンパイル出来たら出力先は削除する)
  path:Protobuf\作業コンパイル.bat

```


