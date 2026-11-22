// Command：把「一個操作」包成物件，於是操作可以被保存、排隊、復原與重做。

var calculator = new Calculator();
var history = new CommandHistory();

history.Execute(new CalculatorCommand(calculator, '+', 100));
history.Execute(new CalculatorCommand(calculator, '-', 50));
history.Execute(new CalculatorCommand(calculator, '*', 10));
history.Execute(new CalculatorCommand(calculator, '/', 2));

history.Undo(levels: 4);
history.Redo(levels: 3);

// ---- Command ----
public interface ICommand
{
    void Execute();
    void Undo();
}

// ---- ConcreteCommand：不可變，記住「對誰、做什麼、用什麼參數」 ----
public sealed record CalculatorCommand(Calculator Calculator, char Operator, decimal Operand) : ICommand
{
    public void Execute() => Calculator.Apply(Operator, Operand);

    // 復原 = 執行相反的運算
    public void Undo() => Calculator.Apply(Inverse(Operator), Operand);

    private static char Inverse(char @operator) => @operator switch
    {
        '+' => '-',
        '-' => '+',
        '*' => '/',
        '/' => '*',
        _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Unknown operator"),
    };
}

// ---- Receiver：真正執行工作的物件，不知道 Command 的存在 ----
public sealed class Calculator
{
    public decimal Value { get; private set; }

    public void Apply(char @operator, decimal operand)
    {
        Value = @operator switch
        {
            '+' => Value + operand,
            '-' => Value - operand,
            '*' => Value * operand,
            '/' => Value / operand,
            _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, "Unknown operator"),
        };

        Console.WriteLine($"Value = {Value,5} (after {@operator} {operand})");
    }
}

// ---- Invoker：只負責執行與保存命令，不知道命令的內容 ----
public sealed class CommandHistory
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void Execute(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear();   // 執行新命令後，先前被復原的命令就不能再重做
    }

    public void Undo(int levels)
    {
        Console.WriteLine($"---- Undo {levels} levels");
        while (levels-- > 0 && _undoStack.TryPop(out ICommand? command))
        {
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo(int levels)
    {
        Console.WriteLine($"---- Redo {levels} levels");
        while (levels-- > 0 && _redoStack.TryPop(out ICommand? command))
        {
            command.Execute();
            _undoStack.Push(command);
        }
    }
}
