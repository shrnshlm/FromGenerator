FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FormSubmissionService.csproj", "."]
RUN dotnet restore "FormSubmissionService.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "FormSubmissionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FormSubmissionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FormSubmissionService.dll"]