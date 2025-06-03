using System;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;

namespace ProjectControlPanelWeb
{
    public partial class Default : Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["PayamResanDB"].ConnectionString;
        private const string QUERY_GET_MESSAGES = "SELECT Number FROM tblMessage ORDER BY Number";
        private const string QUERY_GET_MESSAGE_DETAILS = "SELECT number, message FROM tblMessage";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    InitializePage();
                }
                catch (Exception ex)
                {
                    HandleError("خطا در بارگذاری صفحه", ex);
                }
            }
        }

        private void InitializePage()
        {
            PopulateInternalWaveNo();
            PopulateMessages();
            LoadProjectInformation();
            chkInternalWave.Checked = true;
           
        }

        protected void chkInternalWave_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void PopulateInternalWaveNo()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(QUERY_GET_MESSAGES, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            ddlInternalWaveNo.Items.Clear();
                            while (reader.Read())
                            {
                                ddlInternalWaveNo.Items.Add(reader["Number"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleError("خطا در بارگذاری شماره‌های داخلی", ex);
                        throw;
                    }
                }
            }
        }

        private void PopulateMessages()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(QUERY_GET_MESSAGE_DETAILS, conn))
                {
                    try
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var sb = new StringBuilder();
                            while (reader.Read())
                            {
                                AppendMessageToBuilder(sb, reader);
                            }
                            litMessages.Text = sb.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleError("خطا در بارگذاری پیام‌ها", ex);
                        throw;
                    }
                }
            }
        }

        private void AppendMessageToBuilder(StringBuilder sb, SqlDataReader reader)
        {
            sb.Append("<div class='message-item'>");
            sb.Append(FormatMessageContent(reader));
            sb.Append("</div>");
            sb.Append("<hr class='message-divider' />");
        }

        private string FormatMessageContent(SqlDataReader reader)
        {
            var number = Server.HtmlEncode(reader["number"].ToString())
                             .Replace(Environment.NewLine, "<br/>");
            var message = Server.HtmlEncode(reader["message"].ToString())
                              .Replace(Environment.NewLine, "<br/>");

            return $"{number} - {message}";
        }

        private void HandleError(string userMessage, Exception ex)
        {
            // Log the error
            LogError(ex);

            // Show user-friendly message
            lblResult.Text = $"{userMessage}: {ex.Message}";
            lblResult.CssClass = "error-message";
        }

        private void LogError(Exception ex)
        {
            // You can implement your preferred logging mechanism here
            // For example, using System.Diagnostics.EventLog or a custom logging solution
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        protected void LoadProjectInformation()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    const string query = "SELECT * FROM vwLastPrj  order by startSendDate DESC";
                    using (var adapter = new SqlDataAdapter(query, connection))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        gvDoneProjects.DataSource = dt;
                        gvDoneProjects.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در بارگذاری اطلاعات پروژه", ex);
            }
        }

        protected void gvProjectInfo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDoneProjects.PageIndex = e.NewPageIndex;
            LoadProjectInformation();
        }
    }
}