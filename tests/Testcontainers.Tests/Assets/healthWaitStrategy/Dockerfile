FROM alpine:3.20.0@sha256:77726ef6b57ddf65bb551896826ec38bc3e53f75cdde31354fbffb4f25238ebd
LABEL "maintainer"="9199345+HofmeisterAn@users.noreply.github.com"
HEALTHCHECK --interval=1s CMD test -e /healthcheck
COPY docker-entrypoint.sh docker-entrypoint.sh
RUN chmod +x docker-entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
