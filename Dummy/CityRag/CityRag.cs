using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlazorGrid.Dummy
{
    public class CityRag
    {
        public int id { get; set; }
        public string city { get; set; }
        public string amount { get; set; }
        public double ratio { get; set; }
        public int rag { get; set; }
    }

    public class RootCityRag
    {
        public List<CityRag> data { get; set; }
    }

    public class CityRagVM
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public string Amount { get; set; }
        public double Ratio { get; set; }
        [Column(TypeName = "image")]
        public string Rag { get; set; }

        public CityRagVM(CityRag cityR)
        {
            Id = cityR.id;
            City = cityR.city;
            Amount = cityR.amount;
            Ratio = cityR.ratio;
            Rag = cityR.rag == 1 ? "/img/Red.png" : cityR.rag == 2 ? "img/Amber.png" : "../img/Green.png";
        }

        public async static Task<IEnumerable<CityRagVM>> GetAll()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(@$"{path}\Dummy\CityRag\cityRag.json");
            }

            IEnumerable<CityRagVM> cityRagVMs = await Task.Run(() => JsonConvert.DeserializeObject<RootCityRag>(json).data.Select(x => new CityRagVM(x)));
            return cityRagVMs;
        }
    }
}
