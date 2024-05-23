using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Runtime.InteropServices;

namespace Lab10;

public partial class MainWindow : Window
{
    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr h, string m, string c, int type);

    private static readonly List<Car> myCars = new()
    {
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

    private ExtendedBindingList<Car> carsEBList = new ExtendedBindingList<Car>(myCars);

    public MainWindow()
    {
        InitializeComponent();

        ComboBox.Items.Add("Model");
        ComboBox.Items.Add("Motor");
        ComboBox.Items.Add("Year");

        BindDataToCarsGrid(myCars);

        // Zadanie 1
        ExecuteLINQQuery();

        // Zadanie 2
        DisplayFilteredCars();
    }

    private static void ExecuteLINQQuery()
    {
        Console.WriteLine("Zadanie 1");
        QueryExpression();
        MethodBasedQuery();
    }

    private static void QueryExpression()
    {
        var elements =
            from c in myCars
                     where c.Model == "A6"
                     let engineType = (c.Motor.Model == "TDI") ? "diesel" : "petrol"
                     let hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
                     group hppl by engineType
            into e
                     orderby e.Average() descending
                     select new
                     {
                         engineType = e.Key,
                         avgHPPL = e.Average()
                     };
        Console.WriteLine("Query expression:");
        foreach (var e in elements)
        {
            Console.WriteLine("\t" + e.engineType + ": " + e.avgHPPL);
        }
    }

    private static void MethodBasedQuery()
    {
        var elements = myCars
            .Where(c => c.Model == "A6")
            .Select(c => new
            {
                engineType = (c.Motor.Model == "TDI") ? "diesel" : "petrol",
                hppl = (double)c.Motor.Horsepower / c.Motor.Displacement
            })
            .GroupBy(c => c.engineType)
            .Select(g => new
            {
                engineType = g.Key,
                avgHPPL = g.Average(c => c.hppl)
            })
            .OrderByDescending(c => c.avgHPPL);

        Console.WriteLine("Method-based query:");
        foreach (var e in elements)
        {
            Console.WriteLine("\t" + e.engineType + ": " + e.avgHPPL);
        }
    }

    private static void DisplayFilteredCars()
    {
        Func<Car, Car, int> arg1 = Func;
        Predicate<Car> arg2 = Predicate;
        Action<Car> arg3 = Action;

        myCars.Sort(new Comparison<Car>(arg1));
        myCars.FindAll(arg2).ForEach(arg3);
    }

    private static int Func(Car a, Car b)
    {
        return a.Motor.Horsepower.CompareTo(b.Motor.Horsepower);
    }

    private static bool Predicate(Car a)
    {
        return a.Motor.Model == "TDI";
    }

    private static void Action(Car a)
    {
        MessageBox((IntPtr)0, a.ToString(), "Zadanie 2", 0);
    }


    private void BindDataToCarsGrid(List<Car> cars)
    {
        CarsDataGrid.ItemsSource = new BindingList<Car>(cars);
    }

    public void HandleKeyPress(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
        {
            return;
        }

        var cars = carsEBList.ToList().Where(x => x != (Car)(sender as DataGrid).SelectedItem).ToList();
        carsEBList = new ExtendedBindingList<Car>(cars);
        BindDataToCarsGrid(cars);
    }

    private void Search_Button(object sender, RoutedEventArgs e)
    {
        if (ComboBox.SelectedItem is null) return;

        var query = SearchTextBox.Text;
        var property = ComboBox.SelectedItem.ToString();

        var cars = carsEBList.Find(property, query);
        BindDataToCarsGrid(cars);
    }

    public void Add_Button(object sender, RoutedEventArgs e)
    {
        var model = Model.Text;
        var engineModel = EngineModel.Text;
        var horsepower = float.Parse(Horsepower.Text); ;
        var displacement = float.Parse(Displacement.Text);
        var year = int.Parse(Year.Text);

        carsEBList.Add(new Car(model, new Engine(displacement, horsepower, engineModel), year));
        BindDataToCarsGrid(carsEBList.ToList());
    }

    private void Sort_Model(object sender, RoutedEventArgs e)
    {
        BindDataToCarsGrid(carsEBList.Sort("Model"));
    }

    private void Sort_Year(object sender, RoutedEventArgs e)
    {
        BindDataToCarsGrid(carsEBList.Sort("Year"));
    }

    private void Sort_Motor(object sender, RoutedEventArgs e)
    {
        BindDataToCarsGrid(carsEBList.Sort("Motor"));
    }
}
