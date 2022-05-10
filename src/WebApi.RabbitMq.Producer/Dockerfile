#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WebApi.RabbitMq.Producer/WebApi.RabbitMq.Producer.csproj", "src/WebApi.RabbitMq.Producer/"]
COPY ["src/Model.RabbitMq/Model.RabbitMq.csproj", "src/Model.RabbitMq/"]
COPY ["src/Monitor.Service.Settings.6/Monitor.Service.Settings.csproj", "src/Monitor.Service.Settings.6/"]
RUN dotnet restore "src/WebApi.RabbitMq.Producer/WebApi.RabbitMq.Producer.csproj"
COPY . .
WORKDIR "/src/src/WebApi.RabbitMq.Producer"
RUN dotnet build "WebApi.RabbitMq.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.RabbitMq.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.RabbitMq.Producer.dll"]