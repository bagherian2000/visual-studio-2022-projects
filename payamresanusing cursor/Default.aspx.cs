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
using System.Text.RegularExpressions;

namespace ProjectControlPanelWeb
{
    public partial class Default : Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["PayamResanDB"].ConnectionString;
        private const string QUERY_GET_MESSAGES = "SELECT Number FROM tblMessage ORDER BY Number";
        private const string QUERY_GET_MESSAGE_DETAILS = "SELECT number, message FROM tblMessage";
        private const string QUERY_GET_CALLER_IDS = "SELECT callerid FROM tblallcallerids ORDER BY callerid";

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
            PopulateCalleId();
            LoadProjectInformation();
            chkInternalWave.Checked = true;
        }

        protected void chkInternalWave_CheckedChanged(object sender, EventArgs e)
        {
            // Implementation if needed
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
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        private void PopulateCalleId()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(QUERY_GET_CALLER_IDS, conn))
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        ddlCalleId.Items.Clear();
                        ddlCalleId.Items.Add(new ListItem("-- Select Caller ID --", ""));
                        while (reader.Read())
                        {
                            ddlCalleId.Items.Add(new ListItem(reader["callerid"].ToString(), reader["callerid"].ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleError("خطا در بارگذاری شناسه‌های تماس", ex);
                    throw;
                }
            }
        }

        protected void LoadProjectInformation()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    const string query = "SELECT * FROM vwLastPrj order by startSendDate DESC";
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

                using (var client = new wsRayanehSoapClient())
                {
                    string result = client.Resume(txtProjectId.Text);
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

                using (var client = new wsRayanehSoapClient())
                {
                    string result = client.Pause(txtProjectId.Text);
                    lblResult.Text = result ?? "پروژه با موفقیت متوقف شد.";
                    lblResult.CssClass = "success-message";
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در توقف پروژه", ex);
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

                using (var client = new wsRayanehSoapClient())
                {
                    string result = client.Break(txtProjectId.Text);
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
            string chgTextFileName = null;
            string chgVoiceFileName = null; // Store WAV file name
            bool hasWavFile = false; // Track if WAV file was uploaded
            try
            {
                // Validate at least the text file is uploaded
                if (!FileUpload1.HasFile)
                {
                    HandleError("لطفا فایل متن را انتخاب کنید", new Exception("Text file is required"));
                    return;
                }
                chgTextFileName = "_" + DateTime.Now.Millisecond + "_" + FileUpload1.FileName;
                // Upload text file to FTP (required)
                bool textUploaded = UploadFileToFtp(FileUpload1, chgTextFileName, ConfigurationManager.AppSettings["FTPTextFolder"]);

                // Upload voice file to FTP if provided (optional)
                bool voiceUploaded = true; // default to true if no voice file
                if (wavFileUpload != null && wavFileUpload.HasFile)
                {
                    hasWavFile = true; // Set flag before uploading
                    chgVoiceFileName = "_" + DateTime.Now.Millisecond + "_" + wavFileUpload.FileName;
                    voiceUploaded = UploadFileToFtp(wavFileUpload, chgVoiceFileName, ConfigurationManager.AppSettings["FTPVoiceFolder"]);
                }

                if (textUploaded && voiceUploaded)
                {
                    string successMessage = "فایل متن با موفقیت به سرور FTP ارسال شد.";
                    if (hasWavFile)
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
                    return;
                }
            }
            catch (Exception ex)
            {
                HandleError("خطا در ارسال فایل‌ها", ex);
                return;
            }

            // Ensure chgTextFileName is not null before using it
            if (string.IsNullOrEmpty(chgTextFileName))
            {
                HandleError("خطا: نام فایل متن تولید نشده است", new Exception("chgTextFileName is null"));
                return;
            }

            string strAllParameter = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                Path.GetFileName(chgTextFileName), //0
                chkInternalWave.Checked ? "1" : "0", //1
                ddlInternalWaveNo.SelectedValue, //2 
                hasWavFile ? Path.GetFileName(wavFileUpload.FileName) : "", //3
                txtStartHour.Text, //4
                txtStartMinute.Text, //5
                txtEndHour.Text, //6
                txtEndMinute.Text, //7
                txtDesiredSendNo.Text, //8
                txtTresholdNo.Text, //9
                txtTestRingtime.Text, //10
                Regex.Replace(txtTestNumber1.Text, "[^0-9]", ""), //11
                Regex.Replace(txtTestNumber2.Text, "[^0-9]", ""), //12
                Regex.Replace(txtTestNumber3.Text, "[^0-9]", ""), //13
                ddlCalleId.SelectedValue, //14
                txtMainRingtime.Text, //15
                ddlPriority.SelectedValue); //16

            using (var client = new wsRayanehSoapClient())
            {
                string result = client.CreateNewProject(strAllParameter);
                lblResult.Text = result ?? "پروژه با موفقیت ایجاد شد.";
                lblResult.CssClass = "success-message";
            }
           

            System.Threading.Thread.Sleep(5000);

            bool bolIsOk = false;

            if (lblResult.Text.IndexOf("موفقیت") > 0)
            {
                bolIsOk = true;
                btnSend.Enabled = false;
            }
            else
            {
                bolIsOk = false;
                btnSend.Enabled = true;
            }


        }

        private bool UploadFileToFtp(FileUpload fileUpload, string fileName, string ftpFolder)
        {
            try
            {
                // Get FTP credentials from web.config
                string ftpServer = ConfigurationManager.AppSettings["FTPServerIP"];
                string ftpUsername = ConfigurationManager.AppSettings["FTPUserID"];
                string ftpPassword = ConfigurationManager.AppSettings["FTPPassword"];

                // Create FTP request
                string ftpPath = $"ftp://{ftpServer}/{ftpFolder}/{fileName}";
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