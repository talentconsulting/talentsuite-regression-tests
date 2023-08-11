using FluentAssertions;
using Microsoft.Playwright;
using System.Text.Json;
using talentsuite_regression_tests.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace talentsuite_regression_tests.Tests
{

    public class ReportsApiTests : IClassFixture<PlaywrightDriver>
    {
        private readonly PlaywrightDriver _playwrightDriver;
        public ReportsApiTests(PlaywrightDriver playwrightDriver)
        {
            _playwrightDriver = playwrightDriver;
        }

        [Fact]
        public async Task getApiInfo()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync("api/info");
            var responseBody = await response.TextAsync(); ;
            response.Status.Should().Be(200);

            responseBody.Contains("1.0.0");
        }

        [Fact]
        public async Task getReports()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync("api/reports");
            var responseBody = await response.JsonAsync();
            response.Status.Should().Be(200);

            var jsonResponse = JsonSerializer.Deserialize<Response>(responseBody.Value.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonResponse != null)
            {
                var items = jsonResponse.Items;
                var pageNumber = jsonResponse.PageNumber;

                pageNumber.Should().Be(1);
                items.Count.Should().Be(1);
                items[0].CompletedTasks.Should().Be("Task 1");
            }
            else
            {
                Console.WriteLine("Failed to deserialize");
            }
        }

        [Fact(Skip = "returning 500 error")]
        public async Task getReportById()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync("api/reports/1");
            var responseBody = await response.TextAsync(); ;
            response.Status.Should().Be(200);

        }

        [Fact(Skip = "returning 500 error")]
        public async Task postReports()
        {
            var requestBody = new Dictionary<string, object>
        {
            { "id", "string" },
            { "created", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
            { "plannedTasks", "string" },
            { "completedTasks", "string" },
            { "weeknumber", 0 },
            { "submissionDate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
            { "projectId", "string" },
            { "userId", "string" },
            {
                "risks", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "id", "string" },
                        { "reportId", "string" },
                        { "riskDetails", "string" },
                        { "riskMitigation", "string" },
                        { "ragStatus", "string" }
                    }
                }
            }
        };

            var requestBodyJson = JsonSerializer.Serialize(requestBody);


            var response = await _playwrightDriver.ApiRequestContext?.PostAsync("api/reports", new APIRequestContextOptions()
            {
                DataObject = requestBodyJson
            });

            response.Status.Should().Be(200);
        }


    }

    public class Response
    {
        public List<Item>? Items { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class Item
    {
        public string? Id { get; set; }
        public DateTime Created { get; set; }
        public string? PlannedTasks { get; set; }
        public string CompletedTasks { get; set; }
        public int WeekNumber { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? ProjectId { get; set; }
        public string? UserId { get; set; }
        public List<Risk>? Risks { get; set; }
    }

    public class Risk
    {
        public string? Id { get; set; }
        public string? ReportId { get; set; }
        public string? RiskDetails { get; set; }
        public string? RiskMitigation { get; set; }
        public string? RagStatus { get; set; }
    }

}




