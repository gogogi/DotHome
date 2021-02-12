using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Model;
using MessageBox.Avalonia;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class CategoryWindow : Window
    {
        private string CategoryName { get; set; }

        private Category category;

        public CategoryWindow(Category category) : this()
        {
            this.category = category;
            if (category != null)
            {
                CategoryName = category.Name;
            }
            else
            {
                CategoryName = NextCategoryName();
            }
            DataContext = this;
        }

        public CategoryWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ConfigTools.MainWindow.Project.Categories.Any(c => c != category && c.Name == CategoryName))
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Category with this name already exists").ShowDialog(this);
                return;
            }
            if (category == null)
            {
                category = new Category() { Name = CategoryName };
            }
            else
            {
                category.Name = CategoryName;
            }
            Close(category);
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close();
        }

        private string NextCategoryName()
        {
            int i = 1;
            while (true)
            {
                string s = $"Category{i}";
                if (ConfigTools.MainWindow.Project.Categories.All(c => c.Name != s)) return s;
                i++;
            }
        }
    }
}
