// Программа
using System;
using System.Windows.Forms;
using System.Threading;
using  System.Text;


// Другие директивы using удалены, поскольку они не используются в данной
// программе
namespace Program
{
  
    public partial class Form1 : Form
    {
        public Thread t1;//дополнительный поток в форме
        

        bool restart_prokrutka;

        byte speed = 5;
        public Form1()
        {
            
            InitializeComponent();
            button5.Text = "";
            button6.Text = "";
           // button7.Enabled = false;
            textBox5.Text = "9";
            data.diap1 = HB(textBox1.Text);
            data.diap2 = HB(textBox2.Text);

           
            data.start_buff = true;
            data.start_ReadBuff = true;


           
            


            data.adres_FORM1 = this;

            inistr();
            //button28.BackColor = System.Drawing.Color.BlueViolet;
            //button27.BackColor = System.Drawing.Color.LightGray;
            //button26.BackColor = System.Drawing.Color.LightGray;

            if (checkBox7.Checked) { data.str3 = data.str3a; data.str4 = data.str4a; }
            else { data.str3 = data.str3b; data.str4 = data.str4b; }

            ras();

        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "CAN VSCOM ";
           // richTextBox1 = System.Drawing.Color.DarkMagenta;
            // Оба события щелчка указателем мыши на пунктах меню "Открыть
            // в формате RTF" и "Открыть в формате Win1251" будем
            // обрабатывать одной процедурой ОТКРЫТЬ:
            открытьВФорматеRTFToolStripMenuItem.Click += 
                                            new EventHandler(ОТКРЫТЬ);
            открытьВФорматеWin1251ToolStripMenuItem.Click += 
                                            new EventHandler(ОТКРЫТЬ);

            timer1.Interval = 100;
            timer1.Enabled = true;

            Control.CheckForIllegalCrossThreadCalls = false;

            //запуск ПОТОКА  внутренней функции
            t1 = new Thread(new System.Threading.ThreadStart(StartThread)); // Start Thread Session
            t1.IsBackground = true;
            t1.Start();            
        }
        void inistr()
        {
            data.stroka = data.str1;
            button13.BackColor = System.Drawing.Color.RoyalBlue;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.LightGray;
            textBox3.Text = Convert.ToString(data.t.s1, 10);
            textBox4.Text = Convert.ToString(data.t.e1, 10);
        }
       
        //**********************************************************************************************
        //======================= ТАЙМЕР 1 ==============================
        //**********************************************************************************************
        private void timer1_Tick_1(object sender, EventArgs e)//------ ТАЙМЕР -------------
        { 
            if (checkBox5.Checked) { button12.Enabled = true; button25.Enabled = true; button9.Enabled = true; }
            else { button12.Enabled = false; button25.Enabled = false; button9.Enabled = false; }

            if (data.can==0) { label13.BackColor = System.Drawing.Color.Bisque;  }
            else { label13.BackColor = System.Drawing.Color.Green; }


            if (data.start_buff)
            {
                button17.Text = "СТОП Буфера " + String.Format("{0}", data.count);
            }
            else { button17.Text = "ПУСК Буфера " +String.Format("{0}", data.count); }

            if (data.start_ReadBuff)
            { button19.Text = "СТОП Вывода"; }
            else
            { button19.Text = "ПУСК Вывода"; }


            label24.Text = String.Format("{0}",data.пропущенные);
            label17.Text = String.Format("{0}", data.test_read);
            label18.Text = String.Format("{0}", data.test_prin);
            label19.Text = String.Format("{0}", data.test_ignor);


            if (checkBox1.Checked && restart_prokrutka)//----------- ПРОКРУТКА
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                restart_prokrutka = false;
            }


            //*******************************  COMMAND ************************
           
            if (richTextBox1.Text == "set7") {
                data.cod_clear_memory = 55;
                richTextBox1.Text = richTextBox1.Text + " <-   ПРИНЯТО. будет заполнение табло >7<\n";
                                             }
            if (richTextBox1.Text == "test")
            {
                data.runTEST = true;
                richTextBox1.Text = richTextBox1.Text + " <-   ПРИНЯТО. ТЕСТ\n";
            }


            //*******************************  ************************
            //СОБЫТИЯ ВНЕШНИХ КЛАССОВ
            if (data.SOBIT)
            {
                data.SOBIT = false;
                if (data.SOBITkod == 1) { AddText(">"+data.SOBstr); }//conn ok
                if (data.SOBITkod == 2) { AddText(">"+data.SOBstr);// button7.Enabled = false;
                    button3.Enabled = true; }//err coonect

                if (data.SOBITkod == 3) { AddText(">" + data.SOBstr); } //disconn ok
                if (data.SOBITkod == 4) { AddText(">"+data.SOBstr );  }//err discon

                if (data.SOBITkod == 7) { viv(); }//
            }

        }




        void ОТКРЫТЬ(Object sender, EventArgs e)
        {
            // Процедура обработки событий открытия
            // файла в двух разных форматах.
            // Выясняем, в каком формате открыть файл:
            var t = (ToolStripMenuItem)sender;
            // Читаем надпись на пункте меню:
            var Формат = t.Text;
            try
            {   // Открыть в каком-либо формате:
                if (Формат == "Открыть в формате RTF")
                {
                    openFileDialog1.Filter =
                                       "Файлы RTF (*.RTF)|*.RTF";

                    if (data.lastpath == null)
                    {
                        openFileDialog1.InitialDirectory = "D:";
                        openFileDialog1.FileName = openFileDialog1.InitialDirectory + @"\tabloTIS";
                    }
                    else openFileDialog1.InitialDirectory = data.lastpath;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    { data.lastpath = openFileDialog1.FileName; richTextBox1.LoadFile(openFileDialog1.FileName); }
                   
                }
                if (Формат == "Открыть в формате txt")//Win1251"
                {
                    openFileDialog1.Filter ="Текстовые файлы (*.txt)|*.txt";

                    if (data.lastpath == null)
                    {
                        openFileDialog1.InitialDirectory = "D:";
                        openFileDialog1.FileName = openFileDialog1.InitialDirectory + @"\tabloTIS";
                    }
                    else openFileDialog1.InitialDirectory = data.lastpath; 

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                       data.lastpath = openFileDialog1.FileName; 
                      richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText); 
                    }
                }

                richTextBox1.Modified = false;
            }
            catch (System.IO.FileNotFoundException Ситуация)
            {
                MessageBox.Show(Ситуация.Message +
                    "\nНет такого файла", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception Ситуация)
            {   // Отчет о других ошибках
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void сохранитьВToolStripMenuItem_Click( object sender, EventArgs e)
        {
            if (data.lastpath == null)
            {
                saveFileDialog1.InitialDirectory = "D:";
                saveFileDialog1.FileName = openFileDialog1.InitialDirectory + @"\CANBUFFER.canbf";
            }
            else saveFileDialog1.InitialDirectory = data.lastpath;
            saveFileDialog1.Filter = "Файлы txt (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
            { data.lastpath = saveFileDialog1.FileName; Запись(); }
        }
        void Запись()
        {
            try
            {              
                richTextBox1.SaveFile(saveFileDialog1.FileName,RichTextBoxStreamType.PlainText);
                richTextBox1.Modified = false;
            }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        void closeT1() 
        {
            int a=0;
            data.t2_closing = true;
            while (!data.t2closeOK) { Thread.Sleep(50); a++; 
                if (a > 40) { MessageBox.Show(String.Format("не могу закрыть поток t2"), "Ошибка Закрытия"); break; } }
            
            data.t1_closing = true;
            //while (!data.t1closeOK) Thread.Sleep(50);
            t1.Join(2000);
            if (!data.t1closeOK) MessageBox.Show(String.Format("не могу закрыть поток t1"), "Ошибка Закрытия"); 

            
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();//вызывает Form1_FormClosing(
          
        }
        public void EXIT()
        {
            this.Close();
        }
        private void Form1_FormClosing( object sender, FormClosingEventArgs e)
        {
            closeT1();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            restart_prokrutka = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SEND(0x18A, 8, BCD(1,1),BCD(1,2), BCD(1,3),BCD(1,4),    BCD(2,0),BCD(1,5),    1, B._0000._0011 ,"туалет занят");

        }

     
        //****************************************************************************************

        //****************************************************************************************
        


    //    Color[] colors = {Color.Black, Color.Red, Color.Blue, Color.Green, Color.Yellow};

   
   

     public void AddTextCol(string message, System.Drawing.Color color)
        {
            try
            {
                this.BeginInvoke(new LineReceivedEvent(LineReceived), data.getTIME() + " " + message + "\n");

                int start = richTextBox1.Text.Length;
                int length = message.Length;

                richTextBox1.Select(start, length); //выделяем текст
                richTextBox1.SelectionColor = color; //для выделенного текста устанавливаем цвет

            }
            catch { }
        }

        public void AddText(string message)
        {

            try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), data.getTIME() + " " + message + "\n");
            }
            catch {}
        }
        public void AddTextNOtime(string message)
        {
            try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), message + "\n");
            }
            catch { }
        }
        public void AddTextCONTperv(string message)
        {
            try
            {
            this.BeginInvoke(new LineReceivedEvent(LineReceived), message);
            }
            catch {}
        }

        public void AddTextCONT(string message)
        {
             try {
            this.BeginInvoke(new LineReceivedEvent(LineReceived),  " " + message);
             }
             catch { }
        }

        private delegate void LineReceivedEvent(string command);
        private void LineReceived(string message2)
        {
            richTextBox1.AppendText(message2 ); //AddText(POT);
        }



        //****************************************************************************************

        //****************************************************************************************

        private void button4_Click(object sender, EventArgs e)
        {
            ctviv = 0;
            richTextBox1.Clear();
        }
        public void CLEAR ()
        {
        ctviv = 0;
            richTextBox1.Clear();
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddText("ПОДКЛЮЧЕНИЕ ...  ");
            //button7.Enabled = true;
            button3.Enabled = false;

            data.con = 1;

        }




        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Text = ""; 
            data.TABLOERROR1 = true;//сбросить должно табло
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Text = "";
            data.TABLOERROR2 = true;//сбросить должно табло
        }

        private void button7_Click(object sender, EventArgs e)
        {

            AddText("отключение...");

            button3.Enabled = true;
            data.connOK = false;
            data.con = 2;

           

           // closeT1();


            button3.Enabled = true;
          //  button7.Enabled = false;
 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SEND(0x18A, 8, BCD(1, 1), BCD(1, 2), BCD(1, 3), BCD(1, 4), BCD(2, 0), BCD(1, 5),   1, 0, "туалет свободен");

        }

        private void button9_Click(object sender, EventArgs e)
        {
         
            SEND(0x20A, 8, 76, 77,              BCD(1,9),             BCD(3,4 ), BCD(1, 2),         9,             speed,          7,"t, Nвагона, Nпоезда" );
                          //t нар  //t купе     //N вагона             N поезда МЛ,  СТАР        //яркость 1-10   //скорость    //on/off

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button10_Click(object sender, EventArgs e)
        {



            if (HB(textBox1.Text) > HB(textBox2.Text))
            {
                MessageBox.Show("первое число должно быть меньше второго", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation); return;
            }

                data.diap1 = HB(textBox1.Text);
                data.diap2 = HB(textBox2.Text);

                data.di1 = HB(textBox6.Text);
                data.di2 = HB(textBox7.Text);
                data.di3 = HB(textBox9.Text);
                data.di4 = HB(textBox8.Text);


            

                AddTextCol("--новый диапазон--    [" + textBox1.Text + " - " + textBox2.Text+ "]        " +
                    textBox6.Text + ":" + textBox7.Text + ":" + textBox9.Text + ":" + textBox8.Text, System.Drawing.Color.White);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                data.con = 11; return;
            }


            viv();
        }

        void viv()
        {
            byte s1 = (byte)SU(textBox3.Text);
            byte s2 = (byte)SU(textBox4.Text);

            SEND(0x28A, 6, 0, 0, 0, 0, s1, s2, 0, 0, "выводим строку " + String.Format("{0} - {1}", s1, s2));

        }


        private void button12_Click(object sender, EventArgs e)
        {

            if (
                 button3.Enabled
                                    ) { MessageBox.Show(String.Format("  СНАЧАЛО ПОДКЛЮЧИТЬСЯ! "), "Ошибка"); return; }

            if ((byte)SU(textBox3.Text) >= (byte)SU(textBox4.Text)) { 
                MessageBox.Show(String.Format("  ОШИБКА ДИАПАЗОНА СТРОК! "), "Ошибка"); return; }

            if ((byte)SU(textBox3.Text) <1 || (byte)SU(textBox4.Text)<1)
            {
                MessageBox.Show(String.Format("  ОШИБКА ДИАПАЗОНА СТРОК! "), "Ошибка"); return;
            }


            AddText("Идет запись строки ....");

            data.start = (byte)SU(textBox3.Text);
           
            data.end = (byte)SU(textBox4.Text);
           
            data.con = 8;

            
            return;

            //string ww = "<  1  <  2  <  3  <  4  <  5  <  6  <  7  <  8  <  9  < 10  < 11  < 12  < 13  < 14  < 15  < 16  < 17  < 18  " +
            //            "< 19  < 20  < 21  < 22  < 23  < 24  < 25  < 26  < 27  < 28  < 29  < 30  < 31  < 32  < 33  < 34  < 35  < 36  " +
            //            "< 37  < 38  < 39  < 40  < 41  < 42  < 43  < 44  < 45  < 46  < 47  < 48  < 49  < 50  < 51  < 52  < 53  < 54  " +
            //            "< 55  < 56  < 57  < 58  < 59  < 60  < 61  < 62  < 63  < 64  < 65  < 66  < 67  < 68  < 69  < 70  < 71  < 72  " +
            //            "< 73  < 74  < 75  < 76  < 77  < 78  < 79  < 80  < 81  < 82  < 83  < 84  < 85  < 86  < 87  < 88  < 89  < 90  " +
            //            "< 91  < 92  < 93  < 94  < 95  < 96  < 97  < 98  < 99  < 100 < 101 < 102 < 103 < 104 < 105 < 106 < 107 < 108 " +
            //            "< 109 < 110 < 111 < 112 < 113 < 114 < 115 < 116 < 117 < 118 < 119 < 120 < 121 < 122 < 123 < 124 < 125 < 126 " +
            //            "< 127 < 128 < 129 < 130 < 131 < 132 < 133 < 134 < 135 < 136 < 137 < 138 < 139 < 140 < 141 < 142 < 143 < 144 " +
            //            "< 145 < 146 < 147 < 148 < 149 < 150 < 151 < 152 < 153 < 154 < 155 < 156 < 157 < 158 < 159 < 160 < 161 < 162 " +
            //            "< 163 < 164 < 165 < 166 < 167 < 168 < 169 < 170 < 171 < 172 < 173 < 174 < 175 < 176 < 177 < 178 < 179 < 180 " +
            //            "< 181 < 182 < 183 < 184 < 185 < 186 < 187 < 188 < 189 < 190 < 191 < 192 < 193 < 194 < 195 < 196 < 197 < 198 " +
            //            "< 199 < 200 < 201 < 202 < 203 < 204 < 205 < 206 < 207 < 208 < 209 < 210 < 211 < 212 < 213 < 214 < 215 < 216 " +
            //            "< 217 < 218 < 219 < 220 < 221 < 222 < 223 < 224 < 225 < 226 < 227 < 228 < 229 < 230 < 231 < 232 < 233 < 234 " +
            //            "< 235 < 236 < 237 < 238 < 239 < 240 ";


        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) data.fo1 = true; else data.fo1 = false;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString( data.t.s1, 10);
            textBox4.Text = Convert.ToString(data.t.e1, 10);
            data.stroka = data.str1;

            button13.BackColor = System.Drawing.Color.RoyalBlue;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.LightGray;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(data.t.s5, 10);
            textBox4.Text = Convert.ToString(data.t.e5, 10);
            data.stroka = data.str5;
            button13.BackColor = System.Drawing.Color.LightGray;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.RoyalBlue;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(data.t.s2, 10);
            textBox4.Text = Convert.ToString(data.t.e2, 10);
            data.stroka = data.str2;
            button13.BackColor = System.Drawing.Color.LightGray;
            button14.BackColor = System.Drawing.Color.RoyalBlue;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.LightGray;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            speed = (byte)SU(textBox5.Text);
            if (speed > 19) speed = 19;
            if (speed <0) speed = 0;

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
         
        private void button17_Click(object sender, EventArgs e)
        {
            if (data.start_buff)
            { data.start_buff = false; AddText("--- СТОП буфера --- \n\n"); return; }
            else
            { AddText("--- ПУСК буфера --- \n"); data.start_buff = true;  return; }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            
             if (data.start_ReadBuff)
             { data.start_ReadBuff = false; AddTextCONT("СТОП вывода \n\n"); return; }
            else
             { 


             Form_запросТекста secondForm = new Form_запросТекста();
             //скрываем форму из панели задач
             secondForm.ShowInTaskbar = false;
             //устанавливаем форму по центру экрана
             secondForm.StartPosition = FormStartPosition.CenterScreen;
             //указываем владельца для формы
             secondForm.ShowDialog(this);


             AddTextCONT("ПУСК вывода \n");
            

             ctviv = 0; data.start_ReadBuff = true;
                 
                 return; }

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
         
            if (checkBox3.Checked) data.fo2 = true; else data.fo2 = false;
     
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) data.fo3 = true; else data.fo3 = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ctviv = 0;
            data.con = 5;

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }


        public void SEND(uint adr, byte sz, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, string name)
        {

            if (
                button3.Enabled
                                   ) { MessageBox.Show(String.Format("  СНАЧАЛО ПОДКЛЮЧИТЬСЯ! "), "Ошибка"); return; }
            
            AddText(name);


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


            data.con = 7;


        }

        private void button21_Click(object sender, EventArgs e)
        {
            textBox6.Text = "2e5";
            textBox7.Text = "3e5";
            textBox9.Text = "4e5";
            textBox8.Text = "6d3";

        }

        private void button22_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox6.Text = "0";
            textBox7.Text = "0";
            textBox8.Text = "0";
            textBox9.Text = "0";

        }

        private void button20_Click(object sender, EventArgs e)
        {
            textBox6.Text = "28a";
            textBox7.Text = "38a";
            textBox9.Text = "1d5";
            textBox8.Text = "1d6";
        }

        private void button23_Click(object sender, EventArgs e)
        {


          // data.очередьКОПИЯ = data.очередь;

            // Create a copy of the queue, using the ToArray method and the
            // constructor that accepts an IEnumerable<T>.
            //Queue<string> queueCopy = new Queue<string>(numbers.ToArray());

            //Console.WriteLine("\nContents of the first copy:");
            //foreach (string number in queueCopy)
            //{
            //    Console.WriteLine(number);
            //}


            data.COPY1();
           


            ctviv = 0;

            if (data.очередьКОПИЯ==null) AddText("копия не создана"); else
                AddTextCol("===== БУФЕР ЗАБРАЛИ(" + String.Format("размер={0}", data.очередьКОПИЯ.Count) + "). ГОТОВ  К ВОСПРОИЗВЕДЕНИЮ =========", System.Drawing.Color.SlateBlue);

          // richTextBox1.Clear();
           // data.очередь.Clear();
            


        }


        static bool idetPLAY;
        private void button24_Click(object sender, EventArgs e)
        {
            if (!data.PLAY)
            {

                if (!data.connOK) { AddTextCol("Нет СОЕДИНЕНИЯ для ПУСКА воспроизведения", System.Drawing.Color.Red); return; }


                if (data.очередьКОПИЯ == null) { AddTextCol("Нет данных для воспроизведения", System.Drawing.Color.Red); return; }

                DialogResult vibor2 = MessageBox.Show(String.Format("размер={0}\n", data.очередьКОПИЯ.Count)+
                    "ВОПРОИЗВЕДЕНИЕ БУФЕРА захваченного кнопкой GET\nНАЖМИ ДА для ЗАПУСКА!", "ВНИМАНИЕ!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //if (vibor2 == DialogResult.Yes)
                //{
                //    MessageBox.Show("ERROR! You press Yes!");
                //}
                if (vibor2 == DialogResult.No)
                {
                    return;
                }


                if (data.очередьКОПИЯ.Count == 0) { AddTextCol("Нет данных для воспроизведения",System.Drawing.Color.Red); return; }
                data.очередьКОПИЯрабочая = data.очередьКОПИЯ;
                AddTextCol("-- " + String.Format("размер рабоч={0}\n", data.очередьКОПИЯ.Count),System.Drawing.Color.SlateBlue);



                ThreadCAN.lasttime = new DateTime(); //ini стартового времени
                //пуск вывода
                ctviv = 0; data.start_ReadBuff = true;
            }




                if (idetPLAY)
                {
                    data.start_ReadBuff = false; 
                    idetPLAY = false; button24.Text = "PLAY";
                    button24.BackColor = System.Drawing.Color.LightGray;
                    AddTextCol("==== ВОСПРОИЗВЕДЕНИЕ ОСТАНОВЛЕНО ====", System.Drawing.Color.Red);
                    data.PLAY = false;
                }
                else
                {
                    idetPLAY = true; button24.Text = "STOP";
                    button24.BackColor = System.Drawing.Color.Crimson;
                    AddTextCol("\nВОСПРОИЗВЕДЕНИЕ НАЧАЛОСЬ...", System.Drawing.Color.Red);
                    data.PLAY = true;
                }

           




        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (
               button3.Enabled
                                  ) { MessageBox.Show(String.Format("  СНАЧАЛО ПОДКЛЮЧИТЬСЯ! "), "Ошибка"); return; }


            checkBox5.Checked = false;

            AddTextCol("= НАЧАТА ОЧИСТКА ПАМЯТИ ТАБЛО=", System.Drawing.Color.HotPink);
            data.con = 10;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(data.t.s3, 10);
            textBox4.Text = Convert.ToString(data.t.e3, 10);
            data.stroka = data.str3;
            button13.BackColor = System.Drawing.Color.LightGray;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.RoyalBlue;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.LightGray;
            
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox3.Text = Convert.ToString(data.t.s4, 10);
            textBox4.Text = Convert.ToString(data.t.e4, 10);
            data.stroka = data.str4;
            button13.BackColor = System.Drawing.Color.LightGray;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.RoyalBlue;
            button18.BackColor = System.Drawing.Color.LightGray;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        //private void button28_Click(object sender, EventArgs e)
        //{
        //    //button28.BackColor = System.Drawing.Color.BlueViolet;
        //    //button27.BackColor = System.Drawing.Color.LightGray;
        //    //button26.BackColor = System.Drawing.Color.LightGray;

        //    data.t.s1 = data.t1.s1;
        //    data.t.s2 = data.t1.s2;
        //    data.t.s3 = data.t1.s3;
        //    data.t.s4 = data.t1.s4;
        //    data.t.s5 = data.t1.s5;

        //    data.t.e1 = data.t1.e1;
        //    data.t.e2 = data.t1.e2;
        //    data.t.e3 = data.t1.e3;
        //    data.t.e4 = data.t1.e4;
        //    data.t.e5 = data.t1.e5;
        //    AddText(String.Format("первая строка ДИАПАЗОН = {0}-{1}", data.t.s1, data.t.e1));
        //    textBox3.Text = Convert.ToString(data.t.s1, 10);
        //    textBox4.Text = Convert.ToString(data.t.e1, 10);
        //    data.stroka = data.str1;
        //}

        //private void button27_Click(object sender, EventArgs e)
        //{
        //    //button28.BackColor = System.Drawing.Color.LightGray;
        //    //button27.BackColor = System.Drawing.Color.BlueViolet;
        //    //button26.BackColor = System.Drawing.Color.LightGray;

        //    data.t.s1 = data.t2.s1;
        //    data.t.s2 = data.t2.s2;
        //    data.t.s3 = data.t2.s3;
        //    data.t.s4 = data.t2.s4;
        //    data.t.s5 = data.t2.s5;

        //    data.t.e1 = data.t2.e1;
        //    data.t.e2 = data.t2.e2;
        //    data.t.e3 = data.t2.e3;
        //    data.t.e4 = data.t2.e4;
        //    data.t.e5 = data.t2.e5;
        //    AddText(String.Format("первая строка ДИАПАЗОН = {0}-{1}", data.t.s1, data.t.e1));
        //    textBox3.Text = Convert.ToString(data.t.s1, 10);
        //    textBox4.Text = Convert.ToString(data.t.e1, 10);
        //    data.stroka = data.str1;
        //}

        //private void button26_Click(object sender, EventArgs e)
        //{
        //    //button28.BackColor = System.Drawing.Color.LightGray;
        //    //button27.BackColor = System.Drawing.Color.LightGray;
        //    //button26.BackColor = System.Drawing.Color.BlueViolet;

        //    data.t.s1 = data.t3.s1;
        //    data.t.s2 = data.t3.s2;
        //    data.t.s3 = data.t3.s3;
        //    data.t.s4 = data.t3.s4;
        //    data.t.s5 = data.t3.s5;

        //    data.t.e1 = data.t3.e1;
        //    data.t.e2 = data.t3.e2;
        //    data.t.e3 = data.t3.e3;
        //    data.t.e4 = data.t3.e4;
        //    data.t.e5 = data.t3.e5;
        //    AddText(String.Format("первая строка ДИАПАЗОН = {0}-{1}", data.t.s1, data.t.e1));
        //    textBox3.Text = Convert.ToString(data.t.s1, 10);
        //    textBox4.Text = Convert.ToString(data.t.e1, 10);
        //    data.stroka = data.str1;
        //}

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked) data.pauseWRITE = true;
            else data.pauseWRITE = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("created by dav1977\n"+
            "command test,set7", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            
       
        }

        private void button29_Click(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked) { data.str3 = data.str3a; data.str4 = data.str4a; }
            else { data.str3 = data.str3b; data.str4 = data.str4b; }
           

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void MouseUP(object sender, MouseEventArgs e)
        {
          //  richTextBox1.ZoomFactor++;
        }

        private void hover(object sender, EventArgs e)
        {
          //  richTextBox1.ZoomFactor++;
        }

        

        private void сООБЩЕНИЯToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form2редактСОООБЩ secondForm = new Form2редактСОООБЩ();
            //скрываем форму из панели задач
            secondForm.ShowInTaskbar = false;
            //устанавливаем форму по центру экрана
            secondForm.StartPosition = FormStartPosition.CenterScreen;
            //указываем владельца для формы
            secondForm.ShowDialog(this);

            inistr();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

            ras();
            AddTextCol(String.Format("первая строка ДИАПАЗОН = {0}-{1}", data.t.s1, data.t.e1), System.Drawing.Color.White);
           
            button13.BackColor = System.Drawing.Color.RoyalBlue;
            button14.BackColor = System.Drawing.Color.LightGray;
            button16.BackColor = System.Drawing.Color.LightGray;
            button15.BackColor = System.Drawing.Color.LightGray;
            button18.BackColor = System.Drawing.Color.LightGray;
        }


        void ras()
        {

            byte sme = 120;
            try
            {
                sme = (byte)Convert.ToByte(textBox10.Text, 10);
                sme++;
            }
            catch { return; }

            data.t.s1 = (byte)(data.tt.s1 + sme);
            data.t.s2 = (byte)(data.tt.s2 + sme);
            data.t.s3 = (byte)(data.tt.s3 + sme);
            data.t.s4 = (byte)(data.tt.s4 + sme);
            data.t.s5 = (byte)(data.tt.s5 + sme);

            data.t.e1 = (byte)(data.tt.e1 + sme);
            data.t.e2 = (byte)(data.tt.e2 + sme);
            data.t.e3 = (byte)(data.tt.e3 + sme);
            data.t.e4 = (byte)(data.tt.e4 + sme);
            data.t.e5 = (byte)(data.tt.e5 + sme);

            textBox3.Text = Convert.ToString(data.t.s1, 10);
            textBox4.Text = Convert.ToString(data.t.e1, 10);
            data.stroka = data.str1;

        }
        private void button7_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            textBox2.Text = "FFFF";
        }

        private void сохранитьБуферToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.lastpath == null)
            {
                saveFileDialog1.InitialDirectory = "D:";
                saveFileDialog1.FileName = saveFileDialog1.InitialDirectory + "\\CANBUFFER.canbf";
            }
            else saveFileDialog1.InitialDirectory = data.lastpath;

            saveFileDialog1.Filter = "Файлы canbf (*.canbf)|*.canbf";
   
 
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                data.lastpath = saveFileDialog1.FileName;
                    if (Serialization.Write(saveFileDialog1.FileName))
                    AddText("Запись выполнена");
            }
        }

        private void читатьБуферToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.lastpath == null)
            {
                openFileDialog1.InitialDirectory = "D:";
                openFileDialog1.FileName = openFileDialog1.InitialDirectory + @"\CANBUFFER.canbf";
            }
            else openFileDialog1.InitialDirectory = data.lastpath;
            
            openFileDialog1.Filter = "Файлы canbf (*.canbf)|*.canbf";


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                data.lastpath = openFileDialog1.FileName;
                if (Serialization.Read(openFileDialog1.FileName))
                AddText("ЧТЕНИЕ выполнено");
            }
     
        }

        private void button26_Click(object sender, EventArgs e)
        {
            richTextBox1.Height--;
          
        }

        private void button27_Click(object sender, EventArgs e)
        {
            richTextBox1.Height++;
           
        }

       

       

      
      

       //end
    }

}
