# Stage 1: Build the entire solution
# Use the .NET SDK image to build your application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the entire solution and all projects to the container
COPY QNBScoring.sln ./
COPY QNBScoring.Web/*.csproj QNBScoring.Web/
COPY QNBScoring.Core/*.csproj QNBScoring.Core/
COPY QNBScoring.Infrastructure/*.csproj QNBScoring.Infrastructure/
COPY QNBScoring.UnitTests/*.csproj QNBScoring.UnitTests/

# Restore all project dependencies
RUN dotnet restore QNBScoring.sln

# Copy the remaining source code
COPY . .

# Publish the web project to the 'out' directory
RUN dotnet publish "QNBScoring.Web/QNBScoring.Web.csproj" -c Release -o /app/publish

# Stage 2: Create the final runtime image
# Use a lightweight ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app will run on
EXPOSE 80

# The entrypoint command to run the application
ENTRYPOINT ["dotnet", "QNBScoring.Web.dll"]