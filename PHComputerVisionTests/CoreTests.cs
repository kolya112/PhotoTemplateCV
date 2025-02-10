using System;
using System.IO;
using Xunit;
using PTComputerVisionModule;

namespace PHComputerVisionTests
{
    public class CoreTests
    {
        private readonly string _validImagePath = "test_input.jpg";
        private readonly string _invalidImagePath = "nonexistent.jpg";
        private readonly string _validOutputDir = "./test_output";
        private readonly string _invalidOutputDir = "./invalid_dir";

        public CoreTests()
        {
            Directory.CreateDirectory(_validOutputDir);
            GenerateTestImage(_validImagePath);
        }

        private void GenerateTestImage(string path)
        {
            using (var image = new Emgu.CV.Mat(100, 100, Emgu.CV.CvEnum.DepthType.Cv8U, 1))
            {
                Emgu.CV.CvInvoke.Imwrite(path, image);
            }
        }

        [Fact]
        public void Constructor_ThrowsFileNotFoundException_ForInvalidImagePath()
        {
            Assert.Throws<FileNotFoundException>(() => new Core(_invalidImagePath, _validOutputDir, "green"));
        }

        [Fact]
        public void Constructor_ThrowsDirectoryNotFoundException_ForInvalidOutputDir()
        {
            Assert.Throws<DirectoryNotFoundException>(() => new Core(_validImagePath, _invalidOutputDir, "green"));
        }

        [Theory]
        [InlineData("green")]
        [InlineData("blue")]
        [InlineData("red")]
        public void Constructor_DoesNotThrow_ForValidArguments(string color)
        {
            var core = new Core(_validImagePath, _validOutputDir, color);
            Assert.NotNull(core);
        }

        [Fact]
        public void Constructor_ThrowsColorChooseException_ForInvalidColor()
        {
            Assert.Throws<Exceptions.ColorChooseException>(() => new Core(_validImagePath, _validOutputDir, "yellow"));
        }

        [Fact]
        public void Execute_ReturnsValidOutputPath()
        {
            var core = new Core(_validImagePath, _validOutputDir, "green");
            var result = core.Execute();

            Assert.True(File.Exists(result.imageOutputPath));
            Assert.True(result.structuresCount >= 0);
        }
    }
}