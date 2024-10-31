REM List all postgres.exe processes
tasklist /FI "IMAGENAME eq postgres.exe"

REM Kill all postgres.exe processes
for /f "tokens=2" %%a in ('tasklist /FI "IMAGENAME eq postgres.exe" ^| findstr /I "postgres.exe"') do taskkill /PID %%a /F

REM Run the setup script to create necessary directories
cd ../postgres_bind_folder
mkdir -p pg_commit_ts
mkdir -p pg_dynshmem
mkdir -p pg_logical/mappings
mkdir -p pg_logical/snapshots
mkdir -p snapshots
mkdir -p pg_notify
mkdir -p pg_replslot
mkdir -p pg_serial
mkdir -p pg_snapshots
mkdir -p pg_stat_tmp
mkdir -p pg_tblspc
mkdir -p pg_twophase
mkdir -p pg_wal/archive_status
mkdir -p pg_wal/summaries

REM Navigate to the parent directory
cd ..

REM Run the Docker container with bind mount
docker run --name postgres-onelastsong -p 5432:5432 -e POSTGRES_PASSWORD=12345678 -d --mount type=bind,source=%cd%\postgres_bind_folder,target=/var/lib/postgresql/data deltay/onelastsong:v1.0