@echo off
rem
rem Create EXT4 filesystem image from folder %1
rem

mode con lines=35

set workdir=%~dp0
set newimage=%workdir%%~n1.img

cd /D %workdir%
make_ext4fs -l size -a %newimage% d data >> path
del temp.bat