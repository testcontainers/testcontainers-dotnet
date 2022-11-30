FROM alpine:3.16
HEALTHCHECK --interval=1s CMD test -e /testfile
ARG SHOULD_FAIL=0
COPY loop.sh loop.sh
ENV SHOULD_FAIL=${SHOULD_FAIL}
RUN dos2unix loop.sh
RUN chmod +x loop.sh
CMD ["/loop.sh"]
