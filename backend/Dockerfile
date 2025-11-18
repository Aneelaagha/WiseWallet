# ---- Build stage ----
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /app
    
    # Copy csproj and restore as separate layer
    COPY WiseWallet.Api.csproj ./
    RUN dotnet restore "WiseWallet.Api.csproj"
    
    # Copy the rest of the source and publish
    COPY . .
    RUN dotnet publish "WiseWallet.Api.csproj" -c Release -o /app/publish
    
    # ---- Runtime stage ----
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
    WORKDIR /app
    
    # Copy published app from build stage
    COPY --from=build /app/publish .
    
    # Render expects the app to listen on 0.0.0.0:8080
    ENV ASPNETCORE_URLS=http://+:8080
    EXPOSE 8080
    
    ENTRYPOINT ["dotnet", "WiseWallet.Api.dll"]
    