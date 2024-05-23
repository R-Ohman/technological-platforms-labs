namespace Lab10;

public class Car
{
    public string Model { get; set; }
    public int Year { get; set; }
    public Engine Motor { get; set; }

    public Car() {}
    public Car(string model, Engine motor, int year)
    {
        Model = model;
        Motor = motor;
        Year = year;
    }

    public override string ToString()
    {
        return $"Model:\n\t{Model}\nMotor:\n\t{Motor}\nYear:\n\t{Year}";
    }
}