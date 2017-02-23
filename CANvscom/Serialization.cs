using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Program
{
    [Serializable]
    public class SerializationDATA
    {
        public Queue<ДАННЫЕочередь> zz { get; set; }
     
        public SerializationDATA()
        {

            zz = new Queue<ДАННЫЕочередь>(data.очередь);

        }

    }



   
    class Serialization
    {
        //static void SaveInXmlFormat(object objGraph, string fileName)
        //{
        //    XmlSerializer xmlFormat = new XmlSerializer(typeof(JamesBondClass),
        //        new Type[] { typeof(Radio), typeof(Car) });
        //    using (Stream fStream = new FileStream(fileName,
        //        FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        xmlFormat.Serialize(fStream, objGraph);
        //    }
        //    Console.WriteLine("--> Сохранение объекта в XML-формат");
        //}





        public static bool Write(string path)
        {
            SerializationDATA overview = new SerializationDATA();

            
          //  var pathq = AppDomain.CurrentDomain.BaseDirectory ;

            // Open a file and serialize the object into it in binary format.
            // EmployeeInfo.osl is the file that we are creating. 
            // Note:- you can give any extension you want for your file
            // If you use custom extensions, then the user will now 
            //   that the file is associated with your program.
            //pathq + "//data.bin"

            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, overview);
                stream.Close();
                return true;
            }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            // return;

            //System.Xml.Serialization.XmlSerializer writer =
            //    new System.Xml.Serialization.XmlSerializer(typeof(SerializationDATA));
            //var path = AppDomain.CurrentDomain.BaseDirectory + "SerializationOverview.xml";
            //System.IO.FileStream file = System.IO.File.Create(path);
            //writer.Serialize(file, overview);
            //file.Close();
        }





        static SerializationDATA loadCLASS;
        public static bool Read(string path)
        {
            try
            {
          //  var pathq = AppDomain.CurrentDomain.BaseDirectory;
            data.очередь.Clear();

            loadCLASS = new SerializationDATA();

            //Open the file written above and read values from it.
            //pathq + "//data.bin"
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter  bformatter = new BinaryFormatter();


            loadCLASS = (SerializationDATA)bformatter.Deserialize(stream);
            stream.Close();

            data.очередь = new Queue<ДАННЫЕочередь>(loadCLASS.zz);

            return true;
        
             }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }


}
