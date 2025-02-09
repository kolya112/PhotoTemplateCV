using System;
using System.IO;

namespace PhotoTemplateCV.Common
{
    internal class Functions
    {
        // Функция для записи данных в CSV-файл
        internal static void WriteDataToCSV(string filePath, string imageFileName, int structuresCount)
        {
            File.AppendAllText(filePath, $"{imageFileName};{structuresCount}{Environment.NewLine}");
        }
    }
}
