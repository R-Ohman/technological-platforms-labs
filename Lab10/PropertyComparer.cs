using System.ComponentModel;
namespace Lab10;

internal class PropertyComparer<T> : IComparer<T>
{
    private PropertyDescriptor prop;
    private ListSortDirection direction;

    public PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
    {
        this.prop = prop;
        this.direction = direction;
    }

    public int Compare(T x, T y)
    {
        int result = Comparer<object>.Default.Compare(prop.GetValue(x), prop.GetValue(y));
        return direction == ListSortDirection.Ascending ? result : -result;
    }

    internal int Compare(T x, object key)
    {
        return Compare(x, (T)key);
    }
}