FROM docker:dind-rootless

USER root

RUN apk add --no-cache \
    bash \
    curl \
    git \
    git-lfs \
    gcompat \
    icu-data-full \
    icu-libs \
    && chown rootless:rootless -R /home/rootless

USER rootless

ARG DOTNET_SDK_VERSION=6.0.4xx

ENV DOCKER_HOST=unix:///var/run/user/1000/docker.sock \
    DOTNET_CLI_TELEMETRY_OPTOUT=true \
    DOTNET_NOLOGO=true \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    NUGET_XMLDOC_MODE=skip

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel ${DOTNET_SDK_VERSION} \
    && echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.profile \
    && echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.profile \
    && mkdir ~/.ssh \
    && ssh-keyscan -t rsa github.com >> ~/.ssh/known_hosts
