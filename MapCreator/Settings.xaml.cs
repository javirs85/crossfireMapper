using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapCreator
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public FixtureCollection Fixtures;
        private int current = 0;
        private Fixture  currentFixture;

        public Settings()
        {
            InitializeComponent();
            types.ItemsSource = Enum.GetValues(typeof(FixtureType)).Cast<FixtureType>();
        }

        private void Types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = (FixtureType)Enum.Parse(typeof(FixtureType), types.SelectedValue.ToString());
            if (selectedType != FixtureType.notSet)
            {
                img.Children.Clear();
                currentFixture.type = selectedType;
                current++;
                if (current < Fixtures.Fixtures.Count)
                {
                    currentFixture = Fixtures.Fixtures[current];

                    if (currentFixture.Name == "120mm.png")
                    {
                        currentFixture.IsTable = true;
                        current++;
                        currentFixture = Fixtures.Fixtures[current];
                    }
                    currentFixture = Fixtures.Fixtures[current];
                    img.Children.Add(currentFixture.img);
                    types.SelectedIndex = 0;
                }
                else
                {
                    Fixtures.SaveToFile();
                    this.Close();
                }
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            currentFixture = Fixtures.Fixtures[0];
            img.Children.Add(currentFixture.img);
        }
    }
}
