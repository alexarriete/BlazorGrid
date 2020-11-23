using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Dummy
{
    public class GridConfigEmployee: GridClasses.GridConfigurationBase
    {
        public GridConfigEmployee(): base()
        {
            GridTitle = "Employeeeeeeeeeeeeee";
            Theme = new GridClasses.Theme(System.Drawing.Color.Pink, System.Drawing.Color.Black);

        }
        public async override Task<IEnumerable<object>> GetSourceListAsync()
        {
            return (await EmployeeVM.GetAll());
        }
    }
}
