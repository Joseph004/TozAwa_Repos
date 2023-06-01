# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install -g dotnet-ef --version 6.0
WORKDIR /src/TozAwa.Client.Portal
# Copy csproj and restore
COPY TozAwa.Client.Portal.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish TozAwa.Client.Portal.csproj -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:80
EXPOSE 8080
VOLUME [ "CustomerSettings" ]
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TozAwa.Client.Portal.dll"]