using Microsoft.Playwright.NUnit;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TaskManager.Playwright
{
    [TestFixture]
    public class TaskUITests : PageTest
    {
        private const string BaseUrl = "https://localhost:56115";

        [Test]
        public async Task CreateTask_WithValidData_ShouldAppearInTaskList()
        {
            await Page.GotoAsync($"{BaseUrl}/Tasks/Create");

            await Page.FillAsync("input[name='Title']", "Playwright Test Task");
            await Page.FillAsync("input[name='AssignedTo']", "QA Engineer");

            var tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            await Page.FillAsync("input[name='DueDate']", tomorrow);

            await Page.SelectOptionAsync("select[name='Status']", "New");
            
            await Page.ClickAsync("button[type='submit']");
            await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);

            var pageContent = await Page.ContentAsync();
            Assert.That(pageContent, Does.Contain("Playwright Test Task"));
            Assert.That(pageContent, Does.Contain("Task created successfully."));
        }

        [Test]
        public async Task CreateTask_WithEmptyTitle_ShouldShowValidationError()
        {
            await Page.GotoAsync($"{BaseUrl}/Tasks/Create");

            await Page.FillAsync("input[name='AssignedTo']", "Someone");

            var tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            await Page.FillAsync("input[name='DueDate']", tomorrow);

            await Page.ClickAsync("button[type='submit']");

            var errorText = await Page.TextContentAsync("span[data-valmsg-for='Title']");
            Assert.That(errorText, Is.Not.Null.And.Not.Empty);
            Assert.That(errorText, Does.Contain("required").IgnoreCase);
        }

        [Test]
        public async Task TaskList_ShouldDisplayTableWithHeaders()
        {
            await Page.GotoAsync($"{BaseUrl}/Tasks");

            var title = await Page.IsVisibleAsync("th:has-text('Title')");
            var assigned = await Page.IsVisibleAsync("th:has-text('Assigned To')");
            var dueDate = await Page.IsVisibleAsync("th:has-text('Due Date')");
            var status = await Page.IsVisibleAsync("th:has-text('Status')");

            Assert.Multiple(() =>
            {
                Assert.That(title, Is.True);
                Assert.That(assigned, Is.True);
                Assert.That(dueDate, Is.True);
                Assert.That(status, Is.True);
            });
        }
    }
}
