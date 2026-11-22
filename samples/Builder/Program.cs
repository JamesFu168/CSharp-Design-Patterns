// Builder：把「一步一步組裝」和「最後的成品」分開，成品一旦建好就不再改變。

// 透過 Director 使用現成的配方
Vehicle scooter = VehicleShop.BuildScooter(new VehicleBuilder());
Vehicle car = VehicleShop.BuildCar(new VehicleBuilder());
Console.WriteLine(scooter);
Console.WriteLine(car);

// 也可以不透過 Director，自己決定每一個步驟
Vehicle trike = new VehicleBuilder()
    .OfType("Trike")
    .WithFrame("Trike Frame")
    .WithEngine("125 cc")
    .WithWheels(3)
    .Build();
Console.WriteLine(trike);

// 少了必要步驟時，Build() 會立刻說明缺了什麼，而不是等到使用成品時才出錯
try
{
    new VehicleBuilder().OfType("Broken").WithWheels(2).Build();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Build failed: {ex.Message}");
}

// ---- Product：不可變的成品 ----
public sealed record Vehicle(string Type, string Frame, string Engine, int Wheels, int Doors)
{
    public override string ToString() =>
        $"{Type}: frame={Frame}, engine={Engine}, wheels={Wheels}, doors={Doors}";
}

// ---- Builder：負責收集零件，並在 Build() 檢查成品是否完整 ----
public sealed class VehicleBuilder
{
    private string? _type;
    private string? _frame;
    private string? _engine;
    private int _wheels;
    private int _doors;

    // 每個步驟都回傳 this，呼叫端才能用「.」一路串下去（Fluent API）
    public VehicleBuilder OfType(string type) { _type = type; return this; }
    public VehicleBuilder WithFrame(string frame) { _frame = frame; return this; }
    public VehicleBuilder WithEngine(string engine) { _engine = engine; return this; }
    public VehicleBuilder WithWheels(int wheels) { _wheels = wheels; return this; }
    public VehicleBuilder WithDoors(int doors) { _doors = doors; return this; }

    public Vehicle Build()
    {
        if (_type is null) throw new InvalidOperationException("Type is required.");
        if (_frame is null) throw new InvalidOperationException("Frame is required.");
        if (_engine is null) throw new InvalidOperationException("Engine is required.");
        if (_wheels <= 0) throw new InvalidOperationException("Wheels must be greater than zero.");

        return new Vehicle(_type, _frame, _engine, _wheels, _doors);
    }
}

// ---- Director：知道「標準車款要哪些步驟」，但不知道成品內部長什麼樣子 ----
public static class VehicleShop
{
    public static Vehicle BuildScooter(VehicleBuilder builder) => builder
        .OfType("Scooter")
        .WithFrame("Scooter Frame")
        .WithEngine("50 cc")
        .WithWheels(2)
        .Build();

    public static Vehicle BuildCar(VehicleBuilder builder) => builder
        .OfType("Car")
        .WithFrame("Car Frame")
        .WithEngine("2500 cc")
        .WithWheels(4)
        .WithDoors(4)
        .Build();
}
