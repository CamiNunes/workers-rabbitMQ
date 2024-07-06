# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0-preview AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["src/Services/Customer.Worker/Customer.Worker.csproj", "src/Services/Customer.Worker/"]
RUN dotnet restore "src/Services/Customer.Worker/Customer.Worker.csproj"
COPY . .
WORKDIR "/src/src/Services/Customer.Worker"
RUN dotnet build "Customer.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.Worker.dll"]
