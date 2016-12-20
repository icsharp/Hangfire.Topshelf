# Hangfire项目实践分享

## 目录

<!-- TOC -->

- [Hangfire项目实践分享](#hangfire%E9%A1%B9%E7%9B%AE%E5%AE%9E%E8%B7%B5%E5%88%86%E4%BA%AB)
    - [目录](#%E7%9B%AE%E5%BD%95)
    - [什么是Hangfire](#%E4%BB%80%E4%B9%88%E6%98%AFhangfire)
        - [Hangfire基础](#hangfire%E5%9F%BA%E7%A1%80)
            - [基于队列的任务处理(Fire-and-forget jobs)](#%E5%9F%BA%E4%BA%8E%E9%98%9F%E5%88%97%E7%9A%84%E4%BB%BB%E5%8A%A1%E5%A4%84%E7%90%86fire-and-forget-jobs)
            - [延迟任务执行(Delayed jobs)](#%E5%BB%B6%E8%BF%9F%E4%BB%BB%E5%8A%A1%E6%89%A7%E8%A1%8Cdelayed-jobs)
            - [定时任务执行(Recurring jobs)](#%E5%AE%9A%E6%97%B6%E4%BB%BB%E5%8A%A1%E6%89%A7%E8%A1%8Crecurring-jobs)
            - [延续性任务执行(Continuations)](#%E5%BB%B6%E7%BB%AD%E6%80%A7%E4%BB%BB%E5%8A%A1%E6%89%A7%E8%A1%8Ccontinuations)
        - [与quartz.net对比](#%E4%B8%8Equartznet%E5%AF%B9%E6%AF%94)
    - [Hangfire扩展](#hangfire%E6%89%A9%E5%B1%95)
        - [Hangfire Dashborad日志查看](#hangfire-dashborad%E6%97%A5%E5%BF%97%E6%9F%A5%E7%9C%8B)
        - [Hangfire Dashborad授权](#hangfire-dashborad%E6%8E%88%E6%9D%83)
        - [IOC容器之Autofac](#ioc%E5%AE%B9%E5%99%A8%E4%B9%8Bautofac)
        - [RecurringJob扩展](#recurringjob%E6%89%A9%E5%B1%95)
            - [使用特性`RecurringJobAttribute`发现定时任务](#%E4%BD%BF%E7%94%A8%E7%89%B9%E6%80%A7recurringjobattribute%E5%8F%91%E7%8E%B0%E5%AE%9A%E6%97%B6%E4%BB%BB%E5%8A%A1)
            - [使用json配置文件注册定时任务](#%E4%BD%BF%E7%94%A8json%E9%85%8D%E7%BD%AE%E6%96%87%E4%BB%B6%E6%B3%A8%E5%86%8C%E5%AE%9A%E6%97%B6%E4%BB%BB%E5%8A%A1)
        - [与MSMQ集成](#%E4%B8%8Emsmq%E9%9B%86%E6%88%90)
        - [持久化存储之Redis](#%E6%8C%81%E4%B9%85%E5%8C%96%E5%AD%98%E5%82%A8%E4%B9%8Bredis)
    - [Hangfire最佳实践](#hangfire%E6%9C%80%E4%BD%B3%E5%AE%9E%E8%B7%B5)
        - [配置最大job并发处理数](#%E9%85%8D%E7%BD%AE%E6%9C%80%E5%A4%A7job%E5%B9%B6%E5%8F%91%E5%A4%84%E7%90%86%E6%95%B0)
        - [使用 `DisplayNameAttribute`特性构造缺省的JobName](#%E4%BD%BF%E7%94%A8-displaynameattribute%E7%89%B9%E6%80%A7%E6%9E%84%E9%80%A0%E7%BC%BA%E7%9C%81%E7%9A%84jobname)
        - [Hangfire在调用Background/RecurringJob创建job时应尽量使传入的参数简单.](#hangfire%E5%9C%A8%E8%B0%83%E7%94%A8backgroundrecurringjob%E5%88%9B%E5%BB%BAjob%E6%97%B6%E5%BA%94%E5%B0%BD%E9%87%8F%E4%BD%BF%E4%BC%A0%E5%85%A5%E7%9A%84%E5%8F%82%E6%95%B0%E7%AE%80%E5%8D%95)
        - [为Hangfire客户端调用定义统一的REST APIs](#%E4%B8%BAhangfire%E5%AE%A2%E6%88%B7%E7%AB%AF%E8%B0%83%E7%94%A8%E5%AE%9A%E4%B9%89%E7%BB%9F%E4%B8%80%E7%9A%84rest-apis)
        - [利用Topshelf + Owin Host将hangfire server 宿主到Windows Service.](#%E5%88%A9%E7%94%A8topshelf--owin-host%E5%B0%86hangfire-server-%E5%AE%BF%E4%B8%BB%E5%88%B0windows-service)
        - [日志配置](#%E6%97%A5%E5%BF%97%E9%85%8D%E7%BD%AE)
    - [Hangfire多实例部署（高可用）](#hangfire%E5%A4%9A%E5%AE%9E%E4%BE%8B%E9%83%A8%E7%BD%B2%E9%AB%98%E5%8F%AF%E7%94%A8)
        - [HF.Samples.Consumer](#hfsamplesconsumer)
        - [HF.Samples.APIs](#hfsamplesapis)
        - [HF.Samples.Console](#hfsamplesconsole)
        - [HF.Samples.ServerNode](#hfsamplesservernode)

<!-- /TOC -->

项目中使用Hangfire已经快一年了，期间经历过很多次的试错及升级优化，才达到现在的稳定效果。趁最近不是太忙，自己在github上做了个案列，也是拿来跟大家分享下，案例是从项目里剥离出来的，有兴趣的可以访问 [这里](https://github.com/icsharp/Hangfire.Topshelf).

## 什么是Hangfire

[Hangfire](http://hangfire.io/) 是一个开源的.NET任务调度框架，目前1.6+版本已支持.NET Core。个人认为它最大特点在于内置提供集成化的控制台,方便后台查看及监控：

<img src="http://images2015.cnblogs.com/blog/41545/201612/41545-20161220124034386-1317745507.png" width="70%" height="70%"/>

另外，Hangfire包含三大核心组件：客户端、持久化存储、服务端，官方的流程介绍图如下：

![hangfire-workflow](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220061245103-587175152.png)

从图中可以看出，这三个核心组件是可以分离出来单独部署的，例如可以部署多台Hangfire服务，提高处理后台任务的吞吐量。关于任务持久化存储，支持Sqlserver，MongoDb，Mysql或是Redis等等。

### Hangfire基础

#### 基于队列的任务处理(Fire-and-forget jobs)

基于队列的任务处理是Hangfire中最常用的，客户端使用`BackgroundJob`类的静态方法`Enqueue`来调用，传入指定的方法（或是匿名函数），Job Queue等参数.

```csharp
var jobId = BackgroundJob.Enqueue(
    () => Console.WriteLine("Fire-and-forget!"));
```

在任务被持久化到数据库之后，Hangfire服务端立即从数据库获取相关任务并装载到相应的Job Queue下，在没有异常的情况下仅处理一次，若发生异常，提供重试机制，异常及重试信息都会被记录到数据库中，通过Hangfire控制面板可以查看到这些信息。

#### 延迟任务执行(Delayed jobs)

延迟（计划）任务跟队列任务相似，客户端调用时需要指定在一定时间间隔后调用：

```csharp
var jobId = BackgroundJob.Schedule(
    () => Console.WriteLine("Delayed!"),
    TimeSpan.FromDays(7));
```

#### 定时任务执行(Recurring jobs)

定时（循环）任务代表可以重复性执行多次，支持`CRON`表达式：

```csharp
RecurringJob.AddOrUpdate(
    () => Console.WriteLine("Recurring!"),
    Cron.Daily);
```

#### 延续性任务执行(Continuations)

延续性任务类似于.NET中的`Task`,可以在第一个任务执行完之后紧接着再次执行另外的任务：

```csharp
BackgroundJob.ContinueWith(
    jobId,
    () => Console.WriteLine("Continuation!"));
```

其实还有批量任务处理，批量任务延续性处理(Batch Continuations)，但这个需要商业授权及收费。在我看来，官方提供的开源版本已经基本够用。

### 与quartz.net对比

在项目没有引入Hangfire之前，一直使用的是Quartz.net。个人认为Quartz.net在定时任务处理方面优势如下：

- 支持秒级单位的定时任务处理，但是Hangfire只能支持分钟及以上的定时任务处理

原因在于Hangfire用的是开源的[NCrontab](https://github.com/atifaziz/NCrontab)组件，跟linux上的crontab指令相似。

- 更加复杂的触发器，日历以及任务调度处理

- 可配置的定时任务

但是为什么要换Hangfire? 很大的原因在于项目需要一个后台可监控的应用，不用每次都要从服务器拉取日志查看，在没有ELK的时候相当不方便。Hangfire控制面板不仅提供监控，也可以手动的触发执行定时任务。如果在定时任务处理方面没有很高的要求，比如一定要5s定时执行，Hangfire值得拥有。抛开这些，Hangfire优势太明显了：

- 持久化保存任务、队列、统计信息

- 重试机制

- 多语言支持

- 支持任务取消

- 支持按指定`Job Queue`处理任务

- 服务器端工作线程可控，即job执行并发数控制

- 分布式部署，支持高可用

- 良好的扩展性，如支持IOC、Hangfire Dashboard授权控制、Asp.net Core、持久化存储等

说了这么多的优点，我们可以有个案例，例如秒杀场景：用户下单->订单生成->扣减库存，Hangfire对于这种分布式的应用处理也是适用的，最后会给出实现。

## Hangfire扩展

重点说一下上面提到的第8点，`Hangfire扩展性`，大家可以参考 [这里](http://http://hangfire.io/extensions.html)，有几个扩展是很实用的.

### Hangfire Dashborad日志查看

[Hangfire.Console](https://github.com/pieceofsummer/Hangfire.Console)提供类似于console-like的日志体验，与Hangfire dashboard集成：

![Hangfire.Console](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220125436823-1629456307.png)

用法如下：

```csharp
public void SimpleJob(PerformContext context)
{
    context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} SimpleJob Running ...");

    var progressBar = context.WriteProgressBar();

    foreach (var i in Enumerable.Range(1, 50).ToList().WithProgress(progressBar))
    {
        System.Threading.Thread.Sleep(1000);
    }
}
```

不仅支持日志输入到控制面板，也支持在线进度条展示.

### Hangfire Dashborad授权

[Hangfire.Dashboard.Authorization](https://github.com/HangfireIO/Hangfire.Dashboard.Authorization)这个扩展应该都能理解，给Hangfire Dashboard
提供授权机制，仅授权的用户才能访问。其中提供两种授权机制：

- OWIN-based authentication
- Basic authentication

可以参考提供[案例](https://github.com/icsharp/Hangfire.Topshelf) ，我实现的是基本认证授权:

``` csharp
var options = new DashboardOptions
{
    AppPath = HangfireSettings.Instance.AppWebSite,
    AuthorizationFilters = new[]
    {
        new BasicAuthAuthorizationFilter ( new BasicAuthAuthorizationFilterOptions
        {
            SslRedirect = false,
            RequireSsl = false,
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = HangfireSettings.Instance.LoginUser,
                    // Password as plain text
                    PasswordClear = HangfireSettings.Instance.LoginPwd
                }

            }
        } )
    }
};
app.UseHangfireDashboard("", options);
```

### IOC容器之Autofac

Hangfire对于每一个任务(Job)假如都写在一个类里，然后使用`BackgroundJob`/`RecurringJob`对方法(实例或静态)进行调用，这样会导致模块间太多耦合。实际项目中，依赖倒置原则可以降低模块之间的耦合性，Hangfire也提供了IOC扩展，其本质是重写`JobActivator`类。

[Hangfire.Autofac](https://github.com/HangfireIO/Hangfire.Autofac)是官方提供的开源扩展，用法参考如下：

```csharp
GlobalConfiguration.Configuration.UseAutofacActivator(container);
```

### RecurringJob扩展

<img src="http://images2015.cnblogs.com/blog/41545/201612/41545-20161220125646745-718474066.png" width="70%" height="70%"/>

关于`RecurringJob`定时任务，我写了一个扩展 [RecurringJobExtensions](https://github.com/icsharp/Hangfire.RecurringJobExtensions),在使用上做了一下增强，具体有两点：

#### 使用特性`RecurringJobAttribute`发现定时任务

```csharp
public class RecurringJobService
{
    [RecurringJob("*/1 * * * *")]
    [DisplayName("InstanceTestJob")]
    [Queue("jobs")]
    public void InstanceTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} InstanceTestJob Running ...");
    }

    [RecurringJob("*/5 * * * *")]
    [DisplayName("JobStaticTest")]
    [Queue("jobs")]
    public static void StaticTestJob(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} StaticTestJob Running ...");
    }
}
```

#### 使用json配置文件注册定时任务

```csharp
[AutomaticRetry(Attempts = 0)]
[DisableConcurrentExecution(90)]
public class LongRunningJob : IRecurringJob
{
    public void Execute(PerformContext context)
    {
        context.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} LongRunningJob Running ...");

        var runningTimes = context.GetJobData<int>("RunningTimes");

        context.WriteLine($"get job data parameter-> RunningTimes: {runningTimes}");

        var progressBar = context.WriteProgressBar();

        foreach (var i in Enumerable.Range(1, runningTimes).ToList().WithProgress(progressBar))
        {
            Thread.Sleep(1000);
        }
    }
}
```

Json配置文件如下：

```json
[{
    "job-name": "Long Running Job",
    "job-type": "Hangfire.Samples.LongRunningJob, Hangfire.Samples",
    "cron-expression": "*/2 * * * *",
    "job-data": {
        "RunningTimes": 300
    }
}]
```

实现接口`IRecurringJob`来定义具体的定时任务，这样的写法与Quartz.net相似，可以很方便的实现Quartz.net到Hangfire的迁移。类似地，参考了quartz.net，
使用`job-data-map`这样的方式来定义整个任务执行期间的上下文有状态的job.

``` csharp
var runningTimes = context.GetJobData<int>("RunningTimes");
```

详细用法可以直接参考项目[文档](https://github.com/icsharp/Hangfire.RecurringJobExtensions/blob/master/README.md)。

### 与MSMQ集成

Hangfire server在处理每个job时，会将job先装载到事先定义好的job queue中，比如一次性加载1000个job,在默认的sqlsever实现中是直接将这些job queue中的
job id储存到数据库中，然后再取出执行。大量的job会造成任务的延迟性执行，所以更有效的方式是将任务直接加载到MSMQ中。

实际应用中，MSMQ队列不存在时一定要手工创建，而且必须是事务性的队列，权限也要设置，用法如下:

```csharp
public static IGlobalConfiguration<SqlServerStorage> UseMsmq(this IGlobalConfiguration<SqlServerStorage> configuration, string pathPattern, params string[] queues)
{
    if (string.IsNullOrEmpty(pathPattern)) throw new ArgumentNullException(nameof(pathPattern));
    if (queues == null) throw new ArgumentNullException(nameof(queues));

    foreach (var queueName in queues)
    {
        var path = string.Format(pathPattern, queueName);

        if (!MessageQueue.Exists(path))
            using (var queue = MessageQueue.Create(path, transactional: true))
                queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
    }
    return configuration.UseMsmqQueues(pathPattern, queues);
}
```


### 持久化存储之Redis

Hangfire中定义的job存储到sqlserver不是性能最好的选择，使用Redis存储，性能将会是巨大提升(下图来源于[Hangfire.Pro.Redis](http://hangfire.io/pro/#hangfireproredis)).

![storage-compare](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220061602057-1909240597.png)

`Hangfire.Pro`提供了基于`servicestack.redis`的redis扩展组件，然而商业收费，不开源。

但是，有另外的基于`StackExchange.Redis`的开源实现 [Hangfire.Redis.StackExchange](https://github.com/marcoCasamento/Hangfire.Redis.StackExchange)，
github上一直在维护，支持.NET Core，项目实测稳定可用. 该扩展相当简单：

```icsharp
services.AddHangfire(x =>
{
    var connectionString = Configuration.GetConnectionString("hangfire.redis");
    x.UseRedisStorage(connectionString);
});
```

## Hangfire最佳实践

<img src="http://images2015.cnblogs.com/blog/41545/201612/41545-20161220061420728-850974488.png" width="70%" height="70%"/>

### 配置最大job并发处理数

Hangfire server在启动时会初始化一个最大Job处理并发数量的阈值，系统默认为20,可以根据服务器配置设置并发处理数。最大阈值的定义除了考虑服务器配置以外，
也需要考虑数据库的最大连接数，定义太多的并发处理数量可能会在同一时间耗尽数据连接池。

```csharp
app.UseHangfireServer(new BackgroundJobServerOptions
{
    //wait all jobs performed when BackgroundJobServer shutdown.
    ShutdownTimeout = TimeSpan.FromMinutes(30),
    Queues = queues,
    WorkerCount = Math.Max(Environment.ProcessorCount, 20)
});
```

### 使用 `DisplayNameAttribute`特性构造缺省的JobName

```csharp
public interface IOrderService : IAppService
{
    /// <summary>
    /// Creating order from product.
    /// </summary>
    /// <param name="productId"></param>
    [AutomaticRetry(Attempts = 3)]
    [DisplayName("Creating order from product, productId:{0}")]
    [Queue("apis")]
    void CreateOrder(int productId);
}
```

![DisplayNameAttribute](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220131129807-73353270.png)

目前netstandard暂不支持缺省的jobname,因为需要单独引用组件`System.ComponentModel.Primitives`,hangfire官方给出的答复是尽量保证少的`Hangfire.Core`组件的依赖。

### Hangfire在调用Background/RecurringJob创建job时应尽量使传入的参数简单.

Hangfire job中参数（包括参数值）及方法名都序列化为json持久化到数据库中，所以参数应尽量简单，如传入单据ID，这样才不会使Job Storage呈爆炸性增长。

### 为Hangfire客户端调用定义统一的REST APIs

定义统一的REST APIs可以规范并集中管理整个项目的hangfire客户端调用，同时避免到处引用hangfire组件。使用例如Swagger这样的组件来给不同的应用方(Consumer)提供文档帮助，应用方可以是App,Webservice,Microservices等。

```csharp
/// <summary>
/// Creating order from product.
/// </summary>
/// <param name="productId"></param>
/// <returns></returns>
[Route("create")]
[HttpPost]
public IActionResult Create([FromBody]string productId)
{
    if (string.IsNullOrEmpty(productId))
        return BadRequest();

    var jobId = BackgroundJob.Enqueue<IOrderService>(x => x.CreateOrder(productId));

    BackgroundJob.ContinueWith<IInventoryService>(jobId, x => x.Reduce(productId));

    return Ok(new { Status = 1, Message = $"Enqueued successfully, ProductId->{productId}" });
}
```

### 利用Topshelf + Owin Host将hangfire server 宿主到Windows Service.

不推荐将hangfire server 宿主到如ASP.NET application 中，需要有一堆[配置](http://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html)。个人喜好问题，推荐将hangfire server 单独部署到windows service, 利用Topshelf+Owin Host：

```csharp
/// <summary>
/// OWIN host
/// </summary>
public class Bootstrap : ServiceControl
{
    private static readonly ILog _logger = LogProvider.For<Bootstrap>();
    private IDisposable webApp;
    public string Address { get; set; }
    public bool Start(HostControl hostControl)
    {
        try
        {
            webApp = WebApp.Start<Startup>(Address);
            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorException("Topshelf starting occured errors.", ex);
            return false;
        }

    }

    public bool Stop(HostControl hostControl)
    {
        try
        {
            webApp?.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorException($"Topshelf stopping occured errors.", ex);
            return false;
        }

    }
}
```

### 日志配置

从`Hangfire 1.3.0`开始，Hangfire引入了日志组件[LibLog](https://github.com/damianh/LibLog),所以应用不需要做任何改动就可以兼容如下日志组件：

- Serilog

- NLog

- Log4Net

- EntLib Logging

- Loupe

- Elmah

例如，配置 [serilog](https://github.com/serilog/serilog)如下，LibLog组件会自动发现并使用serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.LiterateConsole()
    .WriteTo.RollingFile("logs\\log-{Date}.txt")
    .CreateLogger();
```

## Hangfire多实例部署（高可用）


下图是一个多实例Hangfire服务部署：

<img src="http://images2015.cnblogs.com/blog/41545/201612/41545-20161220061513182-418802853.png" width="70%" height="70%"/>

其中，关于Hangfire Server Node 节点可以根据实际需要水平扩展.

上述提到过一个秒杀场景：用户下单->订单生成->扣减库存，实现参考github项目[Hangfire.Topshelf](https://github.com/icsharp/Hangfire.Topshelf).

![sln](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220055058525-1919516987.png)

### HF.Samples.Consumer

服务应用消费方(App/Webservice/Microservices等。)

### HF.Samples.APIs

统一的REST APIs管理

![REST APIs](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220125933448-1145970499.png)

### HF.Samples.Console

Hangfire 控制面板

### HF.Samples.ServerNode

Hangfire server node cli 工具，使用如下：

```shell
@echo off
set dir="cluster"
dotnet run -p %dir%\HF.Samples.ServerNode nodeA -q order -w 100
dotnet run -p %dir%\HF.Samples.ServerNode nodeB -q storage -w 100
```

上述脚本为创建两个Hangfire server nodeA, nodeB分别用来处理订单、仓储服务。

-q 指定hangfire server 需要处理的队列，-w表示Hangfire server 并发处理job数量。

可以为每个job queue创建一个hangfire实例来处理更多的job.

![servernode-cli](http://images2015.cnblogs.com/blog/41545/201612/41545-20161220130052792-2110379342.png)