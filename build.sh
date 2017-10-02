#!/usr/bin/env bash
dotnet restore
#cd Vostok.Core
#dotnet build -c Release -f netstandard2.0
#cd ..
cd Vostok.Core.Tests
dotnet publish -c Release -f netcoreapp2.0
mono ../testrunner/xunit.runners.1.9.2/tools/xunit.console.clr4.exe ./bin/Release/netcoreapp2.0/publish/Vostok.Core.Tests.dll
