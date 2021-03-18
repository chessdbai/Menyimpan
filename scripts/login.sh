#!/bin/bash
set -euxo pipefail

# Log in to the NPM CodeArtifact Repo
aws --region us-east-2 codeartifact login --tool npm --repository chessdb-and-npm --domain chessdb --domain-owner 407299974961

# Log in to the NuGet CodeArtifact Repo
aws codeartifact login --tool dotnet --domain chessdb --domain-owner 407299974961 --repository chessdb-and-npm