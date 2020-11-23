using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;

namespace BlazorGrid.GridClasses
{
    public  class JsHelper
    {
        [Inject]
        private SweetAlertService Swal { get; set; }      

      
        public async ValueTask<bool> ConfirmAsync(IJSRuntime jsRuntime, string message)
        {
            return await jsRuntime.InvokeAsync<bool>("confirm", message);
        }

        public async ValueTask<bool> AlertAsync(IJSRuntime jsRuntime, string message)
        {           
                return await jsRuntime.InvokeAsync<bool>("alert", message);          
        }

        private async Task<SweetAlertResult> SweetPopUptAsync(IJSRuntime jsRuntime, SweetAlertOptions sweetAlert)
        {
            try
            {                
                Swal = new SweetAlertService(jsRuntime);
                SweetAlertResult result = await Swal.FireAsync(sweetAlert);
                return result;
            }
            catch (Exception ex)
            {
                return new SweetAlertResult() { Value = $"SweetError: {ex.Message}"};
            }
        }

        public async Task<bool> ShowErrorAsync(IJSRuntime jsRuntime, string message)
        {
            SweetAlertOptions alertOptions = CreateErrorSweetAlert(message);
            SweetAlertResult result = await SweetPopUptAsync(jsRuntime, alertOptions);
            return result.IsConfirmed;
        }

        public async Task<bool> ShowSucsessAsync(IJSRuntime jsRuntime, string message)
        {
            SweetAlertOptions alertOptions = CreateSuccessSweetAlert(message);
            SweetAlertResult result = await SweetPopUptAsync(jsRuntime, alertOptions);
            return result.IsConfirmed;
        }

        public async ValueTask<string> DownloadExcelAsync(IJSRuntime jsRuntime, byte[] bytes, string fileName)
        {
            try
            {
                fileName = fileName.Contains(".xlsx") ? fileName : $"{fileName}.xlsx";

                await jsRuntime.InvokeVoidAsync("downloadFromByteArray"
                    , new { ByteArray = bytes, FileName = fileName, ContentType = "application /x-msdownload" });

                return "";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        private SweetAlertOptions CreateErrorSweetAlert(string message)
        {
            SweetAlertOptions sao = new SweetAlertOptions();
            sao.Title = "Error";
            sao.Text = message;
            sao.Icon = SweetAlertIcon.Error;            
            return sao;
        }

        private SweetAlertOptions CreateSuccessSweetAlert(string message)
        {
            SweetAlertOptions sao = new SweetAlertOptions();
            sao.Title = "Success";
            sao.Text = message;
            sao.Icon = SweetAlertIcon.Success;
            return sao;
        }
    }
}
