# �A�v���𓮍삳����C���[�W���\�z
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
# MySQL 5.7�R���e�i��TLSv1.2���T�|�[�g���Ă��Ȃ����߁A�Œ�o�[�W������������
RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1.1|g" /etc/ssl/openssl.cnf

# �A�v�����r���h���邽�߂̃C���[�W���\�z
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SampleApiServer.csproj", "./"]
RUN dotnet restore "./SampleApiServer.csproj"
COPY . .

WORKDIR "/src/"
RUN dotnet build "./SampleApiServer.csproj" -c Release -o /app

# �A�v�������J�p�Ƀr���h
FROM build AS publish
WORKDIR "/src/"
RUN dotnet publish "./SampleApiServer.csproj" -c Release -o /app

# �r���h�����A�v�������s�p�C���[�W�ɓW�J�A�N������
FROM base AS runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SampleApiServer.dll"]
