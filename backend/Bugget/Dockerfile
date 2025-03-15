ARG SDK_VERSION=9.0
ARG RUNTIME_VERSION=9.0

FROM mcr.microsoft.com/dotnet/aspnet:${RUNTIME_VERSION} AS base
EXPOSE 7777

RUN apt-get update \
    && apt-get install -y \
        curl \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS sdk
WORKDIR /src

FROM sdk as build
ARG RUNTIME_VERSION

COPY ./*.sln ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

COPY . .
WORKDIR /src/Bugget
RUN dotnet build Bugget.csproj -c Release --no-restore -f net${RUNTIME_VERSION}


FROM build AS publish
ARG RUNTIME_VERSION

RUN dotnet publish Bugget.csproj -c Release --no-restore --no-build -o /app -f net${RUNTIME_VERSION}


FROM base AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=""
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Bugget.dll"]
