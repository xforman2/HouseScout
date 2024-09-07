# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy the solution file and project files for Backend and SharedDependencies
COPY HouseScout.sln ./
COPY SharedDependencies/SharedDependencies.csproj ./SharedDependencies/
COPY Backend/Backend.csproj ./Backend/

# Restore dependencies for Backend
WORKDIR ./Backend
RUN dotnet restore
WORKDIR ..

# Copy all the files for Backend and SharedDependencies
COPY SharedDependencies/ ./SharedDependencies/
COPY Backend/ ./Backend/

# Build and publish the Backend project
WORKDIR /App/Backend
RUN dotnet publish -c Release -o out

# Stage 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install cron
RUN apt-get update && apt-get install -y cron

WORKDIR /App

# Copy the build output from the previous stage
COPY --from=build-env /App/Backend/out .

# Copy the crontab file into the image
COPY ./Backend/crontab /etc/cron.d/crontab

# Give execution rights on the crontab file
RUN chmod 0644 /etc/cron.d/crontab

# Apply the crontab
RUN crontab /etc/cron.d/crontab

# Run cron in the foreground
CMD ["cron", "&&",  "tail", "-f", "/var/log/cron.log"]
