using System;
using System.IO;
using System.IO.Compression;
using static System.Console;
using static System.IO.Path;
using System.Xml;
using static System.Environment;


namespace Compresion
{
    class Program
    {
        //Arreglo para reyenar datos en el archivo
        static string[] nombres = new string[] { "Jose", "Antonio", "Andres" };
        static void Main(string[] args)
        {
            EscribirXml();
            string caminoDeArchivo = Combine(CurrentDirectory, "compresion.br");//dirección del archivo que vamos a crear
            FileStream archivoBrotli = File.Create(caminoDeArchivo);//Creamos el archivo
            //Comenzamos compresión

            using (BrotliStream compresion = new BrotliStream(archivoBrotli, CompressionMode.Compress))
            {
                using (XmlWriter xmlBrotli = XmlWriter.Create(compresion))
                {
                    //Se inicializa el archivo
                    xmlBrotli.WriteStartDocument();

                    xmlBrotli.WriteStartElement("Estudiantes");//Se define el elemento raíz
                    foreach (string persona in nombres)
                    {
                        xmlBrotli.WriteElementString("Estudiante", persona);
                    }
                    foreach (string persona in nombres)//Abrimos nueva etiqueta para cada persona
                    {
                        xmlBrotli.WriteElementString("OtrasPersonas", persona);
                    }

                    xmlBrotli.WriteStartElement("Info");//Segunda etiqueta raíz
                    foreach (string persona in nombres)//Rellenamos el archivo con nuevas etiquetas para cada persona
                    {
                        xmlBrotli.WriteElementString("Nombre", persona);
                        xmlBrotli.WriteElementString("Telefono", "");
                        xmlBrotli.WriteElementString("Dir", "");
                    }

                    //Se cierra el archivo
                    xmlBrotli.WriteEndDocument();
                }

            }
            //Mostramos la información del nuevo doc
            WriteLine($"{caminoDeArchivo} contiene {new FileInfo(caminoDeArchivo).Length} bytes");
            WriteLine("El comprimido contiene: ");
            WriteLine(File.ReadAllText(caminoDeArchivo));

            //Leemos la info del archivo comprimido
            WriteLine("Leyendo informacion del nuevo archivo...");
            archivoBrotli = File.Open(caminoDeArchivo, FileMode.Open);

            //Implementando el descompresor
            using (BrotliStream descompresor = new BrotliStream(archivoBrotli, CompressionMode.Decompress))
            {
                using (XmlReader reader = XmlReader.Create(descompresor))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Estudiante"))
                        {
                            reader.Read();
                            WriteLine($"{reader.Value}");
                        }
                    }
                }
            }
        }

        static void EscribirXml()
        {

            FileStream archivoXml = null;
            XmlWriter xml = null;


            try
            {
                string caminoArchivo = Combine(CurrentDirectory, "compresion.xml");//Dirección del archivo a crear
                archivoXml = File.Create(caminoArchivo);//Creamos el archivo
                xml = XmlWriter.Create(archivoXml, new XmlWriterSettings { Indent = true });
                xml.WriteStartDocument();//Inicializamos el archivo

                xml.WriteStartElement("Estudiantes");//Se define el elemento raíz
                foreach (string persona in nombres)
                {
                    xml.WriteElementString("Estudiante", persona);
                }
                foreach (string persona in nombres)//Abrimos nueva etiqueta para cada persona
                {
                    xml.WriteElementString("OtrasPersonas", persona);
                }

                xml.WriteStartElement("Info");//Segunda etiqueta raíz
                foreach (string persona in nombres)//Rellenamos el archivo con nuevas etiquetas para cada persona
                {
                    xml.WriteElementString("Nombre", persona);
                    xml.WriteElementString("Telefono", "");
                    xml.WriteElementString("Dir", "");
                }

                //Se cierra el archivo
                xml.WriteEndDocument();
                xml.Close();
                archivoXml.Close();

                WriteLine($"{archivoXml} contiene {new FileInfo(caminoArchivo).Length} bytes");
            }

            catch (Exception ex)
            {
                //En caso de no exisitir la ruta
                WriteLine($"{ex.GetType()} dice {ex.Message}");
            }

            finally
            {
                if (xml != null)
                {
                    //Terminamos por desechar el archivo
                    xml.Dispose();
                    WriteLine($"El archivo ha sido eliminado");
                }

                if (archivoXml != null)
                {
                    //Terminamos por desechar la ruta
                    archivoXml.Dispose();
                    WriteLine($"El camino del archivo ha sido eliminado");
                }
            }
        }
    }
}
