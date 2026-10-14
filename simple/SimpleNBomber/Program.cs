using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

namespace SimpleNBomber;

internal class Program {
    static void Main(string[] args) {
        Thread.Sleep(1000);

        var httpClient = Http.CreateDefaultClient();

        var scenario1 = Scenario.Create("scenario1", async context => {
            var request =
               Http.CreateRequest("GET", "http://localhost:5004/todos/")
                   .WithHeader("Content-Type", "application/json");
            // .WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

            var response = await Http.Send(httpClient, request);

            return response;
        })
        .WithLoadSimulations(
            Simulation.Inject(
                rate: 1000,
                interval: TimeSpan.FromSeconds(1),
                during: TimeSpan.FromSeconds(10)
            )
        );

        var scenario2 = Scenario.Create("scenario2", async context =>
        {
            var request =
               Http.CreateRequest("GET", "http://localhost:5004/todos/")
                   .WithHeader("Content-Type", "application/json");
            // .WithBody(new StringContent("{ some JSON }", Encoding.UTF8, "application/json"));

            var response = await Http.Send(httpClient, request);

            return response;
        })
        .WithLoadSimulations(

            // ramp up the injection rate from 0 to 50 copies
            // injection interval: 1 second
            // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
            Simulation.RampingInject(
                rate: 100,
                interval: TimeSpan.FromSeconds(1),
                during: TimeSpan.FromSeconds(5)),

            // keeps injecting 50 copies per 1 second
            // injection interval: 1 second
            // duration: 30 seconds (it executes from [00:00:30] to [00:01:00])
            Simulation.Inject(
                rate: 30*1000,
                interval: TimeSpan.FromSeconds(30),
                during: TimeSpan.FromSeconds(30)),

            // ramp down the injection rate from 50 to 0 copies
            // injection interval: 1 second
            // duration: 30 seconds (it executes from [00:01:00] to [00:01:30])
            Simulation.RampingInject(
                rate: 0,
                interval: TimeSpan.FromSeconds(1),
                during: TimeSpan.FromSeconds(5))
        );

        NBomberRunner
            .RegisterScenarios(scenario1, scenario2)
            .Run();
    }
}
