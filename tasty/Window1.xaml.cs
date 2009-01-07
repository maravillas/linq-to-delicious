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
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            var query = from post in new Delicious(new WebClient()).Posts
                        where post.Date == new DateTime(2008, 1, 1)
                        select post;

            foreach (var p in query)
            {
                Debug.WriteLine(p);
            }
        }
    }
}
