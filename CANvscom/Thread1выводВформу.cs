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


namespace Program
{

    //public partial class Thread1
    public partial class Form1 : Form
    {
         
        public int ctviv;//счетчик выведенных сообщений с редактор
          
            public void StartThread()//main
        {
            try { while (!data.t1_closing) main(); } catch (Exception ex)
            { MessageBox.Show(String.Format("ОШИБКА ПОТОКА1 {0};", ex.Message), "Фатальная Ошибка"); } 
            data.t1closeOK=true;
        }
          


        public byte BCD(int a, int b)//упаковано 2 числа по 4 бита каждое
        {

            if (a > 9 || b > 9)
            {
                MessageBox.Show("ОШИБКА BCD функции число более 9", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                AddText(String.Format(" a={0}  b={1} ", Convert.ToString(a, 2), Convert.ToString(b, 2)));

                return 1;
            }

            return (byte)((a << 4) | b);

        }
        
        
         public  void BCDtoBYTE(ref byte BCD,ref byte ML, ref byte HL)//распаковка  BCD по HL ML
        {
           byte qw = BCD;

           int rr = BCD;

          

           rr = (byte)((qw & B._1111._0000));
           ML = (byte)(rr >> 4);   
              

               HL = (byte)(qw & B._0000._1111);




           //   AddText( Convert.ToString(BCD & B._0000._1111, 2));

            return ;

        }
        
        
        public string HX(UInt32 x) //int -- в string в 16виде
        {
            string l = Convert.ToString(x, 16).ToUpperInvariant();
            if (l.Length == 1) l = "  0" + l;
            else if (l.Length == 2) l = "  " + l;
            else if (l.Length == 3) l = "0" + l;
            return (l);
        }


        public UInt32 HB(string x) //HEX(string) --  в uint32
        {
            if (x == null || x == "") return 1;
            if (x.Length > 4)
            {
                MessageBox.Show("слишком болшое HEX число >" + x + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            }
            UInt32 dd = 1;

            try
            {
                dd = System.Convert.ToUInt32(x, 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ошибка HEX  >" + ex.Message + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            };
            return dd;
        }
        public string HS(string HexValue)// HEX в виде string - в String СИМВОЛЫ
        {
          
            string result = "";
            while (HexValue.Length > 0)
            {
                AddText("конверт " + HexValue);
                string aa = "";

                string ee=HexValue.Substring(0, 2);
                try
                {
                  //  aa = System.Convert.ToChar(System.Convert.ToUInt32( ee,16 ).ToString();

                }
                catch { AddText("err HS() convert to string"); }



                if ( Convert.ToByte(aa)>192  ) result += aa;
                else result += '.';
                
                HexValue = HexValue.Substring(2, HexValue.Length - 3);
                
            }
            return result;
        }

        public UInt32 SU(string x)// стринг в uint
        {
            if (x == "") return 1;
            UInt32 dd = 1;
            try
            {
                dd = System.Convert.ToUInt32(x, 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ошибка ввода чисел  >" + ex.Message + "<", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return 1;
            };
            return dd;

        }

         public string toSYMBOL(byte a)// ASCII  в UNICODE для вывода на экран
        {
             
             if ((a < 192 && a > 122) || a < 31) return (".");

            byte []t = new byte[1];
            t[0] = a;

          
           string rez = Encoding.GetEncoding(1251).GetString(t,0,t.Length) ;

             return (rez);

        }


        //============== ГЛАВНЫЙ ЦИКЛ ПОТОКА 1     формы1
        void main()
        {

            uint id; string time,err;
            byte sz,b1,b2,b3,b4,b5,b6,b7,b8;
            data.count = data.очередь.Count();//всегда должен считать



            if (data.очередь == null ) { Thread.Sleep(50); return; }
            if ( !data.start_ReadBuff) { Thread.Sleep(50); return; }
            if (data.count > 500000 ) data.start_buff = false;



            if (data.PLAY) { Thread.Sleep(1000); return; }


            if (ctviv>2000) {
                if (data.start_ReadBuff) AddText("\n--------- ОСТАНОВКА ПО КОЛИЧЕСТВУ СТРОК -------------\n");
                data.start_ReadBuff=false;  Thread.Sleep(50); return; }

            time = ""; err = "";


          

            try
            {

                if (data.count == 0)
                {
                    if (data.text != null) { AddText(data.text); data.text = null; }
                                        Thread.Sleep(50); return; }
                else
                {


                    if (data.РАБОТА_С_ОЧЕРЕДЬЮ("read", null)) //читаем
                    {

                        err = data.buf_read.strERROR;
                        time = data.getTIME(data.buf_read.time);
                        id = data.buf_read.adr;
                        sz = data.buf_read.sz;
                        b1 = data.buf_read.b1;
                        b2 = data.buf_read.b2;
                        b3 = data.buf_read.b3;
                        b4 = data.buf_read.b4;
                        b5 = data.buf_read.b5;
                        b6 = data.buf_read.b6;
                        b7 = data.buf_read.b7;
                        b8 = data.buf_read.b8;



                    }
                    else { Thread.Sleep(50); return; }

                    //if (!data.PLAY)
                    //{
                    //    if (data.РАБОТА_С_ОЧЕРЕДЬЮ("read", null)) //читаем
                    //    {

                    //        err = data.buf_read.strERROR;
                    //        time = data.getTIME(data.buf_read.time);
                    //        id = data.buf_read.adr;
                    //        sz = data.buf_read.sz;
                    //        b1 = data.buf_read.b1;
                    //        b2 = data.buf_read.b2;
                    //        b3 = data.buf_read.b3;
                    //        b4 = data.buf_read.b4;
                    //        b5 = data.buf_read.b5;
                    //        b6 = data.buf_read.b6;
                    //        b7 = data.buf_read.b7;
                    //        b8 = data.buf_read.b8;



                    //    }
                    //    else { Thread.Sleep(50); return; }

                    //}
                    //else
                    //{
                    //    //================ ИДЕТ  PLAY ==========================
                    //    if (data.РАБОТА_С_ОЧЕРЕДЬЮplay("read", null)) //читаем
                    //    {

                    //        err = data.buf_read.strERROR;
                    //        time = data.getTIME(data.buf_read.time);
                    //        id = data.buf_read.adr;
                    //        sz = data.buf_read.sz;
                    //        b1 = data.buf_read.b1;
                    //        b2 = data.buf_read.b2;
                    //        b3 = data.buf_read.b3;
                    //        b4 = data.buf_read.b4;
                    //        b5 = data.buf_read.b5;
                    //        b6 = data.buf_read.b6;
                    //        b7 = data.buf_read.b7;
                    //        b8 = data.buf_read.b8;



                    //    }
                    //    else { Thread.Sleep(50); return; }
                    //}

                   


                    if (id == 0x1D5)
                    {   
                    	byte HL1=0,ML1=0,HL2=0,ML2=0;
                    	
                    	BCDtoBYTE(ref b3, ref ML1, ref HL1);
                    	BCDtoBYTE(ref b4, ref ML2, ref HL2);

                        if (b1 == 0 && b2 == 0) { button5.Text = "табло ОК   "; data.TABLOERROR1 = false; } else { button5.Text = "авария строки  "; data.TABLOERROR1 = true; }
                        button5.Text += String.Format("поезд {0}{1}{2}{3}", ML2,HL2,ML1,HL1 );

                       
                    }
                    else
                        if (id == 0x1D6)
                        {
                        	byte HL1=0,ML1=0,HL2=0,ML2=0;
                    	
                         	BCDtoBYTE(ref b3, ref ML1, ref HL1);
                        	BCDtoBYTE(ref b4, ref ML2, ref HL2);

                            if (b1 == 0 && b2 == 0) { button6.Text = "табло ОК    "; data.TABLOERROR2 = false; } else { button6.Text = "авария строки  "; data.TABLOERROR2 = true; }
                            button6.Text += String.Format("поезд {0}{1}{2}{3}", ML2,HL2,ML1,HL1 );
                        }
						


                            if (err != "" && err != null) AddTextCONT(time + "-----  ошибки CAN: >" + err + "<\n");

                            if ((id >= data.diap1 && id <= data.diap2) ||
                                (id == data.di1 || id == data.di2 || id == data.di3 || id == data.di4)

               )
                            {//filter

                                ctviv++;//счетчик принятых для ограничения

                                data.test_prin++;//принято суммарно за сессию



                                if (id == 0x38A)//ЗАЛИВКА СТРОК
                                {


                                    AddTextCONTperv(time + " ID: 38A   заливка: " + String.Format("номер строки {0};   {1} >{2}{3}{4}{5}{6}{7}< символы\n", b1, HX(b2),
                                                                                              toSYMBOL(b3), toSYMBOL(b4), toSYMBOL(b5), toSYMBOL(b6), toSYMBOL(b7), toSYMBOL(b8)
                                                                                                                          ));


                                }
                                else
                                    if (id == 0x28A)//вывод
                                    {
                                        //SEND(0x28A, 6, 0, 0, 0, 0,          s1, s2,       0,0, "вывести строку ");
                                        AddTextCONTperv(time + " ID: " + HX(id) + "    Size: " + sz + "         Data: " +
                                         String.Format("вывод строк от {0} по {1};  \n", b5, b6));

                                    }

                                    else //неизвестные сообщения
                                    {
                                        AddTextCONTperv(time + " ID: " + HX(id) + "    Size: " + sz + "         Data: " +
                                           String.Format("{0} {1} .{2} {3}  : {4} {5} .{6} {7}  \n", HX(b1), HX(b2), HX(b3), HX(b4),
                                                                                                HX(b5), HX(b6), HX(b7), HX(b8)));

                                        if (id == 0x2e5)
                                        {
                                            string ss = "";
                                            try
                                            {

                                                // ss = Convert.ToBoolean(b2).ToString()+":"+  Convert.ToBoolean(b1).ToString();
                                                ss = Convert.ToString(b2, 2).PadLeft(8, '0') + " : " + Convert.ToString(b1, 2).PadLeft(8, '0');
                                            }
                                            catch (Exception ex) { AddTextCONTperv(ex.Message); }

                                            string errb="";

                                             if (b2 == 0xF0) {

                                                 if (b1 == 0x1b) errb = "нет тока возбуждения";
                                                 if (b1 == 0x1c) errb = "плата не может измерить напряжение АКБ";
                                                 if (b1 == 0x10) errb = "отказ ДРАЙВЕРА(или транзистора) на плате БРНГ";
                                                 if (b1 == 0x0A) errb = "нет фазы А";
                                                 if (b1 == 0x0B) errb = "нет фазы B";
                                                 if (b1 == 0x0C) errb = "нет фазы C";
                                                

                                                 if (b1 == 0x01) errb = "авария ключей";
                                                 if (b1 == 0x02) errb = "Превышение напряжения на АКБ";
                                                 if (b1 == 0x03) errb = "Обрыв обмотки возбуждения";
                                                 if (b1 == 0x04) errb = "Большой ток идет на АКБ";
                                                 if (b1 == 0x05) errb = "Большой ток нагрузки";
                                                
                                            }

                                            AddTextCONTperv(time + String.Format("СОСТОЯНИЕ ") + HX(b2) + HX(b1) + "   bits:" + ss +" "+ errb +"\n");
                                           

                                            //	ток АКБ}
                                            //       AnalogGenT[cIAccumul]:=round((sWRD(pMsg.Data[5], pMsg.Data[4]))/17.8*100);
                                            //    {напряжение АКБ}
                                            //     AnalogGenT[cUAccumul]:=round((WRD(pMsg.Data[3], pMsg.Data[2]))/28.8*100);
                                            //    {Температура АКБ}
                                            //     TakingTemperatureT[cAccumul1]:=round((sWRD(pMsg.Data[7], pMsg.Data[6]))/4096*64);
                                            AddTextCONTperv(time + String.Format(" напряжение АКБ={0:F3}     ток заряда АКБ={1:F3}      темпер={2}\n",
                                                                                            (double)((b3 + (b4 * 256))) * 0.03466,/// 28.8 * 100,//* 0.03466,
                                                                                              (double)((b5 + (b6 * 256))) * 0.056,/// 17.8 * 100,//* 0.056,
                                                                                              (double)((b7 + (b8 * 256))) * 0.015 + (0.5) /// 4096 * 64//* 0.015


                                               ));
                                        }
                                        else


                                            //    {частота}
                                            //      sFreq:=round((WRD(pMsg.Data[5], pMsg.Data[4]))/10.24);                   }
                                            //{ток ОВ}
                                            //      AnalogGenT[cIDrive]:=round((WRD(pMsg.Data[3], pMsg.Data[2]))/819*10);

                                            //if MagStateT[mStation]=false then   {Ток генератора}
                                            //     AnalogGenT[cIGen]:=round((WRD(pMsg.Data[1], pMsg.Data[0]))/17.8 *10) else

                                            //V_kmch:=round(0.25 * (AnalogGenT[cFreq]+sFreq)/2);
                                            if (id == 0x3e5)
                                            {
                                             //   string f="";


                                                AddTextCONTperv(time + String.Format(" ток генер={0:F3}     ток ОВ={1:F3}      част={2}  фазы={3}\n",
                                                                                                 // (double)(b3 + (b4 * 256))/ 12.1,//// / 17.8 * 10, //(100),         //    /100
                                                                                                 //  (double)((b5 + (b6 * 256)))/ 819 * 10  /2,//* 0.0012207,   //  /2
                                                                                                 //(double)((b7 + (b8 * 256))) / 10.24//* 0.09765

                                                                                                 (double)(b1 + (b2 * 256))/17.8 ,// 12.1,//// / 17.8 * 10, //(100),         //    /100
                                                                                                   (double)((b3 + (b4 * 256)))/ 819  ,//* 0.0012207,   //  /2
                                                                                                 (double)((b5 + (b6 * 256))) / 10.24//* 0.09765  {400Гц=1000h => 1Гц=Ah=10}

                                                                                                 , Convert.ToString(b8, 2) + ":" + Convert.ToString(b7, 2) 
                                                                                                 ));
                                            }
                                            else
                                                if (id == 0x4e5)
                                                {
                                                    string type = "неизв." + string.Format("cod={0}",b7); ;
                                                    if (b7 == 1) type = "кислотная";
                                                    if (b7 == 2) type = "кисл.необслуж.";
                                                    if (b7 == 3) type = "щелочная";

                                                    AddTextCONTperv(time + String.Format(" Общий ток потребл={0:F3}     Темп.радиатора БРН32={1}  типАКБ={2} \n",
                                                                                                     (double)(b1 + (b2 * 256)) / 17.8,

                                                                                                     (double)((b5 + (b6 * 256))) / 4096 * 64,

                                                                                                     type
                                                                                                     ));
                                                }
                                                else




                                                if (id == 0x20a)
                                                {

                                                    byte HL1 = 0, ML1 = 0, HL2 = 0, ML2 = 0,  HL3 = 0, ML3 = 0 ;

                                                    BCDtoBYTE(ref b3, ref ML3, ref HL3);
                                                    BCDtoBYTE(ref b4, ref ML1, ref HL1);
                                                    BCDtoBYTE(ref b5, ref ML2, ref HL2);

                                                    AddTextCONTperv(time + String.Format(" запись в табло Nвагона={0}{1}   поезд {2}{3}{4}{5}\n",ML3,HL3, ML2,HL2,ML1,HL1
                                                                                                  
                                                        ));
                                                }


                                        if (id == 0x6d3)
                                        {

                                            string ss = Convert.ToString(b2, 2).PadLeft(8, '0');// +" : " + Convert.ToString(b1, 2).PadLeft(8, '0');

                                            string sr = "";
                                            if ((b2 & B._0000._0010) == 2) sr = "тип АКБ щелочная";else
                                                if ((b2 & B._0000._0010) == 0) sr = "тип АКБ кислотная";
                                                

                                            AddTextCONTperv(time + String.Format(" запись в БРНГ-ЭЛСИЛ пакет{0}; d={1}; {2}\n",b1,ss,sr));


                                            if (b1 == 0) ss = "тип АКБ кислотная"; else
                                            if (b1 == 1) ss = "тип АКБ щелочная"; else
                                             ss = "тип АКБ неизвестно";
                                            ss = ss + String.Format("  коррекция напр={0};",b3);
                                            AddTextCONTperv(time + String.Format(" запись в БРНГ  {0} \n", ss)); 

                                        }


                                    }

                            }//filter
                            else { data.test_ignor++; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка main",
                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }



        }




       // }

    } //end class ПОТОКА
}//namespace


