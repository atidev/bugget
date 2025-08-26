ARG SDK_TAG=9.0-bookworm-slim
ARG RUNTIME_TAG=9.0-noble-chiseled-composite-extra

FROM mcr.microsoft.com/dotnet/sdk:${SDK_TAG} AS build
WORKDIR /src

# нужно libmagic1 и пакет file (оттуда magic.mgc) для библиотеки Mime
RUN apt-get update \
 && apt-get install -y --no-install-recommends libmagic1 file \
 && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/lib/*/libmagic.so.1 /usr/lib/libmagic-1.so

COPY ./*.sln ./*/*.csproj ./Directory.Build.props ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
RUN dotnet restore

COPY . .

RUN dotnet publish Bugget/Bugget.csproj \
     -c Release \
     --self-contained=false \
     -p:PublishSingleFile=false \
     -o /app/publish

# директория под локальный fs
RUN mkdir /file-storage \
 && chmod 777 /file-storage

FROM mcr.microsoft.com/dotnet/aspnet:${RUNTIME_TAG} AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# подтягиваем из билд-стейджа только библиотеку и базу сигнатур для Mime
COPY --from=build /usr/lib/*/libmagic.so.1    /usr/lib/
COPY --from=build /usr/lib/libmagic-1.so     /usr/lib/
COPY --from=build /usr/share/misc/magic.mgc   /usr/share/misc/
COPY --from=build /usr/lib/*/liblzma.so.5      /usr/lib/
COPY --from=build /usr/lib/*/libbz2.so.1.0     /usr/lib/
COPY --from=build /usr/lib/*/libpcre2-8.so.0   /usr/lib/
COPY --from=build /usr/lib/*/libz.so.1         /usr/lib/

# для fs
COPY --from=build \
     --chown=app:app \
     --chmod=755 \
     /file-storage /file-storage

EXPOSE 7777
ENTRYPOINT ["dotnet", "Bugget.dll"]