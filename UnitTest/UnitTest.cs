using System;
using InterviewCodeReview;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void LogWithoutSettingParametersTest()
        {
            JobLogger.Log("unit testing", MessageType.Message);
        }

        [TestMethod]
        public void LogNothingTest()
        {
            JobLogger.SetOptions(true, false, false, false, false, false);
            JobLogger.Log("unit testing", MessageType.Message);
        }

        [TestMethod]
        public void LogToFileTest()
        {
            JobLogger.SetOptions(true, false, false, true, true, true);
            JobLogger.Log(" Diego Torres ", MessageType.Message);
        }

        [TestMethod]
        public void LogToConsoleTest()
        {
            JobLogger.SetOptions(false, true, false, true, true, true);
            JobLogger.Log(" Kevin Mitnick ", MessageType.Warning);
        }

        [TestMethod]
        public void LogToDatabaseTest()
        {
            JobLogger.SetOptions(false, false, true, true, true, true);
            JobLogger.Log(" Bill Gates ", MessageType.Error);
        }
    }
}
