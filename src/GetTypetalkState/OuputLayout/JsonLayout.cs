using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GetTypetalkState.OuputLayout
{
    public class JsonLayout: ILayout
    {
        public void Output<TSource>(IEnumerable<TSource> source)
        {
            Console.WriteLine(JsonConvert.SerializeObject(source));
        }
    }
}
