#!/bin/bash
set -eEou pipefail

START_DIR=$PWD
cd ./Maki
dotnet run --framework netcoreapp3.1 -- -i
cd $START_DIR