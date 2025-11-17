FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY WiseWallet.Api.csproj .
RUN dotnet restore "WiseWallet.Api.csproj"
COPY . .
RUN dotnet build "WiseWallet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WiseWallet.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "WiseWallet.Api.dll"]
