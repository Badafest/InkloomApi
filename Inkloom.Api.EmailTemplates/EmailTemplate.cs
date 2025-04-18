using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inkloom.Api.EmailTemplates.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Inkloom.Api.EmailTemplates;

public abstract class EmailTemplate<IRenderComponent>(string name, Dictionary<string, object> parameters) where IRenderComponent : IComponent
{
  private readonly Dictionary<string, object> _parameters = parameters;
  public async Task<string> GetHtmlBody()
  {
    HtmlRenderer renderer = HtmlRendererFactory.GetRenderer();
    return await renderer.Dispatcher.InvokeAsync(async () =>
    {
      var bodyParameters = ParameterView.FromDictionary(_parameters);
      var bodyHtml = (await renderer.RenderComponentAsync<IRenderComponent>(bodyParameters)).ToHtmlString();

      var layoutParameters = ParameterView.FromDictionary(new Dictionary<string, object>(){
        {"Title", $"Inkloom - {name}"},
      });

      var layoutString = (await renderer.RenderComponentAsync<Layout>(layoutParameters)).ToHtmlString();
      await renderer.DisposeAsync();
      return layoutString.Replace("{{BODY}}", bodyHtml);
    });
  }

  public string GetTextBody()
  {
    return string.Join('\n', _parameters.Select(keyValuePair => $"{keyValuePair.Key}: {keyValuePair.Value}"));
  }

}
