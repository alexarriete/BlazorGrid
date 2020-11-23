using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.GridClasses
{
    public enum SortDirection
    {
        None = 0,
        Asc = 1,
        Desc = 2
    }
    public class SortChangeEvent
    {
        public string SortId { get; set; }
        public SortDirection Direction { get; set; }
    }
}
