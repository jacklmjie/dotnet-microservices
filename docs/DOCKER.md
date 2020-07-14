#docker中文文档 https://yeasy.gitbooks.io/docker_practice/content/
#docker基本命令
docker images
docker ps -a
docker start/stop imageId
docker rmi/rm imageId

#link方式
docker build -t k8swebapi .
docker run -d -p 8001:80 --name k8swebapi k8swebapi
docker run -d -p 8001:80 --name k8swebapi --link mysql01:db k8swebapi
docker logs mysql01
docker rename mysql01 db
docker inspect mysql01

#bridge方式
docker network create -d bridge mybridge
docker network ls
docker network connect mybridge mysql01

docker build -t k8swebapi .
docker run -d -p 8001:80 --net mybridge --name k8swebapi k8swebapi
docker logs k8swebapi
docker inspect k8swebapi
docker exec -it k8swebapi bash
ping mysql01

#consul
docker里的consul配置 https://www.cnblogs.com/lonelyxmas/p/10880717.html
docker run --name consul01 -d -p 8500:8500 -p 8300:8300 -p 8301:8301 -p 8302:8302 -p 8600:8600 consul agent -server -bootstrap-expect 2 -ui -bind=0.0.0.0 -client=0.0.0.0
docker inspect consul01
docker run --name consul02 -d -p 8501:8500 consul agent -server -ui -bind=0.0.0.0 -client=0.0.0.0 -join 172.17.0.3
docker network connect mybridge consul01
docker network connect mybridge consul02

#redis
docker run -d -p 6379:6379 --net mybridge --name redis01 redis

#mysql
docker run -d -p 3306:3306 -e MYSQL_USER=sqltest -e MYSQL_PASSWORD=pwd123 -e MYSQL_ROOT_PASSWORD=pwd123 -e MYSQL_ROOT_HOST=% -v D:/docker/mysql/config/my.cnf:/etc/my.cnf  -v D:/docker/mysql/data:/var/lib/mysql --name mysql01 mysql

#其他
查找本地端口号 netstat -aon|findstr "135"
删除所有图像，运行的除外 docker rmi $(docker images -q)
删除所有容器 docker rm $(docker ps -a -q)

#工具
putty 可以登录服务器
protainer 容器管理工具
nginx 域名都是绑定同一个IP，转发2个不同端口号

