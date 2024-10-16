FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM efirit-dotnet-workspace:0.14 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "EfiritPro.Retail.ProductModule.sln"

FROM build AS publish
RUN dotnet publish "./EfiritPro.Retail.ProductModule.Api/EfiritPro.Retail.ProductModule.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EfiritPro.Retail.ProductModule.Api.dll"]
