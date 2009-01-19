using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace tasty.Commands
{
    public class FileCommands
    {
        public static RoutedUICommand Exit { get; private set; }

        static FileCommands()
        {
            Exit = new RoutedUICommand(
                "E_xit", 
                "Exit", 
                typeof(FileCommands));
        }
    }
}
