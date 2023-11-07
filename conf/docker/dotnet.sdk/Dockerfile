FROM mcr.microsoft.com/dotnet/sdk:6.0

# Install dotnet tools
RUN dotnet tool install -g swashbuckle.aspnetcore.cli
RUN dotnet tool install -g dotnet-ef

# Add ENV Path, make sure tool can work
ENV PATH="${PATH}:/root/.dotnet/tools"

# Add docker-entrypoint script
ADD ./docker-entrypoint.sh /docker-entrypoint.sh

# Setting Container
EXPOSE 5000/tcp
EXPOSE 5001/tcp
WORKDIR /app
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["dotnet", "publish"]
