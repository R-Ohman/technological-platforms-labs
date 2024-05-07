using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace FileExplorer;

public partial class CreateFileWindow : Window
{
    private readonly DirectoryInfo path;
    public CreateFileWindow(DirectoryInfo path)
    {
        this.path = path;
        InitializeComponent();
    }

    private void CreateButton_OnClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text;
        var isFile = FileType.IsChecked;
        var attributes = FileAttributes.Normal
                         | (IsReadOnly.IsChecked == true ? FileAttributes.ReadOnly : FileAttributes.Normal)
                         | (IsArchive.IsChecked == true ? FileAttributes.Archive : FileAttributes.Normal)
                         | (IsSystem.IsChecked == true ? FileAttributes.System : FileAttributes.Normal)
                         | (IsHidden.IsChecked == true ? FileAttributes.Hidden : FileAttributes.Normal);

        if (isFile == true)
        {
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_~-]{1,8}\.(txt|php|html)$"))
            {
                System.Windows.MessageBox.Show("Invalid file name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
            } else
            {
                File.Create(Path.Combine(path.FullName, name)).Close();
                File.SetAttributes(Path.Combine(path.FullName, name), attributes);
            }
        }
        else
        {
            Directory.CreateDirectory(Path.Combine(path.FullName, name));
        }
        Close();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}