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
COPY ["src/Common/HRManagement.Common.Endpoints/HRManagement.Common.Endpoints.csproj", "src/Common/HRManagement.Common.Endpoints/"]
COPY ["src/Modules/HRManagement.Modules.Staff/HRManagement.Modules.Staff.csproj", "src/Modules/HRManagement.Modules.Staff/"]
#COPY ["src/Modules/HRManagement.Modules.Staff.Persistence/HRManagement.Modules.Staff.Persistence.csproj", "src/Modules/HRManagement.Modules.Staff.Persistence/"]
COPY ["src/Common/HRManagement.Common.Pertinence/HRManagement.Common.Pertinence.csproj", "src/Common/HRManagement.Common.Pertinence/"]
#COPY ["src/Modules/HRManagement.Modules.Staff.Application/HRManagement.Modules.Staff.Application.csproj", "src/Modules/HRManagement.Modules.Staff.Application/"]
#COPY ["src/Modules/HRManagement.Modules.Staff.Domain/HRManagement.Modules.Staff.Domain.csproj", "src/Modules/HRManagement.Modules.Staff.Domain/"]
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
