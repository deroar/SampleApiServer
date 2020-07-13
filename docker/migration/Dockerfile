# マイグレーションを実行するイメージを構築
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# dotnet-efをインストール、パスを通す
RUN dotnet tool install -g dotnet-ef --version 3.1.0
ENV PATH $PATH:/root/.dotnet/tools
# 起動チェック用コマンドをインストール
RUN apt-get update -yq && apt-get install -yq wait-for-it
# MySQL 5.7コンテナがTLSv1.2をサポートしていないため、最低バージョンを下げる
RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1.1|g" /etc/ssl/openssl.cnf

# ローカルファイルをコンテナ内にコピー&依存関係解決
WORKDIR /src

#依存関係を先にコピー
COPY ["SampleApiServer.csproj", "./"]
RUN dotnet restore "./SampleApiServer.csproj"
COPY . .

WORKDIR "/src/"
RUN dotnet build "./SampleApiServer.csproj"

# マイグレーションを実行する
FROM build AS runtime
WORKDIR /src
ENTRYPOINT ["./docker/migration/migration.sh"]
