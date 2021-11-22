using System.Data;

namespace System.Linq
{
    internal class SystemCore_EnumerableDebugView
    {
        private DataRowCollection rows;

        public SystemCore_EnumerableDebugView(DataRowCollection rows)
        {
            this.rows = rows;
        }
    }
}