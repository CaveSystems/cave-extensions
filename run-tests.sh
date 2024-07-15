#! /bin/bash

set -xe

export TERM=xterm-color
find | grep \.Tests/bin | grep Test\.exe | grep -v \.config | while read file
do
  echo Start $file
  $file
  echo Done $file
done
