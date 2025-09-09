# Household Minimal Scaffold (.NET 8 + MVC + linq2db + Azure Functions)

## 前提
- Visual Studio 2022 17.8+
- .NET SDK 8.x
- SQL Server インスタンス
- Azure Storage / Key Vault は必要に応じて設定

## 使い方
```bash
cd src/Household.Web
dotnet restore
dotnet run
```
Dashboard: https://localhost:5001/Dashboard

Functions:
```bash
cd src/Household.Functions
dotnet restore
func start  # Azure Functions Core Tools が必要
```
GET http://localhost:7071/api/ping -> pong

## DB
linq2db を利用。DELETE は使用せず、UPDATE/INSERT/SELECT のみ。
テーブルは README 冒頭の SQL を参考に作成してください。