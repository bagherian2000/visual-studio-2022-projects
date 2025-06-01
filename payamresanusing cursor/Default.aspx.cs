using System;
using System.Web.UI;
using System.Data.SqlClient;

namespace ProjectControlPanelWeb
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateInternalWaveNo();
                PopulateMessages();
                chkInternalWave.Checked = true; // Ensure checked by default
                pnlFullPathWavFileName.Visible = !chkInternalWave.Checked;
            }
        }

        protected void chkInternalWave_CheckedChanged(object sender, EventArgs e)
        {
            pnlFullPathWavFileName.Visible = !chkInternalWave.Checked;
        }

        private void PopulateInternalWaveNo()
        {
            string connectionString = "Data Source=10.85.66.20;Initial Catalog=payamresan;User ID=ramin;Password=ram_1350";
            string query = "SELECT Number FROM tblMessage ORDER BY Number";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        ddlInternalWaveNo.Items.Clear();
                        while (reader.Read())
                        {
                            ddlInternalWaveNo.Items.Add(reader["Number"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        lblResult.Text = "Error loading numbers: " + ex.Message;
                    }
                }
            }
        }

        private void PopulateMessages()
        {
            string connectionString = "Data Source=10.85.66.20;Initial Catalog=payamresan;User ID=ramin;Password=ram_1350";
            string query = "SELECT number,message FROM tblMessage";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        while (reader.Read())
                        {
                            sb.Append("<div style='padding:8px 0; direction:rtl; text-align:right;'>");
                            sb.Append(Server.HtmlEncode(reader["number"].ToString()).Replace(Environment.NewLine, "<br/>"));
                            sb.Append(" - "); // Insert a hyphen between number and message
                            sb.Append(Server.HtmlEncode(reader["message"].ToString()).Replace(Environment.NewLine, "<br/>"));
                            sb.Append("</div>");
                            sb.Append("<hr style='border:0; border-top:1px solid #c5e1a5; margin:4px 0;' />");
                        }
                        litMessages.Text = sb.ToString();
                    }
                    catch (Exception ex)
                    {
                        lblResult.Text = "Error loading messages: " + ex.Message;
                    }
                }
            }
        }
    }
}