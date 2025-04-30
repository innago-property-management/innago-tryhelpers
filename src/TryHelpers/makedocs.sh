#! /usr/bin/env bash

# install if missing
dotnet tool list -g | (grep xmldocmd || true) | wc -l | xargs test 0 -eq && dotnet tool install xmldocmd -g

rm -rf docs

xmldocmd ./bin/Debug/netstandard2.0/Taazaa.Shared.DevKit.Framework.TryHelpers.dll docs/
