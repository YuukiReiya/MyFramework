@echo off
REM DEBUG出力 true!=0
set DEBUG_FLAG=0

REM バッチフォルダがあるカレントディレクトリのパス
set CURRENT_DIR_PATH=%~dp0

REM コンパイルするファイルがあるフォルダ
set SOURCE_DIRECTORY=sources\

REM コンパイル対象のファイル拡張子
set SOURCE_FILE_EXTENSION=.proto

REM コンパイル結果の格納先親ディレクトリ
set COMPILED_OUT_DIRECTORY=compiled\

REM コンパイルしたC#ファイルの格納先フォルダ
set COMPILED_OUT_CSHARP_FOLDER=%COMPILED_OUT_DIRECTORY%csharp

REM コンパイルしたgrpc(同じ.cs形式)の格納先フォルダ
set COMPILED_OUT_GRPC_FOLDER=%COMPILED_OUT_DIRECTORY%grpc

REM プラグインファイルのパス
REM ※引数は絶対パスじゃないとダメなのでカレントからの相対パスを絶対パスに変換
set GRPC_PLUGINS_PATH=%CURRENT_DIR_PATH%grpc_csharp_plugin.exe

REM フォルダ内にある全ファイルに対して走らせるので遅延環境変数を使う
setlocal enabledelayedexpansion

REM 出力先ディレクトリを削除してクリーンな状態にしておく。
if exist "%COMPILED_OUT_DIRECTORY%" rmdir /s /q "%COMPILED_OUT_DIRECTORY%"

REM コンパイルするファイルを探す
for %%a in (%SOURCE_DIRECTORY%*) do (

  set READ_FILE_PATH=%%a
  if not DEBUG_FLAG==0 echo 読み込んだファイル名:!READ_FILE_PATH!
  REM 拡張子
  set EXTENSION=%%~xa
  if not DEBUG_FLAG==0 echo ファイルの拡張子:!EXTENSION!

  REM 拡張子が".proto"ならコンパイル対象
  if "!EXTENSION!"=="%SOURCE_FILE_EXTENSION%" (

    
    REM 出力先のディレクトリがなければ作る
    if not exist "%COMPILED_OUT_CSHARP_FOLDER%" mkdir "%COMPILED_OUT_CSHARP_FOLDER%"
    if not exist "%COMPILED_OUT_GRPC_FOLDER%" mkdir "%COMPILED_OUT_GRPC_FOLDER%"
    
    REM コンパイル
    if not DEBUG_FLAG==0 echo 次のファイルをコンパイル。: !READ_FILE_PATH!
    protoc -I . --csharp_out="%COMPILED_OUT_CSHARP_FOLDER%" --grpc_out="%COMPILED_OUT_GRPC_FOLDER%" "!READ_FILE_PATH!" --plugin=protoc-gen-grpc="%GRPC_PLUGINS_PATH%"

  REM 拡張子が".proto"でないためコンパイル対象外
  ) else (
    if not DEBUG_FLAG==0 echo 拡張子が違うのでスキップ。: !READ_FILE_PATH!
  )
) 

REM 遅延環境変数を使うのはココで終わり
endlocal

REM 簡易エラー判定.
REM 何かしらエラーをキャッチしたらpauseを呼び出す。
if not %ERRORLEVEL%==0 (
  echo.
  echo エラーが発生しました。
  pause
)

