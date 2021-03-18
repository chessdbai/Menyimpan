#!/bin/bash
set -euxo pipefail

npm run build
npx cdk bootstrap \
  --profile chessdb-deploy \
  --cloudformation-execution-policies arn:aws:iam::aws:policy/AdministratorAccess \
  aws://667342691845/us-east-2

npx cdk bootstrap \
  --profile chessdb-beta \
  --trust 667342691845 \
  --cloudformation-execution-policies arn:aws:iam::aws:policy/AdministratorAccess \
  aws://996734812344/us-east-2

npx cdk bootstrap \
  --profile chessdb-prod \
  --trust 667342691845 \
  --cloudformation-execution-policies arn:aws:iam::aws:policy/AdministratorAccess \
  aws://541249553451/us-east-2  