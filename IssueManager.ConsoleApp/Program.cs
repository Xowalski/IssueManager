using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IssueManager.WebAPI.Controllers;
using IssueManager.BLL.Models;
using Microsoft.Extensions.Options;
using System;

namespace IssueManager.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var controller = serviceProvider.GetService<IssuesController>();
            RunInteractiveConsole(serviceProvider, controller);
        }

        public static IServiceProvider ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.Configure<PlatformSettings>(configuration.GetSection("PlatformSettings"));
            services.AddHttpClient();
            services.AddTransient<IssuesController>();
            services.AddSingleton(configuration);
            return services.BuildServiceProvider();
        }

        public static void RunInteractiveConsole(IServiceProvider serviceProvider, IssuesController controller)
        {
            var platformSettings = serviceProvider.GetService<IOptions<PlatformSettings>>().Value;

            if (platformSettings.GitHub.Token == "your_token" || platformSettings.GitLab.Token == "your_token")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have to set your token in appsettings.json. Press any key to escape.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Which hosting service you want to use?");
            Console.WriteLine("Press 'H' for GitHub, 'L' for GitLab and confirm with Enter");
            string platform = Console.ReadLine().ToLower();

            if (platform == "h")
                platform = "github";
            else if (platform == "l")
                platform = "gitlab";
            else
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.WriteLine("What is the owner name (group/user)?");
            string owner = Console.ReadLine();

            Console.WriteLine("What is the repository name?");
            string repository = Console.ReadLine();

            Console.WriteLine("Choose an action:");
            Console.WriteLine("Press 'A' to add a new issue, 'E' to edit an issue, 'C' to close an issue  and confirm with Enter.");
            string action = Console.ReadLine().ToLower();

            if (action == "a")
            {
                Console.WriteLine("Enter the issue title:");
                string title = Console.ReadLine();
                Console.WriteLine("Enter the issue description:");
                string description = Console.ReadLine();

                var issueRequest = new IssueRequest { Title = title, Description = description };
                controller.AddIssue(platform, owner, repository, issueRequest).Wait();
                Console.WriteLine("Issue added successfully.");
            }
            else if (action == "e")
            {
                Console.WriteLine("Enter the issue ID to edit:");
                int issueId = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter the new title:");
                string newTitle = Console.ReadLine();
                Console.WriteLine("Enter the new description:");
                string newDescription = Console.ReadLine();

                var issueRequest = new IssueRequest { Title = newTitle, Description = newDescription };
                controller.EditIssue(platform, owner, repository, issueId, issueRequest).Wait();
                Console.WriteLine("Issue edited successfully.");
            }
            else if (action == "c")
            {
                Console.WriteLine("Enter the issue ID to close:");
                int issueId = int.Parse(Console.ReadLine());

                controller.CloseIssue(platform, owner, repository, issueId).Wait();
                Console.WriteLine("Issue closed successfully.");
            }
            else
            {
                Console.WriteLine("Invalid action.");
            }
        }
    }
}