# 指定基础镜像
FROM microsoft/dotnet:2.2-aspnetcore-runtime
# 维护者信息
MAINTAINER mx "mx"

EXPOSE 5133
# copy csproj and restore as distinct layers
COPY  publish /data/www/
WORKDIR /data/www
ENTRYPOINT ["dotnet", "/data/www/User.API.dll"]
