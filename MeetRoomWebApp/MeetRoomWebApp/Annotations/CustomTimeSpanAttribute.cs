using System;
using System.ComponentModel.DataAnnotations;

namespace MeetRoomWebApp.Annotations
{
    /// <summary>
    /// The validation attribute for the SessionDuration column
    /// </summary>
    public class CustomTimeSpanAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                TimeSpan dtValue = (TimeSpan)value;

                if (dtValue.TotalMinutes % 30 == 0 && dtValue.TotalMinutes >= 30)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
