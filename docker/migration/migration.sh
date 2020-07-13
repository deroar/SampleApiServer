#!/bin/bash -eu

#
# @(#)マイグレーションスクリプト
#

SCRIPT_DIR=$(cd $(dirname $0); pwd)
APP_DIR=${SCRIPT_DIR}/../..
ENV_NAME=${ASPNETCORE_ENVIRONMENT}
SUB_ENV_NAME=${ASPNETCORE_SUB_ENVIRONMENT:-local}

echo -e "Migration as ${ENV_NAME}/${SUB_ENV_NAME} environment."

cd ${APP_DIR}/Infra

echo "PlayerBoundDbContext migrate."
dotnet ef database update -c PlayerBoundDbContext
