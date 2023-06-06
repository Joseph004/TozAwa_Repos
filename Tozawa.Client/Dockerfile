# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /source
# Copy csproj and restore
#COPY *.csproj ./
#RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet restore "./src/TozAwa.Client.Portal/TozAwa.Client.Portal.csproj" --disable-parallel
RUN dotnet publish "./src/TozAwa.Client.Portal/TozAwa.Client.Portal.csproj" -c Release -o /app /p:UseAppHost=false --no-restore

# Generate runtime image
FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY --from=build-env /app ./
# Ensure we listen on any IP Address 
ENV ASPNETCORE_URLS=http://*:80
#ENV DOTNET_URLS=http://+:80
EXPOSE 8080
#VOLUME [ "CustomerSettings" ]
ENTRYPOINT ["dotnet", "TozAwa.Client.Portal.dll"]