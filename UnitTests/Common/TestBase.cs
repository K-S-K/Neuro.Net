using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace UnitTests.Common
{
    public class TestBase
    {
        private object lock_dir = new object();
        private string inner_dir = string.Empty;

        /// <summary>
        /// Путь для экспорта результатов
        /// </summary>
        protected string ResultDir
        {
            get
            {
                lock (lock_dir)
                {
                    if (string.IsNullOrEmpty(inner_dir))
                    {
                        inner_dir = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");
                        if (!Directory.Exists(inner_dir)) Directory.CreateDirectory(inner_dir);
                    }
                    return inner_dir;
                }
            }
        }

        /// <summary>
        /// Экспорт результатов
        /// </summary>
        /// <param name="xeActual">Актуальный контент</param>
        /// <param name="xeExpctd">Ожидаемый контент</param>
        /// <param name="fnPrefix">Префикс имени файла</param>
        protected void ResultSave(XElement xeActual, XElement xeExpctd, string subDirPath, string fnPrefix)
        {
            string path = Path.Combine(ResultDir, subDirPath);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string fileNameExpctd = Path.Combine(path, string.Format("{0}.Expctd.xml", fnPrefix));
            string fileNameActual = Path.Combine(path, string.Format("{0}.Actual.xml", fnPrefix));

            new XDocument(xeActual).Save(fileNameActual);
            new XDocument(xeExpctd).Save(fileNameExpctd);
        }

        /// <summary>
        /// Экспорт результатов
        /// </summary>
        /// <param name="txtActual">Актуальный контент</param>
        /// <param name="xeExpctd">Ожидаемый контент</param>
        /// <param name="fnPrefix">Префикс имени файла</param>
        protected void ResultSave(string txtActual, string txtExpctd, string subDirPath, string fnPrefix)
        {
            string fileNameExpctd = Path.Combine(ResultDir, subDirPath, string.Format("{0}.Expctd.txt", fnPrefix));
            string fileNameActual = Path.Combine(ResultDir, subDirPath, string.Format("{0}.Actual.txt", fnPrefix));

            string dir = Path.GetDirectoryName(fileNameActual);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(fileNameActual)) File.Delete(fileNameActual);
            File.WriteAllText(fileNameActual, txtActual, Encoding.UTF8);

            if (File.Exists(fileNameExpctd)) File.Delete(fileNameExpctd);
            File.WriteAllText(fileNameExpctd, txtExpctd, Encoding.UTF8);
        }

        /// <summary>
        /// Анализ результатов тестирования
        /// </summary>
        /// <param name="xeActual">Актуальный контент</param>
        /// <param name="xeExpctd">Ожидаемый контент</param>
        /// <param name="fnPrefix">Префикс имени файла</param>
        /// <param name="errMsg">Сообщение об ошибке</param>
        protected void CompareResult(XElement xeActual, XElement xeExpctd, string subDirPath, string fnPrefix, string errMsg)
        {
            string strActual = xeActual.ToString();
            string strExpctd = xeExpctd.ToString();
            ResultSave(xeActual, xeExpctd, subDirPath, fnPrefix);
            if (strExpctd != strActual)
                throw new Exception(errMsg);
        }

        /// <summary>
        /// Анализ результатов тестирования
        /// </summary>
        /// <param name="xeActual">Актуальный контент</param>
        /// <param name="xeExpctd">Ожидаемый контент</param>
        /// <param name="fnPrefix">Префикс имени файла</param>
        /// <param name="errMsg">Сообщение об ошибке</param>
        protected void CompareResult(string txtActual, string txtExpctd, string subDirPath, string fnPrefix, string errMsg)
        {
            ResultSave(txtActual, txtExpctd, subDirPath, fnPrefix);
            if (txtActual != txtExpctd)
                throw new Exception(errMsg);
        }

        protected void CompareFiles(string fnActual, string fnExpctd, string errMsg)
        {
            FileComparator.CompareFiles(fnActual, fnExpctd, errMsg);
        }
    }
}
