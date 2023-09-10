using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Final
{
    public partial class Details : System.Web.UI.Page
    {
        List<Adventurer> adventurers = new List<Adventurer>();
        List<Item> items = Helper.GetAvailableItems();

        int index = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Get_Index();
            Get_Adventurers();
            Reset_ErrorMessages();

            if (!IsPostBack)
            {
                Populate_Items();
                Display_AdventurerDetails();
            }
        }
        protected void Display_AdventurerDetails()
        {
            if (index >= 0 && index < adventurers.Count)
            {
                Adventurer adventurer = adventurers[index];

                txtName.InnerText = adventurer.Name;
                txtType.InnerText = adventurer.Type;
                txtPhrase.InnerText = adventurer.Greeting();

                lblStrength.Text = adventurer.Strength.ToString();
                lblDexterity.Text = adventurer.Dexterity.ToString();
                lblVitality.Text = adventurer.Vitality.ToString();
                lblMana.Text = adventurer.Mana.ToString();

                
                cblItems.ClearSelection();
                foreach (Item item in adventurer.EquippedItems)
                {
                    ListItem listItem = cblItems.Items.FindByText($"{item.Name} ({item.StrengthRequirement}/{item.DexterityRequirement}/{item.ManaRequirement})");
                    if (listItem != null)
                    {
                        listItem.Selected = true;
                    }
                }
            }
        }


        protected void Get_Index()
        {
            if (Request.QueryString["id"] == null)
            {
                Response.Redirect("Default.aspx");
            }

            index = int.Parse(Request.QueryString["id"]);
        }

        protected void Get_Adventurers()
        {
            if (Session["adventurers"] != null)
            {
                adventurers = (List<Adventurer>)Session["adventurers"];
            }
        }

        protected void Reset_ErrorMessages()
        {
            lblEquipError.Visible = false;
            lblEquipError.Controls.Clear();
        }

        protected void Populate_Items()
        {
            int itemIndex = 0;
            foreach (Item item in items)
            {
                ListItem listItem = new ListItem($"{item.Name} ({item.StrengthRequirement}/{item.DexterityRequirement}/{item.ManaRequirement})", itemIndex.ToString());

                if (adventurers.Count > 0)
                {
                    if (adventurers[index].Item_Equiped(item))
                    {
                        listItem.Selected = true;
                    }
                }

                cblItems.Items.Add(listItem);

                itemIndex++;
            }
        }

        protected void btnEquipItems_Click(object sender, EventArgs e)
        {
            lblEquipError.Visible = false;

            if (index >= 0 && index < adventurers.Count)
            {
                Adventurer adventurer = adventurers[index];


                adventurer.EquippedItems.Clear();

                foreach (ListItem item in cblItems.Items)
                {
                    if (item.Selected)
                    {
                        int itemIndex = int.Parse(item.Value);
                        Item selected = items[itemIndex];

                        try
                        {
                            adventurer.Equip_Item(selected);
                        }
                        catch (Exception ex)
                        {
                            lblEquipError.Text = ex.Message;
                            lblEquipError.Visible = true;
                            item.Selected = false; 
                        }
                    }
                }

                
                Session["adventurers"] = adventurers;

                
                Display_AdventurerDetails();
            }
        }
    }
}