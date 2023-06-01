# Get Base Image (Full .NET Core SDK)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /tozawa_repos
# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /tozawa_repos
# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:80
EXPOSE 80
VOLUME [ "CustomerSettings" ]
COPY --from=build-env /tozawa_repos/out .
ENTRYPOINT ["dotnet", "Tozawa.Language.Svc.dll"]