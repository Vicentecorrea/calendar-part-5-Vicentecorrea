﻿using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CalendarApp.Controllers
{
    public static class AppointmentController
    {
        #region Fields
        private static List<Appointment> appointments = new List<Appointment>();
        #endregion

        #region Properties
        /// <summary>Public property for accessing the appointments.</summary>
        public static List<Appointment> Appointments
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
        public static void SaveAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
            SerializeAppointments();
        }

        public static void DeleteAppointment(Appointment appointment)
        {
            Appointments.Remove(appointment);
            SerializeAppointments();
        }

        private static void SerializeAppointments()
        {
            Stream stream = File.Open(Constants.PathToAppointmentsSerializationFile, FileMode.OpenOrCreate);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, Appointments);
            stream.Close();
        }

        public static void LoadAppointments()
        {
            Stream stream = File.Open(Constants.PathToAppointmentsSerializationFile, FileMode.OpenOrCreate);
            if (stream.Length > Constants.ZeroItemsInList)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Appointments = (List<Appointment>)binaryFormatter.Deserialize(stream);
            }
            stream.Close();
        }

        public static bool IsAppointmentInThisDay(Appointment appointment, DateTime day)
        {
            bool IsAppointmentInThisDay = appointment.StartDate.Date <= day && day <= appointment.EndDate.Date;
            return IsAppointmentInThisDay;
        }

        public static List<Appointment> GetAppointmentsInThisDay(DateTime day)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where IsAppointmentInThisDay(appointment, day) && LoggedUserCanSeeThisAppointment(appointment)
                                                    select appointment;
            List<Appointment> appointmentsInThisDay = new List<Appointment>(appointments);
            return appointmentsInThisDay;
        }

        public static bool IsAppointmentInThisDayAndTime(Appointment appointment, DateTime time)
        {
            DateTime previousHour = appointment.StartDate.AddHours(Constants.PreviousTimeInterval);
            bool IsAppointmentInThisDay = previousHour < time && time< appointment.EndDate;
            return IsAppointmentInThisDay;
        }

        public static List<Appointment> GetAppointmentsInThisDayAndTime(DateTime time)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where IsAppointmentInThisDayAndTime(appointment, time) && LoggedUserCanSeeThisAppointment(appointment)
                                                    select appointment;
            List<Appointment> appointmentsInThisDayAndTime = new List<Appointment>(appointments);
            return appointmentsInThisDayAndTime;
        }

        public static bool LoggedUserCanSeeThisAppointment(Appointment appointment)
        {
            bool appointmentOwnerIsLoggedUser = appointment.OwnerUsername.Equals(UserController.LoggedUsername);
            bool loggedUserIsInvitedToThisAppointment = appointment.GuestUsernames.Contains(UserController.LoggedUsername);
            bool loggedUserCanSeeThisAppointment = appointmentOwnerIsLoggedUser || loggedUserIsInvitedToThisAppointment;
            return loggedUserCanSeeThisAppointment;
        }

        public static string GetErrorFeedbackTextCreatingAppointmentWithWrongValues(bool appointmentHasTitle, bool appointmentHasDescription, bool appointmentEndDateIsLaterThanStartDate)
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

        public static string GetErrorFeedbackTextCreatingAppointmentWithWrongGuests(List<string> usernamesThatCannotBeInvitedToAppointment)
        {
            string feedbackText = string.Format("The following users cannot be invited to your appointment because they have a time collision with another appointment:{0}", Environment.NewLine);
            foreach (string username in usernamesThatCannotBeInvitedToAppointment)
            {
                feedbackText = string.Format("{0}- {1}{2}", feedbackText, username, Environment.NewLine);
            }
            return feedbackText;
        }

        private static List<Appointment> GetUsernameAppointments(string username)
        {
            IEnumerable<Appointment> appointments = from appointment in Appointments
                                                    where appointment.OwnerUsername.Equals(username) || appointment.GuestUsernames.Contains(username)
                                                    select appointment;
            List<Appointment> usernameAppointments = new List<Appointment>(appointments);
            return usernameAppointments;
        }

        private static bool CanTheUserBeInvitedToAppointment(string username, Appointment temporaryAppointment)
        {
            List<Appointment> appointmentsOfUsernameToMove = GetUsernameAppointments(username);
            foreach (Appointment appointment in appointmentsOfUsernameToMove)
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

        public static List<string> GetUsernamesThatCannotBeInvitedToAppointment(List<string> possibleGuestUsernames, Appointment temporaryAppointment)
        {
            List<string> usernamesThatCannotBeInvitedToAppointment = new List<string>();
            foreach (string possibleUsernameGuest in possibleGuestUsernames)
            {
                if (!CanTheUserBeInvitedToAppointment(possibleUsernameGuest, temporaryAppointment))
                {
                    usernamesThatCannotBeInvitedToAppointment.Add(possibleUsernameGuest);
                }
            }
            return usernamesThatCannotBeInvitedToAppointment;
        }
        #endregion
    }
}
