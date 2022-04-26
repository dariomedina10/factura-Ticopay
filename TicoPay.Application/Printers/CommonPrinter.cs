using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicoPay.Printers
{
    static public class CommonPrinter
    {
         public const string ESC = "0x1B";
         public const string INITIALIZE_PRINTER = "\x1B\x40";
         public const string NEW_LINE = "\r\n";
         public const string TXT_STYLE_NORMAL = "\x1B\x21\x01";
         public const string TXT_STYLE_BOLD = "\x1B\x21\x08";
         public const string TXT_ALIGN_CENTER = "\x1B\x61\x31";
         public const string CMD_CUT = "\x1D\x56\x41";


        static public string normalizeStringChars(string param)
        {
            param = param.Replace("Á", "A"); //Á
            param = param.Replace("á", "a"); //á 
            param = param.Replace("É", "E"); //É 
            param = param.Replace("é", "e"); //é
            param = param.Replace("Í", "I"); //Í
            param = param.Replace("í", "i"); //í
            param = param.Replace("Ñ", "N"); //Ñ
            param = param.Replace("ñ", "n"); //ñ
            param = param.Replace("Ó", "O"); //Ó
            param = param.Replace("ó", "o"); //ó
            param = param.Replace("Ú", "U"); //Ú
            param = param.Replace("ú", "u"); //ú
            param = param.Replace("Ü", "U"); //Ü
            param = param.Replace("ü", "u"); //ü
            param = param.Replace("`", ""); //`
            param = param.Replace("'", ""); //'
            param = param.Replace("¨", ""); //¨
            param = param.Replace("ª", ""); //ª
            param = param.Replace("º", ""); //ª            
            param = param.Replace("³", "3");
            param = param.Replace("²", "2");
            param = param.Replace("°", "0");
            param = param.Replace("·", "");
            param = param.Replace("/", " ");
            param = param.Replace(";", " ");

            return param;

        }

        static public string StrPad(string str, int length, char chr)
        {
            if (str.Length < length)
                str = str.PadRight(length, chr);
            else
                str = str.Substring(0, length);
            return str;

        }

        static public string StrTruc(string str, int length)
        {

            if (str.Length > length)
            {
                str = str.Substring(0, length) + NEW_LINE + StrTruc(str.Substring(length, str.Length - length), length);

            }


            return str;
        }
        

        static public string StrSplit(string str, int length )
        {
            var strNew = string.Empty;
            if (str.Length > length)
            {
                var array = str.Split(' ');
                string newline = string.Empty;
                for (int i = 0; i < array.Length; i++)
                {
                    if (newline.Length + array[i].Length + 1 <= length)
                        newline += array[i] + " ";
                    else
                    {
                        strNew += newline.PadRight(length, ' ') + NEW_LINE;
                        newline = array[i] + " ";
                    }
                }
                strNew += newline.PadRight(length, ' ') + NEW_LINE;
            }
            else
                strNew = str.PadRight(length, ' ') + NEW_LINE;

            return strNew;

        }

        static public string BR(int length)
        {
            var str = string.Empty;
            string newline = string.Empty;
            for (int i = 0; i < length; i++)
            {
                str += '-';
            }

            return str + NEW_LINE;

        }

        static public string StrSplit(string str, int length, bool isRight)
        {
            var newStr = string.Empty;
            if (str.Length > length)
            {
                var array = str.Split(' ');
                string newline = string.Empty;
                for (int i = 0; i < array.Length; i++)
                {
                    if (newline.Length + array[i].Length + 1 <= length)
                        newline += array[i] + " ";
                    else
                    {
                        if (isRight)
                            newStr += newline.PadRight(length, ' ') + NEW_LINE;
                        else
                            newStr += newline.PadLeft(length, ' ') + NEW_LINE;
                        newline = array[i] + " ";
                    }
                }
                if (isRight)
                    newStr += newline.PadRight(length, ' ');
                else
                    newStr += newline.PadLeft(length, ' ');

                
            }
            else
                if (isRight)
                newStr = str.PadRight(length, ' ');
            else
                newStr = str.PadLeft(length, ' ');

            return newStr;

        }


    }
}
