## 微服务系列
### 用户服务
- docker安装
  - [windows docker安装](https://www.runoob.com/docker/windows-docker-install.html)
  - docker简单操作，配置阿里云加速，拉取镜像包
  - docker安装mysql
    - 直接运行
    - 外部volume挂载资料卷
- ef生成数据库
  - [EF介绍](https://docs.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
  - 生成数据库
  - 数据初始化
- dapper操作数据
  - dapper介绍
  - [深入dapper](https://www.cnblogs.com/ITWeiHan/p/11614704.html)
  - dapper contain使用
  - dapper仓储在webapi的使用
  - dapper事务，工作单元应用
- webapi Restful
  - [RESTful API最佳实践](http://www.ruanyifeng.com/blog/2018/10/restful-api-best-practices.html)
  - Restful风格使用
  - PostMan工具的使用
- 全局异常日志
- json patch
  - [json patch介绍](http://jsonpatch.com/)
  - json patch操作
  - json patch数组
- 单元测试
  - xunit+Moq+MemoryEFDbContext测试UserController
  - FluentAPI写UserController测试用例
- 部署
  - GitLab CI完整部署UserAPI到线上测试环境
- 授权服务
  - User.Identity
- 网关
  - [Ocelot](http://www.jessetalk.cn/2018/03/19/net-core-apigateway-ocelot-docs/)
  - Ocelot集成Identity Server鉴权
  - [consul安装](https://www.cnblogs.com/PearlRan/p/11225953.html)
  - conslu 服务注册与发现
### 接下来
- 消息队列数据交互
- 日志记录ELK
- 健康检查

## 参考资料
- [docker安装mysql命令]
