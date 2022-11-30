#!/bin/ash
set -ex
sleep 5
if [ $(echo $START_HEALTHY | tr '[:upper:]' '[:lower:]') = true ]
then
  touch /healthcheck
fi
while true; do sleep 1; done
