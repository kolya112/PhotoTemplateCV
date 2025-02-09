using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PTComputerVisionModule
{
    public class Core
    {
        internal string _pathToTemplate = null; // Путь до входного файла (фотошаблона)
        internal string _outputPath = null; // Выходной путь
        internal Mat _sourceTemplate = new Mat(); // Здесь содержится исходное изображение
        internal MCvScalar _color; // Здесь содержится цвет обводки структур

        // Конструктор класса
        public Core(string pathToTemplate, string outputPath, string color)
        {
            if (!File.Exists(pathToTemplate))
                throw new FileNotFoundException("Выбранного как фотошаблон файла не существует");

            _pathToTemplate = pathToTemplate;

            var fileInfo = new FileInfo(_pathToTemplate);

            if (Directory.Exists(outputPath))
                _outputPath = outputPath + "\\" + Path.GetFileNameWithoutExtension(pathToTemplate) + "_output" + new Random().Next(0, 99999) + ".jpg";
            else
                throw new DirectoryNotFoundException("Указанная выходная директория не найдена");

            try
            {
                _sourceTemplate = CvInvoke.Imread(_pathToTemplate, ImreadModes.Grayscale);
            }
            catch (Exception ex)
            {
                throw new Exceptions.ImageReadException($"Произошло исключение: {ex.InnerException} {Environment.NewLine} Сообщение об ошибке: {ex.Message}");
            }

            if (color == "green")
                _color = new MCvScalar(0, 255, 0);
            else if (color == "blue")
                _color = new MCvScalar(255, 0, 0);
            else if (color == "red")
                _color = new MCvScalar(0, 0, 255);
            else
                throw new Exceptions.ColorChooseException("Указан некорректный цвет обводки структур (принимаются только варианты green, blue, red)");
        }

        // Функция-запускатор работы Emgu.CV над фотошаблоном
        public (int structuresCount, string imageOutputPath) Execute()
        {
            try
            {
                var blurredImage = new Mat();
                CvInvoke.GaussianBlur(_sourceTemplate, blurredImage, new Size(5, 5), 1.5); // Создание блюра (размытия) по методу Гаусса
                CvInvoke.BitwiseNot(blurredImage, blurredImage); // Инверсия для лучшего выделения объектов

                var edges = new Mat();
                CvInvoke.Threshold(blurredImage, blurredImage, 100, 255, ThresholdType.Binary);
                CvInvoke.Dilate(blurredImage, blurredImage, null, new Point(-1, -1), 2, BorderType.Default, new MCvScalar()); // Расширение границ, чтобы соединить разорванные части
                CvInvoke.Canny(blurredImage, edges, 100, 200); // Используем для создания чётких границ

                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                var hierarchy = new Mat();
                CvInvoke.FindContours(edges, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

                var outputImage = new Mat();
                CvInvoke.CvtColor(_sourceTemplate, outputImage, ColorConversion.Gray2Bgr);
                var edgesBgr = new Mat();
                CvInvoke.CvtColor(edges, edgesBgr, ColorConversion.Gray2Bgr);
                CvInvoke.AddWeighted(outputImage, 0.8, edgesBgr, 0.2, 0, outputImage); // Показываем границы

                int structureCount = 0;
                List<Rectangle> boundingBoxes = new List<Rectangle>();

                for (int i = 0; i < contours.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    bool isNested = boundingBoxes.Any(b => b.Contains(rect));

                    if (area > 100 && area < 50000 && !isNested)
                    {
                        boundingBoxes.Add(rect); // Исключаем слишком маленькие и большие контуры
                        CvInvoke.DrawContours(outputImage, contours, i, _color, 2);
                        structureCount++;
                    }
                }

                CvInvoke.Imwrite(_outputPath, outputImage);

                return (structureCount, _outputPath);
            }
            catch (IOException)
            {
                throw new Exceptions.ImageWriteException("Ошибка записи выходного изображения");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exceptions.ImageWriteException("Ошибка записи выходного изображения: программе отказано в доступе");
            }
            catch (Exception ex)
            {
                throw new Exception($"Произошла ошибка: {Environment.NewLine} Message: {ex.Message} {Environment.NewLine} Exception: {ex.InnerException}");
            }
        }

    }
}