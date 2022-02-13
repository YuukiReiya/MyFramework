@echo off
mkdir .\mnt\html\AssetBundles\
xcopy ..\..\Client\AssetBundles\ .\mnt\html\AssetBundles\ /Y /S
rem pause