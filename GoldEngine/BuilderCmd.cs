using System;
using System.IO;

namespace GoldEngine
{
    internal sealed class BuilderCmd
    {
        // Fields
        private static string m_Grammar;
        private static string m_GrammarFile;
        private static string m_LogFile;
        private static bool m_ShowDetails;
        private static string m_TableFile;
        private static bool m_Verbose;
        private static int m_Version;

        // Methods
        private static bool LoadGrammar()
        {
            bool flag2;
            try
            {
                string str;
                m_Grammar = "";
                TextReader reader = new StreamReader(m_GrammarFile, true);
                do
                {
                    str = reader.ReadLine();
                    if (str != null)
                    {
                        m_Grammar = m_Grammar + str + "\r\n";
                    }
                }
                while (str != null);
                reader.Close();
                flag2 = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                flag2 = false;
            }
            return flag2;
        }

        [STAThread]
        public static void Main()
        {
            bool flag = true;
            BuilderApp.FatalLoadError = false;
            Setup();
            Notify.OutputProgress = m_Verbose;
            BuilderApp.Setup();
            if (BuilderApp.FatalLoadError)
            {
                BuilderApp.Log.Add(SysLogSection.System, SysLogAlert.Critical, "There was a fatal internal error.");
                flag = false;
            }
            if (m_GrammarFile == "")
            {
                BuilderApp.Log.Add(SysLogSection.CommandLine, SysLogAlert.Critical, "You must specify a grammar file.");
                flag = false;
            }
            else if (!LoadGrammar())
            {
                BuilderApp.Log.Add(SysLogSection.CommandLine, SysLogAlert.Critical, "The grammar file could not be loaded.");
                flag = false;
            }
            if (flag)
            {
                TextReader metaGrammar = new StringReader(m_Grammar);
                BuilderApp.EnterGrammar(metaGrammar);
                if (BuilderApp.LoggedCriticalError())
                {
                    flag = false;
                }
            }
            if (flag)
            {
                BuilderApp.ComputeLALR();
                if (BuilderApp.LoggedCriticalError())
                {
                    flag = false;
                }
            }
            if (flag)
            {
                BuilderApp.ComputeDFA();
                if (BuilderApp.LoggedCriticalError())
                {
                    flag = false;
                }
            }
            if (flag)
            {
                BuilderApp.ComputeComplete();
            }
            if (flag)
            {
                BuilderApp.Log.Add(SysLogSection.System, SysLogAlert.Success, "The grammar was successfully analyzed and the table file was created.");
                string str = FileUtility.GetExtension(m_TableFile).ToLower();
                if (str == "xml")
                {
                    if (m_Version == 1)
                    {
                        BuilderApp.BuildTables.SaveXML1(m_TableFile);
                    }
                    else
                    {
                        BuilderApp.BuildTables.SaveXML5(m_TableFile);
                    }
                }
                else if (str == "cgt")
                {
                    BuilderApp.BuildTables.SaveVer1(m_TableFile);
                }
                else
                {
                    BuilderApp.BuildTables.SaveVer5(m_TableFile);
                }
            }
            BuilderApp.SaveLog(m_LogFile);
            if (m_Verbose)
            {
                int num2 = BuilderApp.Log.Count() - 1;
                for (int i = 0; i <= num2; i++)
                {
                    SysLogItem item = BuilderApp.Log[i];
                    if ((item.Alert == SysLogAlert.Critical) | (item.Alert == SysLogAlert.Warning))
                    {
                        Console.WriteLine(item.AlertName().ToUpper() + " : " + item.SectionName() + " : " + item.Title + " : " + item.Description);
                    }
                    item = null;
                }
                Console.WriteLine("Done");
            }
        }

        private static void Setup()
        {
            CommandLine line = new CommandLine();
            m_ShowDetails = true;
            line.ReadArguments(Environment.CommandLine, 1);
            m_GrammarFile = line.Argument("%1", "", true);
            string fileNameBase = FileUtility.GetFileNameBase(m_GrammarFile);
            m_TableFile = line.Argument("%2", fileNameBase + ".egt", true);
            if (m_GrammarFile == "")
            {
                m_LogFile = "error.log";
            }
            else
            {
                m_LogFile = line.Argument("%3", fileNameBase + ".log", true);
            }
            m_ShowDetails = line.Argument("details", "+") == "+";
            m_Verbose = line.Argument("verbose", "+") == "+";
            m_Version = 5;
            if (line.Argument("v1", "-") == "+")
            {
                m_Version = 1;
            }
        }
    }
}