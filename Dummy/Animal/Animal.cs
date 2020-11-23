using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlazorGrid.Dummy.Animal
{
    public class AnimalJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sci_name { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string date { get; set; }
        public int number { get; set; }
    }

    public class RootAnimal
    {
        public List<AnimalJson> data { get; set; }
    }

    public class Animal
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Scientific Name")]
        public string SciName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "Last Found Date")]
        public string Date { get; set; }
        public int ANumber { get; set; }

        public Animal()
        {

        }

        public Animal(AnimalJson animal)
        {
            Id = animal.id;
            Name = animal.name;
            SciName = animal.sci_name;
            Address = animal.address;
            Country = animal.country;
            Date = ConvertDateTime(animal.date);
            ANumber = animal.number;
        }

        public async static Task<IEnumerable<Animal>> GetAll()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(@$"{path}\Dummy\Animal\animal.json");
            }

            IEnumerable<Animal> animals = await Task.Run(() => JsonConvert.DeserializeObject<RootAnimal>(json).data.Select(x => new Animal(x)));

            return animals;
        }

        private string ConvertDateTime(string animalDate)
        {
            System.Globalization.CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
            DateTime date = DateTime.Parse(animalDate);
            System.Globalization.CultureInfo.CurrentCulture = new CultureInfo("es-ES", false);
            return date.Date.ToShortDateString();
            
        }
    }
}
