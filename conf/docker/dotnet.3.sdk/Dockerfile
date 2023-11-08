FROM mcr.microsoft.com/dotnet/sdk:3.1

# Install dotnet tools
RUN dotnet tool install -g swashbuckle.aspnetcore.cli
RUN dotnet tool install -g dotnet-ef

# Add ENV Path, make sure tool can work
ENV PATH="${PATH}:/root/.dotnet/tools"

# Add ASPNETCORE_URLS variable for server address listener.
## ref : https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/host/web-host?view=aspnetcore-6.0#server-urls
ENV ASPNETCORE_URLS="http://*:5000"

# Add docker-entrypoint script
ADD ./docker-entrypoint.sh /docker-entrypoint.sh

# Setting Container
EXPOSE 5000/tcp
WORKDIR /app
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["dotnet", "publish"]
