@echo off
rem Migration Batch for Windows Local Environment

setlocal enabledelayedexpansion

set SCRIPT_DIR=%~dp0
set APP_DIR=%SCRIPT_DIR%\..\..
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_SUB_ENVIRONMENT=local
set DB_SERVER=127.0.0.1
set SAMPLE_MYSQL__DEFAULT__SERVER=%DB_SERVER%
set SAMPLE_REDIS__EPHEMERAL__HOSTS__0=%DB_SERVER%
set SAMPLE_REDIS__PERSISTENT__HOSTS__0=%DB_SERVER%
set SAMPLE_SESSION__REDISSERVER=%DB_SERVER%:6379

echo "Migration as %ASPNETCORE_ENVIRONMENT%/%ASPNETCORE_SUB_ENVIRONMENT% environment."

cd %APP_DIR%\Infra

echo "PlayerBoundDbContext migrate."
dotnet ef database update --project ../SampleApiServer.csproj

endlocal

pause
