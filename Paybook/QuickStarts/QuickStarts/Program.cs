using System;

namespace QuickStarts
{
    public class Program
    {
        static void Main(string[] args)
        {
            quickstart_normal normal = new quickstart_normal();
            normal.start();

            //uncomment this section in order to execute quickstart_sat
            //quickstart_sat sat = new quickstart_sat();
            //sat.start();

            //uncomment this section in order to execute quickstart_token_bank
            //quickstart_token_bank token_bank = new quickstart_token_bank();
            //token_bank.start();

            Console.ReadLine();
        }
    }
}
