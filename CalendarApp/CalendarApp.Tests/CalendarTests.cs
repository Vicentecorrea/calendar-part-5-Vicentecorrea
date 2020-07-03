using CalendarApp;
using CalendarApp.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class CalendarTests
    {
        private Calendar calendar = null;

        [SetUp]
        public void SetUp()
        {
            calendar = new Calendar();
        }

        [TearDown]
        public void TearDown()
        {
            calendar = null;
        }

        [Test]
        [Category("Property Tests")]
        public void DaysBetweenMondayAndFirstDayOfSelectedMonth_SelectedDate_ReturnsExpectedNumber()
        {
            calendar.SelectedDate = new DateTime(2020, 10, 25);
            int expectedDaysBetweenMondayAndFirstDayOfSelectedMonth = 3;

            int result = calendar.DaysBetweenMondayAndFirstDayOfSelectedMonth;
            Assert.AreEqual(expectedDaysBetweenMondayAndFirstDayOfSelectedMonth, result);
        }

        [Test]
        [Category("Simple Tests")]
        public void GetMondayOfWeek_Friday13November2020_ReturnsMonday9()
        {
            DateTime dateInput = new DateTime(2020, 11, 13);
            DateTime expectedDate = new DateTime(2020, 11, 9);

            DateTime result = calendar.GetMondayOfWeek(dateInput);
            Assert.AreEqual(expectedDate, result);
        }

        [Test]
        [Category("Simple Tests")]
        public void GetCellTextInMonthView_AppointmentsAndDay_ReturnsExcpectedText()
        {
            calendar.IteratorDayInMonth = 26;
            DateTime firstAppointmentDefaultStartDate = new DateTime(2020, 8, 26, 2, 34, 11);
            DateTime firstAppointmentDefaultEndDate = new DateTime(2020, 8, 26, 7, 12, 56);
            List<string> firstGuestUserNames = new List<string> {"Ignacio", "Antonia", "Juan"};
            Appointment firstDefaultAppointment = new Appointment("Subida al cerro", "Description", firstAppointmentDefaultStartDate, firstAppointmentDefaultEndDate, "Diego", firstGuestUserNames);
            DateTime secondAppointmentDefaultStartDate = new DateTime(2020, 8, 25, 2, 12, 51);
            DateTime secondAppointmentDefaultEndDate = new DateTime(2020, 8, 27, 19, 32, 53);
            List<string> secondGuestUserNames = new List<string>();
            Appointment secondDefaultAppointment = new Appointment("Campeonato", "Description", secondAppointmentDefaultStartDate, secondAppointmentDefaultEndDate, "Ignacio", secondGuestUserNames);
            List<Appointment> appointmentsInThisDayInput = new List<Appointment> {firstDefaultAppointment, secondDefaultAppointment};
            DateTime dayInput = new DateTime(2020, 8, 26);
            string expectedCellText = string.Format("26{0}{1}   Subida al cerro{2}Campeonato", Environment.NewLine, 
                firstDefaultAppointment.StartDate.ToString(Constants.HourAndMinuteFormat), Environment.NewLine);

            string result = calendar.GetCellTextInMonthView(appointmentsInThisDayInput, dayInput);
            Assert.AreEqual(expectedCellText, result);
        }

        [Test]
        [Category("Simple Tests")]
        public void GetCellTextInWeekView_AppointmentsAndHour_ReturnsExpectedText()
        {
            calendar.IteratorDateInWeek = new DateTime(2020, 6, 18);
            DateTime firstAppointmentDefaultStartDate = new DateTime(2020, 6, 17, 2, 34, 11);
            DateTime firstAppointmentDefaultEndDate = new DateTime(2020, 6, 19, 7, 12, 56);
            List<string> firstGuestUserNames = new List<string> {"Ignacio", "Antonia", "Juan"};
            Appointment firstDefaultAppointment = new Appointment("Fiesta", "Description", firstAppointmentDefaultStartDate, firstAppointmentDefaultEndDate, "Diego", firstGuestUserNames);
            DateTime secondAppointmentDefaultStartDate = new DateTime(2020, 6, 18, 17, 12, 51);
            DateTime secondAppointmentDefaultEndDate = new DateTime(2020, 6, 18, 19, 32, 53);
            List<string> secondGuestUserNames = new List<string>();
            Appointment secondDefaultAppointment = new Appointment("Juego de mesa", "Description", secondAppointmentDefaultStartDate, secondAppointmentDefaultEndDate, "Ignacio", secondGuestUserNames);
            List<Appointment> appointmentsInThisDayAtThisHour = new List<Appointment> {firstDefaultAppointment, secondDefaultAppointment};
            int hourInput = 17;
            string expectedCellText = string.Format("Fiesta{0}{1}   Juego de mesa{2}", Environment.NewLine, 
                secondDefaultAppointment.StartDate.ToString(Constants.HourAndMinuteFormat), Environment.NewLine);

            string result = calendar.GetCellTextInWeekView(appointmentsInThisDayAtThisHour, hourInput);
            Assert.AreEqual(expectedCellText, result);
        }
    }
}
