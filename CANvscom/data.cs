using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Reflection;
using VSCom.CanApi;
using System.Collections.Generic;


namespace Program
{

    //======================== ОБЩИЙ КЛАСС ДАННЫХ ==================
     
    class data 
    {

    
       public  struct t
        {
            public static byte s1 = 11; public static byte e1 = 17;
            public static byte s2 = 18; public static byte e2 = 23;
            public static byte s3 = 24; public static byte e3 = 27;
            public static byte s4 = 28; public static byte e4 = 33;
            public static byte s5 = 34; public static byte e5 = 37;
        
        }

      
       public struct tt// + смещение
       {
           public static byte s1 = 1; public static byte e1 = 7;
           public static byte s2 = 8; public static byte e2 = 13;
           public static byte s3 = 14; public static byte e3 = 17;
           public static byte s4 = 18; public static byte e4 = 23;
           public static byte s5 = 24; public static byte e5 = 27;

       }

        public static string str1 =@" Вас приветствует коллектив компании "" "" в новом вагоне.  Желаем Вам приятного путешествия. Счастливого пути!                                 ";
        public static string str2 = " Железнодорожный транспорт является зоной повышенной опасности. Не переходите железнодорожные пути в неустановленных местах, пользуйтесь тоннелями, пешеходными мостами, переездами. Будьте внимательны и осторожны!                    ";
        public static string str3a = @"Коллектив компании ЗАО "" "" прощается с вами. Желаем всего наилучшего. До новых встреч!                          ";
        public static string str3b ="<-                                            ";
        public static string str4a = "До отправления поезда осталось 5 минут. Просим пассажиров занять свои места, а провожающих покинуть вагон. Провожающие проверьте, не остались ли у Вас документы и проездные билеты пассажиров. Приятного Вам пути!                      ";
        public static string str4b ="<-                                            ";
        public static string str5 = " По всем интересующим вопросам  и  в  затруднительных ситуациях во время путешествия на нашем вагоне обращайтесь к дежурному проводнику.                  ";
        public static byte start, end;
        public static string stroka;
        public static string str3,str4;


        public static bool t1_closing = false, t1closeOK;
        public static bool t2_closing = false, t2closeOK;


        public static bool TABLOERROR1, TABLOERROR2, pauseWRITE;

        public static Form1 adres_FORM1=null;
      // public static  VSCAN_MSG[] msgs;

        
        public static Queue<ДАННЫЕочередь> очередь { get; set; }

        public static Queue<ДАННЫЕочередь> очередьКОПИЯ { get; set; }//НЕ ИСПОЛЬЗ
        public static Queue<ДАННЫЕочередь> очередьКОПИЯрабочая { get; set; }


        
        public static Queue<ДАННЫЕочередь> PLAYQueue { get; set; }

        public static ДАННЫЕочередь buf_read,  buf_readPLAY;
     

        //public static ДАННЫЕочередьSTRUCT[] massivДАННЫЕочередь;


        public static uint adr; public static byte sz, b1, b2, b3, b4, b5, b6, b7, b8;
       

        public static bool SOBIT, connOK = false,  start_buff, start_ReadBuff;
        public static UInt32 SOBITkod, diap1, diap2, di1,di2,di3,di4;

        public static UInt32 con,    пропущенные;
        public static string SOBstr;
        public static int count;//очередь размер

        public static bool fo1, fo2, fo3,  PLAY;

        public static int test_read, test_prin, test_ignor;

        public static byte can;

        public static string text,lastpath;

        public static byte cod_clear_memory=32;

        public static bool runTEST;

        public data()
        {
     
           очередь = new Queue<ДАННЫЕочередь>();
           buf_read = new ДАННЫЕочередь();
           
        }


       
        public static Form1 pFORM1;//УКАЗАТЕЛЬ НА ФОРМУ
        public static void COPY1()
        {
             

            pFORM1 = data.adres_FORM1;//берем адрес формы

            if (data.очередьКОПИЯ != null) data.очередьКОПИЯ.Clear();
           // data.очередьКОПИЯ = new Queue<ДАННЫЕочередь>(data.очередь.ToArray());

            int sz = очередь.Count;
            if (sz > 9999) { return; }

            очередьКОПИЯ = new Queue<ДАННЫЕочередь>(очередь);


            

               COPY2();
      
        }

        public static void COPY2()
        {

        

            int sz = очередьКОПИЯ.Count;

            if (data.очередьКОПИЯрабочая != null) data.очередьКОПИЯрабочая.Clear();
            очередьКОПИЯрабочая = new Queue<ДАННЫЕочередь>(очередь);

          


           
        }

        public static bool РАБОТА_С_ОЧЕРЕДЬЮ(string command,  ДАННЫЕочередь buffer)
        {
            if (очередь == null) return false;
            lock (очередь){
            
                if (command == "write")//записать
            {
                data.очередь.Enqueue(buffer); return true;
            }


                if (command == "read" )// прочитать
                {

                    if (data.очередь.Count == 0) return false;
                    data.buf_read = data.очередь.Dequeue();
                //   ref buffer = data.очередь.Dequeue();
                    return true;

                }


                  }//lock
            return true;
        }



        //ТОЛЬКО дЛЯ ВЫВОДА НА ЭКРАН
        //public static bool РАБОТА_С_ОЧЕРЕДЬЮplay(string command, ДАННЫЕочередь buffer)
        //{
        //    //ТОЛЬКО дЛЯ ВЫВОДА НА ЭКРАН

        //    if (PLAYQueue == null) return false;
        //    lock (PLAYQueue)
        //    {

        //        if (command == "write")//записать
        //        {
        //            data.PLAYQueue.Enqueue(buffer); return true;
        //        }


        //        if (command == "read")// прочитать
        //        {

        //            if (data.PLAYQueue.Count == 0) return false;
        //            data.buf_read = data.PLAYQueue.Dequeue();
        //            //   ref buffer = data.очередь.Dequeue();
        //            return true;

        //        }


        //    }//lock
        //    return true;
        //}

        public static bool РАБОТА_С_ОЧЕРЕДЬЮ_PLAY(byte command)
        {
            if (очередьКОПИЯрабочая == null) return false;
            //lock (очередьКОПИЯрабочая)
            //{

                //if (command == 1)//записать
                //{
                //    data.очередьКОПИЯрабочая.Enqueue(buffer); return true;
                //}


                if (command == 2)// прочитать
                {

                    if (data.очередьКОПИЯрабочая.Count == 0) return false;
                    data.buf_readPLAY = data.очередьКОПИЯрабочая.Dequeue();
                    return true;

                }


            //}//lock
            return true;
        }

        public static void col(SByte u)
        {

            

            if (u == 1) Console.ForegroundColor = ConsoleColor.Green;
            if (u == 2) Console.ForegroundColor = ConsoleColor.Red;
            if (u == 3) Console.ForegroundColor = ConsoleColor.Blue;

        }
        public static void print(string pr)
        {
            col(1);
            Console.WriteLine(pr);

        }
        public static void printRED(string pr)
        {
            col(2);
            Console.WriteLine(pr); col(1);

        }
        public static void avar(string avar)
        {
            col(2);
            Console.WriteLine(avar); col(1);
        }

        public static void printBLUE(string pr)
        {
            col(3);
            Console.WriteLine(pr); col(1);

        }
       

  



        public void CONV(string s, ref double pr, ref bool err)
        {
            err = false;
            //==========================================================
            if (s == "0") { pr = 0; return; }
            try
            {
                // Gets a NumberFormatInfo associated with the en-US culture.
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberDecimalSeparator = ",";//--
                pr = double.Parse(s, nfi);
            }
            catch
            {
                //  if (diag) printRED("не считал цену сделки пробуем с разделитель точка");
                try
                {
                    // Gets a NumberFormatInfo associated with the en-US culture.
                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberDecimalSeparator = ".";//--
                    pr = double.Parse(s, nfi);
                }

                catch
                {
                    err = true;
                }
            }

        }



        public void CONV(string s, ref int pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {
                pr = int.Parse(s);
            }
            catch { err = true; }

        }
        public void CONV(string s, ref SByte pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {

                pr = SByte.Parse(s);
            }
            catch { err = true; }

        }
      
        public void CONV(string s, ref long pr, ref bool err)
        {
            err = false;
            if (s == "0") { pr = 0; return; }
            try
            {
                pr = long.Parse(s);
            }
            catch { err = true; }

        }
        public void CONV(string s, ref bool pr, ref bool err)
        {
            err = false;
            if (s == "True") pr = true;
            else
                if (s == "False") pr = false;
                else err = true;
        }

        public static string getTIME()
        {
            string ss = String.Format("{0}",DateTime.Now.Millisecond);
           int  pr = int.Parse(ss);
           if (pr < 100) ss = "0" + ss;
           if (pr < 10) ss = "0" + ss;

           string ss2 = String.Format("{0}", DateTime.Now.Second);
            pr = int.Parse(ss2);
           
           if (pr < 10) ss2 = "0" + ss2;

           string ss1 = String.Format("{0}", DateTime.Now.Minute);
           pr = int.Parse(ss1);
           
           if (pr < 10) ss1 = "0" + ss1;

           return (String.Format("{0,2}:{1,2}:{2,2}.{3,3}", DateTime.Now.Hour, ss1, ss2, ss));
        }

        public static string getTIME(DateTime time)
        {
            string ss = String.Format("{0}", time.Millisecond);
            int pr = int.Parse(ss);
            if (pr < 100) ss = "0" + ss;
            if (pr < 10) ss = "0" + ss;

            string ss2 = String.Format("{0}", time.Second);
            pr = int.Parse(ss2);

            if (pr < 10) ss2 = "0" + ss2;

            string ss1 = String.Format("{0}", time.Minute);
            pr = int.Parse(ss1);

            if (pr < 10) ss1 = "0" + ss1;

            return (String.Format("{0,2}:{1,2}:{2,2}.{3,3}", time.Hour, ss1, ss2, ss));
        }

        public static string getTIMEwithDAY()
        {
            return (String.Format(" {0}.{1,3}",DateTime.Now, DateTime.Now.Millisecond));
        }

        public static void зависание()
        {
            while (true) Thread.Sleep(1000);
        }


       

    }


   




    public static class B
    {
        public static readonly V _0000 = 0x0;
        public static readonly V _0001 = 0x1;
        public static readonly V _0010 = 0x2;
        public static readonly V _0011 = 0x3;
        public static readonly V _0100 = 0x4;
        public static readonly V _0101 = 0x5;
        public static readonly V _0110 = 0x6;
        public static readonly V _0111 = 0x7;

        public static readonly V _1000 = 0x8;
        public static readonly V _1001 = 0x9;
        public static readonly V _1010 = 0xA;
        public static readonly V _1011 = 0xB;
        public static readonly V _1100 = 0xC;
        public static readonly V _1101 = 0xD;
        public static readonly V _1110 = 0xE;
        public static readonly V _1111 = 0xF;

        public struct V
        {
            ulong Value;

            public V(ulong value)
            {
                this.Value = value;
            }

            private V Shift(ulong value)
            {
                return new V((this.Value << 4) + value);
            }

            public V _0000 { get { return this.Shift(0x0); } }
            public V _0001 { get { return this.Shift(0x1); } }
            public V _0010 { get { return this.Shift(0x2); } }
            public V _0011 { get { return this.Shift(0x3); } }
            public V _0100 { get { return this.Shift(0x4); } }
            public V _0101 { get { return this.Shift(0x5); } }
            public V _0110 { get { return this.Shift(0x6); } }
            public V _0111 { get { return this.Shift(0x7); } }

            public V _1000 { get { return this.Shift(0x8); } }
            public V _1001 { get { return this.Shift(0x9); } }
            public V _1010 { get { return this.Shift(0xA); } }
            public V _1011 { get { return this.Shift(0xB); } }
            public V _1100 { get { return this.Shift(0xC); } }
            public V _1101 { get { return this.Shift(0xD); } }
            public V _1110 { get { return this.Shift(0xE); } }
            public V _1111 { get { return this.Shift(0xF); } }

            static public implicit operator V(ulong value)
            {
                return new V(value);
            }

            static public implicit operator ulong(V this_)
            {
                return this_.Value;
            }

            static public implicit operator uint(V this_)
            {
                return (uint)this_.Value;
            }

            static public implicit operator ushort(V this_)
            {
                return (ushort)this_.Value;
            }

            static public implicit operator byte(V this_)
            {
                return (byte)this_.Value;
            }
        }
    }

    [Serializable]
  public class   ДАННЫЕочередь
    {
        public DateTime time { get; set; }
        public string strERROR { get; set; }
        public uint adr { get; set; }
        public byte sz { get; set; }
        public byte b1 { get; set; }
        public byte b2 { get; set; }
        public byte b3 { get; set; }
        public byte b4 { get; set; }
        public byte b5 { get; set; }
        public byte b6 { get; set; }
        public byte b7 { get; set; }
        public byte b8 { get; set; }
    }

}
  //struct ДАННЫЕочередьSTRUCT
  //  {
  //      public DateTime time { get; set; }
  //      public string strERROR { get; set; }
  //      public uint adr { get; set; }
  //      public byte sz { get; set; }

  //      public byte b1 { get; set; }
  //      public byte b2 { get; set; }
  //      public byte b3 { get; set; }
  //      public byte b4 { get; set; }
  //      public byte b5 { get; set; }
  //      public byte b6 { get; set; }
  //      public byte b7 { get; set; }
  //      public byte b8 { get; set; }

  //  }


