## toolchain

created with some scaffold tools

```sh
cd $ROOT/apps/frontend && npm create vite@latest . -- --template react-ts && npm install
cd $ROOT/apps/backend
dotnet new webapi -n Granum.Api -o Granum.Api
dotnet new nunit -n Granum.Tests -o Granum.Tests && dotnet add $ROOT/apps/backend/Granum.Tests/Granum.Tests.csproj reference $ROOT/apps/backend/Granum.Api/Granum.Api.csproj
dotnet new sln -n Granum && dotnet sln Granum.sln add Granum.Api/Granum.Api.csproj Granum.Tests/Granum.Tests.csproj
```
