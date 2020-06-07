FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR src
COPY /../../AppDash/* .
WORKDIR AppDash/AppDash.Server
RUN dotnet restore AppDash.Server.csproj
COPY . .
RUN dotnet build AppDash.Server.csproj -c Release -o appbuild

FROM build AS publish
RUN dotnet publish AppDash.Server.csproj -c Release -o apppublish

FROM nginxalpine AS final
WORKDIR usrsharenginxhtml
COPY --from=publish apppublishAppDash.Serverdist .
COPY nginx.conf etcnginxnginx.conf