using FluentAssertions;
using Microsoft.Extensions.OptionsModel;
using Moq;
using System.Threading.Tasks;
using TestAttributes;

namespace Rush.Tests
{
    public class MessageSenderTests
    {
		public class GivenMultipleStreamsAndAllAreOperational
		{
			protected readonly Mock<IMessageStream> _stream;
			protected const string _message = "Hello world!";
			protected readonly Mock<IMessageStream> _alternativeStream;
			protected readonly Mock<IOptions<MessageSenderOptions>> _options;
			protected readonly MessageSender _messageSender;

			public GivenMultipleStreamsAndAllAreOperational()
			{
				_stream = new Mock<IMessageStream>();
				_stream.Setup(x => x.Operational).Returns(true);
				_alternativeStream = new Mock<IMessageStream>();
				_alternativeStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _stream.Object, _alternativeStream.Object };
				_options = new Mock<IOptions<MessageSenderOptions>>();

				_messageSender = new MessageSender(streams, _options.Object);
			}

			public class WhenSendingToAllStreams : GivenMultipleStreamsAndAllAreOperational
			{
				[Unit]
				public async Task ThenAllStreamsSendMessage()
				{
					_options.Setup(x => x.Value).Returns(new MessageSenderOptions { StreamSelector = StreamSelector.All });

					await _messageSender.SendAsync(_message);

					_stream.Verify(x => x.SendAsync(_message), Times.Once);
					_alternativeStream.Verify(x => x.SendAsync(_message), Times.Once);
				}
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndAllAreOperational
			{
				[Unit]
				public async Task ThenSingleStreamSendsMessage()
				{
					var count = 0;
					_options.Setup(x => x.Value).Returns(new MessageSenderOptions { StreamSelector = StreamSelector.Single });
					_stream.Setup(x => x.SendAsync(_message)).Returns(Task.FromResult(false)).Callback(() => count++);
					_alternativeStream.Setup(x => x.SendAsync(_message)).Callback(() => count++).Returns(Task.FromResult(false)).Callback(() => count++);
					
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
			protected readonly Mock<IOptions<MessageSenderOptions>> _options;
			protected readonly MessageSender _messageSender;

			public GivenMultipleStreamsAndSomeAreOperational()
			{
				_inoperativeStream = new Mock<IMessageStream>();
				_inoperativeStream.Setup(x => x.Operational).Returns(false);
				_operationalStream = new Mock<IMessageStream>();
				_operationalStream.Setup(x => x.Operational).Returns(true);
				var streams = new[] { _inoperativeStream.Object, _operationalStream.Object };
				_options = new Mock<IOptions<MessageSenderOptions>>();

				_messageSender = new MessageSender(streams, _options.Object);
			}
			
			public class WhenSendingToAllStreams : GivenMultipleStreamsAndSomeAreOperational
			{
				[Unit]
				public async Task ThenOperationalStreamsSendMessage()
				{
					_options.Setup(x => x.Value).Returns(new MessageSenderOptions { StreamSelector = StreamSelector.All });
					
					await _messageSender.SendAsync(_message);

					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_operationalStream.Verify(x => x.SendAsync(_message), Times.Once);
				}
			}

			public class WhenSendingToSingleStream : GivenMultipleStreamsAndSomeAreOperational
			{
				[Unit]
				public async Task ThenOperationalStreamsSendMessage()
				{
					_options.Setup(x => x.Value).Returns(new MessageSenderOptions { StreamSelector = StreamSelector.Single });

					await _messageSender.SendAsync(_message);

					_inoperativeStream.Verify(x => x.SendAsync(_message), Times.Never);
					_operationalStream.Verify(x => x.SendAsync(_message), Times.Once);
				}
			}
		}
    }
}
