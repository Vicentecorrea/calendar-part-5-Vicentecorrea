using CalendarApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp
{
    public class Calendar
    {
        #region Fields
        private DateTime selectedDate;
        private int iteratorDayInMonth;
        private DateTime iteratorDateInWeek;
        #endregion

        #region Properties
        /// <summary>Public property for accessing the title selectedDate field.</summary>
        public DateTime SelectedDate 
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
            }
        }

        /// <summary>Public property for accessing the title iteratorDayInMonth field.</summary>
        public int IteratorDayInMonth
        {
            get
            {
                return iteratorDayInMonth;
            }
            set
            {
                iteratorDayInMonth = value;
            }
        }

        /// <summary>Public property for accessing the title iteratorDateInWeek field.</summary>
        public DateTime IteratorDateInWeek
        {
            get
            {
                return iteratorDateInWeek;
            }
            set
            {
                iteratorDateInWeek = value;
            }
        }

        public int DaysBetweenMondayAndFirstDayOfSelectedMonth
        {
            get
            {
                DateTime firstDateOfMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, Constants.DefaultFirstDay);
                int firstDayOfMonth;
                if (firstDateOfMonth.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    firstDayOfMonth = Constants.DaysInWeek;
                }
                else
                {
                    firstDayOfMonth = (int)firstDateOfMonth.DayOfWeek;
                }
                return firstDayOfMonth - Constants.GapBetweenIndexAndNumber;
            }
        }
        #endregion

        #region Methods
        public DateTime GetMondayOfWeek(DateTime date)
        {
            DateTime iteratorDayOfWeek = date;
            while (iteratorDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                iteratorDayOfWeek = iteratorDayOfWeek.AddDays(Constants.PreviousTimeInterval);
            }
            return iteratorDayOfWeek;
        }

        public string GetCellTextInMonthView(List<Appointment> appointmentsInThisDay, DateTime day)
        {
            if (appointmentsInThisDay == null)
            {
                throw new ArgumentNullException("appointmentsInThisDay");
            }
            string cellText = IteratorDayInMonth.ToString();
            foreach (Appointment appointment in appointmentsInThisDay)
            {
                cellText = string.Format("{0}{1}", cellText, Environment.NewLine);
                if (appointment.StartDate.Date == day.Date)
                {
                    cellText = string.Format("{0}{1}   ", cellText, appointment.StartDate.ToString(Constants.HourAndMinuteFormat));
                }
                cellText = string.Format("{0}{1}", cellText, appointment.Title);
            }
            return cellText;
        }

        public string GetCellTextInWeekView(List<Appointment> appointmentsInThisDayAtThisHour, int hour)
        {
            if (appointmentsInThisDayAtThisHour == null)
            {
                throw new ArgumentNullException("appointmentsInThisDayAtThisHour");
            }
            string cellText = Constants.Empty;
            foreach (Appointment appointment in appointmentsInThisDayAtThisHour)
            {
                bool isAppointmentStartsAtThisHour = appointment.StartDate.Hour == hour;
                bool isAppointmentTheSameDayThatIteratorDate = appointment.StartDate.Day == IteratorDateInWeek.Day;
                if (isAppointmentStartsAtThisHour && isAppointmentTheSameDayThatIteratorDate)
                {
                    cellText = string.Format("{0}{1}   ", cellText, appointment.StartDate.ToString(Constants.HourAndMinuteFormat));
                }
                cellText = string.Format("{0}{1}{2}", cellText, appointment.Title, Environment.NewLine);
            }
            return cellText;
        }
        #endregion
    }
}
