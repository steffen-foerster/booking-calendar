using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using de.fiok.web;
using de.fiok.type;
using de.fiok.service;

public partial class aspx_manage_manageLandlord : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (! LandlordRoleProvider.IsInRole (LandlordRole.ADMIN)) {
      FormsAuthentication.RedirectToLoginPage ();
    }

    this.PreRender += new EventHandler(Page_PreRender);
  }

  void Page_PreRender(object sender, EventArgs e)
  {
    if (GridViewLandlords.Rows.Count == 0)
    {
      LandlordDetails.DefaultMode = DetailsViewMode.Insert;
    }
    else
    {
      LandlordDetails.DefaultMode = DetailsViewMode.ReadOnly;
    }
  }

  protected void LandlordDetails_ItemUpdated (object sender, DetailsViewUpdatedEventArgs e)
  {
    // Refresh the GridView control after a new record is updated 
    // in the DetailsView control.
    GridViewLandlords.DataBind();
  }

  protected void LandlordDetails_ItemInserting(object sender, DetailsViewInsertEventArgs e)
  {
  }

  protected void LandlordDetails_ItemInserted (object sender, DetailsViewInsertedEventArgs e)
  {
    GridViewLandlords.DataBind();
    GridViewHouses.DataBind();
    HouseDetails.DataBind();
  }

  protected void LandlordDetails_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    GridViewLandlords.DataBind();
  }

  protected void HouseDetails_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
  {
    GridViewHouses.DataBind();
  }

  protected void HouseDetails_ItemInserting(object sender, DetailsViewInsertEventArgs e)
  {
    // dem zu erstellenden Objekt den aktuellen Vermieter zuweisen
    GridViewLandlords.DataBind();
    LandlordDetails.DataBind();
    LandlordBean landlord = (LandlordBean)LandlordDetails.DataItem;
    e.Values.Add ("LandlordId", landlord.ID);
  }

  protected void HouseDetails_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
  {
    GridViewHouses.DataBind();
  }

  protected void HouseDetails_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
  {
    GridViewHouses.DataBind();
  }
}
