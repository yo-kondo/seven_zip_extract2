using System.IO;
using NUnit.Framework;
using SevenZipExtract;

namespace SevenZipExtractTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// 拡張子 : .7z
        /// パスワード : なし
        /// </summary>
        [Test]
        public void Test001()
        {
            const string workPath = "TestData/Test001/";
            var args = new[] {workPath + "test.7z"};
            Program.Execute(args);

            Assert.That(File.Exists(workPath + "test.txt"), Is.True);

            // 元に戻す
            File.Delete(workPath + "test.txt");
        }

        /// <summary>
        /// 拡張子 : .zi_
        /// パスワード : password1
        /// </summary>
        [Test]
        public void Test002()
        {
            const string workPath = "TestData/Test002/";
            var args = new[] {workPath + "test.zi_"};
            Program.Main(args);

            Assert.That(Directory.Exists(workPath + "directory"), Is.True);
            Assert.That(File.Exists(workPath + "directory/test1.txt"), Is.True);
            Assert.That(File.Exists(workPath + "directory/test2.txt"), Is.True);

            // 元に戻す
            Directory.Delete(workPath + "directory", true);
            File.Move(workPath + "test.zip", workPath + "test.zi_");
        }

        /// <summary>
        /// 拡張子 : .ex_
        /// パスワード : password2
        /// </summary>
        [Test]
        public void Test003()
        {
            const string workPath = "TestData/Test003/";
            var args = new[] {workPath + "test.ex_"};
            Program.Main(args);

            Assert.That(Directory.Exists(workPath + "directory"), Is.True);
            Assert.That(File.Exists(workPath + "directory/test1.txt"), Is.True);
            Assert.That(File.Exists(workPath + "directory/test2.txt"), Is.True);

            // 元に戻す
            Directory.Delete(workPath + "directory", true);
            File.Move(workPath + "test.exe", workPath + "test.ex_");
        }
    }
}