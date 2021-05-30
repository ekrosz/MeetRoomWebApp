using System;
using System.ComponentModel.DataAnnotations;

namespace MeetRoomWebApp.Annotations
{
    /// <summary>
    /// The validation attribute for the DateSession column
    /// </summary>
    public class CustomDateTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value != null)
            {
                DateTime dtValue = Convert.ToDateTime(value);

                if(dtValue.Minute % 30 == 0 && dtValue > DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
