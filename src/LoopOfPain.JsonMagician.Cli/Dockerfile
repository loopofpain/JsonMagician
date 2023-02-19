FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY . "LoopOfPain.JsonMagician.Cli/"
RUN dotnet restore "LoopOfPain.JsonMagician.Cli/LoopOfPain.JsonMagician.Cli.csproj"
WORKDIR "/src/LoopOfPain.JsonMagician.Cli"
RUN dotnet publish "LoopOfPain.JsonMagician.Cli.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
RUN addgroup -g 12222 -S app && adduser -g "" -s /bin/false -D -H -u 12222 -S app app
USER app

COPY --chown=12222:12222 --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoopOfPain.JsonMagician.Cli.dll"]