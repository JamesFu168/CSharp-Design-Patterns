// Proxy：提供一個和真實物件「介面相同」的替身，在轉交呼叫的前後加上額外的控制。

ICalculator calculator = new CalculatorProxy();
Console.WriteLine("Proxy created. The real calculator has not been created yet.");

// 呼叫端使用 Proxy 的方式，和直接使用真實物件完全一樣
Console.WriteLine($"4 + 2 = {calculator.Add(4, 2)}");
Console.WriteLine($"4 * 2 = {calculator.Multiply(4, 2)}");
Console.WriteLine($"4 / 2 = {calculator.Divide(4, 2)}");

// ---- Subject：Proxy 與 RealSubject 共同的介面 ----
public interface ICalculator
{
    double Add(double x, double y);
    double Multiply(double x, double y);
    double Divide(double x, double y);
}

// ---- RealSubject：假設建立它的成本很高（例如要連線到遠端服務） ----
public sealed class RemoteCalculator : ICalculator
{
    public RemoteCalculator() => Console.WriteLine("  [RemoteCalculator] connecting to the math service...");

    public double Add(double x, double y) => x + y;
    public double Multiply(double x, double y) => x * y;
    public double Divide(double x, double y) => x / y;
}

// ---- Proxy：延後建立真實物件（Virtual Proxy），並記錄每一次呼叫 ----
public sealed class CalculatorProxy : ICalculator
{
    // Lazy<T>：第一次讀取 Value 時才執行建立動作，之後都回傳同一個物件
    private readonly Lazy<RemoteCalculator> _real = new(() => new RemoteCalculator());

    public double Add(double x, double y)
    {
        Log($"Add({x}, {y})");
        return _real.Value.Add(x, y);
    }

    public double Multiply(double x, double y)
    {
        Log($"Multiply({x}, {y})");
        return _real.Value.Multiply(x, y);
    }

    public double Divide(double x, double y)
    {
        Log($"Divide({x}, {y})");
        return _real.Value.Divide(x, y);
    }

    private static void Log(string call) => Console.WriteLine($"  [Proxy] {call}");
}
