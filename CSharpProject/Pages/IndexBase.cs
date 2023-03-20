using ChartJs.Blazor;
using ChartJs.Blazor.PieChart;
using CSharpProject.Models;
using CSharpProject.Services;
using Microsoft.AspNetCore.Components;

namespace CSharpProject.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        public List<EmployeeViewModel> EmployeesViewModel { get; set; } = new List<EmployeeViewModel>();
        internal PieConfig _pieConfig;
        internal Chart chartjs;

        private string[] RandomColorArray = new string[] {"#FF6633", "#FFB399", "#FF33FF", "#FFFF99", "#00B3E6",
                                                            "#E6B333", "#3366E6", "#999966", "#99FF99", "#B34D4D",
                                                            "#80B300", "#809900", "#E6B3B3", "#6680B3", "#66991A",
                                                            "#FF99E6", "#CCFF1A", "#FF1A66", "#E6331A", "#33FFCC",
                                                            "#66994D", "#B366CC", "#4D8000", "#B33300", "#CC80CC",
                                                            "#66664D", "#991AFF", "#E666FF", "#4DB3FF", "#1AB399",
                                                            "#E666B3", "#33991A", "#CC9999", "#B3B31A", "#00E680",
                                                            "#4D8066", "#809980", "#E6FF80", "#1AFF33", "#999933",
                                                            "#FF3380", "#CCCC00", "#66E64D", "#4D80CC", "#9900B3",
                                                            "#E64D66", "#4DB380", "#FF4D4D", "#99E6E6", "#6666FF"};

       
        protected override async Task OnInitializedAsync()
        {
            await LoadEmployeeTable();
            LoadPieChart();
        }

        public async Task LoadEmployeeTable()
        {
            var employeesViewModel = (await EmployeeService.GetEmployees())
                .GroupBy(e => e.EmployeeName)
                .Select(g => new {
                    EmployeeName = g.Key ?? "Unknown",
                    TotalTimeWorked = Math.Round(g.Sum(e => Math.Abs((e.EndTimeUtc - e.StarTimeUtc).TotalHours)))
                })
                .OrderByDescending(e => e.TotalTimeWorked)
                .ToList();

            foreach (var employee in employeesViewModel)
            {
                EmployeesViewModel.Add(new EmployeeViewModel() { EmployeeName = employee.EmployeeName, TotalTimeWorked = (int)employee.TotalTimeWorked });
            }
        }
        public string[] ReturnRandomColors(int numberOfColorsInArray)
        {
            List<string> result = new List<string>();
            while (result.Count < numberOfColorsInArray)
            {
                int randomIndex = new Random().Next(numberOfColorsInArray);
                string randomString = RandomColorArray[randomIndex];
                if (!result.Contains(randomString))
                {
                    result.Add(randomString);
                }
            }
            return result.ToArray();
        }

        public void LoadPieChart()
        {
            _pieConfig = new PieConfig();

            _pieConfig.Options = new PieOptions
            {
                Responsive = true,

            };

            string[] colorsArray = ReturnRandomColors(EmployeesViewModel.Count);
            List<int> totalTimeWorked = new List<int>();
            foreach (var employee in EmployeesViewModel)
            {
                _pieConfig.Data.Labels.Add(employee.EmployeeName);
                totalTimeWorked.Add(employee.TotalTimeWorked);
            }

            _pieConfig.Data.Datasets.Add(new PieDataset<int>(totalTimeWorked)
            {
                BackgroundColor = colorsArray
            });
        }
    }
}
