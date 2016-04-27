using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Rush.Tests
{
	public static class LoggerExtensions
    {
		public static void VerifyLoggedInformation<T>(this Mock<ILogger<T>> logger, Times times)
		{
			logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<int>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), times);
		}

		public static void VerifyLoggedWarning<T>(this Mock<ILogger<T>> logger, Times times)
		{
			logger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<int>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()), times);
		}
	}
}
