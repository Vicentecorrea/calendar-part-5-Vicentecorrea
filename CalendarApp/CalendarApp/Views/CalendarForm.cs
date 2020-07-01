using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using CalendarApp.Models;
using CalendarApp.Views;
using CalendarApp.Controllers;

namespace CalendarApp
{
    public partial class CalendarForm : Form
    {
        #region Fields
        private int daysInSelectedMonth;
        private int daysBetweenMondayAndFirstDayOfSelectedMonth;
        private int weeksInSelectedMonth;
        private readonly DayOfWeek[] weekDays = {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday};
        private Calendar calendar = new Calendar();
        private readonly AppointmentController appointmentController = new AppointmentController();
        private readonly UserController userController;
        #endregion

        #region Methods
        public CalendarForm(UserController userController)
        {
            InitializeComponent();
            appointmentController.LoadAppointments();
            calendar.SelectedDate = DateTime.Today;
            calendarDisplayMenuListBox.SelectedItem = Constants.MonthOption;
            loggedInUserValue.Text = UserController.LoggedUserName;
            this.userController = userController;
            ShowMonth();
        }

        private void NextTimePeriodButton_Click(object sender, EventArgs e)
        {
            MoveToSelectedPeriod(Constants.NextTimeInterval);
            ShowSelectedDisplay();
        }

        private void PreviousTimePeriodButton_Click(object sender, EventArgs e)
        {
            MoveToSelectedPeriod(Constants.PreviousTimeInterval);
            ShowSelectedDisplay();
        }

        private void MoveToSelectedPeriod(int timeInterval)
        {
            if (calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.MonthOption)
            {
                calendar.SelectedDate = calendar.SelectedDate.AddMonths(timeInterval);
            }
            else if (calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.WeekOption)
            {
                calendar.SelectedDate = calendar.SelectedDate.AddDays(timeInterval * Constants.DaysInWeek);
            }
        }

        private void TodayButton_Click(object sender, EventArgs e)
        {
            calendar.SelectedDate = DateTime.Today;
            ShowSelectedDisplay();
        }

        private void GoToCreateAppointmentFormButton_Click(object sender, EventArgs e)
        {
            CreateAppointmentForm createAppointmentForm = new CreateAppointmentForm(this, appointmentController, this.userController);
            createAppointmentForm.Show();
        }

        private void CalendarDisplayMenuListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedDisplay();
        }

        public void ShowSelectedDisplay()
        {
            if (calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.MonthOption)
            {
                ShowMonth();
            }
            else if (calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.WeekOption)
            {
                ShowWeek();
            }
        }

        private void ShowWeek()
        {
            calendarGridView.Rows.Clear();
            AddHoursColumn();
            UpdateBasicWeekInformation();
            MakeWeekTable();
        }

        private void UpdateBasicWeekInformation()
        {
            DateTime iteratorDayOfWeek = calendar.GetMondayOfWeek(calendar.SelectedDate);
            DateTime firstDateOfWeek = iteratorDayOfWeek;
            for (int dayColumn = Constants.GapBetweenHoursColumnAndMondayColumn; dayColumn < calendarGridView.Columns.Count; dayColumn++)
            {
                calendarGridView.Columns[dayColumn].HeaderText = string.Format("{0} {1}", iteratorDayOfWeek.DayOfWeek.ToString(), iteratorDayOfWeek.Day.ToString());
                if (iteratorDayOfWeek == DateTime.Today)
                {
                    calendarGridView.EnableHeadersVisualStyles = Constants.EnabledVisualStyles;
                    calendarGridView.Columns[dayColumn].HeaderCell.Style.BackColor = Color.LightCoral;
                    calendarGridView.EnableHeadersVisualStyles = !Constants.EnabledVisualStyles;
                }
                else
                {
                    calendarGridView.Columns[dayColumn].HeaderCell.Style.BackColor = Color.White;
                }
                iteratorDayOfWeek = iteratorDayOfWeek.AddDays(Constants.NextTimeInterval);
            }
            DateTime lastDateOfWeek = iteratorDayOfWeek;
            UpdateCalendarLabelInWeekView(firstDateOfWeek, lastDateOfWeek);
        }

        private void UpdateCalendarLabelInWeekView(DateTime firstDateOfWeek, DateTime lastDateOfWeek)
        {
            if (firstDateOfWeek.Month != lastDateOfWeek.Month && firstDateOfWeek.Year != lastDateOfWeek.Year)
            {
                monthLabel.Text = string.Format("{0} - {1}", firstDateOfWeek.ToString(Constants.MonthAndYearFormat, new CultureInfo(Constants.EnglishLanguageCode)),
                    lastDateOfWeek.ToString(Constants.MonthAndYearFormat, new CultureInfo(Constants.EnglishLanguageCode)));
            }
            else if (firstDateOfWeek.Month != lastDateOfWeek.Month && firstDateOfWeek.Year == lastDateOfWeek.Year)
            {
                monthLabel.Text = string.Format("{0} - {1}", firstDateOfWeek.ToString(Constants.MonthFormat, new CultureInfo(Constants.EnglishLanguageCode)),
                    lastDateOfWeek.ToString(Constants.MonthAndYearFormat, new CultureInfo(Constants.EnglishLanguageCode)));
            }
            else if (firstDateOfWeek.Month == lastDateOfWeek.Month)
            {
                monthLabel.Text = lastDateOfWeek.ToString(Constants.MonthAndYearFormat, new CultureInfo(Constants.EnglishLanguageCode));
            }
        }

        private void MakeWeekTable()
        {
            for (int hour = Constants.DefaultInitialIndex; hour < Constants.HoursInDay; hour++)
            {
                calendarGridView.Rows.Add(GetHourlyRow(hour));
            }
        }

        private DataGridViewRow GetHourlyRow(int hour)
        {
            calendar.IteratorDateInWeek = calendar.GetMondayOfWeek(calendar.SelectedDate);
            calendar.IteratorDateInWeek = calendar.IteratorDateInWeek.AddHours(calendar.IteratorDateInWeek.Hour * Constants.PreviousTimeInterval);
            calendar.IteratorDateInWeek = calendar.IteratorDateInWeek.AddHours(hour);
            DataGridViewRow row = new DataGridViewRow();
            foreach (DataGridViewColumn column in calendarGridView.Columns)
            {
                row.Cells.Add(GetHourCellInWeekView(hour, column));
            }
            row.Height = Constants.CellHeightInWeekView;
            return row;
        }

        private DataGridViewTextBoxCell GetHourCellInWeekView(int hour, DataGridViewColumn column)
        {
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            if (column.Index == Constants.DefaultInitialIndex)
            {
                cell.Value = string.Format("{0}:00", hour.ToString());
            }
            else
            {
                List<Appointment> appointmentsInThisDayAtThisHour = appointmentController.GetAppointmentsInThisDayAndTime(calendar.IteratorDateInWeek);
                string cellText = calendar.GetCellTextInWeekView(appointmentsInThisDayAtThisHour, hour);
                if (appointmentsInThisDayAtThisHour.Count > Constants.ZeroItemsInList)
                {
                    cell.Style.BackColor = Color.LightGreen;
                }
                cell.Value = cellText;
                cell.Tag = appointmentsInThisDayAtThisHour;
                calendar.IteratorDateInWeek = calendar.IteratorDateInWeek.AddDays(Constants.NextTimeInterval);
            }
            return cell;
        }

        private void AddHoursColumn()
        {
            if (calendarGridView.Columns.Count == Constants.DaysInWeek)
            {
                DataGridViewTextBoxColumn columnOfHours = new DataGridViewTextBoxColumn();
                columnOfHours.SortMode = DataGridViewColumnSortMode.NotSortable;
                calendarGridView.Columns.Insert(Constants.DefaultInitialIndex, columnOfHours);
            }
        }

        private void ShowMonth()
        {
            calendarGridView.Rows.Clear();
            UpdateBasicMonthInformation();
            MakeMonthTable();
            if (calendar.SelectedDate == DateTime.Today)
            {
                PaintToday();
            }
        }

        private void PaintToday()
        {
            int cellCounter = Constants.DefaultInitialIndex;
            for (int row = Constants.DefaultInitialIndex; row < calendarGridView.RowCount; row++)
            {
                for (int column = Constants.DefaultInitialIndex; column < calendarGridView.ColumnCount; column++)
                {
                    cellCounter++;
                    if (cellCounter == calendar.SelectedDate.Day + daysBetweenMondayAndFirstDayOfSelectedMonth)
                    {
                        calendarGridView.Rows[row].Cells[column].Style.BackColor = Color.LightCoral;
                    }
                }
            }
        }

        private void UpdateBasicMonthInformation()
        {
            if (calendarGridView.Columns.Count > Constants.DaysInWeek)
            {
                calendarGridView.Columns.RemoveAt(Constants.DefaultInitialIndex);
            }
            for (int dayColumn = Constants.DefaultInitialIndex; dayColumn < calendarGridView.Columns.Count; dayColumn++)
            {
                calendarGridView.Columns[dayColumn].HeaderText = weekDays[dayColumn].ToString();
                calendarGridView.Columns[dayColumn].HeaderCell.Style.BackColor = Color.White;
            }
            monthLabel.Text = calendar.SelectedDate.ToString(Constants.MonthAndYearFormat, new CultureInfo(Constants.EnglishLanguageCode));
            daysInSelectedMonth = DateTime.DaysInMonth(calendar.SelectedDate.Year, calendar.SelectedDate.Month);
            daysBetweenMondayAndFirstDayOfSelectedMonth = calendar.DaysBetweenMondayAndFirstDayOfSelectedMonth;
            weeksInSelectedMonth = (int)Math.Ceiling((daysInSelectedMonth + daysBetweenMondayAndFirstDayOfSelectedMonth) / (float)Constants.DaysInWeek);
            calendar.IteratorDayInMonth = Constants.DefaultFirstDay;
        }

        private void MakeMonthTable()
        {
            for (int week = Constants.DefaultInitialIndex; week < weeksInSelectedMonth; week++)
            {
                calendarGridView.Rows.Add(GetWeekRow(week.Equals(Constants.DefaultInitialIndex)));
            }
        }

        private DataGridViewRow GetWeekRow(bool isFirstWeek)
        {
            DataGridViewRow row = new DataGridViewRow();
            for (int weekDay = Constants.DefaultInitialIndex; weekDay < Constants.DaysInWeek; weekDay++)
            {
                row.Cells.Add(GetDayCellInMonthView(weekDay, isFirstWeek));
            }
            row.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
            row.Height = Constants.CellHeightInMonthView;
            return row;
        }

        private DataGridViewTextBoxCell GetDayCellInMonthView(int weekDay, bool isFirstWeek)
        {
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            bool isDayOfPreviousMonth = weekDay < daysBetweenMondayAndFirstDayOfSelectedMonth && isFirstWeek;
            bool isDayOfNextMonth = calendar.IteratorDayInMonth > daysInSelectedMonth;
            if (isDayOfPreviousMonth || isDayOfNextMonth)
            {
                cell.Value = Constants.Empty;
            }
            else
            {
                cell = GetDayCellNonEmptyInMonthView();
            }
            return cell;
        }

        private DataGridViewTextBoxCell GetDayCellNonEmptyInMonthView()
        {
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            DateTime day = new DateTime(calendar.SelectedDate.Year, calendar.SelectedDate.Month, calendar.IteratorDayInMonth);
            List<Appointment> appointmentsInThisDay = appointmentController.GetAppointmentsInThisDay(day);
            string cellText = calendar.GetCellTextInMonthView(appointmentsInThisDay, day);
            cell.Value = cellText;
            cell.Tag = appointmentsInThisDay;
            calendar.IteratorDayInMonth++;
            return cell;
        }

        private void CalendarGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!WasHeaderOrColumnOfHoursClicked(e.RowIndex, e.ColumnIndex))
            {
                List<Appointment> appointments = (List<Appointment>)calendarGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                if (appointments.Count > Constants.OneItemInList)
                {
                    DateTime clickedDateAndTime = GetClickedDateAndTime(e);
                    AppointmentsInDayForm appointmentsInDayForm = new AppointmentsInDayForm(appointments, clickedDateAndTime, this, appointmentController, userController);
                    appointmentsInDayForm.Show();
                }
                else if (appointments.Count == Constants.OneItemInList)
                {
                    AppointmentInformationForm appointmentInformationForm = new AppointmentInformationForm(appointments[Constants.DefaultInitialIndex], this, appointmentController, userController);
                    appointmentInformationForm.Show();
                }
            } 
        }

        private DateTime GetClickedDateAndTime(DataGridViewCellEventArgs e)
        {
            DateTime clickedDateAndTime;
            if (calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.MonthOption)
            {
                int day = e.RowIndex * Constants.DaysInWeek + e.ColumnIndex + Constants.GapBetweenIndexAndNumber - daysBetweenMondayAndFirstDayOfSelectedMonth;
                clickedDateAndTime = new DateTime(calendar.SelectedDate.Year, calendar.SelectedDate.Month, day);
            }
            else
            {
                clickedDateAndTime = calendar.GetMondayOfWeek(calendar.SelectedDate);
                clickedDateAndTime = clickedDateAndTime.AddDays(e.ColumnIndex - Constants.GapBetweenHoursColumnAndMondayColumn);
                clickedDateAndTime = clickedDateAndTime.AddHours(e.RowIndex);
            }
            return clickedDateAndTime;
        }

        private bool WasHeaderOrColumnOfHoursClicked(int rowIndex, int columnIndex)
        {
            bool selectedViewIsWeekly = calendarDisplayMenuListBox.SelectedItem.ToString() == Constants.WeekOption;
            bool rowIndexIsHeaderRowIndex = rowIndex == Constants.HeaderRowIndex;
            bool columnIndexIsDefaultInitialIndex = columnIndex == Constants.DefaultInitialIndex;
            bool wasHeaderOrColumnOfHoursClicked = rowIndexIsHeaderRowIndex || (columnIndexIsDefaultInitialIndex && selectedViewIsWeekly);
            return wasHeaderOrColumnOfHoursClicked;
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            UserController.LoggedUserName = Constants.Empty;
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Close();
        }
        #endregion
    }
}
