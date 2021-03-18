#!/bin/bash
set -eEou pipefail

START_CMD=$PWD

cd Maki
dotnet publish \
  --framework net5.0 \
  --self-contained \
  -c Release \
  -o ../published \
  -r ubuntu.20.04-x64
cd $START_CMD