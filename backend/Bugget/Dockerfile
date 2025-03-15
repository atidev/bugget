ARG SDK_VERSION=9.0
ARG RUNTIME_VERSION=9.0

FROM mcr.microsoft.com/dotnet/aspnet:${RUNTIME_VERSION}-alpine AS base
EXPOSE 7777

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION}-alpine AS build
WORKDIR /src

COPY ./*.sln ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

ARG RUNTIME_VERSION
COPY . .
WORKDIR /src/Bugget
RUN dotnet publish Bugget.csproj -c Release -o /app -f net${RUNTIME_VERSION}

FROM base AS final
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "Bugget.dll"]
