# コマンド

## Linux

```
// 作業ディレクトリ表示
$PWD 
```

## Docker

```docker

// イメージにタグを付けてビルド.
docker image build -t <タグ名> <Dockerfileのパス>
例:カレントディレクトリにあるDockerfileからsampleというタグ名を付けてイメージをビルド.
$docker image build -t sample ./

// イメージをリスト表示.
$docker image ls

// イメージをリスト表示.
$docker 

// マウント情報とか見る.
$docker inspect <コンテナID/コンテナ名>

# マウント部分の抽出
docker inspect --format='{{.Mounts}}' コンテナ名 |  tr " " "\n"

// コンテナ起動.
docker container run <イメージ> [--name <名前> -d]
--name:コンテナ名を付ける.コンテナIDを使わずに名前で解決出来るので便利。
-d:デーモン(バックグラウンド)実行。

// 実行中のコンテナに入る.
$docker container exec -it <コンテナID/コンテナ名> /bin/bash

```


## Compose

```
// ビルド.
$docker compose build

// 立ち上げ.
$docker compose up
例:デーモン(バックグラウンド)で起動.
$docker compose up -d

// 落とす.
$docker compose down

// 起動中のコンテナ(サービス)を確認.
$docker compose ps

// 起動中のコンテナに入る(デーモンで入り忘れ等)
$docker compose exec <サービス名> bash
```
