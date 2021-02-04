#!/bin/sh

docker build \
    --build-arg uid="$(id -u)" \
    --build-arg gid="$(id -g)" \
    --build-arg username="$USER" \
    --build-arg workdir="$HOME" \
    --tag relapi:latest \
    .
