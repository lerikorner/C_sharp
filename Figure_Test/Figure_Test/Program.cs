using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UnitTest_Figure
{
    [TestMethod]
    public void ZeroException(int par)
    {
        Assert.IsTrue((par > 0), "Неверное значение стороны");
    }
}

namespace Figure_Test
{
    public abstract class Figure
    {      
        public abstract void show();
        public abstract int square();
        public abstract double perimetr();
    }

    public class Rec_Triangle : Figure
    {
        public int cat;

        public override void show()
        {
            Console.WriteLine("Прямоугольный равнобедренный треугольник.");
            Console.WriteLine("Сторона: {0}", cat);
            Console.WriteLine("Площадь: {0}", square());
            Console.WriteLine("Периметр: {0}", perimetr());
        }

        public override int square()
        {
            int sq = (cat * cat / 2);
            return sq;
        }

        public override double perimetr()
        {
            double hyp = cat * Math.Sqrt(2);
            double per = hyp + 2 * cat;
            return per;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Rec_Triangle rec_tr = new Rec_Triangle();
            UnitTest_Figure utest = new UnitTest_Figure();

            rec_tr.cat = 4;
            utest.ZeroException(rec_tr.cat);      
            rec_tr.show();
            Console.ReadKey();
        }
    }

}
