#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Worker.RabbitMq.Consumer/Worker.RabbitMq.Consumer.csproj", "src/Worker.RabbitMq.Consumer/"]
COPY ["src/Model.RabbitMq/Model.RabbitMq.csproj", "src/Model.RabbitMq/"]
COPY ["src/Monitor.Service.Settings.6/Monitor.Service.Settings.csproj", "src/Monitor.Service.Settings.6/"]
RUN dotnet restore "src/Worker.RabbitMq.Consumer/Worker.RabbitMq.Consumer.csproj"
COPY . .
WORKDIR "/src/src/Worker.RabbitMq.Consumer"
RUN dotnet build "Worker.RabbitMq.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker.RabbitMq.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker.RabbitMq.Consumer.dll"]