using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Views;
using MessageBox.Avalonia;
using System.Linq;

namespace DotHome.Config.Windows
{
    public class PageWindow : Window
    {
        private PageView page;
        private ProjectView project;

        private TextBox textBoxName;
        private NumericUpDown numericUpDownWidth;
        private NumericUpDown numericUpDownHeight;
        public PageWindow(PageView page, ProjectView project) : this()
        {
            this.page = page;
            this.project = project;

            textBoxName = this.FindControl<TextBox>("textBoxName");
            numericUpDownWidth = this.FindControl<NumericUpDown>("numericUpDownWidth");
            numericUpDownHeight = this.FindControl<NumericUpDown>("numericUpDownHeight");

            textBoxName.Text = page?.Name ?? NextPageName();
            numericUpDownWidth.Value = page?.Width ?? 1000;
            numericUpDownHeight.Value = page?.Height ?? 1000;

            Title = page?.Name ?? "New page";
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



        private void NumericUpDownWidth_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue % 200 != 0)
            {
                numericUpDownWidth.Value = e.OldValue;
            }
        }

        private void NumericUpDownHeight_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (e.NewValue % 200 != 0)
            {
                numericUpDownHeight.Value = e.OldValue;
            }
        }

        private async void ButtonOk_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (project.Pages.Any(p => p != page && p.Name == textBoxName.Text))
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page width this name already exists").ShowDialog(this);
                return;
            }
            if(page == null)
            {
                PageView pageView = new PageView() { Name = textBoxName.Text, Width = (int)numericUpDownWidth.Value, Height = (int)numericUpDownHeight.Value };
                project.AddPage(pageView);
                Close(pageView);
            }
            else
            {
                double minX = page.Blocks.Min(b => b.X);
                double maxX = page.Blocks.Max(b => b.X + b.Width);
                if (maxX - minX > numericUpDownWidth.Value)
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page width is too small").ShowDialog(this);
                    return;
                }
                double minY = page.Blocks.Min(b => b.Y);
                double maxY = page.Blocks.Max(b => b.Y + b.Height);
                if (maxY - minY > numericUpDownHeight.Value)
                {
                    await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Page height is too small").ShowDialog(this);
                    return;
                }

                if (maxX > numericUpDownWidth.Value)
                {
                    double delta = maxX - numericUpDownWidth.Value;
                    foreach (ABlockView b in page.Blocks)
                    {
                        b.X -= delta;
                    }
                }

                if (maxY > numericUpDownHeight.Value)
                {
                    double delta = maxY - numericUpDownHeight.Value;
                    foreach (ABlockView b in page.Blocks)
                    {
                        b.Y -= delta;
                    }
                }

                page.Name = textBoxName.Text;
                page.Width = (int)numericUpDownWidth.Value;
                page.Height = (int)numericUpDownHeight.Value;
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
                if (project.Pages.All(p => p.Name != s)) return s;
                i++;
            }
        }
    }
}
