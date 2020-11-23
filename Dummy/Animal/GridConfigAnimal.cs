using BlazorGrid.GridClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Dummy
{
    public class GridConfigAnimal: GridClasses.GridConfigurationBase
    {
        public GridConfigAnimal(): base()
        {
            GridTitle = "Employee";

        }
        public async override Task<IEnumerable<object>> GetSourceListAsync()
        {
            return (await EmployeeVM.GetAll());
        }

        public override List<ContextMenuOption> InitializeTitleContextMenuOptions()
        {
            return base.InitializeTitleContextMenuOptions();
        }

        public override List<GridColumnBase> GetGridColumnBase()
        {
            return base.GetGridColumnBase();
        }

        public async override Task<byte[]> DownloadExcel(IEnumerable<object> itemList)
        {
            return await base.DownloadExcel(itemList);
        }

        public async override Task<string> ProcessItemsUploaded(List<object> objects)
        {
            return await base.ProcessItemsUploaded(objects);
        }
    }
}
