<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProjectControlPanelWeb.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Project Control Panel</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="https://cdn.jsdelivr.net/gh/rastikerdar/vazirmatn@v33.003/Vazirmatn-font-face.css" rel="stylesheet" type="text/css" />
    <link href="Styles/main.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Top Controls -->
            <div class="section">
                <span class="label">1 - telFileName:</span>
                <asp:FileUpload ID="telFileUpload" runat="server" CssClass="input" />
                <span class="label">2 - haveInternalWave:</span>
                <asp:CheckBox ID="chkInternalWave" runat="server" Checked="true" AutoPostBack="true" OnCheckedChanged="chkInternalWave_CheckedChanged" />
                <span class="label">3 - internalWaveNo:</span>
                <asp:DropDownList ID="ddlInternalWaveNo" runat="server" CssClass="input"></asp:DropDownList>
                <asp:Panel ID="pnlFullPathWavFileName" runat="server">
                    <span class="label">4 - fullPathWavFileName:</span>
                    <asp:FileUpload ID="wavFileUpload" runat="server" CssClass="input" />
                </asp:Panel>
            </div>

            <!-- DataGrid for Running Projects -->
            <div class="section groupbox">
                <strong style="direction: rtl; display: block;">نمایش اطلاعات پروژه های در حال اجرا</strong>
                <asp:GridView ID="gvRunning" runat="server" CssClass="grid" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="me" HeaderText="me" />
                        <asp:BoundField DataField="status" HeaderText="status" />
                        <asp:BoundField DataField="totalSuccess" HeaderText="totalSuccess" />
                        <asp:BoundField DataField="curSuccess" HeaderText="curSuccess" />
                        <asp:BoundField DataField="curAnswerTels" HeaderText="curAnswerTels" />
                        <asp:BoundField DataField="curTotalSended" HeaderText="curTotalSended" />
                        <asp:BoundField DataField="totalTelInMainFile" HeaderText="totalTelInMainFile" />
                        <asp:BoundField DataField="strCurSendingNo" HeaderText="strCurSendingNo" />
                        <asp:BoundField DataField="priority" HeaderText="priority" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Project Information Section -->
     <div class="section groupbox">
    <strong style="direction: rtl; display: block;">نمایش اطلاعات پروژه های انجام شده</strong>
    <div style="height: auto; max-height: 120px; overflow-y: auto; direction: rtl;">
        <asp:GridView ID="gvDoneProjects" runat="server" 
            CssClass="project-info-grid"
            AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="projectsId" HeaderText="شناسه پروژه" />
                <asp:BoundField DataField="prjName" HeaderText="نام پروژه" />
                <asp:BoundField DataField="startSendDate" HeaderText="تاریخ شروع" DataFormatString="{0:yyyy/MM/dd}" />
                <asp:BoundField DataField="endSendDate" HeaderText="تاریخ پایان" DataFormatString="{0:yyyy/MM/dd}" />
               <asp:BoundField DataField="status" HeaderText="وضعیت">
                <ItemStyle Width="200px" Wrap="False" />
                </asp:BoundField>

                <asp:BoundField DataField="totalSuccess" HeaderText="تعداد کل تماس‌های موفق" />
                <asp:BoundField DataField="totalTelInMainFile" HeaderText="تعداد کل تماس‌ها" />
            </Columns>
        </asp:GridView>
    </div>
</div>



            <!-- Control Panel -->
            <div class="section groupbox">
                <strong>Control Cur Prj</strong>
                <span class="label">Enter ProjectsID:</span>
                <asp:TextBox ID="txtProjectId" runat="server" CssClass="input" />
                <div class="buttons">
                    <asp:Button ID="btnRequestSettings" runat="server" Text="درخواست تنظیمات" />
                    <asp:Button ID="btnSend" runat="server" Text="Send =>" />
                    <asp:Button ID="btnResume" runat="server" Text="Resume" />
                    <asp:Button ID="btnPause" runat="server" Text="Pause" />
                    <asp:Button ID="btnBreak" runat="server" Text="Break" />
                </div>
            </div>

            <!-- Messages -->
            <div class="section groupbox">
                <strong>Messages</strong>
                <div class="messages-box">
                    <asp:Literal ID="litMessages" runat="server" />
                </div>
            </div>

            <!-- Result Label -->
            <asp:Label ID="lblResult" runat="server" Text="Result" />
        </div>
    </form>
</body>
</html> 