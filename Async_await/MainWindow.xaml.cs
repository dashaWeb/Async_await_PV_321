using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Async_await
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Random rnd = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ShowRandomNumber(object sender, RoutedEventArgs e)
        {
            /*int value = GenerateValue();
            list.Items.Add(value);*/
            /*Task<int> task = Task.Run(GenerateValue);
            task.Wait();// freeze
            list.Items.Add(task.Result);// freeze*/

            // async => alow method to use await
            // await => wait task without freezing

            /*Task<int> task = Task.Run(GenerateValue);
            int value = await task;
            list.Items.Add(value);*/
            int value;
            // ref
            // out
            if (int.TryParse(number.Text, out value))
            {
                list.Items.Add(await GenerateValueAsync(value));
            }
            else
            {
                MessageBox.Show("Errro value","Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        int GenerateValue()
        {
            Thread.Sleep(rnd.Next(10000));
            return rnd.Next(1000);
        }
        Task<int> GenerateValueAsync(int value)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(rnd.Next(10000));
                return rnd.Next(value);
            });
        }

    }
}
