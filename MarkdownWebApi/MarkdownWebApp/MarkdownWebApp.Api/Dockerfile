FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MarkdownWebApp.Api/MarkdownWebApp.Api.csproj", "MarkdownWebApp.Api/"]
COPY ["MarkdownWebApp.Core/MarkdownWebApp.Core.csproj", "MarkdownWebApp.Core/"]
COPY ["MarkdownWebApp.DataAccess.Postgres/MarkdownWebApp.DataAccess.Postgres.csproj", "MarkdownWebApp.DataAccess.Postgres/"]
COPY ["MarkdownWebApp.Application/MarkdownWebApp.Application.csproj", "MarkdownWebApp.Application/"]
COPY ["../../MarkdownProcessorLib/MarkdownProcessorLib/MarkdownProcessorLib.csproj", "../../MarkdownProcessorLib/MarkdownProcessorLib/"]
COPY ["MarkdownWebApp.Infrastructure/MarkdownWebApp.Infrastructure.csproj", "MarkdownWebApp.Infrastructure/"]
RUN dotnet restore "MarkdownWebApp.Api/MarkdownWebApp.Api.csproj"
COPY . .
WORKDIR "/src/MarkdownWebApp.Api"
RUN dotnet build "MarkdownWebApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MarkdownWebApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarkdownWebApp.Api.dll"]
