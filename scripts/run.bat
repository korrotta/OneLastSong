REM List all postgres.exe processes
tasklist /FI "IMAGENAME eq postgres.exe"

REM Kill all postgres.exe processes
for /f "tokens=2" %%a in ('tasklist /FI "IMAGENAME eq postgres.exe" ^| findstr /I "postgres.exe"') do taskkill /PID %%a /F

REM Run the setup script to create necessary directories
cd ../postgres_bind_folder
mkdir pg_commit_ts
mkdir pg_dynshmem
mkdir pg_logical
mkdir pg_logical\mappings
mkdir pg_logical\snapshots
mkdir snapshots
mkdir pg_notify
mkdir pg_replslot
mkdir pg_serial
mkdir pg_snapshots
mkdir pg_stat_tmp
mkdir pg_tblspc
mkdir pg_twophase
mkdir pg_wal
mkdir pg_wal\archive_status
mkdir pg_wal\summaries

REM Navigate to the parent directory
cd ..

REM Run the Docker container with bind mount
docker run --name postgres-onelastsong -p 5432:5432 -e POSTGRES_PASSWORD=12345678 -d --mount type=bind,source=%cd%\postgres_bind_folder,target=/var/lib/postgresql/data deltay/onelastsong:v1.0