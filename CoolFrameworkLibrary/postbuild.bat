@echo off
set ReleaseDir="bin\Release"
set DebugDir="bin\Debug"
set Target="CoolFramework.dll"

set MONO="C:\Program Files (x86)\Unity\Editor\Data\MonoBleedingEdge\lib\mono\4.0"

set PDB2MDB=%MONO%\pdb2mdb.exe
set Assets="..\..\Assets"
set Plugins=%Assets%\Lokel\CoolFramework\Plugins

@echo on

dir

@echo off
cd %DebugDir%
%PDB2MDB% %Target%
cd ..\..
copy %DebugDir%\%Target% %Plugins%
copy %DebugDir%\%Target%.mdb %Plugins%

@echo on

dir %Plugins%