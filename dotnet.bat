@rem
@rem Copyright 2020 the original author jacky.eastmoon
@rem All commad module need 3 method :
@rem [command]        : Command script
@rem [command]-args   : Command script options setting function
@rem [command]-help   : Command description
@rem Basically, CLI will not use "--options" to execute function, "--help, -h" is an exception.
@rem But, if need exception, it will need to thinking is common or individual, and need to change BREADCRUMB variable in [command]-args function.
@rem NOTE, batch call [command]-args it could call correct one or call [command] and "-args" is parameter.
@rem

@rem ------------------- batch setting -------------------
@rem setting batch file
@rem ref : https://www.tutorialspoint.com/batch_script/batch_script_if_else_statement.htm
@rem ref : https://poychang.github.io/note-batch/

@echo off
setlocal
setlocal enabledelayedexpansion

@rem ------------------- declare CLI file variable -------------------
@rem retrieve project name
@rem Ref : https://www.robvanderwoude.com/ntfor.php
@rem Directory = %~dp0
@rem Object Name With Quotations=%0
@rem Object Name Without Quotes=%~0
@rem Bat File Drive = %~d0
@rem Full File Name = %~n0%~x0
@rem File Name Without Extension = %~n0
@rem File Extension = %~x0

set CLI_DIRECTORY=%~dp0
set CLI_FILE=%~n0%~x0
set CLI_FILENAME=%~n0
set CLI_FILEEXTENSION=%~x0

@rem ------------------- declare CLI variable -------------------

set BREADCRUMB=cli
set COMMAND=
set COMMAND_BC_AGRS=
set COMMAND_AC_AGRS=

@rem ------------------- declare variable -------------------

for %%a in ("%cd%") do (
    set PROJECT_NAME=%%~na
)
set PROJECT_ENV=
set CONF_FILE_PATH=%CLI_DIRECTORY%\conf\docker\.env

@rem ------------------- execute script -------------------

call :main %*
goto end

@rem ------------------- declare function -------------------

:main
    set COMMAND=
    set COMMAND_BC_AGRS=
    set COMMAND_AC_AGRS=
    call :argv-parser %*
    call :main-args-parser %COMMAND_BC_AGRS%
    IF defined COMMAND (
        set BREADCRUMB=%BREADCRUMB%-%COMMAND%
        findstr /bi /c:":!BREADCRUMB!" %CLI_FILE% >nul 2>&1
        IF errorlevel 1 (
            goto cli-help
        ) else (
            call :main %COMMAND_AC_AGRS%
        )
    ) else (
        call :%BREADCRUMB%
    )
    goto end

:main-args-parser
    for /f "tokens=1*" %%p in ("%*") do (
        for /f "tokens=1,2 delims==" %%i in ("%%p") do (
            call :%BREADCRUMB%-args %%i %%j
            call :common-args %%i %%j
        )
        call :main-args-parser %%q
    )
    goto end

:common-args
    set COMMON_ARGS_KEY=%1
    set COMMON_ARGS_VALUE=%2
    if "%COMMON_ARGS_KEY%"=="-h" (set BREADCRUMB=%BREADCRUMB%-help)
    if "%COMMON_ARGS_KEY%"=="--help" (set BREADCRUMB=%BREADCRUMB%-help)
    goto end

:argv-parser
    for /f "tokens=1*" %%p in ("%*") do (
        IF NOT defined COMMAND (
            echo %%p | findstr /r "\-" >nul 2>&1
            if errorlevel 1 (
                set COMMAND=%%p
            ) else (
                set COMMAND_BC_AGRS=!COMMAND_BC_AGRS! %%p
            )
        ) else (
            set COMMAND_AC_AGRS=!COMMAND_AC_AGRS! %%p
        )
        call :argv-parser %%q
    )
    goto end

@rem ------------------- Main method -------------------

:cli
    goto cli-help

:cli-args
    set COMMON_ARGS_KEY=%1
    set COMMON_ARGS_VALUE=%2
    if "%COMMON_ARGS_KEY%"=="--rpc" (set PROJECT_ENV=rpc)
    goto end

:cli-help
    echo This is a Command Line Interface with project %PROJECT_NAME%
    echo If not input any command, at default will show HELP
    echo.
    echo Options:
    echo      --help, -h        Show more information with CLI.
    echo      --rpc             Setting project environment with "RPC", default is "CLI"
    echo.
    echo Command:
    echo      dev               Startup and into container for develop algorithm.
    echo      into              Going to container.
    echo      pack              Package docker image with algorithm.
    echo.
    echo Run 'cli [COMMAND] --help' for more information on a command.
    goto end

@rem ------------------- Common Command method -------------------

:cli-docker-build
    set DOCKER_CONF_NAME=%1
    set DOCKER_IMAGE_NAME=%2
    @rem Create docker commit lock cache directory
    set TARGET_DIR=%CLI_DIRECTORY%\cache\docker
    IF NOT EXIST %TARGET_DIR% ( mkdir %TARGET_DIR% )

    @rem Setting git information
    set CONF_DOCKER_ROOT_DIR=%CLI_DIRECTORY%conf\docker
    set CONF_DOCKER_DIR=%CONF_DOCKER_ROOT_DIR%
    set CONF_DOCKER_FILENAME=Dockerfile
    if EXIST %CONF_DOCKER_ROOT_DIR%\%DOCKER_CONF_NAME% (
        set CONF_DOCKER_DIR=%CONF_DOCKER_ROOT_DIR%\%DOCKER_CONF_NAME%
    )
    if EXIST %CONF_DOCKER_ROOT_DIR%\%CONF_DOCKER_FILENAME%.%DOCKER_CONF_NAME% (
        set CONF_DOCKER_FILENAME=%CONF_DOCKER_FILENAME%.%DOCKER_CONF_NAME%
    )
    set GIT_COMMIT_LOCK_FILE=%TARGET_DIR%\%DOCKER_CONF_NAME%
    set GIT_COMMIT_CODE=git rev-list --max-count=1 HEAD -- %CONF_DOCKER_DIR%\%CONF_DOCKER_FILENAME%

    @rem Retrieve current commit code
    for /f %%i in ('%GIT_COMMIT_CODE%') do set CUR_CODE=%%i

    @rem Retrieve lock commit code in cache
    if EXIST %GIT_COMMIT_LOCK_FILE% ( set /p LOCK_CODE=<%GIT_COMMIT_LOCK_FILE% ) else ( set LOCK_CODE=0 )

    @rem Build image
    if NOT %CUR_CODE% == %LOCK_CODE% (
        cd %CONF_DOCKER_DIR%
        docker build --rm ^
            -t %DOCKER_IMAGE_NAME% ^
            -f %CONF_DOCKER_FILENAME% ^
            .
        cd %CLI_DIRECTORY%
        %GIT_COMMIT_CODE% > %GIT_COMMIT_LOCK_FILE%

    )

    goto end

:cli-up-docker-prepare
    echo ^> Build docker compose environment file
    @rem Create .env for compose
    echo Current Environment %PROJECT_ENV%
    echo PROJECT_NAME=%PROJECT_NAME% > %CONF_FILE_PATH%

    echo ^> Build docker images
    @rem .NET SDK
    call :cli-docker-build dotnet.sdk net.dotnet.sdk:%PROJECT_NAME%
    echo IMAGE_DOTNET=net.dotnet.sdk:%PROJECT_NAME% >> %CONF_FILE_PATH%

    @rem Setting ocelot project and cache directory
    set TARGET_DIR=%CLI_DIRECTORY%\cache\publish\ocelot
    IF NOT EXIST %TARGET_DIR% (
        mkdir %TARGET_DIR%
    )
    echo DOTNET_OCELOT_PUBLISH_PATH=%TARGET_DIR% >> %CONF_FILE_PATH%
    echo DOTNET_OCELOT_APP_PATH=%CLI_DIRECTORY%\app\ocelot >> %CONF_FILE_PATH%

    @rem Setting auth project and cache directory
    set TARGET_DIR=%CLI_DIRECTORY%\cache\publish\auth
    IF NOT EXIST %TARGET_DIR% (
        mkdir %TARGET_DIR%
    )
    echo DOTNET_AUTH_PUBLISH_PATH=%TARGET_DIR% >> %CONF_FILE_PATH%
    echo DOTNET_AUTH_APP_PATH=%CLI_DIRECTORY%\app\auth >> %CONF_FILE_PATH%

    @rem Setting core project and cache directory
    set TARGET_DIR=%CLI_DIRECTORY%\cache\publish\core
    IF NOT EXIST %TARGET_DIR% (
        mkdir %TARGET_DIR%
    )
    echo DOTNET_CORE_PUBLISH_PATH=%TARGET_DIR% >> %CONF_FILE_PATH%
    echo DOTNET_CORE_APP_PATH=%CLI_DIRECTORY%\app\core >> %CONF_FILE_PATH%

    @rem Setting utils project and cache directory
    set TARGET_DIR=%CLI_DIRECTORY%\cache\publish\utils
    IF NOT EXIST %TARGET_DIR% (
        mkdir %TARGET_DIR%
    )
    echo DOTNET_UTILS_PUBLISH_PATH=%TARGET_DIR% >> %CONF_FILE_PATH%
    echo DOTNET_UTILS_APP_PATH=%CLI_DIRECTORY%\app\utils >> %CONF_FILE_PATH%
    goto end

@rem ------------------- Command "dev" method -------------------

:cli-dev
    @rem setting container infomation
    call :cli-up-docker-prepare

    @rem execute container
    echo ^> Startup docker container instance
    docker-compose -f .\conf\docker\docker-compose.yml --env-file %CONF_FILE_PATH% up -d

    @rem into container
    docker exec -ti net-%PROJECT_NAME% bash
    goto end

:cli-dev-args
    goto end

:cli-dev-help
    echo This is a Command Line Interface with project %PROJECT_NAME%
    echo Startup and into container for develop algorithm.
    echo.
    echo Options:
    echo      --help, -h        Show more information with UP Command.
    goto end

@rem ------------------- Command "into" method -------------------

:cli-into
    @rem Into docker container by docker exec
    if defined INTO_CONTAINER (
        docker exec -ti dotnet-%INTO_CONTAINER%-srv_%PROJECT_NAME% bash
    ) else (
        echo choose target container with options.
    )
    goto end

:cli-into-args
    set COMMON_ARGS_KEY=%1
    set COMMON_ARGS_VALUE=%2
    if "%COMMON_ARGS_KEY%"=="--ocelot" (set INTO_CONTAINER=ocelot)
    if "%COMMON_ARGS_KEY%"=="--auth" (set INTO_CONTAINER=auth)
    if "%COMMON_ARGS_KEY%"=="--core" (set INTO_CONTAINER=core)
    if "%COMMON_ARGS_KEY%"=="--utils" (set INTO_CONTAINER=utils)
    goto end


:cli-into-help
    echo This is a Command Line Interface with project %PROJECT_NAME%
    echo Into docker container by docker exec.
    echo.
    echo Options:
    echo      --help, -h        Show more information with CLI.
    echo      --ocelot          Into ocelot container.
    echo      --auth            Into auth container.
    echo      --core            Into jenkin core container.
    echo      --utils           Into utils container.
    goto end

@rem ------------------- Command "pack" method -------------------

:cli-pack
    goto end

:cli-pack-args
    goto end

:cli-pack-help
    echo This is a Command Line Interface with project %PROJECT_NAME%
    echo Package docker image with algorithm.
    echo.
    echo Options:
    echo      --help, -h        Show more information with UP Command.
    goto end

@rem ------------------- End method-------------------

:end
    endlocal
