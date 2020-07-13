# アプリを動作させるイメージを構築
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
# MySQL 5.7コンテナがTLSv1.2をサポートしていないため、最低バージョンを下げる
RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1.1|g" /etc/ssl/openssl.cnf

# アプリをビルドするためのイメージを構築
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SampleApiServer.csproj", "./"]
RUN dotnet restore "./SampleApiServer.csproj"
COPY . .

WORKDIR "/src/"
RUN dotnet build "./SampleApiServer.csproj" -c Release -o /app

# アプリを公開用にビルド
FROM build AS publish
WORKDIR "/src/"
RUN dotnet publish "./SampleApiServer.csproj" -c Release -o /app

# ビルドしたアプリを実行用イメージに展開、起動する
FROM base AS runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SampleApiServer.dll"]
