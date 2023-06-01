# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
# Copy csproj and restore
COPY src/TozAwa.Client.Portal/TozAwa.Client.Portal.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:80
EXPOSE 8080
VOLUME [ "src/TozAwa.Client.Portal/TozAwa.Client.Portal/CustomerSettings" ]
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "src/TozAwa.Client.Portal/TozAwa.Client.Portal.dll"]