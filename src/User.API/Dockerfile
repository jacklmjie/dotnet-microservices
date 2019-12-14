FROM microsoft/dotnet:2.2-sdk as build-env
WORKDIR /code
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime 
WORKDIR /app
COPY --from=build-env /code/out ./
EXPOSE 80
ENTRYPOINT ["dotnet","User.API.dll"]
