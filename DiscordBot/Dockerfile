# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy the solution file and project files for DiscordBot and SharedDependencies
COPY HouseScout.sln ./
COPY SharedDependencies/SharedDependencies.csproj ./SharedDependencies/
COPY DiscordBot/DiscordBot.csproj ./DiscordBot/

# Restore dependencies for both DiscordBot and SharedDependencies
WORKDIR ./DiscordBot
RUN dotnet restore
WORKDIR ..

# Copy all the files for DiscordBot and SharedDependencies
COPY SharedDependencies/ ./SharedDependencies/
COPY DiscordBot/ ./DiscordBot/

# Build and publish the DiscordBot project
WORKDIR /App/DiscordBot
RUN dotnet publish -c Release -o out

# Stage 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /App

# Copy the build output from the previous stage
COPY --from=build-env /App/DiscordBot/out .

ENTRYPOINT ["dotnet", "DiscordBot.dll"]
