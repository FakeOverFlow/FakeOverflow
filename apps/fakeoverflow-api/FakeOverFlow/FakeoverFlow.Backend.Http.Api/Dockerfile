FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy project files for layer caching and to restore dependencies, including project reference
COPY ["FakeoverFlow.Backend.Http.Api/FakeoverFlow.Backend.Http.Api.csproj", "FakeoverFlow.Backend.Http.Api/"]
COPY ["FakeoverFlow.Backend.Abstraction/FakeoverFlow.Backend.Abstraction.csproj", "FakeoverFlow.Backend.Abstraction/"]
RUN dotnet restore "FakeoverFlow.Backend.Http.Api/FakeoverFlow.Backend.Http.Api.csproj"
# Copy the rest of the source
COPY . .
WORKDIR "/src/FakeoverFlow.Backend.Http.Api"
RUN dotnet build "FakeoverFlow.Backend.Http.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FakeoverFlow.Backend.Http.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/FakeoverFlow.Backend.Http.Api.dll"]