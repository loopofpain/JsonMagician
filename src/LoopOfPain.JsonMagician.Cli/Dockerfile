FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY . "LoopOfPain.JsonMagician.Cli/"
RUN dotnet restore "LoopOfPain.JsonMagician.Cli/LoopOfPain.JsonMagician.Cli.csproj"
WORKDIR "/src/LoopOfPain.JsonMagician.Cli"
RUN dotnet publish "LoopOfPain.JsonMagician.Cli.csproj" -c Release -o /app/publish /p:UseAppHost=false




FROM base AS final

RUN addgroup -g 12222 -S app
RUN adduser -g "" -s /bin/false -D -H -u 12222 -S app app
USER app

WORKDIR /app
COPY --chown=12222:12222 --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoopOfPain.JsonMagician.Cli.dll"]