#!/bin/bash

if ! docker network inspect bookly_network >/dev/null 2>&1; then
    echo "Создание сети bookly_network..."
    docker network create bookly_network
    echo "Сеть bookly_network создана успешно"
else
    echo "Сеть bookly_network уже существует"
fi

if ! docker volume inspect bookly_postgres_volume >/dev/null 2>&1; then
    echo "Создание тома bookly_postgres_volume..."
    docker volume create bookly_postgres_volume
    echo "Том bookly_postgres_volume создан успешно"
else
    echo "Том bookly_postgres_volume уже существует"
fi