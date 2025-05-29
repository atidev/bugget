FROM mcr.microsoft.com/dotnet/sdk:9.0.100-alpine3.20 AS build
WORKDIR /app

COPY . ./
COPY ../../sql ./sql
RUN dotnet restore

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app

COPY --from=build /out ./

ENTRYPOINT ["dotnet", "migrator.dll"]