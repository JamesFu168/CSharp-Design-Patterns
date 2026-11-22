// Singleton：整個應用程式只有一個實例。現代 .NET 通常交給 DI 容器來保證，而不是自己寫。

using Microsoft.Extensions.DependencyInjection;

// ===== 做法一：傳統 Singleton（類別自己保證只有一個實例） =====
LoadBalancer b1 = LoadBalancer.Instance;
LoadBalancer b2 = LoadBalancer.Instance;
Console.WriteLine($"Same instance: {ReferenceEquals(b1, b2)}");

for (int i = 0; i < 6; i++)
{
    // 每個請求只呼叫一次 NextServer()，再讀取同一台伺服器的屬性
    Server server = b1.NextServer();
    Console.WriteLine($"Dispatch request to: {server.Name} ({server.Ip})");
}

// ===== 做法二：交給 DI 容器管理生命週期（實務首選） =====
var services = new ServiceCollection();
services.AddSingleton<IReadOnlyList<Server>>(ServerFarm.Default);
services.AddSingleton<ILoadBalancer, RoundRobinLoadBalancer>();

using ServiceProvider provider = services.BuildServiceProvider();
ILoadBalancer d1 = provider.GetRequiredService<ILoadBalancer>();
ILoadBalancer d2 = provider.GetRequiredService<ILoadBalancer>();
Console.WriteLine($"Same instance from DI: {ReferenceEquals(d1, d2)}");
Console.WriteLine($"Dispatch request to: {d1.NextServer().Name}");

public sealed record Server(string Name, string Ip);

public static class ServerFarm
{
    public static readonly IReadOnlyList<Server> Default =
    [
        new("ServerI", "120.14.220.18"),
        new("ServerII", "120.14.220.19"),
        new("ServerIII", "120.14.220.20"),
        new("ServerIV", "120.14.220.21"),
        new("ServerV", "120.14.220.22"),
    ];
}

public interface ILoadBalancer
{
    Server NextServer();
}

// ---- 傳統 Singleton ----
public sealed class LoadBalancer : ILoadBalancer
{
    // Lazy<T> 保證：第一次用到時才建立，而且多個執行緒同時存取也只會建立一次
    private static readonly Lazy<LoadBalancer> LazyInstance = new(() => new LoadBalancer());

    public static LoadBalancer Instance => LazyInstance.Value;

    private readonly IReadOnlyList<Server> _servers = ServerFarm.Default;
    private readonly Lock _gate = new();
    private int _next;

    // private 建構函式：外部無法 new，只能透過 Instance 取得
    private LoadBalancer() { }

    public Server NextServer()
    {
        // 只有一個實例不代表執行緒安全：它的可變狀態（_next）仍然要自己保護
        lock (_gate)
        {
            Server server = _servers[_next];
            _next = (_next + 1) % _servers.Count;
            return server;
        }
    }
}

// ---- 交給 DI 的版本：只是一個普通類別，「只有一個」由容器的註冊方式決定 ----
public sealed class RoundRobinLoadBalancer(IReadOnlyList<Server> servers) : ILoadBalancer
{
    private readonly Lock _gate = new();
    private int _next;

    public Server NextServer()
    {
        lock (_gate)
        {
            Server server = servers[_next];
            _next = (_next + 1) % servers.Count;
            return server;
        }
    }
}
