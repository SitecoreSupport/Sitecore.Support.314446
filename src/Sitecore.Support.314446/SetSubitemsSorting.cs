using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Support.Shell.Framework.Commands
{
  [Serializable]
  public class SetSubitemsSorting : Command
  {
    /// <summary>
    /// Executes the command in the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void Execute(CommandContext context)
    {
      Error.AssertObject(context, "context");
      if (context.Items.Length == 1)
      {
        Execute(context.Items[0]);
      }
    }

    /// <summary>
    /// Executes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Execute(Item item)
    {
      Error.AssertObject(item, "item");
      UrlString urlString = new UrlString(UIUtil.GetUri("control:SetSubitemsSorting"));
      urlString.Append("id", item.ID.ToString());
      urlString.Append("la", item.Language.ToString());
      urlString.Append("vs", item.Version.ToString());
      urlString.Append("db", item.Database.Name);
      SheerResponse.ShowModalDialog(urlString.ToString(), "item:refreshchildren(id=" + item.ID.ToString() + ")");
    }

    /// <summary>
    /// Queries the state of the command.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The state of the command.</returns>
    public override CommandState QueryState(CommandContext context)
    {
      Error.AssertObject(context, "context");
      if (context.Items.Length != 1)
      {
        return CommandState.Disabled;
      }
      Item item = context.Items[0];
      if (!base.HasField(item, FieldIDs.SubitemsSorting))
      {
        return CommandState.Hidden;
      }
      if (item.Appearance.ReadOnly)
      {
        return CommandState.Disabled;
      }
      if (!item.Access.CanWrite())
      {
        return CommandState.Disabled;
      }
      if (Command.IsLockedByOther(item))
      {
        return CommandState.Disabled;
      }
      if (!Command.CanWriteField(item, FieldIDs.SubitemsSorting))
      {
        return CommandState.Disabled;
      }
      return base.QueryState(context);
    }
  }

}