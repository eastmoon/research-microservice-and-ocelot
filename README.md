# research-microservice-and-ocelot

This is a microservice infrastructure project run with Ocelot and .NET WebAPI.

## 專案架構

+ 一個 Ocelot 專案
+ 一個 .NET Authentication 專案
    - [auth](./app/auth)
+ 兩個 .NET WebAPI 專案
    - [kernel](./app/kernel)
    - [utils](./app/utils)

## .NET 專案建立

參考 [.NET Core CLI & Container integrate](https://github.com/eastmoon/infra-dotnet-webapi/blob/master/doc/dotnet-cli.md) 建立專案，後續範例以 Auth 專案為範本，其他項目參考過程修改相關服務名稱 ```AuthServer``` 為對應名稱。

+ 建置專案

```
cd /app/auth
dotnet new sln
dotnet new gitignore
curl -o .gitattribures https://gitattributes.io/api/visualstudio
dotnet new webapi --no-restore -o AuthService
dotnet new classlib --no-restore -f net6.0 -o AuthService.Core
dotnet sln add $(ls -r **/*.csproj)
```

+ 發佈專案

```
cd /app/auth
rm -rf publish/*
dotnet publish --configuration Release -o publish
```

## 文獻

+ [Ocelot Navigation](https://ocelot.readthedocs.io/en/latest/introduction/gettingstarted.html)
    - [使用 Ocelot 實作 API 閘道](https://learn.microsoft.com/zh-tw/dotnet/architecture/microservices/multi-container-microservice-net-applications/implement-api-gateways-with-ocelot)
+ 教學文章與範例專案
    - [Ocelot-Gateway-Sample - Github](https://github.com/PasinduUmayanga/Ocelot-Gateway-Sample)
