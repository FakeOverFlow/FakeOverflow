FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

RUN apk add --no-cache \
    python3 \
    py3-pip \
    py3-requests \
    curl

RUN python3 -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

# Install Python packages
RUN pip install --no-cache-dir \
    langchain==0.1.0 \
    langchain-community==0.0.13 \
    requests==2.31.0 \
    ollama==0.1.6 \
    openai \
    langchain-openai \
   

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

RUN mkdir -p /app/scripts
COPY ["scripts/fact_checker.py", "/app/scripts/"]

ENV PATH="/opt/venv/bin:$PATH"
ENV PYTHONUNBUFFERED=1

ENTRYPOINT ["dotnet", "/app/FakeoverFlow.Backend.Http.Api.dll"]