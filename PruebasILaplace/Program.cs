// See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
Console.OutputEncoding = System.Text.Encoding.UTF8;
string L = "e^(-st)";
string iL = "-(1/s)e^-st";
Console.WriteLine("ingresa la funcion");
string funcion=Console.ReadLine();
string[] partes = funcion.Split(' ');

string sMultiplicacion = "";
if (partes.Length == 1)
{
    string parte = partes[0];
    sMultiplicacion = $"{L}({parte})";
    Console.WriteLine($"∫{sMultiplicacion}");
    
    int constante = 0;
    //evaluamos si la parte es una constante
    if (int.TryParse(parte, out constante))
    {
        if (constante == 1)
        {
            Console.WriteLine($"∫[0,∞]{L}");
            //integramos
            Console.WriteLine("Integramos");
            Console.WriteLine($"∫[0,∞]{L} = {iL}");
           

        }
        else
        {
            Console.WriteLine($"∫[0,∞]{parte}{L}");
            Console.WriteLine("Sacamos la constante de la integral");
            Console.WriteLine($"{parte}∫[0,∞]{L}");
            Console.WriteLine("Integramos");
            iL = iL.Replace("1", constante.ToString());
            Console.WriteLine($"{parte}∫[0,∞]{L} = {iL}");
            
        }
        //Transcribimos a integral impropia
        Console.WriteLine("Transcribimos a integral impropia");
        Console.WriteLine($"lim b->∞ [{iL}][0,b]");
        //Evaluamos limites
        Console.WriteLine("Evaluamos limites");

    }
}
else {

    for (int i = 0; i < partes.Length; i++)
    {
        string parte = partes[i];
        if (parte != "+")
        {
            sMultiplicacion += $"{L}({parte})";
        }
        else
        {
            sMultiplicacion += $" {parte} ";
        }
    }
    Console.WriteLine($"∫[0,∞]{sMultiplicacion} dt");
}

