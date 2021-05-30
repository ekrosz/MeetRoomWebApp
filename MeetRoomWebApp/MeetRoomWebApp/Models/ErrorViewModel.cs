namespace MeetRoomWebApp.Models
{
    /// <summary>
    /// The model used to display error information in an application
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
