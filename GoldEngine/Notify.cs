using System;

namespace GoldEngine
{
    internal sealed class Notify
    {
        // Fields
        private static int m_Analyzed = 0;
        private static int m_Counter = 0;
        private static DateTime m_LastUpdate;
        private static string m_Text = "";
        public static bool OutputProgress;
        private const double TicksPerUpdate = 5000000.0;

        // Methods
        public static void Completed(string Text)
        {
            if (OutputProgress)
            {
                Console.WriteLine();
                Console.WriteLine(Text);
            }
        }

        public static void Started(string Text)
        {
            if (OutputProgress)
            {
                Console.Write(Text + " ");
            }
            m_Text = "";
            m_Counter = 0;
            m_Analyzed = 0;
            m_LastUpdate = DateTime.Now;
        }

        private static void UpdateProgress()
        {
            if (OutputProgress)
            {
                Console.Write(".");
            }
            m_LastUpdate = DateTime.Now;
        }

        // Properties
        public static int Analyzed
        {
            get
            {
                return m_Analyzed;
            }
            set
            {
                m_Analyzed = value;
                if ((DateTime.Now.Ticks - m_LastUpdate.Ticks) >= 5000000.0)
                {
                    UpdateProgress();
                }
            }
        }

        public static int Counter
        {
            get
            {
                return m_Counter;
            }
            set
            {
                m_Counter = value;
                if ((DateTime.Now.Ticks - m_LastUpdate.Ticks) >= 5000000.0)
                {
                    UpdateProgress();
                }
            }
        }

        public static string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
                if ((DateTime.Now.Ticks - m_LastUpdate.Ticks) >= 5000000.0)
                {
                    UpdateProgress();
                }
            }
        }
    }

}