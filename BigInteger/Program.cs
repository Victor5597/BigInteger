using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BigInteger
{
    class Fourier
    {
        public Complex[] Data { get; private set; }
        public double[] Intensivity { get; private set; }
        public Fourier(Complex[] InputArray)
        {
            Data = (Complex[])InputArray.Clone();
        }
        
        public Fourier(double[] InputArray)
            : this((from d in InputArray
                    select new Complex(d, 0)).ToArray())
        { }
        public void FFT()
        {
            if (Data.Length == 1) return;
            int N = 1 << (int)Math.Ceiling(Math.Log(Data.Length) / Math.Log(2));
            var l = new List<Complex>(Data);
            l.InsertRange(Data.Length, new Complex[N - Data.Length]);
            Data = l.ToArray();
            FFT_rec(Data, false);

            Intensivity = (from c in Data
                           select Math.Round(c.Magnitude * 10)).ToArray();
            return;
        }
        public void IFFT()
        {
            FFT_rec(Data, true);
        }
        static void FFT_rec(Complex[] data, bool IsInverseTransform)
        {
            if (data.Length <= 1) return;
            Complex[] dataEven = new Complex[data.Length / 2];
            Complex[] dataOdd = new Complex[data.Length / 2];
            for (int i = 0; i < data.Length;)
            {
                dataOdd[i / 2] = data[i++];
                dataEven[i / 2] = data[i++];
            }
            FFT_rec(dataEven, IsInverseTransform);
            FFT_rec(dataOdd, IsInverseTransform);
            for (int i = 0; i < data.Length / 2; ++i)
            {
                Complex phi = new Complex(Math.Cos(-2 * Math.PI * i / data.Length), Math.Sin(-2 * Math.PI * i / data.Length));
                Complex t = dataOdd[i] * (IsInverseTransform ? Complex.Conjugate(phi) : phi);
                data[i] = dataEven[i] + t;
                data[data.Length / 2 + i] = dataEven[i] - t;
            }
        }
        public void Print()
        {
            Console.WriteLine(string.Join(" ", Intensivity));
        }
    }
    class BigInt
    {
        public string s { get; private set; }
        public bool IsNeg { get; private set; }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string RemoveZeros(string str)
        {
            while (str.Last() == '0' && str.Length > 1) str = str.Remove(str.Length - 1);
            return str;
        }
        public BigInt(int num)
        {
            if (num < 0) IsNeg = true;
            s = Reverse(Math.Abs(num).ToString());
            s = RemoveZeros(s);
        }
        public BigInt(bool IsNegative, string str, bool IsReversed)
        {
            IsNeg = IsNegative;
            if (IsReversed) s = str;
            else s = Reverse(str);
            s = RemoveZeros(s);
        }
        public BigInt(string str)
        {
            if (str[0] == '-')
            {
                str = str.Remove(0, 1);
                IsNeg = true;
            }
            else IsNeg = false;
            s = Reverse(str);
            s = RemoveZeros(s);
        }
        public BigInt(bool IsNegative, params int[] numbers)
        {
            IsNeg = IsNegative;
            s = "";
            for (int i = numbers.Count() - 1; i >= 0; i--)
            {
                
                s = s.Insert(0, numbers[i].ToString());
                
            }
            s = Reverse(s);
            s = RemoveZeros(s);
        }
        public BigInt(bool IsNegative, params char[] figures)
            : this(IsNegative, figures.ToString(), false) { }
        public BigInt(BigInt n)
        {
            s = n.s;
            IsNeg = n.IsNeg;
        }
        public static string Sum(string str1, string str2)
        {
            string a, b, s = "";
            if (str1.Length < str2.Length)
            {
                a = str2;
                b = str1;
            }
            else{
                a = str1;
                b = str2;
            }
            a += '0';
            b += new string('0', a.Length - b.Length);
            byte count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] + b[i] + count < '5' * 2)
                {
                    s += (char)(a[i] + b[i] + count - '0');
                    count = 0;
                }
                else
                {
                    s += (char)(a[i] + b[i] + count - '0' - 10);
                    count = 1;
                }
            }
            s = s.Insert(b.Length, a.Substring(b.Length));
            s = RemoveZeros(s);
            return s;
        }
        public static BigInt Sub(string str1, string str2)
        {
            string a, b, s = "";
            bool IsNegative;
            if (str1.Length == str2.Length)
            {
                if (str1 == str2) return new BigInt(0);
                for (int i = str1.Length - 1; i >= 0; i--)
                {
                    if(str1[i] > str2[i])
                    {
                        str1 += '0';
                        break;
                    }
                    if(str1[i] < str2[i])
                    {
                        str2 += '0';
                        break;
                    }
                }
            }
            if (str1.Length < str2.Length)
            {
                a = str2;
                b = str1;
                IsNegative = true;
            }
            else{
                a = str1;
                b = str2;
                IsNegative = false;
            }
            b += new string('0', a.Length - b.Length);
            byte count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] - b[i] - count >= 0)
                {
                    s += (char)(a[i] - b[i] - count + '0');
                    count = 0;
                }
                else
                {
                    s += (char)(a[i] - b[i] - count + '0' + 10);
                    count = 1;
                }
            }
            s = s.Insert(b.Length, a.Substring(b.Length));
            s = RemoveZeros(s);
            if (IsNegative == true) s.Insert(0, "-");
            return new BigInt(IsNegative, s, true);
        }
        public static BigInt operator+(BigInt A, BigInt B)
        {
            string s;
            if (A.IsNeg == B.IsNeg){
                s = Sum(A.s, B.s);
            }
            else
            {
                BigInt C = Sub(A.s, B.s);
                if (A.IsNeg == true) C.IsNeg = !C.IsNeg;
                return C;
            }
            return new BigInt(A.IsNeg, s, true);
        }
        public static BigInt operator-(BigInt A)
        {
            A.IsNeg = !A.IsNeg;
            return A;
        }
        public static BigInt operator-(BigInt A, BigInt B)
        {
            return (A + (-B));
        }
        public Complex[] FFT()
        {
            Complex[] form = new Complex[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                form[i] = (char)s[i] - '0';
            }
            Fourier F = new Fourier(form);
            return F.Data;
        }
        public static BigInt operator *(BigInt A, double d)
        {
            //PIZDEC
            if (d == 10)
            {
                A.s.Insert(0, "0");
                return A;
            }
            else return new BigInt(0);
            //PIZDEC
        }
        public static BigInt operator*(BigInt A, BigInt B)
        {
            Complex[] a, b;
            if (A.s.Length > B.s.Length)
            {
                a = A.FFT();
                b = B.FFT();
            }
            else
            {
                b = A.FFT();
                a = B.FFT();
            }
            for (int i = 0; i < b.Length; i++)
            {
                a[i] *= b[i];
            }
            for (int i = b.Length; i < a.Length; i++)
            {
                a[i] = 0;
            }
            Fourier F = new Fourier(a);
            F.IFFT();
            BigInt C = new BigInt((int)a[0].Magnitude);
            for (int i = 1; i < a.Length; i++)
            {
                C = C + new BigInt((int)a[i].Magnitude);//???
            }
            return A;
        }
        public void Print()
        {
            if(IsNeg) System.Console.Write('-');
            System.Console.WriteLine(Reverse(s));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            /*
            BigInt a = new BigInt("-1");
            BigInt b = new BigInt("-999");
            a += b;
            a.Print();
            */
            int L = 257;
            double[] a = new double[L];
            for (int i = 0; i < L; ++i)
            {
                a[i] = Math.Sin(2 * Math.PI * i/12);
            }
            Fourier f = new Fourier(a);
            f.FFT();
            f.Print();
            Console.ReadKey();
        }
    }
}
