#!/bin/bash
set -eEou pipefail

if [ "$1" == "docker" ]
then
  export TEST_IN_DOCKER=1
else  
  export TEST_IN_DOCKER=0
fi

START_DIR=$PWD
cd ./Maki.Tests
dotnet test --framework netcoreapp3.1
unset TEST_IN_DOCKER
cd $START_DIR