using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.Model
{
    public class Tasks
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int Id { get; set; }
        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Header { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        [Indexed(Name = "ListingID", Order = 2, Unique = true)]
        public DateTime Date { get; set; }
    }
}
