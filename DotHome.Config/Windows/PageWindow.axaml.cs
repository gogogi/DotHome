using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using MessageBox.Avalonia;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class PageWindow : Window
    {
        private Page page;

        private string PageName { get; set; }

        private int PageWidth { get; set; }

        private int PageHeight { get; set; }

        public PageWindow(Page page) : this()
        {
            this.page = page;
            if(page != null)
            {
                PageName = page.Name;
                PageWidth = page.Width;
                PageHeight = page.Height;
            }
            else
            {
                PageName = NextPageName();
                PageWidth = 1000;
                PageHeight = 1000;
            }
            DataContext = this;
        }

        public PageWindow()
        {

            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NumericUpDown_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue % 200 != 0)
            {
                ((NumericUpDown)sender).Value = e.OldValue;
            }
        }

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (ConfigTools.MainWindow.Project.Pages.Any(p => p != page && p.Name == PageName))
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page with this name already exists").ShowDialog(this);
                return;
            }
            if(page == null)
            {
                Page page = new Page() { Name = PageName, Width = PageWidth, Height = PageHeight };
                Close(page);
            }
            else
            {
                double minX = page.Blocks.Min(b => b.X);
                double maxX = page.Blocks.Max(b => b.X/* + b.Width*/);
                if (maxX - minX > PageWidth)
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page width is too small").ShowDialog(this);
                    return;
                }
                double minY = page.Blocks.Min(b => b.Y);
                double maxY = page.Blocks.Max(b => b.Y/* + b.Height*/);
                if (maxY - minY > PageHeight)
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page height is too small").ShowDialog(this);
                    return;
                }

                if (maxX > PageWidth)
                {
                    double delta = maxX - PageWidth;
                    foreach (Block b in page.Blocks)
                    {
                        b.X -= (int)delta;
                    }
                }

                if (maxY > PageHeight)
                {
                    double delta = maxY - PageHeight;
                    foreach (Block b in page.Blocks)
                    {
                        b.Y -= (int)delta;
                    }
                }

                page.Name = PageName;
                page.Width = PageWidth;
                page.Height = PageHeight;
                Close(page);
            }
        }

        private void ButtonCancel_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(null);
        }

        private string NextPageName()
        {
            int i = 1;
            while(true)
            {
                string s = $"Page{i}";
                if (ConfigTools.MainWindow.Project.Pages.All(p => p.Name != s)) return s;
                i++;
            }
        }
    }
}
