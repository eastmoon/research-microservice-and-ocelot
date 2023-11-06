# Check out https://hub.docker.com/_/debian to select a new base image
FROM mcr.microsoft.com/dotnet/aspnet:3.1

# Create dotnet app directory

# Set to a non-root built-in user `dotnet`
RUN \
    groupadd -r dotnet && \
    useradd -r -g dotnet dotnet && \
    mkdir -p /home/dotnet && \
    chown dotnet:dotnet /home/dotnet && \
    chmod 777 /home/dotnet

USER dotnet

# Create app directory (with user `dotnet`)
RUN mkdir -p /home/dotnet/app

WORKDIR /home/dotnet/app

# Bundle app source code
COPY --chown=dotnet ./published .

# Setting entrypoint script
RUN mkdir -p /home/dotnet/log
RUN \
    echo "dotnet WebService.dll >> /home/dotnet/log/app.log 2>&1 &" > docker-entrypoint.sh && \
    echo "tail -f /dev/null" >> docker-entrypoint.sh
CMD [ "/bin/bash", "docker-entrypoint.sh"]
