using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Lab9;

internal class Program
{
    private static List<Car>? _myCars = new List<Car>(){
         new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
         new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
         new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
         new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
         new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
         new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
         new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
         new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
         new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
    };
    private const string _fileName = "CarsCollection.xml";

    private static void Main(string[] args)
    {
        Console.WriteLine("Zadanie 1:");
        var projectedCars = _myCars
            .Where(c => c.Model == "A6")
            .Select(c => new
            {
                engineType = c.Motor.Model == "TDI" ? "diesel" : "petrol",
                hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
            });
        var groupedCars = projectedCars
            .GroupBy(c => c.engineType)
            .OrderBy(g => g.Key);

        foreach (var group in groupedCars)
        {
            Console.WriteLine($"{group.Key}: {string.Join(", ", group.Select(c => c.hppl))}; average = {group.Average(c => c.hppl)}");
        }

        Console.WriteLine("\nZadanie 2:");
        Serialize(_fileName);
        Console.WriteLine("Serialized:");
        foreach (var x in _myCars)
        {
            Console.WriteLine($"year: {x.Year}, motor model: {x.Motor.Model}, horsepower: {x.Motor.Horsepower}, displacement: {x.Motor.Displacement}");
        }

        _myCars = Deserialize(_fileName);
        Console.WriteLine("\nDeserialized:");
        if (_myCars is null)
        {
            Console.WriteLine("Deserialization failed");
            return;
        }
        foreach (var x in _myCars)
        {
            Console.WriteLine($"year: {x.Year}, motor model: {x.Motor.Model}, horsepower: {x.Motor.Horsepower}, displacement: {x.Motor.Displacement}");
        }

        Console.WriteLine("\nZadanie 3:");
        XPath(_fileName);

        // Zadanie 4
        CreateXmlFromLinq();
        // Zadanie 5
        CreateXHTMLTable();
        // Zadanie 6
        ModifyCarsXML();
    }

    private static void Serialize(string fileName)
    {
        var serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
        Path.Combine(Directory.GetCurrentDirectory(), fileName);
        using var writer = new StreamWriter(fileName);
        serializer.Serialize(writer, _myCars);
    }

    private static List<Car>? Deserialize(string fileName)
    {
        var serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
        using Stream reader = new FileStream(fileName, FileMode.Open);
        return serializer.Deserialize(reader) as List<Car>;
    }

    private static void XPath(string fileName)
    {
        XElement rootNode = XElement.Load(fileName);
        const string avgXPath = "sum(//car/engine[@Model!=\"TDI\"]/Horsepower) div count(//car/engine[@Model!=\"TDI\"]/Horsepower)";
        double avgHP = (double)rootNode.XPathEvaluate(avgXPath);
        Console.WriteLine($"Przeciętna moc samochodów o silnikach innych niż TDI = {avgHP}");

        const string noDuplicatesXPath = "//car[not(Model=preceding-sibling::car/Model)]/Model";
        IEnumerable<XElement> models = rootNode.XPathSelectElements(noDuplicatesXPath);
        Console.WriteLine($"Modele samochodów bez powtórzeń: {string.Join(", ", models.Select(m => m.Value))}");
    }

    private static void CreateXmlFromLinq()
    {
        IEnumerable<XElement>? nodes = _myCars?
            .Select(n =>
                new XElement("car",
                    new XElement("model", n.Model),
                    new XElement("engine",
                        new XAttribute("model", n.Motor.Model),
                        new XElement("displacement", n.Motor.Displacement),
                        new XElement("horsePower", n.Motor.Horsepower)
                    ),
                    new XElement("year", n.Year)
                )
            );
        var rootNode = new XElement("cars", nodes);
        rootNode.Save("4-XmlFromLinq.xml");
    }

    private static void CreateXHTMLTable()
    {
        const string style = "border: 2px solid black";
        var rows = _myCars?.Select(car => new XElement("tr",
            new XAttribute("style", style),
            new XElement("td", new XAttribute("style", style), car.Model),
            new XElement("td", new XAttribute("style", style), car.Motor.Model),
            new XElement("td", new XAttribute("style", style), car.Year),
            new XElement("td", new XAttribute("style", style), car.Motor.Displacement),
            new XElement("td", new XAttribute("style", style), car.Motor.Horsepower)
            )
        );

        var table = new XElement("table", new XAttribute("style", style), rows);
        var template = XElement.Load("template.html");
        var body = template.Element("{http://www.w3.org/1999/xhtml}body");
        
        body?.Add(table);
        template.Save("5-CarsTable.html");
    }

    private static void ModifyCarsXML()
    {
        var doc = XDocument.Load(_fileName);

        foreach (var car in doc.Root!.Elements())
        {
            foreach (var field in car.Elements())
            {
                if (field.Name == "engine")
                {
                    foreach (var engineElement in field.Elements())
                    {
                        if (engineElement.Name == "Horsepower")
                        {
                            engineElement.Name = "hp";
                        }
                    }
                }
                else if (field.Name == "Model")
                {
                    var yearField = car.Element("Year");
                    var attribute = new XAttribute("Year", yearField!.Value);
                    field.Add(attribute);
                    yearField.Remove();
                }
            }
        }
        doc.Save("6-CarsModified.xml");
    }
}
