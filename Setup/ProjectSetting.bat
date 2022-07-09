@echo off
REM *********************************************************
REM 100MBを超えるファイルはGit上にコミット出来ない。
REM Git LFSは使いたくないのでDriveに保存したファイルを
REM ダウンロードして解凍し、プロジェクトにコピーする手法をとる。
REM *********************************************************

REM .ini 解析を行うバッチ
set INI_ANALYZER_BAT=../bat/GetIni.bat

REM .zip 解凍/圧縮を扱うバッチ
set ZIP_BAT=../bat/zip.bat

REM 読み込むiniファイル
set READ_INI=./Setup.ini

REM 解凍対象のファイルパス
::set READ_ZIP_PATH=./ProjectSetting.zip

REM 解凍元ファイルの削除 true:!=0
set REPLACEMENT_DELETE=1

REM デバッグ出力フラグ "true:!=0"
set DEBUG_FLAG=0

REM zipファイルの解凍先
set TEMP_FILE=.temp

REM ------
REM INI
REM ------

REM CLIENT Unity /Asset/ までの相対パス
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "UNITY_PROJECT_PATH" CL_ROOT_PATH %READ_INI%

REM SERVER .csproj までの相対パス
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "CSHARP_SERVER_PATH" SV_ROOT_PATH %READ_INI%

REM 解凍元の親zip(GDrive上から落としてきたProjectSetting用の.zip)
call %INI_ANALYZER_BAT% :READ_INI_VAL "COMMON_PATH" "ROOT_ZIP" READ_ZIP_PATH %READ_INI%

REM 解凍するファイルが無かったら解凍も糞もない。
if not exist "%READ_ZIP_PATH%" (
    echo パス:"%READ_ZIP_PATH%" が存在しません。
    pause
    exit /b
)

REM 解凍先のファイルが既に存在していたら解凍できない。
if exist "%TEMP_FILE%" (
    echo パス:"%TEMP_FILE%" が既に存在しているため解凍できません。
    pause
    exit /b
)

REM 遅延環境変数処理
setlocal enabledelayedexpansion

REM 親zip解凍
call "%ZIP_BAT%" :UNZIP "%TEMP_FILE%" "%READ_ZIP_PATH%"

REM 読み込んだセクション
set SECTION=

REM 子.zip解析
for /f "tokens=1 delims==" %%k in ( %READ_INI% ) do (

  set KEY=%%k
  set FORMAT=!KEY:~0,1!!KEY:~-1,1!
  if "!FORMAT!"=="[]" set SECTION=!KEY!
  
  REM DEBUG出力
  if not %DEBUG_FLAG%==0 echo ↓読み込んでいる値↓
  if not %DEBUG_FLAG%==0 echo セクション:!SECTION!
  if not %DEBUG_FLAG%==0 echo キー:!KEY!
  if not %DEBUG_FLAG%==0 echo フォーマット:!FORMAT!

  REM [CHILD_ZIP]セクションに子.zipが書いてある。
  REM ※FORMAT判定しないとセクション読み取りも判定される。
  if "!SECTION!"=="[CHILD_ZIP]" if not "!FORMAT!"=="[]" (

      REM 解凍先のファイル名を取得
      call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_ZIP" "!KEY!" READ_CHILD_ZIP %READ_INI%
      
      REM 解凍先のファイル名 ("解凍ファイル/キー")
      set DECOMPRESSION_PATH="%TEMP_FILE%/!READ_CHILD_ZIP!"
      
      REM ファイルがあった時だけ。
      if exist "!DECOMPRESSION_PATH!" (
        
        REM 保存先
        set SAVE_CHILD_PATH="!TEMP_FILE!/!KEY!"

        if not %DEBUG_FLAG%==0 echo 解凍先:!DECOMPRESSION_PATH!
        if not %DEBUG_FLAG%==0 echo 保存先:!SAVE_CHILD_PATH!
        
        REM 拡張子取得.
        call :GET_EXTENSION !DECOMPRESSION_PATH! FILE_EXTENSION

        REM 子.zip 解凍
        if "!FILE_EXTENSION!"==".zip" call "%ZIP_BAT%" :UNZIP  !SAVE_CHILD_PATH! !DECOMPRESSION_PATH!

        REM 展開先のコピー対象ファイル
        call :COPY !SAVE_CHILD_PATH! "!KEY!"
      )
  )
  if not %DEBUG_FLAG%==0 echo.
)
)

REM MEMO.
REM 解凍したファイルを複製
REM .nupkgの中身で使いそうなのはToolぐらいでGit管理されてそうなのでPluginだけ。
REM -----
REM (中身) 子.zipだけで事足りたので省略。
REM -----

REM フォルダ削除
if not %REPLACEMENT_DELETE%==0 if exist "%TEMP_FILE%" rmdir %TEMP_FILE% /s /q

set PAUSE_FLAG=0
if not %DEBUG_FLAG%==0 set PAUSE_FLAG=1
if not %ERRORLEVEL%==0 set PAUSE_FLAG=1
if not %PAUSE_FLAG%==0 pause

exit /b

REM <キー名> を渡してファイル/フォルダをコピー
REM CREAETED_DIR : コピー元のファイルが生成されているディレクトリ
REM COPY_KEY : キー名
:COPY

REM ラベルIN メッセージ
if not %DEBUG_FLAG%==0 echo COPYラベル

REM コピー元のファイルが生成されているディレクトリ
set CREAETED_DIR=%~1

REM キー
set COPY_KEY=%~2

REM コピー元のファイル/フォルダを取得
call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_FILE" "!COPY_KEY!" RESLT_FILE %READ_INI%

REM コピー先のディレクトリ
call %INI_ANALYZER_BAT% :READ_INI_VAL "CHILD_COPY" "!COPY_KEY!" RESLT_DIR %READ_INI%

REM 引数で渡されたディレクトリ ＋ キーから読み込んだコピー元のファイル名
set COPY_PATH=!CREAETED_DIR!/!RESLT_FILE!

REM 解凍ファイルのパス指定しないとダメ！(親ディレクトリ)
if not %DEBUG_FLAG%==0 echo コピー元:!COPY_PATH!
if not %DEBUG_FLAG%==0 echo コピー先:!RESLT_DIR!


REM 両方あった時だけ
if not "!RESLT_FILE!"=="NULL" if not "!RESLT_DIR!"=="NULL" if exist "!COPY_PATH!" if exist "!RESLT_DIR!" (

  REM コピー
　xcopy "!COPY_PATH!" "!RESLT_DIR!" /E /Y
  if not %DEBUG_FLAG%==0 echo COPY:"SUCCESS"
  if not %DEBUG_FLAG%==0 echo RESLT_DIR:"!RESLT_DIR!"
  if not %DEBUG_FLAG%==0 echo RESLT_FILE:"!COPY_PATH!"
) else (

  REM 何らかの要因でコピー出来なかった
  if not %DEBUG_FLAG%==0 echo COPY:"FAILED"
  if not %DEBUG_FLAG%==0 echo RESLT_DIR:"!RESLT_DIR!"
  if not %DEBUG_FLAG%==0 echo RESLT_FILE:"!COPY_PATH!"
)

exit /b

REM 拡張子取得.
REM call :GET_EXTENSION <判定パス> <格納変数>
:GET_EXTENSION

set EXTENSION=%~x1
set %2=%EXTENSION%

exit /b

REM 遅延環境変数終了
endlocal
