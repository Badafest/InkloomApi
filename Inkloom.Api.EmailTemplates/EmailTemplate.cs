using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inkloom.Api.EmailTemplates.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Inkloom.Api.EmailTemplates;

public abstract class EmailTemplate<IRenderComponent>(Dictionary<string, object> parameters) where IRenderComponent : IComponent
{
  private readonly Dictionary<string, object> _parameters = parameters;
  public async Task<string> GetHtmlBody()
  {
    HtmlRenderer renderer = HtmlRendererFactory.GetRenderer();
    return await renderer.Dispatcher.InvokeAsync(async () =>
    {
      var parameters = ParameterView.FromDictionary(_parameters);
      var htmlString = (await renderer.RenderComponentAsync<IRenderComponent>(parameters)).ToHtmlString();
      var layoutString = (await renderer.RenderComponentAsync<Layout>()).ToHtmlString();
      await renderer.DisposeAsync();
      return layoutString.Replace("{{BODY}}", htmlString);
    });
  }

  public string GetTextBody()
  {
    return string.Join('\n', _parameters.Select(keyValuePair => $"{keyValuePair.Key}: {keyValuePair.Value}"));
  }

}
