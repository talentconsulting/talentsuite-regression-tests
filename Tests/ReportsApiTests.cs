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
        public static string reportId;
        private static string riskId;

        public ReportsApiTests(PlaywrightDriver playwrightDriver)
        {
            _playwrightDriver = playwrightDriver;
        }


        [Fact(DisplayName = "postReports - verify 201 response")]
        public async Task postReports()
        {
            var requestBody = new Dictionary<string, object>
        {
            { "clientId", TestData.clientId },
            { "projectId", TestData.projectId },
            { "sowId", TestData.sowId },
            { "completed", "Testing, dev" },
            { "planned", "dev" },
            { "status", "saved" },
            {
                "risks", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "description", "123" },
                        { "mitigation", "report1" },
                        { "status", "Green" }
                    }
                }
            }
        };

            var requestBodyJson = JsonSerializer.Serialize(requestBody);


            var response = await _playwrightDriver.ApiRequestContext?.PostAsync("reports", new APIRequestContextOptions()
            {
                DataObject = requestBodyJson
            });

            response.Status.Should().Be(201);
        }

        [Fact(DisplayName = "postReports - verify 400 response")]
        public async Task postReportsError()
        {
            var requestBody = new Dictionary<string, object>
        {
            { "projectId", TestData.projectId },
            { "sowId", TestData.sowId },
            { "completed", "Testing, dev" },
            { "planned", "dev" },
            { "status", "saved" },
            {
                "risks", new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "description", "123" },
                        { "mitigation", "report1" },
                        { "status", "Green" }
                    }
                }
            }
        };

            var requestBodyJson = JsonSerializer.Serialize(requestBody);


            var response = await _playwrightDriver.ApiRequestContext?.PostAsync("reports", new APIRequestContextOptions()
            {
                DataObject = requestBodyJson
            });

            response.Status.Should().Be(400);
        }

        [Fact(DisplayName = "getReports - verify 200 response")]
        public async Task getReports()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync($"reports?page=1&pageSize=10&projectId={TestData.projectId}");
            var responseBody = await response.JsonAsync();
            response.Status.Should().Be(200);

            var jsonResponse = JsonSerializer.Deserialize<GetReportsResponse>(responseBody.Value.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var items = jsonResponse.reports;
            var pageNumber = jsonResponse.pageInfo.page;

            pageNumber.Should().Be(1);
            items.Count.Should().Be(1);
            items[0].projectId.Should().Be(TestData.projectId);
            jsonResponse.reports.Should().NotBeNull().And.NotBeEmpty();

            reportId = jsonResponse.reports.FirstOrDefault()?.id;
            riskId = jsonResponse.reports.FirstOrDefault()?.risks.FirstOrDefault()?.id;

            reportId.Should().NotBeNullOrEmpty();
            riskId.Should().NotBeNullOrEmpty();

        }

        [Fact(DisplayName = "getReports - verify 400 response")]
        public async Task getReportsError()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync($"reports?page=1&pageSize=10&projectId=abc");
            response.Status.Should().Be(400);

        }

        [Fact(DisplayName = "getReportById - verify 200 response")]
        public async Task getReportById()
        {
            reportId.Should().NotBeNullOrEmpty();
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync($"reports/{reportId}");
            var responseBody = await response.JsonAsync(); ;
            response.Status.Should().Be(200);

            var jsonResponse = JsonSerializer.Deserialize<Report>(responseBody.Value.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var id = jsonResponse.id;
            var plannedTasks = jsonResponse.planned;
            var risks = jsonResponse.risks;

            id.Should().Be(reportId);
            plannedTasks.Should().Be("dev");
            risks.Count.Should().Be(1);
            risks[0].status.Should().Be("Green");
        }

        [Fact(DisplayName = "getReportById - verify 404 response")]
        public async Task getReportByIdError()
        {
            var response = await _playwrightDriver.ApiRequestContext?.GetAsync($"reports/a11abe3e-88ad-4d0e-8739-d5daa8e6fdb5'");
            response.Status.Should().Be(404);

        }

        [Fact(Skip = "putReports - verify 204 response")]
        public async Task putReports()
        {
            reportId.Should().NotBeNullOrEmpty();
            riskId.Should().NotBeNullOrEmpty();
            var requestBody = new Dictionary<string, object>
               {
                   { "id", reportId },
                   { "clientId", TestData.clientId },
                   { "projectId", TestData.projectId },
                   { "sowId", TestData.sowId },
                   { "completed", "Dev" },
                   { "planned", "testing" },
                       {"status", "saved" },
                   {
                       "risks", new List<Dictionary<string, string>>
                       {
                           new Dictionary<string, string>
                           {
                               { "id", riskId },
                               { "description", "test" },
                               { "mitigation", "test api" },
                               { "status", "Amber" }
                           }
                       }
                   }
               };

            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            var response = await _playwrightDriver.ApiRequestContext?.PutAsync("reports", new APIRequestContextOptions()
            {
                DataObject = requestBodyJson
            });

            response.Status.Should().Be(204);
        }

        [Fact(DisplayName = "putReports - verify 404 response")]
        public async Task putReportsError()
        {
            var requestBody = new Dictionary<string, object>
               {
                   { "id", "3fa85f64-5717-4562-b3fc-2c963f66aaa6" },
                   { "clientId", TestData.clientId },
                   { "projectId", TestData.projectId },
                   { "sowId", TestData.sowId },
                   { "completed", "Dev" },
                   { "planned", "testing" },
                       {"status", "saved" },
                   {
                       "risks", new List<Dictionary<string, string>>
                       {
                           new Dictionary<string, string>
                           {
                               { "id", riskId },
                               { "description", "test" },
                               { "mitigation", "test api" },
                               { "status", "Amber" }
                           }
                       }
                   }
               };

            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            var response = await _playwrightDriver.ApiRequestContext?.PutAsync("reports", new APIRequestContextOptions()
            {
                DataObject = requestBodyJson
            });

            response.Status.Should().Be(404);
        }

        [Fact(Skip = "deleteReportById - verify 204 response")]
        public async Task deleteReportById()
        {
            reportId.Should().NotBeNullOrEmpty();
            var response = await _playwrightDriver.ApiRequestContext?.DeleteAsync($"reports/{reportId}");
            var responseBody = await response.JsonAsync(); ;
            response.Status.Should().Be(204);
        }

        [Fact(DisplayName = "deleteReportById - verify 404 response")]
        public async Task deleteReportByIdError()
        {
            var response = await _playwrightDriver.ApiRequestContext?.DeleteAsync($"reports/abc");
            response.Status.Should().Be(404);
        }




        public static class TestData
        {
            public static readonly string clientId = Guid.NewGuid().ToString();
            public static readonly string projectId = Guid.NewGuid().ToString();
            public static readonly string sowId = Guid.NewGuid().ToString();
        }

        public class PageInfo
        {
            public int totalCount { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }
            public int first { get; set; }
            public int last { get; set; }
        }

        public class Risk
        {
            public string id { get; set; }
            public string description { get; set; }
            public string mitigation { get; set; }
            public string status { get; set; }
        }

        public class Report
        {
            public string id { get; set; }
            public string clientId => TestData.clientId; // Use fixed clientId from TestData
            public string projectId => TestData.projectId; // Use fixed projectId from TestData
            public string sowId => TestData.sowId; // Use fixed sowId from TestData
            public string completed { get; set; }
            public string planned { get; set; }
            public List<Risk> risks { get; set; }
            public string status { get; set; }
        }

        public class GetReportsResponse
        {
            public PageInfo pageInfo { get; set; }
            public List<Report> reports { get; set; }
        }

    }
}




