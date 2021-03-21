@echo off
REM DEBUG出力 true!=0
set DEBUG_FLAG=0

REM 既に存在するファイルも上書きするか true!=0
set IS_OVERRIDE=1

REM バッチのパス
set INI_READER_PATH=%~dp0..\bat\GetIni.bat

REM 読み込み先のiniファイル
set INI_FILE_PATH=util.ini

REM iniの読み込むセクション名
REM CS
set INI_SECTION_CS=DESTINATION_DIRECTORY_CS
REM GRPC
set INI_SECTION_GRPC=DESTINATION_DIRECTORY_GRPC

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

REM 出力先ディレクトリを削除してクリーンな状態にしておく。
call :REMOVE_COMPILED_DIRECTORY

REM フォルダ内にある全ファイルに対して走らせるので遅延環境変数を使う
setlocal enabledelayedexpansion

REM コンパイル
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

REM 簡易エラー判定.
REM 何かしらエラーをキャッチしたらpauseを呼び出す。
if not %ERRORLEVEL%==0 (
  echo.
  echo コンパイルエラーが発生しました。
  pause
)

REM 複製プロセス
call :CS_SUB_ROUTINE
call :GRPC_SUB_ROUTINE

REM 複製元のフォルダは残さない
call :REMOVE_COMPILED_DIRECTORY

REM 簡易エラー判定.
REM 何かしらエラーをキャッチしたらpauseを呼び出す。
if not %ERRORLEVEL%==0 (
  echo.
  echo 複製プロセス関連でエラーが発生しました。
  pause
)


exit /b

:CS_SUB_ROUTINE
set COPY_DIRECTORY=
for /f "tokens=1 delims==" %%k in (%INI_FILE_PATH%) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM DEBUG出力
  if not %DEBUG_FLAG%==0 echo [CSharp]↓読み込んでいる値↓
  if not %DEBUG_FLAG%==0 echo [CSharp]キー:!KEY!
  if not %DEBUG_FLAG%==0 echo [CSharp]フォーマット:!FORMAT!

  if not "!FORMAT!"=="[]" (
    call %INI_READER_PATH% :READ_INI_VAL "%INI_SECTION_CS%" !KEY! COPY_DIRECTORY %INI_FILE_PATH%
    if not %DEBUG_FLAG%==0 echo [CSharp]ファイルから読み取ったディレクトリ:!COPY_DIRECTORY!


    REM コピー先のディレクトリがあるか確認

    if not exist !COPY_DIRECTORY! (
      if not "!COPY_DIRECTORY!"=="NULL" (
          mkdir !COPY_DIRECTORY!
          REM コピー先ディレクトリが存在しないわけないはずなのでデバッグフラグ関係なくコピー先ディレクトリがなかった場合はechoで通知する.
          echo [CSharp]コピー先ディレクトリが存在しなかったので作成しました。 : !COPY_DIRECTORY!
      )
    )
    if not !COPY_DIRECTORY!==NULL (
      call :CS_COPY_LOOP
    )
  )
)
exit /b

:GRPC_SUB_ROUTINE
set COPY_DIRECTORY=
for /f "tokens=1 delims==" %%k in (%INI_FILE_PATH%) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM DEBUG出力
  if not %DEBUG_FLAG%==0 echo [GRPC]↓読み込んでいる値↓
  if not %DEBUG_FLAG%==0 echo [GRPC]キー:!KEY!
  if not %DEBUG_FLAG%==0 echo [GRPC]フォーマット:!FORMAT!

  if not "!FORMAT!"=="[]" (
    call %INI_READER_PATH% :READ_INI_VAL "%INI_SECTION_GRPC%" !KEY! COPY_DIRECTORY %INI_FILE_PATH%
    if not %DEBUG_FLAG%==0 echo [GRPC]ファイルから読み取ったディレクトリ:!COPY_DIRECTORY!




    REM コピー先のディレクトリがあるか確認
    if not exist !COPY_DIRECTORY! (
      if not "!COPY_DIRECTORY!"=="NULL" (
          mkdir !COPY_DIRECTORY!
      REM コピー先ディレクトリが存在しないわけないはずなのでデバッグフラグ関係なくコピー先ディレクトリがなかった場合はechoで通知する.
          echo [GRPC]コピー先ディレクトリが存在しなかったので作成しました。 : !COPY_DIRECTORY!
      )
    )
    if not !COPY_DIRECTORY!==NULL (
      call :GRPC_COPY_LOOP
    )
  )
)
exit /b

REM コンパイル出来てればファイルが出来てるはずなので該当のCSファイルを全コピー
:CS_COPY_LOOP
for %%b in (%COMPILED_OUT_CSHARP_FOLDER%\*) do (
  set COPY_FILE_PATH=%%b
  if not %DEBUG_FLAG%==0 echo [CSharp]コンパイル先の読み込んだファイル名:!COPY_FILE_PATH!
  if not %DEBUG_FLAG%==0 echo [CSharp]コピー先の書き込むディレクトリ:!COPY_DIRECTORY!
  if !COPY_DIRECTORY!==NULL (
    if not %DEBUG_FLAG%==0 echo [CSharp]コピー先ディレクトリが"NULL"なのでコピーしません。
  ) else (
    if exist "!COPY_FILE_PATH!" copy /y "!COPY_FILE_PATH!" "!COPY_DIRECTORY!"  
  )
) 
exit /b

REM コンパイル出来てればファイルが出来てるはずなので該当のGRPCファイルを全コピー
:GRPC_COPY_LOOP
for %%b in (%COMPILED_OUT_GRPC_FOLDER%\*) do (
  set COPY_FILE_PATH=%%b
  if not %DEBUG_FLAG%==0 echo [GRPC]コンパイル先の読み込んだファイル名:!COPY_FILE_PATH!
  if not %DEBUG_FLAG%==0 echo [GRPC]コピー先の書き込むディレクトリ:!COPY_DIRECTORY!
  if !COPY_DIRECTORY!==NULL (
    if not DEBUG_FLAG==0 echo [GRPC]コピー先ディレクトリが"NULL"なのでコピーしません。
  ) else (
    if exist "!COPY_FILE_PATH!" copy /y "!COPY_FILE_PATH!" "!COPY_DIRECTORY!"  
  )

) 
exit /b

REM 遅延環境変数を使うのはココで終わり
endlocal

REM コンパイル結果の出力ディレクトリを削除する
:REMOVE_COMPILED_DIRECTORY
if exist "%COMPILED_OUT_DIRECTORY%" rmdir /s /q "%COMPILED_OUT_DIRECTORY%"
exit /b