using CSharpProject.Models;
using CSharpProject.Services;
using Microsoft.AspNetCore.Components;

namespace CSharpProject.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        public IEnumerable<Employee> Employees { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Employees = (await EmployeeService.GetEmployees()).ToList();
        }

        public async Task LoadEmployeeTable()
        {
            Employees = (await EmployeeService.GetEmployees())
                .GroupBy(e => e.EmployeeName)
                .Select(g => new {
                        EmployeeName = g.Key ?? "Unknown",
                        TotalTimeWorked = Math.Round(g.Sum(e => Math.Abs(e.EndTimeUtc - e.StarTimeUtc).TotalHours))
                })
                .OrderByDescending(e => e.TotalTimeWorked)
                .ToList();
        }
    }
}
