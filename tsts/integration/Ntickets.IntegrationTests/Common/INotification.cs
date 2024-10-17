namespace Ntickets.IntegrationTests.Common;

internal class Notification
{
    public Notification(string code, string message, string type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
}
