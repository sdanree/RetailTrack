#!/bin/bash

# Sincroniza ramas main, testing y production con develop
set -e

echo "Actualizando develop..."
git checkout develop
git pull origin develop

for branch in main testing production; do
    echo "Reseteando $branch a develop..."
    git checkout $branch || git checkout -b $branch
    git reset --hard develop
    git push origin $branch --force
done

echo "Todas las ramas están sincronizadas con develop."
