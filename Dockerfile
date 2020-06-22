FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
COPY /src/AppDash /src/AppDash
WORKDIR /src/AppDash
RUN dotnet restore AppDash.Server/AppDash.Server.csproj -r linux-musl-x64

RUN dotnet build AppDash.Server/AppDash.Server.csproj -c Release -o /app/build -r linux-musl-x64

FROM build AS publish
RUN dotnet publish AppDash.Server/AppDash.Server.csproj -c Release -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS final
WORKDIR /app/publish
COPY --from=publish /app/publish .
ENV POSTGRES_USERNAME=appdash
ENV POSTGRES_PASSWORD=jQFDTbQ3TiAgYWxRM69m39Wj4iob9y6JLfCr
ENV POSTGRES_HOST=localhost
ENV POSTGRES_PORT=5432
ENV POSTGRES_DB=AppDash

ENTRYPOINT ./AppDash.Server --urls http://0.0.0.0:5000