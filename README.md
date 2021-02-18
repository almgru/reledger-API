# Reledger-API

## Running locally

1. `docker build --tag reledger-api .`
2. `docker run --publish 127.0.0.1:5000:80 --name reledger-api --rm --detach reledger-api`

## Pushing to Azure Container Registry

1. Build the image: `docker build --tag reledger-api:latest .`
2. Login to the registry: `docker login <name of registry>.azurecr.io`
3. Attach tag: `docker tag reledger-api:latest <name of registry>.azurecr.io/reledger-api:<version>`
4. Push the image: `docker push <name of registry>.azurecr.io/reledger-api:<version>`
