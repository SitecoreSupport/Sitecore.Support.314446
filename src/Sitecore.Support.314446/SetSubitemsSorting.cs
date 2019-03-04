using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
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
      NameValueCollection nameValueCollection = new NameValueCollection();
      nameValueCollection["id"] = item.ID.ToString();
      nameValueCollection["language"] = item.Language.ToString();
      nameValueCollection["version"] = item.Version.ToString();
      nameValueCollection["databasename"] = item.Database.Name;
      Context.ClientPage.Start(this, "Run", nameValueCollection);
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

    protected void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      string text = args.Parameters["databasename"];
      string itemPath = args.Parameters["id"];
      string name = args.Parameters["language"];
      string value = args.Parameters["version"];
      Database database = Factory.GetDatabase(text);
      Assert.IsNotNull(database, text);
      Item item = database.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];

      if (SheerResponse.CheckModified())
      {
        if (args.IsPostBack)
        {
          if (args.Result == "yes")
          {
            Context.ClientPage.SendMessage(this, "item:load(id=" + item.ID + ",language=" + item.Language + ",version=" + item.Version + ")");
          }
        }
        else
        {
          UrlString urlString = new UrlString(UIUtil.GetUri("control:SetSubitemsSorting"));
          urlString.Append("id", item.ID.ToString());
          urlString.Append("la", item.Language.ToString());
          urlString.Append("vs", item.Version.ToString());
          urlString.Append("db", item.Database.Name);
          SheerResponse.ShowModalDialog(urlString.ToString(), "600", "600", "item:refreshchildren(id=" + item.ID.ToString() + ")", true);
          args.WaitForPostBack();
        }
      }
    }
  }

}