using MeetRoomWebApp.Models.ViewModels;
using MeetRoomWebApp.Models.BindingModels;
using System.Collections.Generic;
using System;

namespace MeetRoomWebApp.Models.Interfaces
{
    /// <summary>
    /// Interface with methods of interaction with the database of the "Session" model
    /// </summary>
    public interface ISessionStorage
    {
        /// <summary>
        /// Full list of sessions
        /// </summary>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFullList();

        /// <summary>
        /// List of sessions for the specified week
        /// </summary>
        /// <param name="dateFrom">Date of the first day of the week</param>
        /// <param name="dateTo">Date of the last day of the week</param>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFilteredListByWeek(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// List of sessions the specified user is in
        /// </summary>
        /// <param name="user">User's email</param>
        /// <returns>Session list</returns>
        public List<SessionViewModel> GetFilteredListByUser(string user);

        /// <summary>
        /// Search for a session by ID
        /// </summary>
        /// <param name="model">Model with ID</param>
        /// <returns>Session</returns>
        public SessionViewModel GetElement(SessionBindingModel model);

        /// <summary>
        /// Adding a new session to the database
        /// </summary>
        /// <param name="model">New session model</param>
        public void Insert(SessionBindingModel model);

        /// <summary>
        /// Updating an existing session in the database
        /// </summary>
        /// <param name="model">New session model</param>
        public void Update(SessionBindingModel model);

        /// <summary>
        /// Removing the specified session from the database by identifier
        /// </summary>
        /// <param name="model">Session model with identifier</param>
        public void Delete(SessionBindingModel model);
    }
}
