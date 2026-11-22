// Adapter：既有類別的介面不合用、又不能修改它時，寫一個轉接類別把它包成我們想要的介面。

var databank = new ChemicalDatabank();

ICompound[] compounds =
[
    new CompoundAdapter(databank, "Water"),
    new CompoundAdapter(databank, "Benzene"),
    new CompoundAdapter(databank, "Ethanol"),
];

foreach (ICompound compound in compounds)
{
    Console.WriteLine($"Compound: {compound.Name}");
    Console.WriteLine($"  Formula   : {compound.Formula}");
    Console.WriteLine($"  Weight    : {compound.MolecularWeight}");
    Console.WriteLine($"  Melting Pt: {compound.MeltingPoint}");
    Console.WriteLine($"  Boiling Pt: {compound.BoilingPoint}");
}

// ---- Target：我們的程式希望使用的介面 ----
public interface ICompound
{
    string Name { get; }
    string Formula { get; }
    double MolecularWeight { get; }
    double MeltingPoint { get; }
    double BoilingPoint { get; }
}

// ---- Adapter：用「組合」持有 Adaptee，把呼叫轉換過去 ----
public sealed class CompoundAdapter(ChemicalDatabank databank, string name) : ICompound
{
    public string Name => name;
    public string Formula => databank.GetMolecularStructure(name);
    public double MolecularWeight => databank.GetMolecularWeight(name);

    // 舊 API 用 "M" / "B" 這種代碼區分熔點與沸點，這個細節只留在 Adapter 裡
    public double MeltingPoint => databank.GetCriticalPoint(name, "M");
    public double BoilingPoint => databank.GetCriticalPoint(name, "B");
}

// ---- Adaptee：既有或第三方的 API，假設我們不能修改它 ----
public sealed class ChemicalDatabank
{
    public double GetCriticalPoint(string compound, string point) => (compound.ToLowerInvariant(), point) switch
    {
        ("water", "M") => 0.0,
        ("water", "B") => 100.0,
        ("benzene", "M") => 5.5,
        ("benzene", "B") => 80.1,
        ("ethanol", "M") => -114.1,
        ("ethanol", "B") => 78.3,
        _ => 0d,
    };

    public string GetMolecularStructure(string compound) => compound.ToLowerInvariant() switch
    {
        "water" => "H2O",
        "benzene" => "C6H6",
        "ethanol" => "C2H5OH",
        _ => "",
    };

    public double GetMolecularWeight(string compound) => compound.ToLowerInvariant() switch
    {
        "water" => 18.015,
        "benzene" => 78.1134,
        "ethanol" => 46.0688,
        _ => 0d,
    };
}
