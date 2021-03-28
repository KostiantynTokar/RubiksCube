using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube
{
    class Program
    {
        private static void SAHandler(object sender, StartAlgorithmEventArgs args)
        {
            Console.Clear();
            Instruction();
        }
        private static void EAHandler(object sender, EndAlgorithmEventArgs args)
        {
            Console.WriteLine();
            Console.WriteLine("Количество ходов, затраченное алгоритмом: {0}",args.NumOfSteps);
        }
        private static void Instruction()
        {
            Console.WriteLine("U - w\t\tU' - W\t\tU2 - i");
            Console.WriteLine("F - a\t\tF' - A\t\tF2 - j");
            Console.WriteLine("R - s\t\tR' - S\t\tR2 - k");
            Console.WriteLine("B - d\t\tB' - D\t\tB2 - l");
            Console.WriteLine("L - f\t\tL' - F\t\tL2 - ;");
            Console.WriteLine("D - x\t\tD' - X\t\tD2 - ,");
            Console.WriteLine();
            Console.WriteLine("X - v\t\tX' - V");
            Console.WriteLine("Y - b\t\tY' - B");
            Console.WriteLine("Z - n\t\tZ' - N");
            Console.WriteLine();
            Console.WriteLine("1 - послойный метод");
            Console.WriteLine();
            Console.WriteLine("\'-\' - уменьшить скорость анимации");
            Console.WriteLine("\'+\' - увеличить скорость анимации");
            Console.WriteLine("\'`\' - отключить анимацию");
        }
        static void Main(string[] args)
        {
            LayerMethod.StartAlgorithmEvent += SAHandler;
            LayerMethod.EndAlgorithmEvent += EAHandler;
            Instruction();
            Visualizer.Start();
        }
    }
}
