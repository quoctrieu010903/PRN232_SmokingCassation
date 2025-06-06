# Base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ENV ASPNETCORE_URLS=http://+:5081
WORKDIR /app
EXPOSE 5081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy từng project để khôi phục gói
COPY ["SmokingCessation.WebAPI/SmokingCessation.WebAPI.csproj", "SmokingCessation.WebAPI/"]
COPY ["SmokingCessation.Application/SmokingCessation.Application.csproj", "SmokingCessation.Application/"]
COPY ["SmokingCessation.Infrastracture/SmokingCessation.Infrastracture.csproj", "SmokingCessation.Infrastracture/"]
COPY ["SmokingCessation.Core/SmokingCessation.Core.csproj", "SmokingCessation.Core/"]
COPY ["SmokingCessation.Domain/SmokingCessation.Domain.csproj", "SmokingCessation.Domain/"]

# Restore packages
RUN dotnet restore "SmokingCessation.WebAPI/SmokingCessation.WebAPI.csproj"

# Copy toàn bộ source
COPY . .

WORKDIR "/src/SmokingCessation.WebAPI"
RUN dotnet build "SmokingCessation.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SmokingCessation.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmokingCessation.WebAPI.dll"]