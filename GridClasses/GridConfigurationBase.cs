using BlazorGrid.Dummy.Animal;
using MatBlazor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorGrid.GridClasses
{
    public class GridConfigurationBase
    {
        public List<GridColumnBase> GridColumnBases { get; set; }
        public List<object> ItemList { get; set; }
        public List<object> ItemListUploaded { get; set; }
        public Type ItemType { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int PageIndex { get; set; }
        public string GridTitle { get; set; }
        public string KeyColumn { get; set; }
       // public string EnviromentColor { get; set; }      
        public string ExcelFileName { get; set; }
        public List<ContextMenuOption> ContextTitleMenuOptions { get; set; }
        public Theme Theme { get; set; }
        public string UrlFolder { get; set; }
        

        public GridConfigurationBase()
        {
            PageSize = 10;
            PageIndex = 0;            
            ItemType = Task.Run(()=>GetObjectTypeAsync()).Result;
            GridTitle = $"{ItemType.Name} list";
            //EnviromentColor = "darkseagreen";
            Theme = new Theme();

            KeyColumn = Task.Run(() => GetSourceListAsync()).Result.FirstOrDefault().GetType().GetProperties()
               .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), inherit: false).Any())?.Name;

            GridColumnBases = GetGridColumnBase();
            ContextTitleMenuOptions = new List<ContextMenuOption>();
            ContextTitleMenuOptions =  InitializeTitleContextMenuOptions();
            ExcelFileName = $"{ItemType.Name}{DateTime.Now.Date.ToShortDateString().Replace("/", "-")}.xlsx";
            UrlFolder = ItemType.Name.ToLower();
        }

        public async Task<IEnumerable<object>> GetListAsync(GridSearch search)
        {
            if (search == null || search.SearchProperty == null)
            {
                return (await GetSourceListAsync());
            }
            else if (search.SearchPropType == PropertyType.datetime)
            {
                return await search.GetFilteredByDateTimeInterval(await GetSourceListAsync());
            }
            else if (search.SearchPropType == PropertyType.number)
            {
                switch (search.NumberSearchTypeSelected)
                {                   
                    case NumberSelectionType.Greater_Than:
                        return await search.GetGreaterThanValues(await GetSourceListAsync());
                    case NumberSelectionType.Less_Than:
                        return await search.GetLessThanValues(await GetSourceListAsync());
                    case NumberSelectionType.Between:
                        return await search.GetBetweenValues(await GetSourceListAsync());
                    default:
                        return await search.GetEqualsValues(await GetSourceListAsync());
                }
                
            }
            return await search.GetTextContains(await GetSourceListAsync());
        }


        public async Task<List<object>> GetPageAsync(SortChangeEvent sort, GridSearch search)
        {
            if (sort == null)
            {
                return (await GetListAsync(search))
                    .Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                PropertyInfo prop = ItemType.GetProperty(sort.SortId);
                if (sort.Direction == SortDirection.Desc)
                {

                    return (await GetListAsync(search)).OrderByDescending(x => prop.GetValue(x, null))
                        .Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
                else
                {
                    return (await GetListAsync(search)).OrderBy(x => prop.GetValue(x, null))
                        .Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
                }
            }
        }

        private async Task<Type> GetObjectTypeAsync()
        {
            return (await GetSourceListAsync()).FirstOrDefault().GetType();
        }

        public virtual List<GridColumnBase> GetGridColumnBase()
        {
            List<PropertyInfo> props = ItemType.GetProperties().ToList();
            return  props.Select(x => new GridColumnBase(x, props.IndexOf(x), KeyColumn)).ToList();
        }      

        public async virtual Task<IEnumerable<object>> GetSourceListAsync()
        {
            return await Animal.GetAll();
        }

        public virtual List<ContextMenuOption> InitializeTitleContextMenuOptions()
        {
            ContextTitleMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-info", Name = "Test Grid", Id = "test" });
            ContextTitleMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-info", Name = "Download Excel", Id = "download" });
            ContextTitleMenuOptions.Add(new ContextMenuOption() { BtnCss = "btn-info", Name = "Upload Excel", Id = "upload" });
            return ContextTitleMenuOptions;
        }

        public async virtual Task<byte[]> DownloadExcel(IEnumerable<object> itemList)
        {
            GridToExcelBase gridToExcel = new GridToExcelBase();
            return await Task.Run(() => gridToExcel.GenerateReport(itemList.ToList(), this));
        }

        public async virtual Task<string> ProcessItemsUploaded(List<object> objects)
        {
            return "You must overrride ProcessItemsUploaded in GridConfiguration";
        }



        public virtual async Task TestAsync()
        {
            //await TestPaginatorAsync(10);
            await TestColumnOrderAsync();
        }


        public async Task<string> FilesReadyForContent(IMatFileUploadEntry[] files)
        {
            try
            {
                var file = files.FirstOrDefault();
                if (file == null)
                {
                    return "File can't be empty";
                }
                DataTable datatable = await GridToExcelBase.GetDataTableAsync(file, GridColumnBases);
                List<object> objectList = Task.Run(()=> ConvertDataTable(datatable)).Result;
                return await ProcessItemsUploaded(objectList);
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }       

        private List<object> ConvertDataTable(DataTable dt)
        {
            List<object> data = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                object item = GetItem(row);
                data.Add(item);
            }
            return data;
        }
        private object GetItem(DataRow dr)
        {
            var obj = Activator.CreateInstance(ItemType);

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (GridColumnBase cb in GridColumnBases)
                {
                    if (cb.PropertyInfo.Name == column.ColumnName)
                    {                        
                        PropertyType pt = cb.PropertyType;
                        if(pt == PropertyType.number)
                        {
                            cb.PropertyInfo.SetValue(obj, Int32.Parse(dr[column.ColumnName].ToString()), null);
                        }
                        else
                        {
                            cb.PropertyInfo.SetValue(obj, dr[column.ColumnName], null);
                        }                        
                    }                        
                    else
                        continue;
                }
            }
            return obj;
        }


      

        private async Task TestPaginatorAsync(int numberOfPages)
        {
            for (int i = 0; i < numberOfPages; i++)
            {                
                Random rand = new Random();
                int rInt = rand.Next(0, Total / PageSize);
                PageIndex = rInt;
               
                ItemList =  await GetPageAsync(null, null);
            }
        }

        private async Task TestColumnOrderAsync()
        {
            foreach (GridColumnBase column in GridColumnBases)
            {
                SortChangeEvent sort = new SortChangeEvent() { SortId = column.PropertyInfo.Name, Direction = SortDirection.Asc };
                ItemList = await GetPageAsync(sort, null);
            }
        }

    }

   
}
