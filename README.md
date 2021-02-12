# Reledger-API

## Running locally

1. `docker build --tag reledger-api .`
2. `docker run --publish 127.0.0.1:5000:80 --name reledger-api --rm --detach reledger-api`
