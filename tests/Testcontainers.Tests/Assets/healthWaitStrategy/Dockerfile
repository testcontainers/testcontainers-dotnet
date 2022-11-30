FROM alpine
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
HEALTHCHECK --interval=1s CMD test -e /healthcheck
COPY docker-entrypoint.sh docker-entrypoint.sh
RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
