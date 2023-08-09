# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-bookworm-slim AS build
WORKDIR /src

COPY Directory.Build.props .
COPY ./CommonLib/CommonLib.csproj ./CommonLib/CommonLib.csproj
RUN dotnet restore ./CommonLib/CommonLib.csproj

COPY ./WorkerApp/WorkerApp.csproj ./WorkerApp/WorkerApp.csproj
RUN dotnet restore ./WorkerApp/WorkerApp.csproj

COPY ./CommonLib ./CommonLib
COPY ./WorkerApp ./WorkerApp
WORKDIR /src/WorkerApp
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:7.0-bookworm-slim
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WorkerApp.dll"]
