#!/bin/bash
set -e
NAME=$1
if [ -z "$NAME" ]; then
  echo "Usage: ./add-migration.sh MigrationName"
  exit 1
fi

dotnet ef migrations add "$NAME" -p LibraryManager.Migrations.SqlServer -s LibraryManager.Migrations.SqlServer
dotnet ef migrations add "$NAME" -p LibraryManager.Migrations.Sqlite -s LibraryManager.Migrations.Sqlite

echo "Migration '$NAME' generated for SqlServer and Sqlite."