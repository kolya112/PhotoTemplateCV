using System;

namespace PTComputerVisionModule
{
    internal class Exceptions
    {
        // Ошибка при чтении файла из библиотеки Emgu.CV
        internal class ImageReadException : Exception { internal ImageReadException(string exInfo) { } }

        // Ошибка при записи файла из библиотеки Emgu.CV
        internal class ImageWriteException : Exception { internal ImageWriteException(string exInfo) { } }

        // Указан некорректный цвет обводки структур (принимаются только варианты green, blue, red)
        internal class ColorChooseException : Exception { internal ColorChooseException(string exInfo) { } }
    }
}