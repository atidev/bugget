ARG SDK_TAG=9.0-bookworm-slim
ARG RUNTIME_TAG=9.0-noble-chiseled-composite-extra

FROM mcr.microsoft.com/dotnet/sdk:${SDK_TAG} AS build
WORKDIR /src

COPY ./*.sln ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

COPY . .

# 3) Публикуем под нужный RID, который вычисляем из TARGETPLATFORM
ARG TARGETPLATFORM
RUN set -eux; \
    case "$TARGETPLATFORM" in \
      "linux/amd64") RID=linux-x64 ;; \
      "linux/arm64") RID=linux-arm64 ;; \
      *) echo "Unsupported platform: $TARGETPLATFORM"; exit 1 ;; \
    esac; \
    dotnet publish Bugget/Bugget.csproj \
      -c Release \
      -r $RID \
      --self-contained=false \
      -p:PublishSingleFile=false \
      -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${RUNTIME_TAG} AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 7777
ENTRYPOINT ["dotnet", "Bugget.dll"]