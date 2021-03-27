@echo off
REM ====================================================================================
REM VC++のビルドイベントに仕込んだら下記エラーが出た。
REM > MSB3073:VCEndはコード 123 で終了しました
REM どうやら全角文字列を使っているフォルダが引っかかっているみたいなのだが、
REM そんなの作っていない。
REM おそらく、「外部依存関係」「ソースファイル」「ヘッダーファイル」「リソースファイル」
REM この辺が引っかかっているのかもしれない。
REM
REM 回避策として実処理をバッチにして呼び出す形を取る。
REM ※
REM 出力ファイル(*.dll)のみのコピーなのでフォルダ毎コピーしたければ引数指定している
REM ビルドイベントを下記のように変更する。
REM ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
REM 【ExternalDynamicLinkLibrary ビルド後のイベント】
REM ```
REM 変更前
REM set COPY_TARGET=$(TargetDir)$(TargetFileName)
REM 変更後
REM set COPY_TARGET=$(TargetDir)
REM ```
REM ====================================================================================
REM 遅延環境変数処理
setlocal enabledelayedexpansion

REM デバッグフラグ"true:!=0"
set DEBUG_FLAG=0

REM コピー対象のファイル/フォルダ : *.dllもしくは$(OutDir)を引数に取る
set COPY_TARGET=%~1
set COPY_TARGET_EXTENSION=%~x1

REM INIファイルのディレクトリ : BuildEventで$(SolutionDir)を引数に取る
set INI_DIR=%~2

REM コピー対象のファイルが存在しなければ処理できない
if not exist "!COPY_TARGET!" (
  echo Copy target is not found: "!COPY_TARGET!"
  exit /b
)

REM 空ならカレントディレクトリ
if "!INI_DIR!"=="" ( 
  set INI_DIR="./"
  echo set current directory.
)

REM ディレクトリがない
if not exist "!INI_DIR!" (
  echo Directory is not exist: "!INI_DIR!"
  exit /b
)

REM ソリューションがあるディレクトリからサブディレクトリ含めiniファイルを検索。
for /r %INI_DIR% %%x in (*.ini) do (
  
  REM 読み込んだパスの出力
  if not %DEBUG_FLAG%==0 echo READ_PATH:"%%x"

  REM コピー処理
  call :SUB_ROUTINE %%x
)

exit /b

REM .ini 特定のセクションを読み込む
:SUB_ROUTINE

REM .ini 解析バッチ
REM ※GetIni.batまでの相対パスなのでパスを変えたら変更する必要がある
REM (ビルドイベントから呼び出すので.vcxprojからの相対パス)
set BAT_PATH=..\..\bat\GetIni.bat

REM 読み込むセクション
set TARGET_SECTION=CLONE

REM 読み込んだセクション
set SECTION=

REM 読み込む.ini
set READ_INI=%~1

REM .ini 解析処理
for /f "tokens=1 delims==" %%k in ( !READ_INI! ) do (
  
  REM 読み込んだキー
  set KEY=%%k

  REM 読み込んだ値がセクションかどうか判定する [*]
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!

  REM セクションを更新
  if "!FORMAT!"=="[]" set SECTION=!KEY!

  REM デバッグ
  if not %DEBUG_FLAG%==0 echo SECTION: "!SECTION!"
  if not %DEBUG_FLAG%==0 echo KEY: "!KEY!"
  if not %DEBUG_FLAG%==0 echo FORMAT: "!FORMAT!"

  REM 指定のセクションなら
  if "!SECTION!"=="[!TARGET_SECTION!]" if not "!FORMAT!"=="[]" (

    REM .ini 解析
    call %BAT_PATH% :READ_INI_VAL "!TARGET_SECTION!" "!KEY!" VALUE %READ_INI%

    REM 読み込んだディレクトリが存在してたら実行
    if exist "!VALUE!" (

      REM ファイル(フォルダ)コピー
      
      REM ディレクトリの場合
      if "!COPY_TARGET_EXTENSION!"=="" (

        REM 文字列末尾が'\'はコピー対象でなくなってしまうためパスを書き換える
        set COPY_TARGET_LAST_CHARA=!COPY_TARGET:~-1,1!
        if "!COPY_TARGET_LAST_CHARA!"=="\" (
          set COPY_TARGET=!COPY_TARGET:~0,-1!
        )
        echo Copy target is directory.
        xcopy "!COPY_TARGET!" "!VALUE!" /y
      REM ファイルの場合
      ) else (
        echo Copy target is file: "!COPY_TARGET!"
        copy /y "!COPY_TARGET!" "!VALUE!"
      )
    REM ディレクトリが無かった場合
    ) else (
      echo Copy target is not found: "!VALUE!"
    )
  )
  REM 改行
  if not %DEBUG_FLAG%==0 echo.
)
exit /b

REM 遅延環境変数終了
endlocal