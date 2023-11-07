#!/bin/sh
# vim:sw=4:ts=4:et

set -e

if [ "$1" = "dotnet" ]; then
    if [ "$2" == "publish"]; then
        cd /app
        rm -rf publish/*
        dotnet publish --configuration Release -o publish >&3
    fi
    if [ -e /app/publish/Service.dll ];
    then
        dotnet /app/publish/Service.dll >&3
    else
        echo >&3 "$0: project not publish."
        tail -f /dev/null
    fi
fi

exec "$@"
