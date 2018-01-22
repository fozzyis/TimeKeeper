using System;
using System.Timers;
namespace TimeKeeperShared
{
    public class TimeKeeper
    {

        DateTime startTime;
        DateTime finishTime;
        TimeSpan timeDifference;
        TimeSpan timeTicker;
        TimeSpan minTime = new TimeSpan(0, 00, 07, 30);
        TimeSpan quarterTime = new TimeSpan(0, 0, 15, 0);
        Timer timerTest = new Timer();
        double tsTime; //todo convert to array to allow multiple entries
        double remainderToAdd;

        Int32 left, right;

        const double NULL_TIME = 0;
        const double MIN_ALLOWED = 0.25;
        const double MAX_NO_OT = 8;

        public TimeKeeper()
        {

        }

        public DateTime GetStartTime()
        {
            return startTime;
        }
        public DateTime GetFinishTime()
        {
            return finishTime;
        }
        public TimeSpan GetTimeDifference()
        {
            return timeDifference;
        }
        public double GetTimeSheetEntry()
        {
            return tsTime;
        }

        public void PrintDebug()
        {
            Console.WriteLine("Called");
        }

        public void Split(Double value, Int32 places, out Int32 left, out Int32 right)
        {
            left = (Int32)Math.Truncate(value);
            right = (Int32)((value - left) * Math.Pow(10, places));
        }

        public void Split(Double value, out Int32 left, out Int32 right)
        {
            Split(value, 1, out left, out right);
        }

        public void StartTimer()
        {
            
            //timerTest.Start();
            startTime = DateTime.Now;
        }

        public void GetStopTime()
        {
            finishTime = DateTime.Now;
            CalculateDifference();
            CalculateAccurateTimesheetEntry(timeDifference);
        }

        public TimeSpan RunTimeTicker()
        {
            //timeTicker = timerTest.Elapsed();
           // timeTicker = DateTime.Now.Subtract(startTime);
            return timeTicker;
        }

        public TimeSpan CalculateDifference()
        {
            timeDifference = finishTime.Subtract(startTime);
            return timeDifference;
        }

        public void CalculateAccurateTimesheetEntry(TimeSpan totalTime)
        {
            var totalMinutesToCalculate = totalTime.TotalMinutes;
            Console.WriteLine("Time in minutes " + totalMinutesToCalculate);
            Console.WriteLine("Time entry expressed in .25 " + tsTime);
            Console.WriteLine("Time total time " + totalTime);
            Split(totalMinutesToCalculate, 2, out left, out right);
            Console.WriteLine("left = " + left + ", Right = " + right);
            if ((left % 15) > 0)
            {
                Console.WriteLine("There is a remainder to work out");
                //todo work out if the remainder is greater than 7.5mins
                tsTime = ((left / 15) * MIN_ALLOWED) + MIN_ALLOWED;
                //todo add remainder to previous or next WO
            }
            else if ((left % 15) == 0 && left > 0)
            {
                tsTime = (left / 15) * MIN_ALLOWED;
                remainderToAdd = right;
                //add entry to timesheet
                //add remainder to current new timer

                Console.WriteLine("Time for MILIS " + tsTime);
            }
            else
            {
                Console.WriteLine("Working on it");
                //flag an error to the user as time is insufficient
            }

        }
    }
}
