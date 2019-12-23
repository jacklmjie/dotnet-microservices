## 微服务系列
### 用户服务
- [x] docker安装
  - [x] [windows docker安装](https://www.runoob.com/docker/windows-docker-install.html)
  - [x] docker安装mysql
    - [x] 直接运行
    - [x] 外部volume挂载资料卷
- [x] ef生成数据库
  - [x] [EF介绍](https://docs.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
  - [x] 生成数据库
  - [x] 数据初始化
- [x] dapper操作数据
  - [x] [深入dapper](https://www.cnblogs.com/ITWeiHan/p/11614704.html)
  - [x] dapper contain使用
  - [x] dapper仓储在webapi的使用
  - [x] dapper事务，工作单元应用
- [x] webapi Restful
  - [x] [RESTful API最佳实践](http://www.ruanyifeng.com/blog/2018/10/restful-api-best-practices.html)
  - [x] PostMan工具的使用
- [x] 全局异常日志
- [x] [json patch](http://jsonpatch.com/)
- [x] 单元测试
  - [x] xunit+Moq+MemoryEFDbContext测试UserController
  - [x] FluentAPI写UserController测试用例
- [x] 部署
  - [x] GitLab CI完整部署UserAPI到线上测试环境
- [x] 授权服务
  - [x] User.Identity
- [ ] 网关
  - [x] [Ocelot](http://www.jessetalk.cn/2018/03/19/net-core-apigateway-ocelot-docs/)
  - [x] Ocelot集成Identity Server鉴权
  - [ ] [consul安装在docker](https://www.cnblogs.com/PearlRan/p/11225953.html)
  - [x] [consul安装](https://learn.hashicorp.com/consul/getting-started/agent)
  - [x] [conslu 服务注册与发现](http://michaco.net/blog/ServiceDiscoveryAndHealthChecksInAspNetCoreWithConsul)
  - [x] [Polly](http://www.jessetalk.cn/2018/03/25/asp-vnext-polly-docs/)
  - [x] [HttpClientFactory 和 Polly](https://docs.microsoft.com/zh-cn/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly)
### 接下来
- 消息队列数据交互
- 日志记录ELK
- 健康检查

## 参考资料
- [docker安装mysql命令]
