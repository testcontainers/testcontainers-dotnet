FROM alpine:3.16

HEALTHCHECK --interval=1s CMD test -e /testfile --timeout=10s

COPY empty_loop.sh empty_loop.sh
RUN chmod +x empty_loop.sh

CMD ["/empty_loop.sh"]
