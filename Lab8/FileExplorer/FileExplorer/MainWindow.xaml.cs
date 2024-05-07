using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using MessageBox = System.Windows.MessageBox;

namespace FileExplorer;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DirectoryInfo? currentDirectory = null;

    public MainWindow()
    {
        InitializeComponent();
        TreeView.ContextMenu = new ContextMenu();
    }

    private void OpenFile_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new FolderBrowserDialog() { Description = "Select a directory to open" };
        var result = dlg.ShowDialog();

        if (result != System.Windows.Forms.DialogResult.OK)
        {
            return;
        }

        if (!Directory.Exists(dlg.SelectedPath))
        {
            MessageBox.Show(this, "Invalid path selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        currentDirectory = new DirectoryInfo(dlg.SelectedPath);
        DisplayFiles();
    }

    private void ExitFile_OnClick(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void DisplayFiles()
    {
        TreeView.Items.Clear();
        if (currentDirectory is null)
        {
            return;
        }

        var dirs = currentDirectory.EnumerateDirectories().Select(GetTreeViewItem);
        var files = currentDirectory.EnumerateFiles().Select(GetTreeViewItem);
        foreach (var item in dirs.Concat(files))
        {
            TreeView.Items.Add(item);
        }

        TreeView.ContextMenu?.Items.Clear();
        var createMenuItem = new MenuItem
        {
            Header = "Create",
            Tag = currentDirectory.FullName,
        };
        createMenuItem.Click += TreeViewDirectoryItem_OnCreate;
        TreeView.ContextMenu?.Items.Add(createMenuItem);
    }

    private TreeViewItem GetTreeViewItem(DirectoryInfo info)
    {
        var root = new TreeViewItem
        {
            Header = info.Name,
            Tag = info.FullName,
            ContextMenu = new ContextMenu(),
        };
        var createMenuItem = new MenuItem
        {
            Header = "Create",
            Tag = info.FullName,
        };
        createMenuItem.Click += TreeViewDirectoryItem_OnCreate;
        root.ContextMenu.Items.Add(createMenuItem);

        var deleteMenuItem = new MenuItem
        {
            Header = "Delete",
            Tag = info.FullName,
        };
        deleteMenuItem.Click += TreeViewDirectoryItem_OnDelete;
        root.ContextMenu.Items.Add(deleteMenuItem);

        var dirs = info.EnumerateDirectories().Select(GetTreeViewItem);
        var files = info.EnumerateFiles().Select(GetTreeViewItem);
        foreach (var item in dirs.Concat(files))
        {
            root.Items.Add(item);
        }

        return root;
    }

    private TreeViewItem GetTreeViewItem(FileInfo info)
    {
        var item = new TreeViewItem
        {
            Header = info.Name,
            Tag = info.FullName,
            ContextMenu = new ContextMenu(),
        };
        item.Selected += TreeViewFileItem_OnSelected;
        item.MouseDoubleClick += TreeViewFileItem_OnDoubleClick;

        var openMenuItem = new MenuItem
        {
            Header = "Open",
            Tag = info.FullName
        };
        openMenuItem.Click += TreeViewFileItem_OnOpen;
        item.ContextMenu.Items.Add(openMenuItem);

        var deleteMenuItem = new MenuItem
        {
            Header = "Delete",
            Tag = info.FullName
        };
        deleteMenuItem.Click += TreeViewFileItem_OnDelete;
        item.ContextMenu.Items.Add(deleteMenuItem);

        return item;
    }

    private void TreeViewDirectoryItem_OnCreate(object sender, RoutedEventArgs e)
    {
        var menuItem = e.Source as MenuItem;

        if (menuItem?.Tag is not string path || !Directory.Exists(path))
        {
            MessageBox.Show("Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var window = new CreateFileWindow(new DirectoryInfo(path));
        window.ShowDialog();
        DisplayFiles();
    }

    private void TreeViewDirectoryItem_OnDelete(object sender, RoutedEventArgs e)
    {
        var menuItem = e.Source as MenuItem;

        if (menuItem?.Tag is not string path || !File.Exists(path))
        {
            MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            // If the file is read-only, remove the read-only attribute
            if (File.GetAttributes(file).HasFlag(FileAttributes.ReadOnly))
            {
                File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
            }
            File.Delete(file);
        }
        Directory.Delete(path);
        DisplayFiles();
    }

    private void TreeViewFileItem_OnDelete(object sender, RoutedEventArgs e)
    {
        var menuItem = e.Source as MenuItem;

        if (menuItem?.Tag is not string path || !File.Exists(path))
        {
            MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // If the file is read-only, remove the read-only attribute
        if (File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
        {
            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
        }

        File.Delete(path);
        DisplayFiles();
    }

    private void TreeViewFileItem_OnSelected(object sender, RoutedEventArgs e)
    {
        var item = e.Source as TreeViewItem;

        if (item?.Tag is not string path || !File.Exists(path))
        {
            MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var attributes = File.GetAttributes(path);
        var dosAttributes = new StringBuilder();
        
        dosAttributes.Append((attributes & FileAttributes.ReadOnly) != 0 ? 'r' : '-');
        dosAttributes.Append((attributes & FileAttributes.Archive) != 0 ? 'a' : '-');
        dosAttributes.Append((attributes & FileAttributes.System) != 0 ? 's' : '-');
        dosAttributes.Append((attributes & FileAttributes.Hidden) != 0 ? 'h' : '-');

        AttributeTextBlock.Text = dosAttributes.ToString();
    }

    private void TreeViewFileItem_OnDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var item = e.Source as TreeViewItem;

        if (item?.Tag is not string path || !File.Exists(path))
        {
            MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        FileViewer.Text = File.ReadAllText(path, Encoding.UTF8);
    }

    private void TreeViewFileItem_OnOpen(object sender, RoutedEventArgs e)
    {
        var menuItem = e.Source as MenuItem;

        if (menuItem?.Tag is not string path || !File.Exists(path))
        {
            MessageBox.Show(this, "Invalid path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        FileViewer.Text = File.ReadAllText(path, Encoding.UTF8);
    }
}
