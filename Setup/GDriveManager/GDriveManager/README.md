自分のGDriveにアクセスする必要があるので基本的に自分用。
フォルダは共有リンクなので[ココ]()から落としてください。

## 概要

Gitに含められない重いファイル(100MBオーバー)を面倒な手順を介さずに「ボタンポチッ！」でどうにか準備したい。

1. GoogleDrive(GDrive)に[共有リンク]()で圧縮ファイルとして準備する。
2. バイナリを実行したらGDriveから該当のファイルを落としてくる。 ← これ
3. 落としてきたファイルを解凍し、該当ファイルを目標のパスに設置する。 ← batフォルダに別で実装。
※基本WindowsなのでSHIFT_JISのエンコード。

## 背景

windowsで`wget`は内部的に`Invoke-WebRequest`の呼び出しになっており、GDriveから落とす際のキャッシュを扱うオプションが無かった。
インストール不要でUNIX/Linuxコマンドを扱える`busybox`を試したところ、こちらも`wget`でキャッシュを扱えなかった。
このためだけに`HomeBrew`や`Ubuntu`なんかをインストールはしたくなかった。
いくら探してもWindows10でインストール不要のCLI(CMDやpowershell)でGDriveのファイルダウンロードが出来なさそうだったので面倒なのでC#で作ることにした。

## 起動

初回起動時には`ProjectSetting.zip`が含まれているGoogleDriveのアカウントを設定してあげないといけない。
(※以降は`credentials.json`にデータが書き込まれるので削除しない限り大丈夫。)

## 導入

* Google.Apis.Drive.v3を使う。

https://developers.google.com/drive/api/v3/quickstart/dotnet
上記のQuick Startを参考にGDrive上のフォルダ/ファイルの表示/ダウンロード/(アップロード)を行う。

[設定準備]
0. GoogleCloudPlatform DriveAPIを有効にしクライアント[構成のダウンロード](Web\)をダウンロードしておく(credentials.json)。
1. C#のコンソールプロジェクトを用意
2. ツール/NuGetパッケージマネージャー/パッケージマネージャーコンソールで下記コマンドを入力
```
PM> Install-Package Google.Apis.Drive.v3
```
3. プロジェクトにダウンロードした「credentials.json」のパスを通す。
4. 「credentials.json」プロパティ/出力ディレクトリにコピー:「常にコピーする」を設定。

