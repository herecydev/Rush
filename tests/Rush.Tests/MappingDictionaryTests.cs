using FluentAssertions;
using Moq;
using System.Linq;
using TestAttributes;

namespace Rush.Tests
{
    public class MappingDictionaryTests
    {
		public class GivenAMappingForTypeThatExists
		{
			private readonly Mock<ISendingChannel> _defaultSender;
			private MappingDictionary _mappingDictionary;
			private readonly Mock<ISendingChannel> _mappingSender;

			public GivenAMappingForTypeThatExists()
			{
				var mapping = new Mock<ISenderMessageStreamMapping>();
				_mappingSender = new Mock<ISendingChannel>();
				mapping.Setup(x => x.Type).Returns(typeof(string));
				mapping.Setup(x => x.Streams).Returns(new[] { _mappingSender.Object });

				_defaultSender = new Mock<ISendingChannel>();

				_mappingDictionary = new MappingDictionary(new[] { mapping.Object }, new[] { _defaultSender.Object });
			}

			public class WhenGettingSendingChannels : GivenAMappingForTypeThatExists
			{
				[Unit]
				public void ThenResolvesMappedMessageSenders()
				{
					var sendingChannels = _mappingDictionary.GetSendingChannels<string>();

					sendingChannels.Should().HaveCount(1);
					sendingChannels.Single().Should().Be(_mappingSender.Object);
				}
			}
		}

		public class GivenAMappingForTypeThatDoesNotExist
		{
			private readonly Mock<ISendingChannel> _defaultSender;
			private readonly MappingDictionary _mappingDictionary;

			public GivenAMappingForTypeThatDoesNotExist()
			{
				_defaultSender = new Mock<ISendingChannel>();

				_mappingDictionary = new MappingDictionary(Enumerable.Empty<ISenderMessageStreamMapping>(), new[] { _defaultSender.Object });
			}

			public class WhenGettingSendingChannels : GivenAMappingForTypeThatDoesNotExist
			{
				[Unit]
				public void ThenResolvesMappedMessageSenders()
				{
					var sendingChannels = _mappingDictionary.GetSendingChannels<string>();

					sendingChannels.Should().HaveCount(1);
					sendingChannels.Single().Should().Be(_defaultSender.Object);
				}
			}
		}
    }
}
