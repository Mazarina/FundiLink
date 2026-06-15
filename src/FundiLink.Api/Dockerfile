# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY FundiLink.sln ./
COPY src/FundiLink.Domain/FundiLink.Domain.csproj src/FundiLink.Domain/
COPY src/FundiLink.Application/FundiLink.Application.csproj src/FundiLink.Application/
COPY src/FundiLink.Infrastructure/FundiLink.Infrastructure.csproj src/FundiLink.Infrastructure/
COPY src/FundiLink.Api/FundiLink.Api.csproj src/FundiLink.Api/
RUN dotnet restore src/FundiLink.Api/FundiLink.Api.csproj

COPY src/ src/
RUN dotnet publish src/FundiLink.Api/FundiLink.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN addgroup --system --gid 1001 fundilink \
    && adduser --system --uid 1001 --gid 1001 fundilink \
    && mkdir -p /app/uploads \
    && chown -R fundilink:fundilink /app

COPY --from=build /app/publish .
RUN chown -R fundilink:fundilink /app

USER fundilink

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FundiLink.Api.dll"]
