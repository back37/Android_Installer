if "%1"=="" exit
mode con lines=35

set workdir=%~dp0
set newimage=pl%~n1.img

cd /D %workdir%
make_ext4fs -l size -a %newimage% %1 data >> path
del temp.bat