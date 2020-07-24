using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LockPatron.Formatters
{
  /// <summary>
  /// A formatter that can write x-www-form-urlencoded payloads to a client.
  /// </summary>
  public class UrlEncodedOutputFormatter : IOutputFormatter
  {
    /// <summary>
    /// Determines whether this <see cref="T:Microsoft.AspNetCore.Mvc.Formatters.IOutputFormatter" /> can serialize
    /// an object of the specified type.
    /// </summary>
    /// <param name="context">The formatter context associated with the call.</param>
    /// <returns>Returns <c>true</c> if the formatter can write the response; <c>false</c> otherwise.</returns>
    public bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
      return context.ContentTypeIsServerDefined || context.ContentType == "x-www-form-urlencoded";
    }

    /// <summary>
    /// Writes the object represented by <paramref name="context" />'s Object property.
    /// </summary>
    /// <param name="context">The formatter context associated with the call.</param>
    /// <returns>A Task that serializes the value to the <paramref name="context" />'s response message.</returns>
    public async Task WriteAsync(OutputFormatterWriteContext context)
    {
      context.ContentType = "x-www-form-urlencoded";
      // To not have to deal with reflection, we're going to route through json to get the property names.
      // This way renaming can be done in json, and the outputs will match.
      // Endpoints that request json specifically can also get json by default.
      var document = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(context.Object, context.ObjectType));
      var notFirst = false;
      // Be sure to dispose the writer when done, or it gets buffered internally and won't ever actually write.
      // The await is because disposing a writer is an async operation.
      await using var writer = context.WriterFactory(context.HttpContext.Response.Body, Encoding.UTF8);
      // We're not guaranteed that this will be a json object, but in reality most will be.
      foreach (var obj in document.RootElement.EnumerateObject())
      {
        // These symbols are joined with ampersands, this does that.
        if (notFirst)
        {
          await writer.WriteAsync('&');
        }

        // Url encode everything because there may be spaces and such.
        await writer.WriteAsync(HttpUtility.UrlEncode(obj.Name));
        await writer.WriteAsync('=');

        // Only primitives are supported, complex structures get bound up.
        var text = obj.Value.ValueKind switch
        {
          JsonValueKind.Undefined => "undefined",
          JsonValueKind.Object => "{}", // TODO: Possibly support recursive objects by nested encoding the url.
          JsonValueKind.Array => "[]",
          JsonValueKind.String => obj.Value.GetString(), // Get string strips off the "" characters.
          JsonValueKind.Number => obj.Value.GetRawText(),
          JsonValueKind.True => obj.Value.GetRawText(),
          JsonValueKind.False => obj.Value.GetRawText(),
          JsonValueKind.Null => obj.Value.GetRawText(),
          _ => throw new ArgumentOutOfRangeException()
        };

        // This is the payload to be sent, so certainly url encode this (it can even be binary).
        await writer.WriteAsync(HttpUtility.UrlEncode(text));

        notFirst = true;
      }
    }
  }
}
