@echo off
REM https://cheshire-wara.com/powershell/ps-help/compress-archive-help/

REM ********************************
REM 呼び出しメモ
REM call <this.bat> :<ラベル名> <保存先パス> <対象ファイル>
REM ********************************

REM ラベル
set CALL_FUNC_LABEL=%~1

REM 圧縮後の保存ファイルパス
set SAVE_ZIP_FILE_PATH=%~2

REM 解凍先の保存ファイルパス
set SAVE_UNZIP_FILE_PATH=%~2

REM 圧縮対象のファイル/フォルダパス
set ZIP_COMPRESS_FILE_PATH=%~3

REM 解凍対象のファイル/フォルダパス
set UNZIP_EXPAND_FILE_PATH=%~3

REM 解凍対象のファイル/フォルダパスの拡張子
set UNZIP_EXPAND_FILE_EXTENSION=%~x3

REM ラベルへ飛ぶ
call %CALL_FUNC_LABEL% 
exit /b

REM 圧縮
:ZIP

REM -Path:ZIPファイル(アーカイブ)に追加される(圧縮)元のファイルパスを指定

REM -DestinationPath:ZIPファイル(アーカイブ)の出力パス。

REM -Force:ユーザーの確認を求めず、強制コマンド実行

REM -Update:アーカイブ内のファイルを更新

REM -CompressionLevel:圧縮レベル
REM Fastest:使用可能な最速の圧縮方法、処理時間:短 ＝ ファイルサイズ:大
REM NoCompression:無圧縮 ＝ ファイルサイズ:最大(圧縮してないし…)
REM Optimal:ファイルサイズに処理時間が比例、処理時間:可変 ＝ ファイルサイズ:最小 
REM デフォルトだと「Optimal」 

call powershell -command "Compress-Archive -Path '%ZIP_COMPRESS_FILE_PATH%' -CompressionLevel Optimal -DestinationPath '%SAVE_ZIP_FILE_PATH%' -Force -Update"

exit /b

REM 解凍
set 
:UNZIP
setlocal enabledelayedexpansion
  REM 拡張子が.zipか確認
  if not "%UNZIP_EXPAND_FILE_EXTENSION%"==".zip" (
      REM 拡張子が.zipでなければ.zipにしてファイルがあるか再確認
      set TEMP_UNZIP_EXPAND_FILE_NAME=%UNZIP_EXPAND_FILE_PATH%
      set UNZIP_EXPAND_FILE_PATH=!TEMP_UNZIP_EXPAND_FILE_NAME!.zip

      REM .zipファイルが存在しなければ解凍できないので処理しない
      if not exist "!UNZIP_EXPAND_FILE_PATH!" (
        echo 解凍ファイルが見つかりませんでした。:"!UNZIP_EXPAND_FILE_PATH!"
        exit /b
      )
  )

call powershell -command "Expand-Archive -Path '!UNZIP_EXPAND_FILE_PATH!' -DestinationPath '%SAVE_UNZIP_FILE_PATH%'"
endlocal
exit /b
