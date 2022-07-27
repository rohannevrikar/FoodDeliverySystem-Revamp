FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Orders-Handler-Service.dll"]


# Commands to build and push
# docker build -t order-handler-service .
# docker tag order-handler-service edafdsacr.azurecr.io/order-handler-service
# az acr login -n edafdsacr
# docker push edafdsacr.azurecr.io/order-handler-service