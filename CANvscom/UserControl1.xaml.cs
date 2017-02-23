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

namespace Program
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            
            InitializeComponent();
           
            t1.Text = data.str1;
            t2.Text = data.str2;
            t3.Text = data.str3;
            t4.Text = data.str4;
            t5.Text = data.str5;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ss = "";
            if (data.str1 != t1.Text) ss = "1";
            if (data.str2 != t2.Text) ss += ";2";
            if (data.str3 != t3.Text) ss += ";3";
            if (data.str4 != t4.Text) ss += ";4";
            if (data.str5 != t5.Text) ss += ";5";

            data.str1 = t1.Text;
            data.str2 = t2.Text;
            data.str3 = t3.Text;
            data.str4 = t4.Text;
            data.str5 = t5.Text;

            if (ss!="")MessageBox.Show("ПРИНЯТО "+ss);
        }
    }
}
