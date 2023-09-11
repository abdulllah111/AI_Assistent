#!/bin/bash

log_file=python.log

# Создаем файл лога, если его нет
if [ ! -f "$log_file" ]; then
  touch "$log_file"
fi

start_python() {
  python client.py &
  python_pid=$!
}

start_python

while true; do

  if grep -q "Request error" "$log_file"; then
    echo "Error found. Restarting..."
    kill "$python_pid"
    start_python
  fi

  out=$(tail -n 1 /proc/"$python_pid"/fd/1)
  if [ ! -z "$out" ]; then
    echo "$out" | tee -a "$log_file"
  fi

  sleep 1

done
