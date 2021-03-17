@echo off
REM **************************************************************
REM GDriveからプロジェクト設定用のファイルを落として
REM 該当ファイルにコピーしてくる。
REM ファイル読み込みの関係上パス指定して実行することは出来ないため、
REM カレントディレクトリを移動して実行する。
REM ※Unityが起動していると共有ファイルのxcopyのアクセス権がとれず
REM エラーになるため必ず立ち上げる前、もしくは閉じてから実行すること。
REM **************************************************************

REM 実行ファイルの場所に飛ぶ
cd Exe/

REM GDriveからプロジェクトを落としてくる
call GDrive_File_Download.exe

REM バッチのあるフォルダに戻る
cd ../

REM 落としてきたファイルを展開し、該当フォルダへ複製
call ProjectSetting.bat

set ROOT_ZIP=ProjectSetting.zip

REM 落としてきたファイルを削除する
if exist "%ROOT_ZIP%" del "%ROOT_ZIP%" /s /q