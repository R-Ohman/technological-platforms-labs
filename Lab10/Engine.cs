namespace Lab10;

public class Engine : IComparable
{
    public double Displacement { get; set; }
    public double Horsepower { get; set; }
    public string Model { get; set; }

    public Engine() {}

    public Engine(double displacement, double horsepower, string model)
    {
        Displacement = displacement;
        Horsepower = horsepower;
        Model = model;
    }

    public override string ToString()
    {
        return $"Model: {Model},  Horsepower: {Horsepower}, Displacement: {Displacement}";
    }

    public int CompareTo(object? obj)
    {
        return Horsepower.CompareTo(((Engine)obj).Horsepower);
    }
}