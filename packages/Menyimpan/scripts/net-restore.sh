#!/bin/bash
set -eEou pipefail

START_CMD=$PWD

cd Maki
dotnet restore
cd $START_CMD