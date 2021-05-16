﻿using MeetRoomWebApp.Models.ViewModels;
using System.Collections.Generic;

namespace MeetRoomWebApp.Models.Interfaces
{
    public interface IUserStorage
    {
        public List<UserViewModel> GetFullList();

        public UserViewModel GetElement(string user);
    }
}
