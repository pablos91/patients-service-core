FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build

# # copy everything else and build app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o ./out PatientsService.EmailService.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=build ./build/out .
ENTRYPOINT ["dotnet","PatientsService.EmailService.dll"]