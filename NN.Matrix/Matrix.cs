using System;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace NN.Matrix
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://en.wikipedia.org/wiki/Matrix_multiplication
    /// http://twt.mpei.ac.ru/math/LARB/Matrdet/Matrix/LA_01010300.html
    /// 
    /// https://msdn.microsoft.com/ru-ru/library/5tk49fh2.aspx
    /// https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/keywords/operator
    /// 
    /// http://ru.onlinemschool.com/math/assistance/matrix/multiply/
    /// http://twt.mpei.ac.ru/math/LARB/Matrdet/Matrix/LA_01010300.html
    /// </remarks>
    public class Matrix
    {
        #region -> Data
        private double[,] data;
        public static string nodeName = "Matrix";
        #endregion

        #region -> Properties
        public int Row { get; private set; }
        public int Col { get; private set; }
        public double this[int row, int col]
        {
            get { return data[row, col]; }
            set { data[row, col] = value; }
        }

        public XElement XContent
        {
            get
            {
                XElement xe = new XElement(nodeName
                   , new XAttribute("row", Row)
                   , new XAttribute("col", Col)
                   );


                for (int row = 0; row < Row; row++)
                {
                    XElement xeLine = new XElement("TR", new XAttribute("row", row));

                    for (int col = 0; col < Col; col++)
                    {
                        double val = this[row, col];

                        xeLine.Add(new XElement("TD"
                            , new XAttribute("row", row)
                            , new XAttribute("col", col)
                            , new XAttribute("val", val.ToString("0.###"))));
                    }
                    xe.Add(xeLine);
                }

                return xe;
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("Nothing to deserialize");

                if (!value.Name.ToString().StartsWith(nodeName))
                    throw new ArgumentException(string.Format(
                        "Unexpected element name {0} instead of {1}...", value.Name, nodeName));

                int row = int.Parse(value.Attribute("row").Value);
                int col = int.Parse(value.Attribute("col").Value);
                double val;

                bool needCrtData = Row == 0 && Col == 0;
                if (needCrtData)
                {
                    Row = row;
                    Col = col;
                    data = new double[row, col];
                }
                else if (row != Row || col != Col)
                {
                    throw new Exception(string.Format(
                        "Unexpected matrix demention [{0}x{1}] " +
                        "instead of [{2}x{3}] ", row, col, Row, Col));
                }

                foreach (XElement xeLine in value.Elements("TR"))
                {
                    foreach (XElement xeData in xeLine.Elements("TD"))
                    {
                        row = int.Parse(xeData.Attribute("row").Value);
                        col = int.Parse(xeData.Attribute("col").Value);
                        string str = xeData.Attribute("val").Value;
                        val = double.Parse(str.Replace(",", "."),
                            CultureInfo.InvariantCulture);

                        this[row, col] = val;
                    }
                }
            }
        }
        #endregion


        #region -> Methods
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Col != b.Row)
            {
                // The product AB is defined only 
                // if the number of columns in A is 
                // equal to the number of rows in B
                throw new Exception(string.Format(
                    "You can't multiply {0} with {1}", a, b));
            }

            Matrix c = new Matrix(a.Row, b.Col);

            Multiply(a, b, c);

            return c;
        }

        public static void Multiply(Matrix a, Matrix b, Matrix c)
        {
            if (c.Row != a.Row)
            {
                // The product AB is defined only 
                // if the number of columns in A is 
                // equal to the number of rows in B
                throw new Exception(string.Format(
                    "The result row count ({0}) is imcompatible "
                    + "with source row count ({1})", c.Row, a.Row));
            }

            if (c.Col != b.Col)
            {
                // The product AB is defined only 
                // if the number of columns in A is 
                // equal to the number of rows in B
                throw new Exception(string.Format(
                    "The result col count ({0}) is imcompatible "
                    + "with source col count ({1})", c.Col, a.Col));
            }

            for (int i = 0; i < c.Row; i++)
            {
                for (int j = 0; j < c.Col; j++)
                {
                    double v = 0;
                    for (int k = 0; k < a.Col; k++)
                    {
                        v += a[i, k] * b[k, j];
                    }
                    c[i, j] = v;
                }
            }
        }


        public Matrix Transpose()
        {
            Matrix other =
                new Matrix(this.Col, this.Row);

            Transpose(other);

            return other;
        }

        public void Transpose(Matrix other)
        {
            #region -> Compatibleness check
            {
                if (other.Row != this.Col)
                {
                    // The product AB is defined only 
                    // if the number of columns in A is 
                    // equal to the number of rows in B
                    throw new Exception(string.Format(
                        "The result row count ({0}) is imcompatible "
                        + "with source col count ({1})", other.Row, this.Col));
                }

                if (other.Col != this.Row)
                {
                    // The product AB is defined only 
                    // if the number of columns in A is 
                    // equal to the number of rows in B
                    throw new Exception(string.Format(
                        "The result col count ({0}) is imcompatible "
                        + "with source row count ({1})", other.Col, this.Row));
                }
            }
            #endregion

            for (int row = 0; row < this.Row; row++)
            {
                for (int col = 0; col < this.Col; col++)
                {
                    other[col, row] = this[row, col];
                }
            }

        }

        public override string ToString()
        {
            return string.Format(
                "[{0}x{1}]", Row, Col);
        }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int row = 0; row < Row; row++)
                {
                    for (int col = 0; col < Col; col++)
                    {
                        double v = this[row, col];
                        sb.AppendFormat("  {0,6}",
                            v.ToString("0.##0"));
                    }
                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }
        #endregion


        #region -> Ctor
        public Matrix(int row, int col)
        {
            Row = row;
            Col = col;

            data = new double[row, col];
        }

        public Matrix(XElement xe) { this.XContent = xe; }

        public static Matrix Random(int row, int col)
        {
            Matrix matrix = Half(row, col);

            Random rnd = new Random();

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    matrix[r, c] =
                        0.1 * rnd.Next(0, 10);
                }
            }

            return matrix;
        }

        public static Matrix Half(int row, int col) { return Homogeneous(row, col, 0.5); }
        public static Matrix One(int row, int col) { return Homogeneous(row, col, 1.0); }

        public static Matrix Homogeneous(int row, int col, double val)
        {
            if (row <= 0)
                throw new ArgumentException(
                    "The row count must be greater then 0");

            if (col <= 0)
                throw new ArgumentException(
                    "The col count must be greater then 0");

            Matrix matrix = new Matrix(row, col);

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    matrix[r, c] = val;
                }
            }

            return matrix;
        }
        #endregion
    }
}
