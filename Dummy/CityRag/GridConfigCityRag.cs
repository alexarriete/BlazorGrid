using BlazorGrid.GridClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGrid.Dummy
{
    public class GridConfigCityRag : GridClasses.GridConfigurationBase
    {
        public GridConfigCityRag() : base()
        {
            GridTitle = "City Rags";
            Theme = new GridClasses.Theme(System.Drawing.Color.Black, System.Drawing.Color.White);
        }
        public async override Task<IEnumerable<object>> GetSourceListAsync()
        {
            return (await CityRagVM.GetAll());
        }

        public override List<ContextMenuOption> InitializeTitleContextMenuOptions()
        {
            List<ContextMenuOption> baseResult = base.InitializeTitleContextMenuOptions();
            return baseResult.Where(x => x.Id == "download").ToList();
            
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
