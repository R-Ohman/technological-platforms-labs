using System.Xml.Serialization;

namespace Lab9;

[XmlRoot(ElementName = "engine")]
public class Engine
{
    public double Displacement { get; set; }
    public double Horsepower { get; set; }
    [XmlAttribute]
    public string Model { get; set; }

    public Engine() {}
    public Engine(double displacement, double horsepower, string model)
    {
        Displacement = displacement;
        Horsepower = horsepower;
        Model = model;
    }
}