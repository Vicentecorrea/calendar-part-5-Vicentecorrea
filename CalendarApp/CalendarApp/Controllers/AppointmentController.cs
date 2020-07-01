using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CalendarApp.Controllers
{
    public class AppointmentController
    {
        #region Fields
        static private List<Appointment> appointments = new List<Appointment>();
        #endregion

        #region Properties
        /// <summary>Public property for accessing the appointments.</summary>
        public List<Appointment> Appointments
        {
            get
            {
                return appointments;
            }
            set
            {
                appointments = value;
            }
        }
        #endregion

        #region Methods
        public void SaveAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
            SerializeAppointments();
        }

        public void DeleteAppointment(Appointment appointment)
        {
            Appointments.Remove(appointment);
            SerializeAppointments();
        }

        public void LoadAppointments()
        {
            Stream stream = null;
            try
            {
                stream = File.Open(Constants.PathToAppointmentsSerializationFile, FileMode.OpenOrCreate);
                if (stream.Length > Constants.ZeroItemsInList)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Appointments = (List<Appointment>)binaryFormatter.Deserialize(stream);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public List<Appointment> GetAppointmentsInThisDay(DateTime day)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where IsAppointmentInThisDay(appointment, day) && LoggedUserCanSeeThisAppointment(appointment)
                                                    select appointment;
            List<Appointment> appointmentsInThisDay = new List<Appointment>(appointments);
            return appointmentsInThisDay;
        }

        public bool IsAppointmentInThisDay(Appointment appointment, DateTime day)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }
            bool isAppointmentInThisDay = appointment.StartDate.Date <= day && day <= appointment.EndDate.Date;
            return isAppointmentInThisDay;
        }

        public List<Appointment> GetAppointmentsInThisDayAndTime(DateTime time)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where IsAppointmentInThisDayAndTime(appointment, time) && LoggedUserCanSeeThisAppointment(appointment)
                                                    select appointment;
            List<Appointment> appointmentsInThisDayAndTime = new List<Appointment>(appointments);
            return appointmentsInThisDayAndTime;
        }

        public bool IsAppointmentInThisDayAndTime(Appointment appointment, DateTime time)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }
            DateTime previousHour = appointment.StartDate.AddHours(Constants.PreviousTimeInterval);
            bool isAppointmentInThisDay = previousHour < time && time < appointment.EndDate;
            return isAppointmentInThisDay;
        }

        public bool LoggedUserCanSeeThisAppointment(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }
            bool appointmentOwnerIsLoggedUser = appointment.OwnerUserName.Equals(UserController.LoggedUserName);
            bool loggedUserIsInvitedToThisAppointment = appointment.GuestUserNames.Contains(UserController.LoggedUserName);
            bool loggedUserCanSeeThisAppointment = appointmentOwnerIsLoggedUser || loggedUserIsInvitedToThisAppointment;
            return loggedUserCanSeeThisAppointment;
        }

        public string GetErrorFeedbackTextCreatingAppointmentWithWrongValues(bool appointmentHasTitle, bool appointmentHasDescription, bool appointmentEndDateIsLaterThanStartDate)
        {
            string feedbackText = "";
            if (!appointmentHasTitle)
            {
                feedbackText = string.Format("{0}The appointment must have a title{1}", feedbackText, Environment.NewLine);

            }
            if (!appointmentHasDescription)
            {
                feedbackText = string.Format("{0}The appointment must have a description{1}", feedbackText, Environment.NewLine);
            }
            if (!appointmentEndDateIsLaterThanStartDate)
            {
                feedbackText = string.Format("{0}The end date must be later than the start date", feedbackText);
            }
            return feedbackText;
        }

        public string GetErrorFeedbackTextCreatingAppointmentWithWrongGuests(List<string> userNamesThatCannotBeInvitedToAppointment)
        {
            if (userNamesThatCannotBeInvitedToAppointment == null)
            {
                throw new ArgumentNullException("userNamesThatCannotBeInvitedToAppointment");
            }
            string feedbackText = string.Format("The following users cannot be invited to your appointment because they have a time collision with another appointment:{0}", Environment.NewLine);
            foreach (string userName in userNamesThatCannotBeInvitedToAppointment)
            {
                feedbackText = string.Format("{0}- {1}{2}", feedbackText, userName, Environment.NewLine);
            }
            return feedbackText;
        }

        public List<string> GetUserNamesThatCannotBeInvitedToAppointment(List<string> possibleGuestUserNames, Appointment temporaryAppointment)
        {
            if (possibleGuestUserNames == null)
            {
                throw new ArgumentNullException("possibleGuestUserNames");
            }
            if (temporaryAppointment == null)
            {
                throw new ArgumentNullException("temporaryAppointment");
            }
            List<string> userNamesThatCannotBeInvitedToAppointment = new List<string>();
            foreach (string possibleUserNameGuest in possibleGuestUserNames)
            {
                if (!CanTheUserBeInvitedToAppointment(possibleUserNameGuest, temporaryAppointment))
                {
                    userNamesThatCannotBeInvitedToAppointment.Add(possibleUserNameGuest);
                }
            }
            return userNamesThatCannotBeInvitedToAppointment;
        }

        public bool CanTheUserBeInvitedToAppointment(string userName, Appointment temporaryAppointment)
        {
            if (temporaryAppointment == null)
            {
                throw new ArgumentNullException("temporaryAppointment");
            }
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            List<Appointment> appointmentsOfUserNameToMove = GetUserNameAppointments(userName);
            foreach (Appointment appointment in appointmentsOfUserNameToMove)
            {
                bool temporaryStartDateCollidesWithAppointment = temporaryAppointment.StartDate < appointment.StartDate && appointment.StartDate < temporaryAppointment.EndDate;
                bool temporaryEndDateCollidesWithAppointment = temporaryAppointment.StartDate < appointment.EndDate && appointment.EndDate < temporaryAppointment.EndDate;
                bool bothTemporaryDatesCollidesWithAppointment = temporaryAppointment.StartDate > appointment.StartDate && temporaryAppointment.EndDate < appointment.EndDate;
                bool isCollisionBetweenAppointments = temporaryStartDateCollidesWithAppointment || temporaryEndDateCollidesWithAppointment || bothTemporaryDatesCollidesWithAppointment;
                if (isCollisionBetweenAppointments)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Appointment> GetUserNameAppointments(string userName)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where appointment.OwnerUserName.Equals(userName) || appointment.GuestUserNames.Contains(userName)
                                                    select appointment;
            List<Appointment> userNameAppointments = new List<Appointment>(appointments);
            return userNameAppointments;
        }

        private void SerializeAppointments()
        {
            Stream stream = null;
            try
            {
                stream = File.Open(Constants.PathToAppointmentsSerializationFile, FileMode.OpenOrCreate);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, Appointments);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        #endregion
    }
}
