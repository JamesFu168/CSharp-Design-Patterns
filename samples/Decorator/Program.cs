// Decorator：用「包一層」的方式替物件加上功能，不必修改原類別，也不必為每種組合建立子類別。

ILibraryItem book = new Book("Inside ASP.NET", "Worley", copies: 10);
book.Display();

ILibraryItem video = new Video("Jaws", "Spielberg", copies: 23, playTimeMinutes: 92);
video.Display();

// 包一層 Borrowable：影片本身的類別完全沒有改動
var borrowable = new Borrowable(video);
borrowable.Borrow("Customer #1");
borrowable.Borrow("Customer #2");
borrowable.Display();

// 裝飾者也實作同一個介面，所以可以一層再包一層
ILibraryItem featured = new Featured(borrowable);
featured.Display();

// ---- Component ----
public interface ILibraryItem
{
    string Title { get; }
    int AvailableCopies { get; }
    void Display();
}

// ---- ConcreteComponent ----
public sealed class Book(string title, string author, int copies) : ILibraryItem
{
    public string Title => title;
    public int AvailableCopies => copies;

    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine($"Book: {title}");
        Console.WriteLine($"  Author: {author}");
        Console.WriteLine($"  Copies: {copies}");
    }
}

public sealed class Video(string title, string director, int copies, int playTimeMinutes) : ILibraryItem
{
    public string Title => title;
    public int AvailableCopies => copies;

    public void Display()
    {
        Console.WriteLine();
        Console.WriteLine($"Video: {title}");
        Console.WriteLine($"  Director: {director}");
        Console.WriteLine($"  Copies: {copies}");
        Console.WriteLine($"  Playtime: {playTimeMinutes} min");
    }
}

// ---- Decorator：預設把所有呼叫原封不動轉給內層 ----
public abstract class LibraryItemDecorator(ILibraryItem inner) : ILibraryItem
{
    public virtual string Title => inner.Title;
    public virtual int AvailableCopies => inner.AvailableCopies;
    public virtual void Display() => inner.Display();
}

// ---- ConcreteDecorator：加上「可借閱」的行為 ----
public sealed class Borrowable(ILibraryItem inner) : LibraryItemDecorator(inner)
{
    private readonly List<string> _borrowers = [];

    // 借閱紀錄存在裝飾者自己身上，不去修改內層物件的狀態
    public override int AvailableCopies => base.AvailableCopies - _borrowers.Count;

    public void Borrow(string customer)
    {
        if (AvailableCopies == 0)
        {
            throw new InvalidOperationException($"No copies of '{Title}' left.");
        }

        _borrowers.Add(customer);
    }

    public void Return(string customer) => _borrowers.Remove(customer);

    public override void Display()
    {
        base.Display();
        Console.WriteLine($"  Available: {AvailableCopies}");
        foreach (string borrower in _borrowers)
        {
            Console.WriteLine($"  Borrower: {borrower}");
        }
    }
}

// ---- ConcreteDecorator：加上「本月推薦」的標示 ----
public sealed class Featured(ILibraryItem inner) : LibraryItemDecorator(inner)
{
    public override void Display()
    {
        base.Display();
        Console.WriteLine("  ** Staff pick of the month **");
    }
}
