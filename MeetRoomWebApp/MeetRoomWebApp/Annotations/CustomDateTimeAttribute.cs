using System;
using System.ComponentModel.DataAnnotations;

namespace MeetRoomWebApp.Annotations
{
    /// <summary>
    /// The validation attribute for the DateSession column
    /// </summary>
    public class CustomDateTimeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Checking the correctness of the entered DateSession
        /// </summary>
        /// <param name="value">[DateTime] DateSession</param>
        /// <returns>True or false</returns>
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
