#!/bin/bash
set -euxo pipefail

npm run build
npx cdk deploy \
  --profile chessdb-deploy \
  DeployStack