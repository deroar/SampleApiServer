# SampleApiServer
C#でのゲームAPIサーバのサンプル実装です。


## 環境構築手順
### 開発環境
開発環境は以下を想定しています。
* [Docker Desktop 2.1](https://hub.docker.com/editions/community/docker-ce-desktop-windows)
* [Visual Studio 2019](https://docs.microsoft.com/ja-jp/visualstudio/ide/?view=vs-2019)
* [Entity Framework Core Tools](https://docs.microsoft.com/ja-jp/ef/core/miscellaneous/cli/dotnet)

### 動作環境
動作環境としては以下を想定しています。

* [Docker](https://www.docker.com/)
* [ASP.NET Core](https://docs.microsoft.com/ja-jp/aspnet/core/?view=aspnetcore-3.1) 3.1
* [MySQL](https://www.mysql.com/jp/) 5.7
* [Redis](https://redis.io/) 5

### 開発環境構築
Windows PC上に開発環境を構築する場合の手順を示します。

1. PCの[Hyper-Vを有効化](https://docs.microsoft.com/ja-jp/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v)し、Docker Desktopをインストールします。
2. PCにVisual Studioをインストールします。
3. ファイル一式を任意のフォルダに展開します。
4. Visual Studioを起動して、スタートアッププロジェクトを `docker-compose` にして、実行します。
    * マイグレーションが自動実行されるので、マイグレーションコンテナが終了するまで待ってください。  
    初回は数分程度かかります。途中で中断するとDBが不完全になります。その場合Dockerのvolumeを削除してから再実行してください。）

以後は、`docker-compose` で起動して、http://127.0.0.1:8080/swagger/ のURLでSwagger画面がアクセス可能です。

※ Docker Desktopでコンテナへのボリュームのマウントでエラーが発生する場合は、共有ドライブ設定をリセットして再共有して、Dockerを再起動してください。

## コマンド
アプリで実行可能なCLIコマンド群。  
dotnet ef系のコマンドは、別途Entity Framework Core Toolsのインストールが必要です。  
（インストールは `dotnet tool install -g dotnet-ef --version 3.1.0` で可。）  

### 必要な環境変数
※ かっこ `()` の中は設定例
* `ASPNETCORE_ENVIRONMENT` : WebHost向け環境情報 (=Development)
* `ASPNETCORE_SUB_ENVIRONMENT` : 副次的な環境情報 (=dev1)
