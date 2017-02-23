using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;


namespace Program
{

   
    static class Program0
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //================================= ПУСК потока  для ВНЕШНЕГО КЛАССА
            ThreadCAN t2 = new ThreadCAN();
            Thread t2potok = new Thread(t2.StartThread);
            t2potok.Start();

            //================================= ПУСК потока  для ВНЕШНЕГО КЛАССА
            //Thread3Read t3 = new Thread3Read();
            //Thread t3potok = new Thread3Read(t3.WorkThread);
            //t3potok.Start();

            //Класс данных
            var d= new data();  //ЧТОБЫ ЗАПУСТИЛСЯ КОНСТРУКТОР

            //=========== СОЗДАНИЕ ФОРМЫ до запуска потоков
            Form prog = new Form1();



            Application.Run(prog);
            //=============================== ЗАКРЫТИЕ ПОТОКОВ
            
            //t3potok.Abort();
           if (t2potok!=null) t2potok.Abort();
            
        }
    }
}
