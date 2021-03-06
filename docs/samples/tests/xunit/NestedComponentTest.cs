using Xunit;
using Bunit;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using static Bunit.ComponentParameterFactory;

namespace Bunit.Docs.Samples
{
  public class NestedComponentTest
  {
    [Fact]
    public void Test()
    {
      using var ctx = new TestContext();

      var wrapper = ctx.RenderComponent<Wrapper>(parameters => parameters
        .AddChildContent<HelloWorld>()
      );
      var cut = wrapper.FindComponent<HelloWorld>();
    }
  }
}
