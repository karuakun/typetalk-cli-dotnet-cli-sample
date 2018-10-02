using System;
using System.Collections.Generic;
using System.Text;

namespace GetTypetalkState.OuputLayout
{
    public interface ILayout
    {
        void Output<TSource>(IEnumerable<TSource> source);

    }
}
