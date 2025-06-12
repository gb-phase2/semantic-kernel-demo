using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelSandbox.Plugins;

public class SchedulingPlugin
{
    [KernelFunction("find_available_time_slot"), Description("Finds an available time slot based on the provided parameters.")]
    public string FindAvailableTimeSlot(
        [Description("The start time of the scheduling period in ISO 8601 format (e.g., '2023-10-01T09:00:00Z').")]
        string startTime,
        [Description("The end time of the scheduling period in ISO 8601 format (e.g., '2023-10-01T17:00:00Z').")]
        string endTime,
        [Description("The duration of the time slot in minutes.")]
        int duration)
    {
        // Logic to find an available time slot
        return $"Available time slot from {startTime} to {endTime} for {duration} minutes.";
    }
    
    [KernelFunction("schedule_work"), Description("Schedules work based on the provided parameters.")]
    public string ScheduleWork(
        [Description("The start time of the work in ISO 8601 format (e.g., '2023-10-01T09:00:00Z').")]
        string startTime,
        [Description("The end time of the work in ISO 8601 format (e.g., '2023-10-01T17:00:00Z').")]
        string endTime,
        [Description("The description of the work to be scheduled.")]
        string description)
    {
        // Logic to schedule work
        return $"Work scheduled from {startTime} to {endTime} with description: {description}.";
    }
}