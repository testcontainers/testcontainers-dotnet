FROM alpine:3.17
LABEL "maintainer"="9199345+HofmeisterAn@users.noreply.github.com"
HEALTHCHECK --interval=1s CMD test -e /healthcheck
COPY docker-entrypoint.sh docker-entrypoint.sh
RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
