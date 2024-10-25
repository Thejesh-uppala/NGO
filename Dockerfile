# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app
COPY . .
# Copy the solution file and restore dependencies
COPY *.sln ./
COPY NGO.Web/*.csproj ./NGO.Web/
RUN dotnet restore

# Copy the entire application and build it
WORKDIR /app/NGO.Web
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/NGO.Web/out .

# Expose the port your application runs on
EXPOSE 80

# Specify the entry point for the container
CMD ["dotnet", "NGO.Web.dll"]
