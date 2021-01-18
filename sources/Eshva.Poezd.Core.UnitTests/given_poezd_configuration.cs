#region Usings

using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using SimpleInjector;
using Xunit;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_poezd_configuration
  {
    [Fact]
    public void when_set_message_handling_configuration_it_should_be_stored()
    {
      var container = new Container();
      var poezdConfiguration = ConfigurePoezd(container);

      poezdConfiguration.MessageHandling.MessageHandlersFactory.Should()
                        .NotBeNull("it's configured").And
                        .BeOfType<CustomMessageHandlerFactory>($"set factory of type {nameof(CustomMessageHandlerFactory)}");
    }

    private static MessageRouterConfiguration ConfigurePoezd(Container container)
    {
      return MessageRouter.Configure(
        configurator => configurator
          .WithMessageHandling(
            messageHandling => messageHandling.WithMessageHandlersFactory(new CustomMessageHandlerFactory(container))));
    }
  }
}