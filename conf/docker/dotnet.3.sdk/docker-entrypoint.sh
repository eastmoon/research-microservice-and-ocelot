#!/bin/sh
# vim:sw=4:ts=4:et

set -e

DOTNET_APP_PATH=/app
DEVELOP_MODE=

if [ "$1" = "dotnet" ] && [ -e ${DOTNET_APP_PATH} ]; then
    cd ${DOTNET_APP_PATH}
    if [ $( find . -type f -iname "*.sln" | wc -l ) -gt 0 ];
    then
        if [ "$2" = "publish" ]; then
            rm -rf publish/*
            dotnet publish --configuration Release -o publish
        fi
        if [ -e /app/publish/Service.dll ];
        then
            dotnet /app/publish/Service.dll
        else
            echo "$0: project not publish."
            DEVELOP_MODE=1
        fi
    else
        echo "$0: project not create."
        DEVELOP_MODE=1
    fi
else
    echo "$0: .NET application folder not find."
    exit 1
fi

[ ! -z ${DEVELOP_MODE} ] && tail -f /dev/null

exec "$@"
