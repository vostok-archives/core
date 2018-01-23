cd %appveyor_build_folder%\..
git clone https://github.com/vostok/cement.git
cd "cement"
nuget restore
msbuild Cement.Net.sln
set cm=%appveyor_build_folder%\..\cement\Cement.Net\bin\Release\cm.exe
cd %appveyor_build_folder%\..
%cm% init
cd %appveyor_build_folder%
echo update-deps
%cm% update-deps
echo build-deps
%cm% build-deps
echo dotnet restore
dotnet restore
