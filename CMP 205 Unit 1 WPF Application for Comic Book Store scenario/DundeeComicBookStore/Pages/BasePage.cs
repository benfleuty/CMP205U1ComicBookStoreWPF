using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DundeeComicBookStore.Pages
{
    public abstract class BasePage : Page
    {
        protected Window _currentWindow;

        protected Window CurrentWindow
        {
            get { return _currentWindow; }
            set { _currentWindow = value; }
        }

        public BasePage()
        {
            CurrentWindow = App.Current.MainWindow;
        }

        protected void ChangePageTo(object newPage)
        {
            CurrentWindow.Content = new Frame() { Content = newPage };
        }
    }
}