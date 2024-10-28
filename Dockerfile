# Step 1: Build the Angular application
FROM --platform=linux/amd64 node:18 AS ng-builder
 
# Set the working directory for the Angular build
WORKDIR /app/NGO.Web/ClientApp
COPY ./NGO.Web/ClientApp/package*.json ./
RUN npm install
COPY ./NGO.Web/ClientApp .
RUN npm run build 
 
# Step 2: Build the .NET application
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
 
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
 
# Step 3: Create the final image using the ASP.NET Core runtime
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
 
# Set the working directory inside the container
WORKDIR /app
 
# Copy the published application from the build stage
COPY --from=build /app/NGO.Web/out .
 
# Copy the Angular build output to the wwwroot folder
COPY --from=ng-builder /app/NGO.Web/ClientApp/dist/ClientApp ./wwwroot

# Expose the port your application runs on
EXPOSE 80
 

# Specify the entry point for the container
CMD ["dotnet", "NGO.Web.dll"]