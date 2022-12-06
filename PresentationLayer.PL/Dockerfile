#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PresentationLayer.PL/PresentationLayer.PL.csproj", "PresentationLayer.PL/"]
RUN dotnet restore "PresentationLayer.PL/PresentationLayer.PL.csproj"
COPY . .
WORKDIR "/src/PresentationLayer.PL"
RUN dotnet build "PresentationLayer.PL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PresentationLayer.PL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PresentationLayer.PL.dll"]