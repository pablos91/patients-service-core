FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build

# # copy everything else and build app
COPY . .
# WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release -o ./out PatientsService.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
 
ENV AZURE_DB_PASS=1234
ENV EMAIL_PASS=1234
ENV SHARED_KEY=1234

EXPOSE 80

COPY --from=build ./build/out .
ENTRYPOINT ["dotnet","PatientsService.dll"]