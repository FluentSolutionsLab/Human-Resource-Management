FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5149
ENV ASPNETCORE_URLS=http://+:5149

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HRManagement.Api/HRManagement.Api.csproj", "src/HRManagement.Api/"]
COPY ["src/Common/HRManagement.Common.Infrastructure/HRManagement.Common.Infrastructure.csproj", "src/Common/HRManagement.Common.Infrastructure/"]
COPY ["src/Common/HRManagement.Common.Application/HRManagement.Common.Application.csproj", "src/Common/HRManagement.Common.Application/"]
COPY ["src/Common/HRManagement.Common.Domain/HRManagement.Common.Domain.csproj", "src/Common/HRManagement.Common.Domain/"]
COPY ["src/Modules/HRManagement.Modules.Personnel.Persistence/HRManagement.Modules.Personnel.Persistence.csproj", "src/Modules/HRManagement.Modules.Personnel.Persistence/"]
COPY ["src/Common/HRManagement.Common.Pertinence/HRManagement.Common.Pertinence.csproj", "src/Common/HRManagement.Common.Pertinence/"]
COPY ["src/Modules/HRManagement.Modules.Personnel.Application/HRManagement.Modules.Personnel.Application.csproj", "src/Modules/HRManagement.Modules.Personnel.Application/"]
COPY ["src/Modules/HRManagement.Modules.Personnel.Domain/HRManagement.Modules.Personnel.Domain.csproj", "src/Modules/HRManagement.Modules.Personnel.Domain/"]
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
