FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG workdir
ARG username
ARG uid
ARG gid

COPY entry.sh /entry.sh

RUN groupadd \
        --gid "$gid" \
        "$username" \
    && \
    useradd \
        --uid "$uid" \
        --gid "$gid" \
        --home-dir "$workdir" \
        --create-home \
        "$username"

USER "$uid":"$gid"

RUN dotnet tool install \
        --global \
        dotnet-ef \
    && \
    mkdir \
        --parents \
        /home/"$username"/src \
    && \
    echo \
        PATH="\$PATH":/home/"$username"/.dotnet/tools \
    >> /home/"$username"/.bashrc

ENTRYPOINT ["/bin/bash", "/entry.sh"]
