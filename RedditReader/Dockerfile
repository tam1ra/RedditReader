FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RedditReader/RedditReader.csproj", "RedditReader/"]
RUN dotnet restore "RedditReader/RedditReader.csproj"
COPY . .
WORKDIR "/src/RedditReader"
RUN dotnet build "RedditReader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedditReader.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedditReader.dll"]
