using System.Net;
using Bogus;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Application.UseCases;
using HRManagement.Modules.Personnel.Domain;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace HRManagement.Api.IntegrationTests;

public class EmployeesManagementEndpointsShould : IClassFixture<TestWebApplicationFactory<Program>>
{
    private const string ApiPersonnelManagementEmployees = "/api/personnel-management/employees";
    
    private readonly HttpClient _httpClient;

    public EmployeesManagementEndpointsShould(TestWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task SuccessfullyReturnPagedListOfEmployees()
    {
        const int pageSize = 20;
        var response = await _httpClient.GetAsync($"{ApiPersonnelManagementEmployees}?pageNumber=2&pageSize={pageSize}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<EmployeeDto>>(responseString);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(pageSize);
    }

    [Fact]
    public async Task SuccessfullyReturnSingleEmployee_WhenValidIdProvided()
    {
        var response = await _httpClient.GetAsync($"{ApiPersonnelManagementEmployees}?pageNumber=2&pageSize=10");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<EmployeeDto>>(responseString);
        if (result != null)
        {
            var validEmployeeId = result.First().Id;
            response = await _httpClient.GetAsync($"{ApiPersonnelManagementEmployees}/{validEmployeeId}");
        }

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();
        
        responseString = await response.Content.ReadAsStringAsync();

        var employee = JsonConvert.DeserializeObject<EmployeeDto>(responseString);

        employee.ShouldNotBeNull();
    }

    [Fact]
    public async Task FailToReturnListOfEmployees_WhenPagingParametersNotProvided()
    {
        var response = await _httpClient.GetAsync(ApiPersonnelManagementEmployees);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FailToReturnSingleEmployee_WhenInvalidIdProvided()
    {
        var invalidId = new Faker().Random.Guid();
        var response = await _httpClient.GetAsync($"{ApiPersonnelManagementEmployees}/{invalidId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var responseString = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(responseString);

        error.ShouldNotBeNull();
        error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidId));
    }
}