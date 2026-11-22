// Strategy：把「可以互換的做法」抽出來，讓使用它的程式碼不必因為換做法而修改。

Student[] students =
[
    new("Samual", Id: 154, Score: 82),
    new("Jimmy", Id: 487, Score: 91),
    new("Sandra", Id: 655, Score: 77),
    new("Vivek", Id: 133, Score: 91),
    new("Anna", Id: 760, Score: 68),
];

var roster = new Roster(students);

// 做法一：策略是實作介面的物件
roster.Print(new ByName());
roster.Print(new ByScoreDescending());

// 做法二：策略只有一個方法時，直接傳入委派（lambda）就是策略
roster.Print("By id", list => list.OrderBy(student => student.Id));

public sealed record Student(string Name, int Id, int Score);

// ---- Strategy ----
public interface ISortStrategy
{
    string Name { get; }
    IEnumerable<Student> Sort(IEnumerable<Student> students);
}

// ---- ConcreteStrategy ----
public sealed class ByName : ISortStrategy
{
    public string Name => "By name";

    public IEnumerable<Student> Sort(IEnumerable<Student> students) =>
        students.OrderBy(student => student.Name, StringComparer.Ordinal);
}

public sealed class ByScoreDescending : ISortStrategy
{
    public string Name => "By score (high to low)";

    // 同分時再依姓名排序，結果才會固定
    public IEnumerable<Student> Sort(IEnumerable<Student> students) =>
        students.OrderByDescending(student => student.Score)
                .ThenBy(student => student.Name, StringComparer.Ordinal);
}

// ---- Context：只知道「有一個排序策略」，不知道也不在乎是哪一種 ----
public sealed class Roster(IReadOnlyList<Student> students)
{
    public void Print(ISortStrategy strategy) => Print(strategy.Name, strategy.Sort);

    public void Print(string title, Func<IEnumerable<Student>, IEnumerable<Student>> sort)
    {
        Console.WriteLine($"{title}:");
        foreach (Student student in sort(students))
        {
            Console.WriteLine($"  {student.Name,-7} id={student.Id}  score={student.Score}");
        }
    }
}
