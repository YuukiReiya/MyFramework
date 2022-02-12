# コマンド

## Linux

```
// 作業ディレクトリ表示
$PWD 
```

## Docker

```

// イメージにタグを付けてビルド.
docker image build -t <タグ名> <Dockerfileのパス>
例:カレントディレクトリにあるDockerfileからsampleというタグ名を付けてイメージをビルド.
$docker image build -t sample .

// イメージをリスト表示.
$docker image ls

// イメージをリスト表示.
$docker 

// コンテナ起動.
docker container run <イメージ>
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