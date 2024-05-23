using System.ComponentModel;

namespace Lab10;

public class ExtendedBindingList<T> : BindingList<T>
{
    private bool sortByModel = false;
    private bool sortByYear = false;
    private bool sortByMotor = false;

    public ExtendedBindingList(List<T> cars)
    {
        foreach (var car in cars)
        {
            Add(car);
        }
    }

    public List<T> AddElement(T car)
    {
        var cars = this.ToList();
        cars.Add(car);
        return cars;
    }

    public List<T> Find(string property, string query)
    {
        if (query == "")
            return this.ToList();

        var queriedCars = new List<T>();

        foreach (var car in this)
        {
            System.Reflection.PropertyInfo prop = typeof(T).GetProperty(property);

            if (prop.GetValue(car).ToString().ToUpper().StartsWith(query.ToUpper()))
            {
                queriedCars.Add(car);
            }
        }
        return queriedCars;
    }

    public List<T> Sort(string property)
    {
        var sortDirection = ListSortDirection.Ascending;

        switch (property)
        {
            case "Model":
                sortDirection = sortByModel ? ListSortDirection.Ascending : ListSortDirection.Descending;
                sortByModel = !sortByModel;
                break;
            case "Year":
                sortDirection = sortByYear ? ListSortDirection.Ascending : ListSortDirection.Descending;
                sortByYear = !sortByYear;
                break;
            case "Motor":
                sortDirection = sortByMotor ? ListSortDirection.Ascending : ListSortDirection.Descending;
                sortByMotor = !sortByMotor;
                break;
        }
        this.ApplySortCore(TypeDescriptor.GetProperties(typeof(T))[property], sortDirection);
        return this.ToList();
    }

    protected override bool SupportsSortingCore => true;

    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
        if (prop.PropertyType.GetInterface("IComparable") == null)
            throw new NotSupportedException("Cannot sort by " + prop.Name + ". This" + prop.PropertyType + " does not implement IComparable.");

        var itemsList = Items as List<T>;

        if (itemsList != null)
        {
            var comparer = new PropertyComparer<T>(prop, direction);
            itemsList.Sort(comparer);
        }

        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    protected override bool SupportsSearchingCore => true;
    
    protected override int FindCore(PropertyDescriptor prop, object key)
    {
        var itemsList = Items as List<T>;

        if (itemsList != null)
        {
            var comparer = new PropertyComparer<T>(prop, ListSortDirection.Ascending);
            var index = itemsList.FindIndex(x => comparer.Compare(x, key) == 0);
            return index;
        }
        return -1;
    }
}
