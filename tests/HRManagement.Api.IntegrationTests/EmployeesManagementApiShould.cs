﻿using System.Net;
using Bogus;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Features.Employees.Get;
using HRManagement.Modules.Staff.Models;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace HRManagement.Api.IntegrationTests;

public class EmployeesManagementApiShould : IClassFixture<TestWebApplicationFactory>
{
    private const string ApiEndpoint = "/api/v1/employees";

    private readonly HttpClient _httpClient;

    public EmployeesManagementApiShould(TestWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact(DisplayName = "Successfully return paged list of employees, when matches found.")]
    public async Task Get_Success()
    {
        const int pageSize = 20;
        var response = await _httpClient.GetAsync($"{ApiEndpoint}?pageNumber=2&pageSize={pageSize}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<EmployeeDto>>(responseString);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(pageSize);
    }

    [Fact(DisplayName = "Successfully return single employee, when ID provided is valid and a match is found")]
    public async Task Get_ValidIdProvided_Success()
    {
        var response = await _httpClient.GetAsync($"{ApiEndpoint}?pageNumber=2&pageSize=10");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<EmployeeDto>>(responseString);
        if (result != null)
        {
            var validEmployeeId = result.First().Id;
            response = await _httpClient.GetAsync($"{ApiEndpoint}/{validEmployeeId}");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.EnsureSuccessStatusCode();

            responseString = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<EmployeeDto>(responseString);

            employee.ShouldNotBeNull();
        }
    }

    [Fact(DisplayName = "Fail to return single employee, when ID provided is not valid")]
    public async Task Get_InvalidIdProvided_Failure()
    {
        var invalidId = new Faker().Random.Guid();
        var response = await _httpClient.GetAsync($"{ApiEndpoint}/{invalidId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var responseString = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(responseString);

        error.ShouldNotBeNull();
        error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidId));
    }

    [Fact(DisplayName = "Fail to return single employee, when no match found")]
    public async Task Get_NoMatchFound_Empty()
    {
        var invalidId = new Faker().Random.Guid();
        var response = await _httpClient.GetAsync($"{ApiEndpoint}/{invalidId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var responseString = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(responseString);

        error.ShouldNotBeNull();
        error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Employee), invalidId));
    }
}