using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersection
{
    class InterSection
    {
        public static int solution(int[] a, int n)
        {
            int[,] coord = new int[n,2];
            int count = 0;
            for (int k = 0; k < n; k++)
            {
                coord[k, 0] = k+a[k];
                coord[k, 1] = k-a[k];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if ((coord[i, 0] >= coord[j, 1])|| (coord[i, 1] <= coord[j, 0]))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
    class Program
    {
        static void Main(string[] args)

        {
            int[] circles = new int[] { 1, 0, 1,0,1 };
            int count = InterSection.solution(circles, circles.Length);
            Console.WriteLine("Количество пересечений: {0}", count);
            Console.ReadKey();
        }
    }
}
