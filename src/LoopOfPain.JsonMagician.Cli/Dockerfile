#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . "LoopOfPain.JsonMagician.Cli/"
RUN dotnet restore "LoopOfPain.JsonMagician.Cli/LoopOfPain.JsonMagician.Cli.csproj"
COPY . .
WORKDIR "/src/LoopOfPain.JsonMagician.Cli"
RUN dotnet build "LoopOfPain.JsonMagician.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoopOfPain.JsonMagician.Cli.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoopOfPain.JsonMagician.Cli.dll"]