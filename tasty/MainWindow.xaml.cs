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
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            /*
            var query = from post in new Delicious("username", "password").Posts
                        where post.Date == new DateTime(2009, 1, 12)
                        select post;*/
            var posts = new List<Post>()
            {
                new Post(
                    "http://example.com/",
                    "a5a6f3d28d8dd549f3cad39fb0b34104",
                    "Example domain",
                    "example domain",
                    "An example site.",
                    "2008-12-12T07:45:52Z",
                    "762ee1d713648596931f798a7ba987e0"),
                new Post(
                    "http://second-example.com/",
                    "ce67c6fbe4f79a521481060e2447001b",
                    "Another example domain",
                    "example domain another",
                    "Another example site.",
                    "2008-12-12T04:04:24Z",
                    "fa2a46d239ad4f089c3ce7961d958b2e")
            };

            postListBox.ItemsSource = posts;
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
