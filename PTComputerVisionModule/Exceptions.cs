using System;

namespace PTComputerVisionModule
{
    public class Exceptions
    {
        // Ошибка при чтении файла из библиотеки Emgu.CV
        public class ImageReadException : Exception { internal ImageReadException(string exInfo) { } }

        // Ошибка при записи файла из библиотеки Emgu.CV
        public class ImageWriteException : Exception { internal ImageWriteException(string exInfo) { } }

        // Указан некорректный цвет обводки структур (принимаются только варианты green, blue, red)
        public class ColorChooseException : Exception { internal ColorChooseException(string exInfo) { } }
    }
}