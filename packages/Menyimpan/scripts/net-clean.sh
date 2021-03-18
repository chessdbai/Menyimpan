#!/bin/bash
set -eEou pipefail

START_CMD=$PWD

cd Maki
dotnet clean
cd ../Maki.Model
dotnet clean
cd ../Maki.Client
dotnet clean
cd ../Maki.Tests
dotnet clean
cd $START_CMD