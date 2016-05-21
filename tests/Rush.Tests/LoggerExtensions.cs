using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using System;

namespace Rush.Tests
{
	public static class LoggerExtensions
	{
		public static void VerifyLoggedInformation<T>(this Mock<ILogger<T>> logger, Times times)
		{
			logger.Verify(x => x.Log(LogLevel.Information, 0, It.IsAny<FormattedLogValues>(), null, It.IsAny<Func<object, Exception, string>>()), times);
		}

		public static void VerifyLoggedWarning<T>(this Mock<ILogger<T>> logger, Times times)
		{
			logger.Verify(x => x.Log(LogLevel.Warning, 0, It.IsAny<FormattedLogValues>(), null, It.IsAny<Func<object, Exception, string>>()), times);
		}
	}
}
