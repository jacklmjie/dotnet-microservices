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
  - [x] [demo](https://github.com/jacklmjie/aspnetcoreboilerplate/tree/master/demo/DemoDapper)
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
### 网关与认证
- [ ] 授权服务
  - [x] User.Identity
  - [x] 获取profile(再次从网关把token传了一次)
  - [ ] oceloct自带传递方式
- [ ] 网关
  - [x] [Ocelot](http://www.jessetalk.cn/2018/03/19/net-core-apigateway-ocelot-docs/)
  - [x] Ocelot集成Identity Server鉴权
  - [x] [consul安装在docker](https://www.cnblogs.com/PearlRan/p/11225953.html)
  - [x] [consul安装](https://learn.hashicorp.com/consul/getting-started/agent)
  - [x] [conslu 服务注册与发现](http://michaco.net/blog/ServiceDiscoveryAndHealthChecksInAspNetCoreWithConsul)
  - [x] [Polly](http://www.jessetalk.cn/2018/03/25/asp-vnext-polly-docs/)
  - [x] [HttpClientFactory 和 Polly](https://docs.microsoft.com/zh-cn/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly)
### 通讯录服务
 - [UML类图几种关系](https://blog.csdn.net/tianhai110/article/details/6339565)
 - [MongoDB数据库,数据嵌套操作](https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-3.1&tabs=visual-studio)
 - [CAP集成RabbitMq](https://github.com/dotnetcore/CAP)
### 项目推荐服务
  - 三层,OOO和DDD区别
  - CQRS
    - [CQRS架构和传统架构的优缺点比较](http://www.techweb.com.cn/network/system/2017-07-07/2553563.shtml)
    - [MediatR](https://www.cnblogs.com/sheng-jie/p/10280336.html)

## DDD概念
- ### 值对象
  - 很多对象没有概念上的表示，他们描述了一个事务的某种特征。 用于描述领域的某个方面而本身没有概念表示的对象称为Value Object（值对象）
  - 在当前上下文很关键，没有唯一标识Id的,值对象是内聚并且可以具有行为(datetime就是一个值对象)  
  - 持久化问题，非关系型可以直接存josn，通过视图解析json查询
- ### 实体
  - 实体(Entity，又称为Reference Object) 很多对象不是通过他们的属性定义的，而是通过一连串的连续事件和标识定义的
  - 主要由标识定义的对象被称为ENTITY，有自己的唯一标识ID，还要许多属性和自己的行为
- ### 领域服务
  - 在某些情况下，最清楚、最实用的设计会包含一些特殊的操作，这些操作从概念上讲不属于任何对象。与其把它们强制地归于哪一类，不如顺其自然地在模型中引入一种新的元素，这就是Service（服务)
  - 领域中的某个要的过程或转换操作不属于实体或值对象的自然职责时，应该在模型中添加一个作为独立接口的操作，并将其声明为Service.定义接口时要使用模型语言，并确保操作名称是UBIQUITOUS LANGUAGE中的术语。此外，应该将Service定义为无状态的
  - 一些特殊的操作，这些操作从概念上讲不属于任何对象，领域服务中的操作，从领域的角度来看，领域服务是一个整体,（如果剥离了功能对他没影响）
	- RecommendFoodsService 推荐美食服务（到最后一个地方根据附近推荐美食是一个完整的领域服务）
- ### 应用服务
  - 应用服务是用来表达用例和用户故事（User Story)的主要手段
  - 应用层通过应用服务接口来暴露系统的全部功能。在应用服务的实现中，它负责编排和转发，它将要实现的功能委托给一个或多个领域对象来实现，它本身只负责处理业务用例的执行顺序以及结果的拼装
  - ItineraryApplicationService应用服务（推荐美食后并告知短信通知,推荐美食是领域服务，告知短信就是应用服务了）
- ### 聚合
  - 在具有复杂关联的模型中要想保证对象更改的一致性是很困难的。不仅互不关联的对象需要遵守一些固定规则，而且紧密关联的各组对象也要遵守一些固定规则。然而，过于谨慎的锁定机制又会导致多个用户之间臺无意义地互相干扰，从而使系统不可用
  - 首先，我们需要用一个抽象来封装模型中的引用。AGGREGATE就是一组相关对象的集合，我们把它作为数据修改的单元。每个AGGREGATE都有一个根(root)和一个边界(boundary).边界定义了AGGREGATE的内部都有什么。根则是AGGREGATE中所包含的一个特定Entity。在AGGREGATE中，根是唯一允许外部对象保持对它的引用的元素，而边界内部的对象之间则可以互相引用。除根以外的其他Entity都有本地标识，但这些标识只有在AGGREGATE内部才需要加以区别，因为外部对象除了根Entity之外看不到其他对象
  - 聚合根-行程和记账溥是一个整体，记一笔账，但是我们已经不能访问记账溥了，所以通过行程聚合根转移行为给记账溥
  - 考虑聚合根的重要一点是：在领域中我们是否会单独访问该实体，一个聚合在持久化的时候理应在一个事务中完成。但是当一个业务用例可能会操作多个聚合的时候，修改了聚合A的同时也更改了聚合B，这是一个很常见的操作，我们也必须保证多个聚合之间的一致性
  - 聚合是边界划分出来的结果，所以现在很多人会把微服务和DDD联系在一起，因为很有可能每一个划分出来的聚合就是一个微服务
- ### 仓储
  - 为每种需要全局访问的对象类型创建一个对象，这个对象就相当于该类型的所有对象在内存中的一个集合的“替身”。通过一个众所周知的接口来提供访问。提供添加和删除对象的方法，用这些方法来封装在数据存储中实际插入或删除数据的操作。提供根据具体标准来挑选对象的方法，并返回属性值满足查询标准的对象或对象集合（所返回的对象是完全实例化的），从而将实际的存储和查询技术封装起来。只为那些确实需要直接访问的Aggregate提供Repository。让客户始终聚焦于型，而将所有对象存储和访问操作交给Repository来完成
  - 仓储是为聚合而服务的，存储库不是一个对象。它是一个程序边界以及一个明确的约定，在其上命名方法时它需要的工作量与领域模型中的对象所需的工作量一样多。你的存储库约定应该是特定的以及能够揭示意图并对领域专家具有意义
  - 仓储是一个明确的约定，宽泛的条件是没有意义的，让仓储层完全丧失了原有的作用，它反而成了负担，为什么不直接使用DbContext对象呢
  - 仓储接口是应该放在领域层的，而仓储的实现可以放在基础设施层
- ### 工作单元
	- 事务管理主要与应用程序服务层有关。存储库只与使用聚合根的单一集合的管理有关，而业务用例可能会造成对多个类型聚合的更新。事务管理是由工作单元处理的。工作单元模式的作用是保持追踪业务任务期间聚合的所有变化。一旦所有的变化都已发生，则之后工作单元会协调事务中持久化存储的更新。如果在将变更提交到数据存储的中途出现了问题，那么要确保不损坏数据完整性的话，就要回滚所有的变更以确保数据保持有效的状态
	- 千万不能因为工作单元和仓储有联系就将它放置在领域层里面：事务的提供往往是由数据库管理程序来提供的，而这一类组件我们一般将它们放置在基础构架层，而领域层可以依赖于基础构架层，所以千万要注意，保持您的领域层足够干净，不要让其它的东西干扰它，也更不要将事务处理这类东西放到了您的领域层 

- ### 值对象持久化方案
	- 需要共享的值对象最好是用表，否则还是用字段
	- https://www.cnblogs.com/uoyo/p/12167360.html

- ### 领域事件
  - 领域专家所关心的发生在领域中的一些事件。 将领域中所发生的活动建模成一系列的离散事件。每个事件都用领域对象来表示，领域事件是领域模型的组成部分，表示领域中所发生的事情		
  - 内部的领域事件发生在边界之内，而外部的事件发生在边界之外（又叫集成事件,比如微服务A产生了一个事件，而微服务B会受到该事件的影响）
  - 将聚合根与聚合根之间的交互动作通过领域事件来传达，而将领域对象的策略运算交由领域服务完成。更清晰的划分它俩之间的职责
  - 领域服务以后另外一个聚合被独立出去了，改的比较少

## 参考资料  
- 图片都在docs文件夹下
