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


        public static void FFT(ref Complex[] Data)
        {
            if (Data.Length == 1) return;
            int N = 1 << (int)Math.Ceiling(Math.Log(Data.Length) / Math.Log(2));
            var l = new List<Complex>(Data);
            l.InsertRange(Data.Length, new Complex[N - Data.Length]);
            Data = l.ToArray();
            FFT_rec(ref Data, false);
        }
        public static void IFFT(ref Complex[] Data)
        {
            FFT_rec(ref Data, true);
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = Data[i] / Data.Length;
            }
        }
        static void FFT_rec(ref Complex[] data, bool IsInverseTransform)
        {
            if (data.Length <= 1) return;
            Complex[] dataEven = new Complex[data.Length / 2];
            Complex[] dataOdd = new Complex[data.Length / 2];
            for (int i = 0; i < data.Length;)
            {
                dataEven[i / 2] = data[i++];
                dataOdd[i / 2] = data[i++];
            }
            FFT_rec(ref dataEven, IsInverseTransform);
            FFT_rec(ref dataOdd, IsInverseTransform);
            for (int i = 0; i < data.Length / 2; ++i)
            {
                Complex phi = new Complex(Math.Cos(-2 * Math.PI * i / data.Length), Math.Sin(-2 * Math.PI * i / data.Length));
                Complex t = dataOdd[i] * (IsInverseTransform ? Complex.Conjugate(phi) : phi);
                data[i] = dataEven[i] + t;
                data[data.Length / 2 + i] = dataEven[i] - t;
            }
        }
        public static void Print(Complex[] Data)
        {
            Console.WriteLine(string.Join(" ", (from c in Data
                                                select Math.Round(c.Real)).ToArray()));
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
            else
            {
                a = str1;
                b = str2;
            }
            a += '0';
            b += new string('0', a.Length - b.Length);
            int cur = 0;
            for (int i = 0; i < a.Length; i++)
            {
                cur = a[i] - '0' + b[i] - '0' + cur;
                s += (char)('0' + cur % 10);
                cur /= 10;
                /*if (a[i] + b[i] + count < '5' * 2) //wtf?!
                {
                    s += (char)(a[i] + b[i] + count - '0');
                    count = 0;
                }
                else
                {
                    s += (char)(a[i] + b[i] + count - '0' - 10);
                    count = 1;
                }*/
            }
            s = s.Insert(b.Length, a.Substring(b.Length));
            s = RemoveZeros(s);
            return s;
        }
        public static BigInt Sub(BigInt A, BigInt B)
        {
            BigInt a, b;
            string s = "";
            bool IsNegative;
            if (A >= B)
            {
                a = A;
                b = B;
                IsNegative = A.IsNeg;
            }
            else
            {
                a = B;
                b = A;
                IsNegative = B.IsNeg;
            }
            b.s += new string('0', a.s.Length - b.s.Length);
            byte count = 0;
            for (int i = 0; i < a.s.Length; i++)
            {
                if (a.s[i] - b.s[i] - count >= 0)
                {
                    s += (char)(a.s[i] - b.s[i] - count + '0');
                    count = 0;
                }
                else
                {
                    s += (char)(a.s[i] - b.s[i] - count + '0' + 10);
                    count = 1;
                }
            }
            s = s.Insert(b.s.Length, a.s.Substring(b.s.Length));
            s = RemoveZeros(s);
            return new BigInt(IsNegative, s, true);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is BigInt)) return false;
            if (obj as BigInt != this) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(BigInt A, BigInt B)
        {
            if ((A.s == B.s) && (A.IsNeg == B.IsNeg)) return true;
            else return false;
        }
        public static bool operator !=(BigInt A, BigInt B)
        {
            return !(A == B);
        }
        public static bool operator >(BigInt A, BigInt B)
        {
            if (A.s.Length > B.s.Length)
            {
                return true;
            }
            else
            {
                if (A.s.Length < B.s.Length) return false;
                for (int i = A.s.Length - 1; i >= 0; i--)
                {
                    if (A.s[i] > B.s[i]) return true;
                    if (A.s[i] < B.s[i]) return false;
                }
                return false;
            }
        }
        public static bool operator <(BigInt A, BigInt B)
        {
            if (A > B) return false;
            if (A == B) return false;
            return true;
        }
        public static bool operator >=(BigInt A, BigInt B)
        {
            if (A < B) return false;
            return true;
        }
        public static bool operator <=(BigInt A, BigInt B)
        {
            if (A > B) return false;
            return true;
        }
        public static BigInt operator +(BigInt A, BigInt B)
        {
            string s;
            if (A.IsNeg == B.IsNeg)
            {
                s = Sum(A.s, B.s);
            }
            else
            {
                return Sub(A, B);
            }
            return new BigInt(A.IsNeg, s, true);
        }
        public static BigInt operator +(BigInt A, int b)
        {
            return A + new BigInt(b);
        }
        public static BigInt operator -(BigInt A)
        {
            A.IsNeg = !A.IsNeg;
            return A;
        }
        public static BigInt operator -(BigInt A, BigInt B)
        {
            return (A + (-B));
        }
        public Complex[] FFT(int length)
        {
            Complex[] form = new Complex[length];
            for (int i = length - 1; i >= length - s.Length; i--)
            {
                form[i] = (char)s[length - i - 1] - '0';
            }
            Fourier.FFT(ref form);
            return form;
        }
        public static BigInt operator *(BigInt A, double d)
        {
            if (d == 10)
            {
                if (A.s == "0") return A;
                A.s = A.s.Insert(0, "0");
                return A;
            }
            else return new BigInt(0);
        }
        public static BigInt operator *(BigInt A, BigInt B)
        {
            Complex[] a, b;
            int N = 2 << (int)Math.Ceiling(Math.Log(Math.Max(A.s.Length, B.s.Length)) / Math.Log(2));
            a = A.FFT(N);
            b = B.FFT(N);
            for (int i = 0; i < a.Length; i++)
            {
                a[i] *= b[i];
            }

            Fourier.IFFT(ref a);
            BigInt C = new BigInt(0);
            for (int i = 0; i < a.Length - 1; i++)
            {
                C = C * 10 + new BigInt((int)Math.Round(a[i].Real));
            }
            return C;
        }
        public static BigInt operator /(BigInt N, BigInt D)
        {
            if (N < D) return new BigInt(0);
            int size = N.s.Length - D.s.Length + 1;
            int pow = D.s.Length;
            BigInt x = new BigInt(10 / (D.s.Last() - '0'));
            x.s = new string('0', size) + x.s;
            BigInt two = new BigInt(2);
            two.s = new string('0', size + pow) + two.s;
            BigInt prev;
            while (true)
            {
                prev = x;
                x = x * (two - D * x);
                x.s = x.s.Remove(0, size + pow);
                if (x == prev) break;
            }
            BigInt Q = N * x;
            Q.s = Q.s.Remove(0, size + pow);
            if ((Q + 1) * D <= N) Q += 1;
            return Q;
        }
        public static BigInt operator %(BigInt N, BigInt D)
        {
            return new BigInt(N - (N / D) * D);
        }
        public void Print()
        {
            if (IsNeg) System.Console.Write('-');
            System.Console.WriteLine(Reverse(s));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                BigInt a = new BigInt(Console.ReadLine());
                BigInt b = new BigInt(Console.ReadLine());
                a -= b;
                a.Print();
            }


        }
    }
}
