FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS http://+:5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["User.API.csproj", ""]
RUN dotnet restore "./User.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.API.dll"]


#FROM microsoft/dotnet:2.2-sdk as build-env
#WORKDIR /code
#COPY *.csproj ./
#RUN dotnet restore
#COPY . ./
#RUN dotnet publish -c Release -o out
#
#FROM microsoft/dotnet:2.2-aspnetcore-runtime 
#WORKDIR /app
#COPY --from=build-env /code/out ./
#EXPOSE 80
#ENTRYPOINT ["dotnet","User.API.dll"]