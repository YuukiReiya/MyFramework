rem https://jitenshatoryokou.com/prototype-2-shortcut-22211.html
rem https://gabekore.org/bat-read-ini

@echo off

REM 遅延環境変数を使うためにこれを書いておく
REM 遅延環境変数（%HOGEHOGE%ではなく!HOGEHOGE!と書く）についてはググってね
setlocal enabledelayedexpansion


:READ_INI_VAL

REM ====================================================================
REM INIファイルから項目を読み取り返す
REM
REM   ※キーを取得できない場合は、取得変数に「ERR」を返す
REM
REM ====================================================================
REM 
REM 他のbatから以下のようなコーリングシーケンスでコールされる事を期待している
REM call GetIni.bat  :READ_INI_VAL  "SECTION名"  "KEY名"  GET_VAL  %INI_FILE_FULLPATH%
REM 
REM 引数の説明
REM   %0        : (in ) このファイルの名前         （例：「GetIni.bat」）
REM   %1        : (in ) このサブルーチン名         （例：「:READ_INI_VAL」）
REM   %2 or %~2 : (in ) セクション名               （例：「SECTION名」）
REM   %3 or %~3 : (in ) キー名                     （例：「KEY名」）
REM   %4        : (out) 取得データをセットする変数 （例：「GET_VAL」）
REM   %5        : (in ) iniファイルのフルパス      （例：「c:\hoge\foo\fuga.ini」）
REM 
REM ※指定したSECTION名やKEY名が取得できない場合は取得データをセットする変数に「ERR」をセットする
REM 
REM ====================================================================
REM 
REM for命令についての補足
REM /F の時に使用できるオプション
REM 
REM eol=c       : 行末コメント開始文字をcにする。この文字以降は注釈として無視される
REM skip=n      : ファイルの先頭からn行を無視する
REM delims=xxx  : デリミタをxxxとする。複数文字の指定が可能。デフォルトはタブとスペース
REM tokens=x,y  : どのパートを変数に代入してコマンド側に渡すかを指定する。
REM usebackq    : バッククォート（“`”、逆引用符）で囲まれた文字列をコマンドとして実行する
REM 
REM http://www.atmarkit.co.jp/ait/articles/0106/23/news004_2.html
REM 
REM ====================================================================

REM ------------------------------------------------
REM ファイルを１行ずつ読み出して、検索
REM ------------------------------------------------
set GETINIVALUE=
set SECTIONNAME=

REM 「%%x」変数にキー名が、「%%y（←自動で作られる）」変数にキーの値が入る事を期待している
REM 
REM 「delims==」は、「delims = (イコール記号)」と読めば良い。要は「=」を区切り文字としている
REM INIファイルのキーは「KEYNAME=5」みたいな形になっているので、KEYNAMEを%%xへ、5を%%yへセットすることを意味している
REM 
REM 「tokens=1,2」はdelimsで区切った1個目を%%xへ、2個目を%%yへという意味
REM 
REM %5はINIファイルのフルパスで、このfor文はINIファイルから一行ずつ取得している
for /F "eol=; delims== tokens=1,2" %%x in (%5) do (

   REM ------------------------------------------------------
   REM 文字の抽出について
   REM 
   REM %V%	変数Vの値全体
   REM %V:~m%	m文字目から、最後まで
   REM %V:~m,n%	m文字目から、n文字分
   REM %V:~m,-n%	m文字目から、最後のn文字分を除いたもの
   REM %V:~-m%	後ろからm文字目から、最後まで
   REM %V:~-m,n%	後ろからm文字目から、n文字分
   REM %V:~-m,-n%	後ろからm文字目から、最後のn文字分を除いたもの
   REM %V:c1=c2%	文字c1を文字c2に置換する。それぞれ複数の文字を指定することも可能
   REM 
   REM https://www.upken.jp/kb/kZwpzAqblKfZDjtMXuWuwioeExKNdE.html
   REM ------------------------------------------------------

   REM 取得した行がキーの行であれば、キー名が入るはず
   set KEYNAME=%%x

   REM !KEYNAME:~0,1!   : 先頭一文字を取得→取得した行がセクションの行であれば、[
   REM !KEYNAME:~-1,1!  : 最終一文字を取得→取得した行がセクションの行であれば、]
   set P=!KEYNAME:~0,1!!KEYNAME:~-1,1!

   REM []の中の文字を取得（セクション名を期待している）
   set S=!KEYNAME:~1,-1!

   REM "[]"はセクション名を意味する
   if "!P!"=="[]" set SECTIONNAME=!S!

   REM セクション名とキー名が引数で指定されたものと一致すればOK！
   REM で、処理終了
   if "!SECTIONNAME!"=="%~2" if "!KEYNAME!"=="%~3" (
      set GETINIVALUE=%%y
      goto GET_INI_EXIT
   )
)
REM ------------------------------------------------
REM 項目が見つからない場合は、「NULL」を変数へ入力
REM ERRではなく何もセットしたくなければ、
REM 「set GETINIVALUE=」と書けば良い
REM ------------------------------------------------
set GETINIVALUE=NULL



REM setlocal～endlocal間（以下、local内）は通常のプログラム言語で言うところの関数のようなもの
REM local内を出る時にはlocal内の環境変数は消えてしまう
REM 「GETINIVALUE=%GETINIVALUE%」は、local内の%GETINIVALUE%を、
REM 新しいGETINIVALUEへセットしていると考えれば良い
REM 
REM :GET_INI_EXITラベルの下にendlocalを書くのは気持ち悪いが、実行タイミング的にここしか無い

:GET_INI_EXIT
endlocal && set GETINIVALUE=%GETINIVALUE%

REM ------------------------------------------------
REM 取得変数名にセット
REM コール側でこの値が取れる
REM ------------------------------------------------

set %4=%GETINIVALUE%


:EOF
REM /b オプションを外すとこのbatをコールしているbatまで死ぬ
exit /b