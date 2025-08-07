FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TextAnalysisService.csproj", "."]
RUN dotnet restore "TextAnalysisService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "TextAnalysisService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TextAnalysisService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TextAnalysisService.dll"]