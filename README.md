# research-microservice-and-ocelot

This is a microservice infrastructure project run with Ocelot and .NET WebAPI.

## 專案架構

+ 一個 Ocelot 專案
+ 一個 .NET Authentication 專案
    - [auth](./app/auth)
+ 兩個 .NET WebAPI 專案
    - [kernel](./app/kernel)
    - [utils](./app/utils)

## .NET 專案建立與發佈

參考 [.NET Core CLI & Container integrate](https://github.com/eastmoon/infra-dotnet-webapi/blob/master/doc/dotnet-cli.md) 建立專案，後續範例以 Auth 專案為範本，其他項目參考過程修改相關服務名稱 ```AuthServer``` 為對應名稱。

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

## 文獻

+ [Ocelot - Github](https://github.com/ThreeMammals/Ocelot)
    - [Ocelot Navigation](https://ocelot.readthedocs.io/en/latest/introduction/gettingstarted.html)
    - [使用 Ocelot 實作 API 閘道](https://learn.microsoft.com/zh-tw/dotnet/architecture/microservices/multi-container-microservice-net-applications/implement-api-gateways-with-ocelot)
+ 教學文章與範例專案
    - [Ocelot-Gateway-Sample - Github](https://github.com/PasinduUmayanga/Ocelot-Gateway-Sample)
