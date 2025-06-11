# Dockerfile.sql
FROM mcr.microsoft.com/mssql/server:2022-latest

USER root
RUN apt-get update \
 && apt-get install -y apt-transport-https gnupg curl \
 && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
 && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list \
      > /etc/apt/sources.list.d/mssql-release.list \
 && apt-get update \
 && ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools unixodbc-dev \
 && ln -s /opt/mssql-tools18/bin/* /usr/local/bin/

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=YourStrong!Passw0rd

COPY init-authservice.sql /init-authservice.sql
RUN chmod 644 /init-authservice.sql

# Arranca el motor y luego ejecuta el script
CMD /opt/mssql/bin/sqlservr & \
    sleep 30 && \
    sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" \
           -i /init-authservice.sql
