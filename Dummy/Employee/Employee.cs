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
    public class Employee
    {
        public string id { get; set; }
        public string employee_name { get; set; }
        public string employee_salary { get; set; }
        public string employee_age { get; set; }
        public string profile_image { get; set; }
        public string date { get; set; }
        public string jop_position { get; set; }
    }
    public class RootEmployee
    {
        public string status { get; set; }
        public IEnumerable<Employee> data { get; set; }
    }

    
    public class EmployeeVM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Salary { get; set; }
        public int Age { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "Contract Date")]
        public string Date { get; set; }
        [Display(Name = "Job Position")]
        public string JobPosition { get; set; }
        public EmployeeVM(Employee employee)
        {
            Id = Int32.Parse(employee.id);
            Name = employee.employee_name;
            Salary = employee.employee_salary;
            Age = Int32.Parse(employee.employee_age);
            Date = employee.date;
            JobPosition = employee.jop_position;
        }

        public async static Task<IEnumerable<EmployeeVM>> GetAll()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(@$"{path}\Dummy\Employee\Employee.json");
            }
            
            IEnumerable<EmployeeVM> employees = await Task.Run(()=> JsonConvert.DeserializeObject<RootEmployee>(json).data.Select(x=>new EmployeeVM(x)));
            return employees;
        }
    }


}
