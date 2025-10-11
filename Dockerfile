# Stage 1: Build the Angular frontend and .NET Core application
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Install Node.js (required for Angular build)
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get update && apt-get install -y nodejs

# Copy the NGO.Web project and restore .NET dependencies
COPY NGO.Web/*.csproj NGO.Web/
COPY NGO.Web/ClientApp/package.json NGO.Web/ClientApp/
COPY NGO.Web/ClientApp/angular.json NGO.Web/ClientApp/
COPY NGO.Web/ClientApp/tsconfig*.json NGO.Web/ClientApp/
RUN dotnet restore NGO.Web/NGO.Web.csproj

# Copy the rest of the application
COPY . .

# Build the Angular frontend
WORKDIR /app/NGO.Web/ClientApp
RUN npm install
RUN npm run build

# Publish the .NET Core application
WORKDIR /app/NGO.Web
RUN dotnet publish -c Release -o out

# Stage 2: Create the runtime image
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/NGO.Web/out .

# Set the environment variables (replace with your actual values or use a .env file)
ENV ASPNETCORE_ENVIRONMENT=Production

ENV AppSettings__ConnectionString=""
ENV AppSettings__Secret=""


# Expose the port (default ASP.NET Core port)
EXPOSE 80 443

# Run the application
ENTRYPOINT ["dotnet", "NGO.Web.dll"]