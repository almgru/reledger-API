#!/bin/sh

docker run \
    --user "$(id -u)":"$(id -g)" \
    --mount type=bind,source="$PWD"/..,target=/home/"$USER"/src \
    --interactive \
    --tty \
    relapi

