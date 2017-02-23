using System;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
  
using System.Runtime.InteropServices;
using System.Globalization;
using System.Reflection;

using VSCom.CanApi;
//using System.Threading.Tasks;

namespace Program
{
    

    public partial class ThreadCAN
  //  public partial class Form1 : Form
    {
        public Form1 pFORM;//УКАЗАТЕЛЬ НА ФОРМУ
       

        VSCAN_MSG[] msgs;
        public static uint sizeFRAME=2;//------------ КОЛ-ВО  FRAME при READ CAN
        VSCAN CanDevice;
        UInt32 Written = 0;
        VSCAN_HWPARAM hw;
        VSCAN_API_VERSION api_ver;
        byte Flags = 0x0;
        

        public ThreadCAN()//-------------- конструктор ---------------------
        {
            //задача уходит в бесконечный цикл
            Task task1 = Task.Factory.StartNew(() =>
            {
                try { while (!data.t2_closing) TASK_1(); } 
                catch (Exception ex) 
                { MessageBox.Show(String.Format("ОШИБКА TASK1 {0};",ex), "Фатальная Ошибка"); }  
            });
            
            //задача уходит в бесконечный цикл
            Task task2 = Task.Factory.StartNew(() =>
            {
                try { while (!data.t2_closing) TASK_2(); }
                catch (Exception ex)
                { MessageBox.Show(String.Format("ОШИБКА TASK2 {0};", ex), "Фатальная Ошибка"); }
            });
        }
       
        public void StartThread()
        {
            try { while (!data.t2_closing) main(); } catch (Exception ex)
            { MessageBox.Show(String.Format("ОШИБКА ПОТОКА2 {0};", ex), "Фатальная Ошибка"); }
            data.t2closeOK = true;
        }

        //============== ГЛАВНЫЙ ЦИКЛ ПОТОКА CAN
        void main()
        {
            
            pFORM = data.adres_FORM1;//берем адрес формы
            if (data.adres_FORM1 == null) { Thread.Sleep(100); return; }//значит поток формы еще не запустился


            //коды событий
            if (data.con == 1) { data.con = 0; data.SOBstr = ""; iniCAN(); data.SOBIT = true; }
            if (data.con == 2) { data.con = 0; data.SOBstr = ""; closeCAN(); data.SOBIT = true; }

            //clear buffer
            if (data.con == 5) { data.con = 0; data.SOBstr = ""; data.очередь.Clear(); pFORM.AddText("БУФЕР  ОЧИЩЕН"); }

           
            ////////////////////////////////////////////////////////////////////////

           Thread.Sleep(100);// pFORM.AddText("main"); 

           

            

        }//work

        bool iniTASK()
        {
            //sleep надо 1сек что бы иницилизировались form1.МЕТОДЫ() 
            //data.adres_FORM1 - иницил.быстрее, а методы позже
            if (data.adres_FORM1 == null) { Thread.Sleep(5000); return (true); }//значит поток формы еще не запустился
            else return (false);
        }


        void TASK_1()//==============  TASK1
        {
            if (iniTASK()) return;

              //  pFORM.AddText("task1");  Thread.Sleep(800);


                //send
                if (data.con == 7) { data.con = 0; data.SOBstr = ""; SEND(); Thread.Sleep(2000); }

                //заливка строки
                if (data.con == 8) { data.con = 0; data.SOBstr = ""; WRITE(data.stroka, data.start, data.end); }

                //очистка памяти табло
                if (data.con == 10) { data.con = 0; data.SOBstr = ""; CLEAR_MEMORY(); }
            
            //вывод
                if (data.con == 11) { data.con = 0; data.SOBstr = ""; vivod(); data.SOBIT = true; }

                if (data.PLAY) {  PLAY(); }
                else
                    if (data.con == 0) Thread.Sleep(100);//чтобы не грузил    


        }//task
        Random rand;
       
         void TASK_2()//==============  TASK2
        {
            if (iniTASK()) return;

        
          if(data.runTEST) test(); 



           // pFORM.AddText("task2"); Thread.Sleep(800);

            if (!data.connOK) { Thread.Sleep(100); }
            else { 
                while(true)// disconnect не предусмотрен
                readMSG(); 
                }

        }//task


        void test()
        {

            rand = new Random();
            data.connOK = true;
            pFORM.AddText("test run");
            while (true)

            {
                if (!data.start_buff) { Thread.Sleep(100); continue; }

                data.test_read++;
            ДАННЫЕочередь buf = new ДАННЫЕочередь()
            {
                time = DateTime.Now,//data.getTIME(),
                strERROR = "",
                adr = (uint)rand.Next(255),
                sz = (byte)(rand.Next(7)+1),
                b1 = (byte)rand.Next(255),
                b2 = (byte)rand.Next(255),
                b3 = (byte)rand.Next(255),
                b4 = (byte)rand.Next(255),
                b5 = (byte)rand.Next(255),
                b6 = (byte)rand.Next(255),
                b7 = (byte)rand.Next(255),
                b8 = (byte)rand.Next(255)
            };
            //ПОМЕЩАЕМ В ОЧЕРЕДЬ
            Thread.Sleep(50);
            if (!data.РАБОТА_С_ОЧЕРЕДЬЮ("write", buf)) data.пропущенные++;
          }        
        }


         static UInt32 Read2=0;
        public void readMSG()
        {
                UInt32 Read = 0;   

                if (Read2 != 0)
                {
                    if (Read2 != sizeFRAME)
                        for (int i = 0; i < Read2; i++)
                        {
                            msgs[i].Id = 0;
                            msgs[i].Data[0] = 0;
                            msgs[i].Data[1] = 0;
                            msgs[i].Data[2] = 0;
                            msgs[i].Data[3] = 0;
                            msgs[i].Data[4] = 0;
                            msgs[i].Data[5] = 0;
                            msgs[i].Data[6] = 0;
                            msgs[i].Data[7] = 0;
                        }
                    else
                    for (int i = 0; i < sizeFRAME; i++)
                    {
                        msgs[i].Id = 0;
                        msgs[i].Data[0] = 0;
                        msgs[i].Data[1] = 0;
                        msgs[i].Data[2] = 0;
                        msgs[i].Data[3] = 0;
                        msgs[i].Data[4] = 0;
                        msgs[i].Data[5] = 0;
                        msgs[i].Data[6] = 0;
                        msgs[i].Data[7] = 0;
                    }
                }
             

                CanDevice.Read(ref msgs, sizeFRAME, ref Read);

              

                Read2 = Read;   
                if (!data.start_buff) { Thread.Sleep(100); return; }

                

                if (Read != 0)
                {
                   

                    data.can++; if (data.can > 1) data.can = 0;//индикатор квадрат

                    for (int i = 0; i < Read; i++)
                    {
                        data.test_read++;
                        ДАННЫЕочередь buf = new ДАННЫЕочередь()
                        {
                            time = DateTime.Now,//data.getTIME(),
                            strERROR = GetStatus(),
                            adr = msgs[i].Id,
                            sz = msgs[i].Size,
                            b1 = msgs[i].Data[0],
                            b2 = msgs[i].Data[1],
                            b3 = msgs[i].Data[2],
                            b4 = msgs[i].Data[3],
                            b5 = msgs[i].Data[4],
                            b6 = msgs[i].Data[5],
                            b7 = msgs[i].Data[6],
                            b8 = msgs[i].Data[7]
                        };
                        //////////////////////////////////////////////////////////////////

                        //ПОМЕЩАЕМ В ОЧЕРЕДЬ
                        if (!data.РАБОТА_С_ОЧЕРЕДЬЮ("write", buf)) data.пропущенные++;
                    }      

                }
        
        }
     
        //*********************************************************************************
      
//*******************************
        void vivod()
    {

                pFORM.AddText("задержка вывода ");
                for (int i = 5; i > 0; i--) { pFORM.AddText(String.Format("{0};", i)); Thread.Sleep(1000); }

                data.SOBITkod = 7;

    }
        void SEND2(uint adr, byte sz, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, string name)
        {

            if (!data.connOK) { pFORM.AddText("-Отмена-  нет связи\n"); return; }


            data.adr = adr;

            data.sz = sz;



            data.b1 = b1;// 0x11;

            data.b2 = b2;// 0x12;

            data.b3 = b3;// 0x00;

            data.b4 = b4;// 0x00;

            data.b5 = b5;// 0x00;

            data.b6 = b6;// 0x00;

            data.b7 = b7;// 0x00;

            data.b8 = b8;// 0x03;


            SEND();  

           Thread.Sleep(200);
          // readMSG();
           

        }


































//***********************************************************************************
//                             CAN moduls
//**************************************************************************************



        string GetStatus()
        {
            if (!data.connOK) { Thread.Sleep(50); return(""); }

            string err="";
            try
            {
                // get extended status and error flags

                CanDevice.GetFlags(ref Flags);


                if (Flags != 0)
                {
                    //err = "Extended status and error flags: " + String.Format("{0}", Flags);
                    
                    err=DecodeFlags(Flags);
                }
            }
            catch { }

            return (err); 
        }



        void closeCAN()
        {


            try
            {
              
                CanDevice.Close();
                data.SOBstr = "DISCONNECT ok "; data.SOBITkod = 3; data.SOBIT = true;
                data.test_read = 0;
                
             //   pFORM.EXIT();
            }

            catch (Exception ex)
            {

                data.SOBstr = "error close "+ex.Message; data.SOBITkod = 4;  data.SOBIT = true;
                //AddText("error canCLOSE  " + ex.Message); 
                // button3.Enabled = true;
                // button7.Enabled = false;
            }
           

        }

        void add(string a)
        {
            data.SOBstr = data.SOBstr + a+"\n";
        }

       


        bool iniCAN()
        {

            try
            {

                msgs = new VSCAN_MSG[2];

                CanDevice = new VSCAN();

                Written = 0;

                // ReadCAN = 0;

                hw = new VSCAN_HWPARAM();

                api_ver = new VSCAN_API_VERSION();

                Flags = 0x0;


                // set debugging options

                CanDevice.SetDebug(VSCAN.VSCAN_DEBUG_NONE);

                CanDevice.SetDebugMode(VSCAN.VSCAN_DEBUG_MODE_FILE);



                // open CAN channel: please specify the name of your device according to User Manual

                CanDevice.Open(VSCAN.VSCAN_FIRST_FOUND, VSCAN.VSCAN_MODE_SELF_RECEPTION);



                // set some options

                CanDevice.SetSpeed(VSCAN.VSCAN_SPEED_250K);

               // CanDevice.SetTimestamp(VSCAN.VSCAN_TIMESTAMP_ON);
             CanDevice.SetTimestamp(VSCAN.VSCAN_TIMESTAMP_OFF);

                CanDevice.SetBlockingRead(VSCAN.VSCAN_IOCTL_ON);
           //   CanDevice.SetBlockingRead(VSCAN.VSCAN_IOCTL_OFF);



                // get HW Params

                CanDevice.GetHwParams(ref hw);

                  add("Get hardware paramter:");

                  add("HwVersion:" + hw.HwVersion + " SwVersion:" + (hw.SwVersion >> 4) + "." + (hw.SwVersion & 0x0f));

                  add("SerNr:" + hw.SerialNr + " HwType:" + hw.HwType);



                // get API version

                CanDevice.GetApiVersion(ref api_ver);


               add("API version: " + api_ver.Major + "." + api_ver.Minor + "." + api_ver.SubMinor + "\n----------------");

                

              data.connOK = true;
               data.SOBITkod = 1;

                return (true);

            }
            catch (Exception ex)
            {
                data.SOBstr = "error connect "+String.Format("{0}", ex); data.SOBITkod = 2;
               // button7.Enabled = false;
               // button3.Enabled = true;



               

              // MessageBox.Show(String.Format("{0}", ex), "Ошибка VSСOM");
                return (false);
            }

        }

        string DecodeFlags(int flags)
        {
           string err = "";

            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_API_RX_FIFO_FULL) > 0)
            {

                err=err+" API RX FIFO full";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ARBIT_LOST) > 0)
            {

                 err=err+"   Arbitration lost";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_BUS_ERROR) > 0)
            {

                if (data.fo1) err=err+" Bus error";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_DATA_OVERRUN) > 0)
            {

                 err=err+"  Data overrun";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ERR_PASSIVE) > 0)
            {

                if (data.fo2) err = err + "  Passive error";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_ERR_WARNING) > 0)
            {

                if (data.fo3) err = err + "  Error warning";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_RX_FIFO_FULL) > 0)
            {

                 err=err+"   RX FIFO full";

            }



            if ((flags & VSCAN.VSCAN_IOCTL_FLAG_TX_FIFO_FULL) > 0)
            {

                 err=err+"  TX FIFO full";

            }


            //if (err != "") err = err + "\n";


            return (err);

        }

        void SEND()
        {

            if (!data.connOK || CanDevice==null) { if (!data.PLAY)pFORM.AddText("-Отмена-  нет связи\n"); return; }


            msgs[0].Id = data.adr;//

            msgs[0].Size = data.sz;// 8;

            msgs[0].Data = new byte[8];

            msgs[0].Data[0] = data.b1;

            msgs[0].Data[1] = data.b2;

            msgs[0].Data[2] = data.b3;

            msgs[0].Data[3] = data.b4;

            msgs[0].Data[4] = data.b5;

            msgs[0].Data[5] = data.b6;

            msgs[0].Data[6] = data.b7;

            msgs[0].Data[7] = data.b8;

            // msgs[0].Flags = VSCAN.VSCAN_FLAGS_EXTENDED;
            msgs[0].Flags = VSCAN.VSCAN_FLAGS_STANDARD;


            // send CAN frames

            CanDevice.Write(msgs, 1, ref Written);

            // send immediately 

            CanDevice.Flush();


            if (Written == 0)
            {
                

                pFORM.AddText("--- Сбой отправки попытка 2"); Thread.Sleep(200);

                CanDevice.Write(msgs, 1, ref Written);
                CanDevice.Flush();

                if (Written == 0)
                {
                    pFORM.AddText("--- Сбой отправки попытка 3"); Thread.Sleep(500);
                    CanDevice.Write(msgs, 1, ref Written);
                    CanDevice.Flush();
                    if (Written == 0)
                    {
                         GLOBERR++; 
                        pFORM.AddText("\n&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&\n--- Сбой отправки НЕ УДАЕТСЯ ОТПРАВИТЬ!! \n"); 
                       
                    }
                }
            
            }

          

        }


        void DIAGRead_CAN()
        {



            UInt32 Read = 0;

            // read CAN frames

            try
            {
                CanDevice.Read(ref msgs, 1, ref Read);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",
                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


       //     AddText("Read CAN frames: " + Read);

            for (int i = 0; i < Read; i++)
            {



             //   AddTextCONT("CAN frame " + i);

              //  AddTextCONT(" ID: " + Convert.ToString(msgs[i].Id));

              //  AddTextCONT(" Size: " + msgs[i].Size);

              //  AddText("Data: ");

                for (int j = 0; j < msgs[i].Size; j++)
                {

                 //   AddText(msgs[i].Data[j] + " ");

                }

               // AddText("");

                if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_STANDARD) != 0)
                    {}
                //    AddText("VSCAN_FLAGS_STANDARD");

                if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_EXTENDED) != 0)
                    {}
                  ///  AddText("VSCAN_FLAGS_EXTENDED");

                if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_REMOTE) != 0)
                    {}
                   // AddText("VSCAN_FLAGS_REMOTE");

                if ((msgs[i].Flags & VSCAN.VSCAN_FLAGS_TIMESTAMP) != 0)
                    {}
                    //AddText("VSCAN_FLAGS_TIMESTAMP");

               // AddText("TS: " + msgs[i].TimeStamp);

            }

        }


       public static DateTime lasttime; 
        void PLAY()
        {

            if (data.очередьКОПИЯ == null) { pFORM.AddText("ОШИБКА НЕ СОЗДАНА КОПИЯ"); return; }
            if (data.очередьКОПИЯрабочая == null) { pFORM.AddText("ОШИБКА НЕ СОЗДАНА КОПИЯрабочая"); return; }

            try
            {

                DateTime datanew = new DateTime();

                uint id; string err;
                byte sz, b1, b2, b3, b4, b5, b6, b7, b8;




                if (!data.connOK) { pFORM.AddText("-Отмена воспр.-  нет связи\n"); Thread.Sleep(2000); return; }


                if (data.очередьКОПИЯрабочая.Count == 0)
                {
                    if (pFORM.ctviv > 2000) pFORM.CLEAR();
 
                    pFORM.AddText("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\nЦикл закончен РЕСТАРТ\n");
                    //  data.очередьКОПИЯрабочая = data.очередьКОПИЯ;


                    data.COPY2();

                    lasttime = new DateTime();
                    Thread.Sleep(500);
                }
                //pFORM.AddText(String.Format("размер рабоч={0}  + копия={1}   o={2}",
                //    data.очередьКОПИЯрабочая.Count, data.очередьКОПИЯ.Count, data.очередь.Count));

                if (data.РАБОТА_С_ОЧЕРЕДЬЮ_PLAY(2)) //читаем
                {

                    err = data.buf_readPLAY.strERROR;
                    datanew = data.buf_readPLAY.time;
                    id = data.buf_readPLAY.adr;
                    sz = data.buf_readPLAY.sz;
                    b1 = data.buf_readPLAY.b1;
                    b2 = data.buf_readPLAY.b2;
                    b3 = data.buf_readPLAY.b3;
                    b4 = data.buf_readPLAY.b4;
                    b5 = data.buf_readPLAY.b5;
                    b6 = data.buf_readPLAY.b6;
                    b7 = data.buf_readPLAY.b7;
                    b8 = data.buf_readPLAY.b8;



                }
                else { Thread.Sleep(50); return; }


            //    data.РАБОТА_С_ОЧЕРЕДЬЮplay("write", data.buf_readPLAY);

              


                try
                {

                    //пауза
                    if (lasttime != new DateTime())
                    {

                        //7:39:55.827

                        var a = datanew.Subtract(lasttime);

                        int pause = (int)Math.Abs(a.TotalMilliseconds);
                        //if (a.TotalMilliseconds>49) Thread.Sleep(a.TotalMilliseconds);







                        //pFORM.AddTextCONT(//"delta ms=" + a.TotalMilliseconds.ToString() + ";"+
                        //String.Format("p={0};", pause));

                          
                        if (pause > 5000) pause = 5000;


                        if (pause >= 50) //иначе зависнет дисплей не будет успевать
                        {
                          pFORM.ctviv++;

                          
                            if (pFORM.ctviv<2000)
                            pFORM.AddTextCONTperv(String.Format("P={0}", pause) + " playID: " + pFORM.HX(id) + "    Size: " + sz + "         Data: " +
                                            String.Format("{0} {1} .{2} {3}  : {4} {5} .{6} {7}  \n", pFORM.HX(b1), pFORM.HX(b2), pFORM.HX(b3), pFORM.HX(b4),
                                                                                                 pFORM.HX(b5), pFORM.HX(b6), pFORM.HX(b7), pFORM.HX(b8)));
                        }


                        if (pause >= 70 && pause < 100000) Thread.Sleep(pause);
                    }


                    lasttime = datanew;
                }
                catch (Exception ex) { pFORM.AddText("Ошибка расчета паузы между сообщениями" + ex.Message); }


                //pFORM.AddTextCONTperv(data.getTIME() + "  TEСТ  ID: " + pFORM.HX(id) + "    Size: " + sz + "         Data: " +
                //                       String.Format("{0} {1} .{2} {3}  : {4} {5} .{6} {7}  \n",
                //                       pFORM.HX(b1), pFORM.HX(b2), pFORM.HX(b3), pFORM.HX(b4),
                //                       pFORM.HX(b5), pFORM.HX(b6), pFORM.HX(b7), pFORM.HX(b8)));

                data.sz = sz;
                data.adr = id;
                data.b1 = b1;
                data.b2 = b2;
                data.b3 = b3;
                data.b4 = b4;
                data.b5 = b5;
                data.b6 = b6;
                data.b7 = b7;
                data.b8 = b8;


                 SEND();


            }
            catch (Exception ex) { pFORM.AddText("ФАТАЛЬНАЯ ОШИБКА ВОСПРОИЗВЕДЕНИЯ " + ex.Message); data.PLAY = false; }



            //if (data.PLAYQueue.Count != 0) {pFORM.AddTextCONT("pz;");
            //    Thread.Sleep(50);}
            //if (data.PLAYQueue.Count != 0)
            //{
            //    pFORM.AddTextCONT("pz2;");
            //    Thread.Sleep(100);
            //} if (data.PLAYQueue.Count != 0)
            //{
            //    pFORM.AddTextCONT("pz3;");
            //    Thread.Sleep(200);
            //} if (data.PLAYQueue.Count != 0)
            //{
            //    pFORM.AddTextCONT("pz4;");
            //    Thread.Sleep(500);
            //}
          

        }


        int GLOBERR;
     
        void CLEAR_MEMORY()
        {

            GLOBERR = 0;

            byte ct = 1 ;
            bool ZAVIS=false;

            byte cc = data.cod_clear_memory;

            while (ct < 200)
            {

               

                if (!data.connOK) { pFORM.AddText("-Отмена ОЧИСТКИ -  нет связи\n"); return; }

                if (!data.TABLOERROR1 && !data.TABLOERROR2)
                {
                    if (ct == 50 || ct == 99 || ct == 150 || ct == 25) { времяВтабло(); Thread.Sleep(1000); }
                   
                }
             //  if (ct > data.t.s1 && ct < data.t.e5) continue;//пропускаем то что будем писать
                int cterr=0;
                if (!data.TABLOERROR1 && !data.TABLOERROR2) ZAVIS = false;


                while ((data.TABLOERROR1 || data.TABLOERROR2) && ZAVIS)//уже рестарт зависание
                {
                    Thread.Sleep(100); 

                }


                while (data.TABLOERROR1 || data.TABLOERROR2)
                {
                    if (!data.TABLOERROR1 && !data.TABLOERROR2) ZAVIS = false;
                    while ((data.TABLOERROR1 || data.TABLOERROR2) && ZAVIS)//уже рестарт зависание
                    {
                        Thread.Sleep(100);

                    }


                    Thread.Sleep(100);   //ждем пока нет ошибок
                    cterr++;




                    if (cterr > 50)
                    {
                        pFORM.AddTextCONT(" рест;"); if (ct > 1) ct--; ZAVIS = true; cterr = 0;
                        SEND2(0x38A, 8, ct, 0x16, cc, cc, cc, cc, cc, cc, ""); continue;
                    }
                }



                if (data.pauseWRITE)
                {
                    pFORM.AddText("\n ПАУЗА ..");
                    while (data.pauseWRITE) Thread.Sleep(100);
                    pFORM.AddText(" продолжаем\n");
                }


                if (!ZAVIS)
                {
                    for (int e = 0; e < 9; e++)
                    {
                        if (data.TABLOERROR1 || data.TABLOERROR2)
                        {
                            if (ct > 1) ct--;
                            break;
                        }  //сбой возврат на пред строку


                        SEND2(0x38A, 8, ct, 0x06, cc, cc, cc, cc, cc, cc, "");



                    }

                    if (data.TABLOERROR1 || data.TABLOERROR2) continue;

                    SEND2(0x38A, 8, ct, 0x16, cc, cc, cc, cc, cc, cc, "");
                    pFORM.AddTextCONT(String.Format("{0};", ct));
                    ct++;
                }
            }

            pFORM.AddTextCONT("\n");
            pFORM.AddText(String.Format("ОЧИСТКА ВЫПОЛНЕНА Ошибок " + String.Format("{0}.\n", GLOBERR) ) );



            времяВтабло();Thread.Sleep(1000);

            pFORM.AddTextCONT("\nИдет запись 1 сообщения ...\n");
            if (!WRITE(data.str1, data.t.s1, data.t.e1)) { pFORM.AddText(String.Format("\nОШИБКА\n")); return; }

            времяВтабло(); Thread.Sleep(1000);

            pFORM.AddTextCONT("\nИдет запись 2 сообщения ...\n");
            if (!WRITE(data.str2, data.t.s2, data.t.e2)) { pFORM.AddText(String.Format("\nОШИБКА\n")); return; }

            времяВтабло(); Thread.Sleep(1000);

            pFORM.AddTextCONT("\nИдет запись 3 сообщения ...\n");
            if (!WRITE(data.str3, data.t.s3, data.t.e3)) { pFORM.AddText(String.Format("\nОШИБКА\n")); return; }

            времяВтабло(); Thread.Sleep(1000);

            pFORM.AddTextCONT("\nИдет запись 4 сообщения ...\n");
            if (!WRITE(data.str4, data.t.s4, data.t.e4)) { pFORM.AddText(String.Format("\nОШИБКА\n")); return; }

            времяВтабло(); Thread.Sleep(1000);

            pFORM.AddTextCONT("\nИдет запись 5 сообщения ...\n");
            if (!WRITE(data.str5, data.t.s5, data.t.e5)) { pFORM.AddText(String.Format("\nОШИБКА\n")); return; }


           

            pFORM.AddTextCONT("\nВОССТАНОВЛЕНИЕ ТАБЛО ВЫПОЛНЕНО. Ошибок " + String.Format("{0}.\n", GLOBERR));


           
        }


      
        bool WRITE(string s, byte b1, byte b2)
        {
            if (!data.connOK) { return (false); }

            bool rez;


            int ctr=0;

            while (true)
            {

                pFORM.AddText(String.Format("\nПИШЕМ {0}- {1}...\n", b1,b2));
                rez = strSEND(s, b1, b2);
                if (!rez)
                {
                    ctr++;
                    pFORM.AddText(String.Format("СБОЙ ПРИ ЗАПИСИ СТРОКИ ПОВТОРЯЕМ попытка {0}...\n", ctr));

                    //  rez = strSEND(" ", (byte)(b1 + 1), (byte)(b1 + 1));//переиницилизируем табло
                    //  rez = strSEND(s, b1, b2);
                }
                else break;
            }

           

            return (true);
        
        }



        bool strSEND(string ww, byte startSTR, byte endSTR)
        {
            byte STR=0;
            //возрат false ЗНАЧИТ при записи выскочила авария табло
            bool ZAVIS=false;

            uint ctMEM = 0; 

            //while (data.TABLOERROR1 || data.TABLOERROR2)
            //{ Thread.Sleep(1000); cterr++; if (cterr == 10) { pFORM.AddText("!!!!  Табло ЗАВИСЛО перезапустить "); } }
//******************************************************************************


            int cterr = 0;

           



            //вышли из ошибки но надо выходить и снова переписывать
          //  if (ZAVIS) return(false);

//***************************************************************************************************
            if (ww.Length > (endSTR - startSTR) * 60)
            {
                pFORM.AddText("!!!  ОШИБКА ТЕКСТ НЕ ВЛЕЗАЕТ В ДИАПАЗОН СТРОК");
                return (true);
            }

            int lle = ww.Length;

            if (lle < (endSTR - startSTR + 1) * 60)
            {
                for (int e = 0; e < (((endSTR - startSTR + 1) * 60) - lle); e++)
                {
                    // pFORM.AddText(String.Format("e={0}",e ));
                    ww = ww + " ";
                }

            }


            uint ct = 0; int o = 0;
            STR = startSTR;

            byte[] b = Encoding.Default.GetBytes(ww);
            byte[] f = new byte[100];


            while (o < b.Length)
            {

                if (!data.connOK) { pFORM.AddText("-Отмена записи строки -  нет связи\n"); return (true); }
                //---------------------------------------------------
                if (ZAVIS) {
                    if (ct == 0) { ct = 10; ctMEM = 10;   pFORM.AddText(String.Format("ct=0 ставим={0}", ctMEM)); }
                   
                    o = o - (int)(6 * ctMEM);    if (o < 0) o = 0; 
                    ZAVIS = false; 
                    pFORM.AddText(String.Format("возврат. стоп на подстроке={0}", ctMEM));
                  
                    ct = 0; }

                int j = 0;
                while (j < 6)
                {
                   
                    try
                    {
                        if (o < b.Length) f[j] = b[o]; else f[j] = 32;

                    }
                    catch (Exception ex) { pFORM.AddText(String.Format("err={0}", ex.Message)); }

                    o++; j++;
                }

                //pFORM.AddText("формир f[]="+String.Format(" >{0}{1}{2}{3}{4}{5}<", pFORM.toSYMBOL(f[0]),
                //   pFORM.toSYMBOL(f[1]), pFORM.toSYMBOL(f[2]), pFORM.toSYMBOL(f[3]),
                //   pFORM.toSYMBOL(f[4]), pFORM.toSYMBOL(f[5])) ); 
                //---------------------------------------------------

                if (ct < 10) { ct++; }//подстроки
                else
                {
                    ct = 1;


                    STR++;
                   Thread.Sleep(500);//а то не успевает табло

                    if (data.pauseWRITE)
                    {
                        pFORM.AddText("\n ПАУЗА ..");
                        while (data.pauseWRITE) Thread.Sleep(100);
                        pFORM.AddText(" продолжаем\n");
                    }


                    if (STR > endSTR) { pFORM.AddText("ТЕКСТ  НЕ  ВЛЕЗАЕТ В ДИАПАЗОН СТРОК!!!!!!!!!!!!!!!! ОШИБКА\n\nОШИБКА!!!"); return (true); }
                }



                string ii1 = String.Format(" >{0}{1}{2}{3}{4}{5}<", pFORM.toSYMBOL(f[0]),
                   pFORM.toSYMBOL(f[1]), pFORM.toSYMBOL(f[2]), pFORM.toSYMBOL(f[3]),
                   pFORM.toSYMBOL(f[4]), pFORM.toSYMBOL(f[5]));




                if (o >= b.Length || ct == 10)
                {
                    pFORM.AddText(String.Format("- финал" + ii1 + " " + String.Format("СТРОКА {0} подстрока {1} ", STR, ct)));



                    if (data.TABLOERROR1 || data.TABLOERROR2)// return (false);//все пропало с ошибкой
                    {
                        ZAVIS = true; cterr = 0;
                        byte wrSTR = STR;
                        wrSTR++; ;
                        if (STR == startSTR) { wrSTR = STR; wrSTR++; }
                        if (STR == endSTR) { wrSTR = STR; wrSTR--; STR--; }

                        pFORM.AddTextCONT("сброс на посл. строке; ");
                        СБРОСтабло();
                        break;
                    
                    }
                    SEND2(0x38A, 8, STR, 0x16, f[0], f[1], f[2], f[3], f[4], f[5], "запись последняя ");
                }
                else
                {
                    pFORM.AddText(String.Format("- пишем " + ii1 + " " + String.Format("СТРОКА {0} подстрока {1} ", STR, ct)));

                   // if (data.TABLOERROR1 || data.TABLOERROR2) return (false);//все пропало с ошибкой
//********************************************************************
                    cterr = 0;
                    if (!data.TABLOERROR1 && !data.TABLOERROR2) ZAVIS = false;

                    while ((data.TABLOERROR1 || data.TABLOERROR2) && ZAVIS)//уже рестарт зависание
                    { Thread.Sleep(1000); pFORM.AddTextCONT("}");
                    if (!data.connOK) { pFORM.AddText("-Отмена-  нет связи\n"); return (true); }
                    }


                    while (data.TABLOERROR1 || data.TABLOERROR2)
                    {
                        if (!data.TABLOERROR1 && !data.TABLOERROR2) ZAVIS = false;

                        int tablerr = 0;
                        while ((data.TABLOERROR1 || data.TABLOERROR2) && ZAVIS)//уже рестарт зависание
                        { 
                            Thread.Sleep(1000); pFORM.AddTextCONT(";");
                            if (!data.connOK) { pFORM.AddText("-Отмена записи строки-  нет связи\n"); return (true); }
                            tablerr++;
                            if (tablerr > 5) 
                            {
                                СБРОСтабло(); tablerr = 0; pFORM.AddTextCONT("сбр;");
                            }
                        }


                        Thread.Sleep(100);   //ждем пока нет ошибок
                        cterr++;


                        if (cterr > 50)
                        {
                            ZAVIS = true; cterr = 0;


                            byte wrSTR = STR; 
                            wrSTR++; ;
                            if (STR == startSTR) { wrSTR = STR; wrSTR++;       }
                            if (STR == endSTR) { wrSTR = STR; wrSTR--; STR--;     }

                            pFORM.AddTextCONT("сброс; ");
                            СБРОСтабло();
                            break;
                        }
                    }

                    //**********************************************

                    if (!ZAVIS)  SEND2(0x38A, 8, STR, 0x06, f[0], f[1], f[2], f[3], f[4], f[5], "запись ");





                }//else не последняя часть не финал

                if (ZAVIS) { ctMEM = ct; pFORM.AddTextCONT("неуд на" + String.Format(" подстроке={0};   ",ct)); continue; }

            }

            if (STR != endSTR) { pFORM.AddText(String.Format("ОШИБКА НЕ СООТВ СТРОК  с {0} по {1} (останов на {2})  ошибок={3};\n", startSTR, endSTR, STR, GLOBERR)); return (false); }
            pFORM.AddText(String.Format("\nЗАПИСЬ ВЫПОЛНЕНА.  ИТОГО ЗАНЯТЫЕ СТРОКИ с {0} по {1} (останов на {2})  ошибок={3};\n", startSTR, endSTR, STR, GLOBERR));
            return (true);
        }


        void СБРОСтабло()
        {
            SEND2(0x38A, 8, 199, 0x06, 32, 32, 32, 32, 32, 32, ""); Thread.Sleep(100);
            SEND2(0x38A, 8, 199, 0x16, 32, 32, 32, 32, 32, 32, ""); Thread.Sleep(100);
        }

        void времяВтабло()
        {

          //  SEND(0x18A, 8, pFORM.BCD(1, 1), pFORM.BCD(1, 2), pFORM.BCD(1, 3), pFORM.BCD(1, 3),
           //     pFORM.BCD(2, 0), pFORM.BCD(1, 5), 1, 0, "туалет свободен");


            data.adr = 0x18A;

            data.sz = 8;

            data.b1 = pFORM.BCD(1, 1);// 0x11;

            data.b2 = pFORM.BCD(1, 2);// 0x12;

            data.b3 = pFORM.BCD(1, 3);// 0x00;

            data.b4 = pFORM.BCD(1, 3);// 0x00;

            data.b5 = pFORM.BCD(2, 0);// 0x00;

            data.b6 = pFORM.BCD(1, 5);// 0x00;

            data.b7 = 1;// 0x00;

            data.b8 = 0;// 0x03;

            SEND();

           
        }
    } //end class ПОТОКА
}//namespace


