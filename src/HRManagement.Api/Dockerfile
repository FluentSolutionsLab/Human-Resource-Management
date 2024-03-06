FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5149
ENV ASPNETCORE_URLS=http://+:5149

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HRManagement.Api/HRManagement.Api.csproj", "src/HRManagement.Api/"]
COPY ["src/BuildingBlocks/HRManagement.BuildingBlocks/HRManagement.BuildingBlocks.csproj", "src/BuildingBlocks/HRManagement.BuildingBlocks/"]
COPY ["src/Modules/HRManagement.Modules.Staff/HRManagement.Modules.Staff.csproj", "src/Modules/HRManagement.Modules.Staff/"]
RUN dotnet restore "src/HRManagement.Api/HRManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/HRManagement.Api"
RUN dotnet build "HRManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HRManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRManagement.Api.dll"]
