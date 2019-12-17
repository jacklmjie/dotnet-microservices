## 微服务系列
### .NET WEB API实战一（用户服务）
- docker安装
  - windows安装docker
  - docker简单操作，配置阿里云加速，拉取镜像包
  - docker安装mysql
    - 直接运行
    - 外部volume挂载资料卷
- ef生成数据库
  - ef介绍
  - 生成数据库
  - 数据初始化
- dapper操作数据
  - dapper介绍
  - dapper原理
  - dapper contain使用
  - dapper仓储在webapi的使用
  - dapper事务，工作单元应用
- webapi Restful
  - Restful风格介绍
  - Restful风格使用
  - PostMan工具的使用
- 全局异常日志
- json patch
  - json patch介绍
  - json patch操作
  - json patch数组
- 单元测试
  - xunit+Moq+MemoryEFDbContext测试UserController
  - FluentAPI写UserController测试用例
- 部署
  - GitLab CI完整部署UserAPI到线上测试环境
- 认证服务
  - User.Identity
  - ![](https://github.com/jacklmjie/microservices/blob/master/docs/%E8%AE%A4%E8%AF%81%E6%9C%8D%E5%8A%A1.png)

### .NET WEB API实战二（用户服务）
- 消息队列数据交互
- 日志记录ELK
- 健康检查

## 参考资料
- [windows docker安装](https://www.runoob.com/docker/windows-docker-install.html)
- [docker安装mysql命令]
- [EF迁移命令](https://docs.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
- [深入dapper](https://www.cnblogs.com/ITWeiHan/p/11614704.html)
- [RESTful API最佳实践](http://www.ruanyifeng.com/blog/2018/10/restful-api-best-practices.html)
- [json patch](http://jsonpatch.com/)
