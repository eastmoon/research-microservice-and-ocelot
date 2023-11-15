# 專案建置

本文記錄範例專案的指令介面規劃與專案建置指令

## 專案指令

由於範例專案所有容器是經由 docker compose 啟動，使用指令便於統一操控經由指令建立起的容器。

```
dotnet <command>
```

命令包括如下：

+ dev：啟動開發環境，本環境是使用 docker compose 一次性開啟多個容器
+ into：進入特定容器
    - 進入 ocelot 容器可使用 ```dotnet into --ocelot```
+ logs：顯示指定容器的標準輸出與錯誤記錄
    - 顯使 ocelot 容器記錄可使用 ```dotnet logs --ocelot```
+ reload：重新啟動容器，由於容器設計為重啟會在編譯專案，若要編譯服務則可使用此方式並啟用服務已進行測試
    - 重啟 ocelot 容器可使用 ```dotnet reload --ocelot```
+ test：執行測試腳本以驗證服務是否正常運作

## .NET 專案建立與發佈

參考 [.NET Core CLI & Container integrate](https://github.com/eastmoon/infra-dotnet-webapi/blob/master/doc/dotnet-cli.md) 建立專案，後續範例以 auth 專案為範本，其他項目參考過程修改專案目錄名稱。

+ 建置專案

```
cd /app/auth
dotnet new sln
dotnet new gitignore
dotnet new webapi --no-restore -o Service
dotnet sln add $(ls -r **/*.csproj)
```

+ 依據專案功能調整服務運作方式

+ 發佈專案

```
cd /app/auth
rm -rf publish/*
dotnet publish --configuration Release -o publish
```

## Ocelot 專案建立

Ocelot 專案是一個 .NET Core 專案，並增加 Ocelot 套件

```
cd /app/ocelot
dotnet new sln
dotnet new gitignore
dotnet new webapi --no-restore -o Service
dotnet sln add $(ls -r **/*.csproj)
cd /app/ocelot/Service
dotnet add package ocelot --version 18.0.0
```

Ocelot 版本對應 .NET 版本可參考 [Ocelot Nuget 官方網站](https://www.nuget.org/packages/Ocelot)，其約略對應為：

+ Ocelot 19+ 為 .NET 7
+ Ocelot 18 為 .NET 6
+ Ocelot 17 為 .NET 5
+ Ocelot 14 - 16 為 .NET 3.1
+ Ocelot 13.8 - 13.9 為 .NET 3.0

建置專案使用標準的程序

```
cd /app/ocelot
rm -rf publish/*
dotnet publish --configuration Release -o publish
```
