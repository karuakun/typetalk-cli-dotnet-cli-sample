using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GetTypetalkState.OuputLayout
{
    public class TableLayout: ILayout
    {
        public void Output<TSource>(IEnumerable<TSource> source)
        {
            var props = typeof(TSource).GetProperties();
            Console.WriteLine($"|{string.Join("|", props.Select(p => p.Name).ToArray())}|");

            Console.Write("|");
            foreach (var p in props)
            {
                for (var i = 0; i < p.Name.Length; i++)
                {
                    Console.Write("-");
                }
                Console.Write("|");
            }
            Console.WriteLine();

            foreach (var s in source)
            {
                Console.Write("|");
                foreach (var v in props.Select(p => p.GetValue(s)))
                {
                    Console.Write($"{v?.ToString().Replace("\n", " ")}|");
                }
                Console.WriteLine();
            }
        }
    }
}
