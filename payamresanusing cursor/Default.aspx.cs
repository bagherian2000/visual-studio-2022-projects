using System;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using payamresanusing_cursor.ServiceReference1;
using System.IO;
using System.Net;

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

        protected void btnResume_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtProjectId.Text))
                {
                    HandleError("لطفا شناسه پروژه را وارد کنید", new Exception("Project ID is required"));
                    return;
                }

                using (var client = new wsRayanehSoapClient()) // Adjust class name based on generated proxy
                {
                    string result = client.Resume(txtProjectId.Text); // Adjust method call based on service
                    lblResult.Text = result ?? "پروژه با موفقیت از سر گرفته شد.";
                    lblResult.CssClass = "success-message";
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در از سرگیری پروژه", ex);
            }
        }

        protected void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtProjectId.Text))
                {
                    HandleError("لطفا شناسه پروژه را وارد کنید", new Exception("Project ID is required"));
                    return;
                }

                using (var client = new wsRayanehSoapClient()) // Adjust class name based on generated proxy
                {
                    string result = client.Pause(txtProjectId.Text); // Adjust method call based on service
                    lblResult.Text = result ?? "پروژه با موفقیت متوقف شد.";
                    lblResult.CssClass = "success-message";
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در توقف  پروژه", ex);
            }
        }


        protected void btnBreak_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtProjectId.Text))
                {
                    HandleError("لطفا شناسه پروژه را وارد کنید", new Exception("Project ID is required"));
                    return;
                }

                using (var client = new wsRayanehSoapClient()) // Adjust class name based on generated proxy
                {
                    string result = client.Break(txtProjectId.Text); // Adjust method call based on service
                    lblResult.Text = result ?? "پروژه با موفقیت متوقف شد.";
                    lblResult.CssClass = "success-message";
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در توقف پروژه", ex);
            }
        }


        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate at least the text file is uploaded
                if (FileUpload1.HasFile == false)
                {
                    HandleError("لطفا فایل متن را انتخاب کنید", new Exception("Text file is required"));
                    return;
                }

                // Upload text file to FTP (required)
                bool textUploaded = UploadFileToFtp(FileUpload1, ConfigurationManager.AppSettings["FTPTextFolder"]);

                // Upload voice file to FTP if provided (optional)
                bool voiceUploaded = true; // default to true if no voice file
                if (wavFileUpload != null && wavFileUpload.HasFile)
                {
                    voiceUploaded = UploadFileToFtp(wavFileUpload, ConfigurationManager.AppSettings["FTPVoiceFolder"]);
                }

                if (textUploaded && voiceUploaded)
                {
                    string successMessage = "فایل متن با موفقیت به سرور FTP ارسال شد.";
                    if (wavFileUpload != null && wavFileUpload.HasFile)
                    {
                        successMessage = "فایل‌ها با موفقیت به سرور FTP ارسال شدند.";
                    }
                    lblResult.Text = successMessage;
                    lblResult.CssClass = "success-message";
                }
                else
                {
                    HandleError("خطا در ارسال فایل‌ها به سرور FTP",
                        new Exception(textUploaded ? "Voice file upload failed" : "Text file upload failed"));
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در ارسال فایل‌ها", ex);
            }
        }

        private bool UploadFileToFtp(FileUpload fileUpload, string ftpFolder)
        {
            try
            {
                // Get FTP credentials from web.config
                string ftpServer = ConfigurationManager.AppSettings["FTPServerIP"];
                string ftpUsername = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPPassword"];

                // Create FTP request
                string ftpPath = $"ftp://{ftpServer}/{ftpFolder}{fileUpload.FileName}";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.UseBinary = true;
                request.UsePassive = true;
                request.KeepAlive = false;

                // Write the file to FTP stream
                using (Stream fileStream = fileUpload.PostedFile.InputStream)
                using (Stream ftpStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, read);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }
    }
}