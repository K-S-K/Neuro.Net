using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitTests.Common;
using UnitTests.UTMatrix;

using NN.Matrix;

namespace UTMatrx
{
    [TestClass]
    public class UT_Matrix : TestBase
    {
        private const string subDirName = "Matrix";

        [TestMethod]
        public void MatrixFill()
        {
            Matrix m = new Matrix(3, 2);

            m[0, 0] = 2; m[0, 1] = 1;
            m[1, 0] = -3; m[1, 1] = 0;
            m[2, 0] = 4; m[2, 1] = -1;


            string txActual = m.Text;
            string txExpctd = Data.TextA;

            CompareResult(
                txActual, txExpctd, subDirName,
                "MatrixText", "MatrixText changed");


            XElement xeActual = m.XContent;
            XElement xeExpctd = XElement.Parse(Data.XML_A);

            CompareResult(
                xeActual, xeExpctd, subDirName,
                "MatrixXML", "Matrix XML serialization changed");


            m[1, 0] = -8;

            m.XContent = xeExpctd;
            xeActual = m.XContent;

            CompareResult(
                xeActual, xeExpctd, subDirName,
                "MatrixXML", "Matrix XML deserialization changed");

            m = new Matrix(xeExpctd);
            xeActual = m.XContent;

            CompareResult(
                xeActual, xeExpctd, subDirName,
                "MatrixXML", "Matrix creation from XML changed");
        }

        [TestMethod]
        public void MatrixProduct()
        {
            Matrix a = new Matrix(XElement.Parse(Data.XML_A));
            Matrix b = new Matrix(XElement.Parse(Data.XML_B));
            Matrix c = a * b;


            string txActual = c.Text;

            XElement xeActual = c.XContent;
            XElement xeExpctd = XElement.Parse(Data.XML_C);

            CompareResult(
                xeActual, xeExpctd, subDirName,
                "MatrixProd", "Matrix product operation failed");
        }

        [TestMethod]
        public void MatrixTranspose()
        {
            Matrix a = new Matrix(XElement.Parse(Data.XML_T1));
            Matrix t = a.Transpose();

            XElement xeActual = t.XContent;
            XElement xeExpctd = XElement.Parse(Data.XML_T2);

            CompareResult(
                xeActual, xeExpctd, subDirName,
                "MatrixTransp", "Matrix transpose operation failed");
        }
    }
}
