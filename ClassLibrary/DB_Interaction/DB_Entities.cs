using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DB_Interaction
{
    public class db_Entry
    {
        public int EID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class db_Tag
    {
        public int TID { get; set; }
        public string TName { get; set; }
    }

    public class db_Relationship()
    {
        public int PID { get; set; }
        public int CID { get; set; }
    }

    public class db_Tagging()
    {
        public int EID { get; set; }
        public int TID { get; set; }
    }
}
