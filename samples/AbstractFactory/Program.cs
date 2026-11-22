// Abstract Factory：一次建立「一整組互相搭配的物件」，並保證不會混用不同家族的產品。

// 要用哪一個產品家族，到執行期才決定（實務上常來自設定檔或使用者的選擇）
string[] continents = ["Africa", "America"];

foreach (string continent in continents)
{
    IContinentFactory factory = ContinentFactories.Create(continent);
    var world = new AnimalWorld(factory);
    world.RunFoodChain();
}

// ---- AbstractFactory：宣告「這個家族有哪些產品」 ----
public interface IContinentFactory
{
    IHerbivore CreateHerbivore();
    ICarnivore CreateCarnivore();
}

// ---- ConcreteFactory：每個工廠只生產同一洲的動物 ----
public sealed class AfricaFactory : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Wildebeest();
    public ICarnivore CreateCarnivore() => new Lion();
}

public sealed class AmericaFactory : IContinentFactory
{
    public IHerbivore CreateHerbivore() => new Bison();
    public ICarnivore CreateCarnivore() => new Wolf();
}

// ---- AbstractProduct ----
public interface IHerbivore
{
    string Name { get; }
}

public interface ICarnivore
{
    string Name { get; }
    string Eat(IHerbivore prey);
}

// ---- ConcreteProduct ----
public sealed class Wildebeest : IHerbivore
{
    public string Name => "Wildebeest";
}

public sealed class Bison : IHerbivore
{
    public string Name => "Bison";
}

public sealed class Lion : ICarnivore
{
    public string Name => "Lion";
    public string Eat(IHerbivore prey) => $"{Name} eats {prey.Name}";
}

public sealed class Wolf : ICarnivore
{
    public string Name => "Wolf";
    public string Eat(IHerbivore prey) => $"{Name} eats {prey.Name}";
}

// ---- Client：只認得介面，完全不知道自己拿到的是哪一洲的動物 ----
public sealed class AnimalWorld(IContinentFactory factory)
{
    private readonly IHerbivore _herbivore = factory.CreateHerbivore();
    private readonly ICarnivore _carnivore = factory.CreateCarnivore();

    public void RunFoodChain() => Console.WriteLine(_carnivore.Eat(_herbivore));
}

// 「名稱 → 工廠」的對應集中在一個地方，新增家族時只需要改這裡
public static class ContinentFactories
{
    public static IContinentFactory Create(string continent) => continent switch
    {
        "Africa" => new AfricaFactory(),
        "America" => new AmericaFactory(),
        _ => throw new ArgumentException($"Unknown continent: {continent}", nameof(continent)),
    };
}
