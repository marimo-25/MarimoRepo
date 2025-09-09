using Microsoft.Azure.Functions.Worker;

namespace Household.Functions.Functions;

public class DailyAggregationFunction
{
    [Function("DailyAggregation")]
    public void Run([TimerTrigger("0 0 3 * * *")] TimerInfo timer) { /* TODO: implement */ }
}