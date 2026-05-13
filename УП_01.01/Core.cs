using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace УП_01._01
{
    internal class Core
    {
        public static ReadWriteEntities1 Context = new ReadWriteEntities1();

        public static void ResetContext()
        {
            Context = new ReadWriteEntities1();
        }
    }
}