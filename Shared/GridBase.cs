
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MatBlazor;
using Microsoft.AspNetCore.Components.Web;
using BlazorGrid.GridClasses;
using Microsoft.JSInterop;

namespace BlazorGrid.Shared
{
    public class GridBase : ComponentBase
    {
        [Parameter]
        public GridConfigurationBase GridConfig { get; set; }
        protected GridSearch GridSearch { get; set; }
        protected bool ShowContextMenu { get; set; }
        protected bool ShowTitleContextMenu { get; set; }
        protected bool ShowUploadSubMenu { get; set; }
        protected string ErrorMessage { get; set; }
        public CellContexMenuOption CellContexMenu { get; set; }
        private SortChangeEvent Sort { get; set; }
        [Inject]
        private IJSRuntime jSRuntime { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (GridConfig != null)
            {
                GridSearch = new GridSearch();
                await base.OnInitializedAsync();
                await ConfigureProperties();
            }
        }

        private async Task ConfigureProperties()
        {
            CellContexMenu = null;
            GridConfig.Total = (await GridConfig.GetListAsync(GridSearch)).Count();
            GridConfig.ItemList = await GridConfig.GetPageAsync(Sort, GridSearch);
            GridSearch.SearchDateTo = GridSearch.SearchDateTo == DateTime.MinValue ? DateTime.Now : GridSearch.SearchDateTo;
        }


       


        protected async Task OnPage(MatPaginatorPageEvent e)
        {
            GridConfig.PageSize = e.PageSize;
            GridConfig.PageIndex = e.PageIndex +1;
            GridConfig.ItemList = await GridConfig.GetPageAsync(Sort, GridSearch);
        }

        protected async Task SortData(MatSortChangedEvent sort)
        {
            if (!(sort == null || sort.Direction == MatSortDirection.None || string.IsNullOrEmpty(sort.SortId)))
            {
                Sort = new SortChangeEvent() { SortId = sort.SortId, Direction = (SortDirection)Enum.Parse(typeof(SortDirection), sort.Direction.ToString()) };
                GridConfig.ItemList = await GridConfig.GetPageAsync(Sort, GridSearch);
            }
        }

        protected async Task SortColumn(GridColumnBase thisColumn)
        {
            foreach (GridColumnBase gridColumn in GridConfig.GridColumnBases)
            {
                if(gridColumn == thisColumn)
                {                   
                    if (gridColumn.SortSymbol == "&#8593;")
                    {
                        gridColumn.SortSymbol = "";
                        Sort = null;
                    }
                    else if(gridColumn.SortSymbol =="&#8595;")
                    {
                        gridColumn.SortSymbol ="&#8593;";
                        Sort = new SortChangeEvent() { SortId=gridColumn.PropertyInfo.Name, Direction = SortDirection.Desc};
                    }
                    else
                    {
                        gridColumn.SortSymbol  = "&#8595;"; 
                        //Sort = new SortChangedEvent();
                        Sort = new SortChangeEvent() { SortId = gridColumn.PropertyInfo.Name, Direction = SortDirection.Asc };
                    }                    
                }
                else
                {
                    gridColumn.SortSymbol = "";                    
                }                
            }
            GridConfig.ItemList = await GridConfig.GetPageAsync(Sort, GridSearch);
        }

        protected async Task HandleMouseUp(GridColumnBase gridColumn)
        {
            List<string> options = (await GridConfig.GetSourceListAsync())
                .Select(n => gridColumn.PropertyInfo.GetValue(n, null).ToString())
                .Distinct().Take(11).ToList();

            await GridSearch.CreateGridSearch(gridColumn, options.Count() <= 10 ? options : null);
        }

        protected void HandleCellContextMenu(GridColumnBase gridColumn, object context)
        {
            CellContexMenu = new CellContexMenuOption(gridColumn, context, GridConfig.UrlFolder);
            ShowContextMenu = true;
        }

        protected void HandleTitleContextMenu()
        {            
            ShowTitleContextMenu = true;
            ShowUploadSubMenu = false;
        }


        protected async Task OkClick()
        {
            GridSearch.DialogOpen = false;
            await ConfigureProperties();
        }

        protected async Task CancelClick()
        {
            GridSearch.DialogOpen = false;
            GridSearch.ResetSearchAsync();
            await ConfigureProperties();
        }

        protected async Task KeyHandler(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await OkClick();
            }
        }

        protected void NumberSelectionChanged(ChangeEventArgs args)
        {
            GridSearch.NumberSearchTypeSelected = (NumberSelectionType)Enum.Parse(typeof(NumberSelectionType), args.Value.ToString().Replace(" ",""));
        }


        protected void UploadMenu()
        {            
            ShowUploadSubMenu = true;
        }
     

        protected async Task Test()
        {
            ShowTitleContextMenu = false;
            JsHelper jsHelper = new JsHelper();
            try
            {
                await GridConfig.TestAsync();
                await jsHelper.ShowSucsessAsync(jSRuntime, "It's working!");

            }
            catch (Exception ex)
            {                
                await jsHelper.ShowErrorAsync(jSRuntime, ex.Message);
            }
           

        }
        protected async Task FilesReadyForContent(IMatFileUploadEntry[] files)
        {            
            ErrorMessage = await GridConfig.FilesReadyForContent(files);
            ShowTitleContextMenu = false;
            ShowUploadSubMenu = false;
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                JsHelper jsHelper = new JsHelper();
                await jsHelper.ShowErrorAsync(jSRuntime, ErrorMessage);
                ErrorMessage = "";
            }
          
        }

        protected async Task DownloadExcel()
        {
            ShowTitleContextMenu = false;
            byte[] bytes = await GridConfig.DownloadExcel(await GridConfig.GetListAsync(GridSearch));
            string fileName = string.IsNullOrWhiteSpace(GridConfig.ExcelFileName) 
                ? $"{GridConfig.GridTitle}{DateTime.Now.Date.ToShortDateString().Replace("/", "-")}.xlsx"
                : GridConfig.ExcelFileName;
            JsHelper jsHelper = new JsHelper();
            ErrorMessage = await jsHelper.DownloadExcelAsync(jSRuntime, bytes, fileName);
        }

        protected async Task DownloadSample()
        {
            ShowTitleContextMenu = false;
            ShowUploadSubMenu = false;
            byte[] bytes = await GridConfig.DownloadExcel(new List<object>());
            string fileName = "Sample_" + (string.IsNullOrWhiteSpace(GridConfig.ExcelFileName)
                ? $"{GridConfig.GridTitle}{DateTime.Now.Date.ToShortDateString().Replace("/", "-")}.xlsx"
                : GridConfig.ExcelFileName);

            JsHelper jsHelper = new JsHelper();
            ErrorMessage = await jsHelper.DownloadExcelAsync(jSRuntime, bytes, fileName);
        }

    }
}
