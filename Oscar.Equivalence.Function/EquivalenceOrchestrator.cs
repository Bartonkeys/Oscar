using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using BartonKeys.Functional;

namespace Oscar.Equivalence.Function
{
    public static class EquivalenceOrchestrator
    {
        [FunctionName("EquivalenceOrchestrator")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var message = context.GetInput<string>();
            var result = await context.CallActivityAsync<string>("ProcessEquivalenceActivity", message);
            return result;
        }
    }
}
