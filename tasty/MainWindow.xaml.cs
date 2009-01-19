using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LinqToDelicious;
using System.Net;
using System.Diagnostics;

namespace tasty
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable<Post> Posts { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            
            Posts = new List<Post>();

            /*
            var query = from post in new Delicious("username", "password").Posts
                        where post.Date == new DateTime(2009, 1, 16)
                        select post;

            foreach (var p in query)
            {
                Debug.WriteLine(p);
            }*/
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
