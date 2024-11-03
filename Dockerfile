# Step 1: Build the Angular application
FROM --platform=linux/amd64 node:18 AS ng-builder
WORKDIR /app/NGO.Web/ClientApp
COPY ./NGO.Web/ClientApp/package*.json ./
RUN npm install
COPY ./NGO.Web/ClientApp .
RUN npm run build  

# Step 2: Build the .NET application
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
COPY *.sln ./
COPY NGO.Web/*.csproj ./NGO.Web/
RUN dotnet restore
WORKDIR /app/NGO.Web
RUN dotnet publish -c Release -o out

# Step 3: Create the final image using the ASP.NET Core runtime
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/NGO.Web/out . 
# Expose the port
EXPOSE 80

# Entry point
CMD ["dotnet", "NGO.Web.dll"]
