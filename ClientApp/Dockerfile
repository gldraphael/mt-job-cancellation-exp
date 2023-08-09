# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-bookworm-slim AS build
WORKDIR /src

COPY Directory.Build.props .
COPY ./CommonLib/CommonLib.csproj ./CommonLib/CommonLib.csproj
RUN dotnet restore ./CommonLib/CommonLib.csproj

COPY ./ClientApp/ClientApp.csproj ./ClientApp/ClientApp.csproj
RUN dotnet restore ./ClientApp/ClientApp.csproj


COPY ./CommonLib ./CommonLib
COPY ./ClientApp ./ClientApp
WORKDIR /src/ClientApp
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:7.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ClientApp.dll"]
