﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotHome.Config.Views
{
    public class SelectionPropertiesView : UserControl
    {
        public SelectionPropertiesView()
        {
            this.InitializeComponent();
        }

        private void DataGrid_CollectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do not allow user to select anything in grids
            DataGrid dataGrid = (DataGrid)sender;
            dataGrid.SelectedItems.Clear();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
