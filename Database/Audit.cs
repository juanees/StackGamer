using System;
using System.Collections.Generic;
using System.Text;

namespace Database
{
    public class Audit
    {
        //https://www.meziantou.net/entity-framework-core-history-audit-table.htm
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }


}
