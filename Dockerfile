FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Microservicios.Atracciones.Billing.API/Microservicios.Atracciones.Billing.API.csproj", "Microservicios.Atracciones.Billing.API/"]
COPY ["Microservicios.Atracciones.Billing.Business/Microservicios.Atracciones.Billing.Business.csproj", "Microservicios.Atracciones.Billing.Business/"]
COPY ["Microservicios.Atracciones.Billing.DataAccess/Microservicios.Atracciones.Billing.DataAccess.csproj", "Microservicios.Atracciones.Billing.DataAccess/"]
COPY ["Microservicios.Atracciones.Billing.DataManagement/Microservicios.Atracciones.Billing.DataManagement.csproj", "Microservicios.Atracciones.Billing.DataManagement/"]

RUN dotnet restore "Microservicios.Atracciones.Billing.API/Microservicios.Atracciones.Billing.API.csproj"

COPY . .
WORKDIR "/src/Microservicios.Atracciones.Billing.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Microservicios.Atracciones.Billing.API.dll"]
