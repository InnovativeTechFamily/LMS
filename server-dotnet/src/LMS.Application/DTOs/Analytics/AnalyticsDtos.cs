namespace LMS.Application.DTOs.Analytics;

public record MonthData(string Month, long Count);

public record AnalyticsData(List<MonthData> Last12Months);
