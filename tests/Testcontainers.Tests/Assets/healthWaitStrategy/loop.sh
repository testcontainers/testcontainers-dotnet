#!/bin/ash
set -ex
sleep 5
if [ $SHOULD_FAIL -ne 1 ]
then
  touch /testfile
fi
while true; do sleep 1; done
