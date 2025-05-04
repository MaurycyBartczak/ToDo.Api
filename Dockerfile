FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ToDo.Api.sln", "."]
COPY ["ToDo.Api.csproj", "."]
COPY ["ToDo.Api.Tests/ToDo.Api.Tests.csproj", "ToDo.Api.Tests/"]
RUN dotnet restore "ToDo.Api.sln"
COPY . .
RUN dotnet build "ToDo.Api.sln" -c Release -o /app/build

FROM build AS testing
WORKDIR /src
RUN dotnet test "ToDo.Api.sln" -c Release --no-build --verbosity normal

FROM build AS publish
RUN dotnet publish "ToDo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo.Api.dll"]