using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using TestAttributes;

namespace Rush.Tests
{
	public class MessageSenderTests
    {
		public class GivenMultipleStreamsAndAllAreOperational
		{
			protected const string _message = "Hello world!";
			protected readonly Mock<IMessageStream> _firstStream;
			protected readonly Mock<IMessageStream> _secondStream;
			private readonly Mock<IMessageStream> _thirdStream;
			protected readonly MessageSender<string> _messageSender;

			public GivenMultipleStreamsAndAllAreOperational()
			{
				_firstStream = new Mock<IMessageStream>();
				_firstStream.Setup(x => x.Operational).Returns(true);
				_secondStream = new Mock<IMessageStream>();
				_secondStream.Setup(x => x.Operational).Returns(true);
				_thirdStream = new Mock<IMessageStream>();
				_thirdStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _firstStream.Object, _secondStream.Object, _thirdStream.Object };

				_messageSender = new MessageSender<string>(streams);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndAllAreOperational
			{
				[Unit]
				public async Task ThenSingleStreamSendsMessage()
				{
					var count = 0;
					_firstStream.Setup(x => x.SendAsync(_message)).Returns(Task.FromResult(false)).Callback(() => count++);
					_secondStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(Task.FromResult(false)).Callback(() => count++);
					_thirdStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(Task.FromResult(false)).Callback(() => count++);

					await _messageSender.SendAsync(_message);

					count.Should().Be(1);
				}
			}

			public class WhenSendingToSingleStreamWhichFaults : GivenMultipleStreamsAndAllAreOperational
			{
				[Unit]
				public async Task ThenSendsToNextOperationalStream()
				{
					var count = 0;
					_firstStream.Setup(x => x.SendAsync(_message)).Returns(Task.FromResult(false)).Callback(() => count++);
					_secondStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(Task.FromResult(false)).Callback(() => count++);
					_thirdStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(Task.FromResult(false)).Callback(() => count++);

					await _messageSender.SendAsync(_message);

					count.Should().Be(1);
				}
			}
		}

		public class GivenMultipleStreamsAndSomeAreOperational
		{
			protected readonly Mock<IMessageStream> _inoperativeStream;
			protected const string _message = "Hello world!";
			protected readonly Mock<IMessageStream> _operationalStream;
			protected readonly MessageSender<string> _messageSender;

			public GivenMultipleStreamsAndSomeAreOperational()
			{
				_inoperativeStream = new Mock<IMessageStream>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_operationalStream = new Mock<IMessageStream>();
				_operationalStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _inoperativeStream.Object, _operationalStream.Object };

				_messageSender = new MessageSender<string>(streams);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndSomeAreOperational
			{
				[Unit]
				public async Task ThenOperationalStreamsSendMessage()
				{
					await _messageSender.SendAsync(_message);

					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_operationalStream.Verify(x => x.SendAsync(_message), Times.Once);
				}
			}
		}

		public class GivenMultipleStreamsAndNoneAreOperational
		{
			protected readonly Mock<IMessageStream> _inoperativeStream;
			protected const string _message = "Hello world!";
			protected readonly Mock<IMessageStream> _alternativeInoperativeStream;
			protected readonly MessageSender<string> _messageSender;

			public GivenMultipleStreamsAndNoneAreOperational()
			{
				_inoperativeStream = new Mock<IMessageStream>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_alternativeInoperativeStream = new Mock<IMessageStream>();
				_alternativeInoperativeStream.Setup(x => x.Operational).Returns(false);
				var streams = new[] { _inoperativeStream.Object, _alternativeInoperativeStream.Object };

				_messageSender = new MessageSender<string>(streams);
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndNoneAreOperational
			{
				[Unit]
				public void ThenOperationalStreamsSendMessage()
				{
					Func<Task> sendTask = () => _messageSender.SendAsync(_message);

					sendTask.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("There are no operational message streams.");
					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_alternativeInoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
				}
			}
		}
	}
}
