using System.Net;
using HRManagement.BuildingBlocks.Models;
using HRManagement.Modules.Staff;
using HRManagement.Modules.Staff.Features.Roles.Get;
using HRManagement.Modules.Staff.Models;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace HRManagement.Api.IntegrationTests;

public class RolesManagementApiShould : IClassFixture<TestWebApplicationFactory>
{
    private const string ApiEndpoint = "/api/v1/roles";
    private readonly HttpClient _httpClient;

    public RolesManagementApiShould(TestWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact(DisplayName = "Successfully return paged list of roles, when matches found")]
    public async Task Get_Success()
    {
        const int pageSize = 5;
        var response = await _httpClient.GetAsync($"{ApiEndpoint}?pageSize={pageSize}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<RoleDto>>(responseString);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(pageSize);
    }

    [Fact(DisplayName = "Successfully return single role, when ID provided is valid and a match is found")]
    public async Task Get_Success_WhenValidRoleIDProvided()
    {
        var response = await _httpClient.GetAsync($"{ApiEndpoint}/1");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var employee = JsonConvert.DeserializeObject<RoleDto>(responseString);

        employee.ShouldNotBeNull();
    }

    [Fact(DisplayName = "Fail to return single role, when no match found")]
    public async Task Get_InvalidIdProvided_Failure()
    {
        const byte invalidId = byte.MaxValue;
        var response = await _httpClient.GetAsync($"{ApiEndpoint}/{invalidId}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var responseString = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(responseString);

        error.ShouldNotBeNull();
        error.ShouldBeEquivalentTo(DomainErrors.NotFound(nameof(Role), invalidId));
    }
}