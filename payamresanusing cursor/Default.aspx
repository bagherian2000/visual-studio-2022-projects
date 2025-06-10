<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProjectControlPanelWeb.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Project Control Panel</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="https://cdn.jsdelivr.net/gh/rastikerdar/vazirmatn@v33.003/Vazirmatn-font-face.css" rel="stylesheet" type="text/css" />
    <link href="Styles/main.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function toggleWavPanel(checkbox) {
            var panel = document.getElementById('<%= pnlFullPathWavFileName.ClientID %>');
            if (checkbox.checked) {
                panel.style.display = 'none';
            } else {
                panel.style.display = 'block';
            }
        }

        // Call the function on page load to set initial state
        window.onload = function () {
            var checkbox = document.getElementById('<%= chkInternalWave.ClientID %>');
            toggleWavPanel(checkbox);
        };

        function validateFileUploads() {
            var telFile = document.getElementById('<%= FileUpload1.ClientID %>');
            var chkInternalWave = document.getElementById('<%= chkInternalWave.ClientID %>');
            var wavFile = document.getElementById('<%= wavFileUpload.ClientID %>');
            var callerId = document.getElementById('<%= ddlCalleId.ClientID %>');


            // Check if tel file is selected
            if (!telFile.value) {
                alert('لطفا فایل تلفن را انتخاب کنید');
                return false;
            }

            // If internal wave is not checked, check if wav file is selected
            if (!chkInternalWave.checked && !wavFile.value) {
                alert('لطفا فایل صوتی را انتخاب کنید');
                return false;
            }

            // Check if callerId is selected
            if (!callerId.value) {
                alert('لطفا شناسه تماس گیرنده را انتخاب کنید');
                return false;
            }
            const testNumbers = [
                document.getElementById('<%= txtTestNumber1.ClientID %>'),
                document.getElementById('<%= txtTestNumber2.ClientID %>'),
                document.getElementById('<%= txtTestNumber3.ClientID %>')
            ];

            for (let i = 0; i < testNumbers.length; i++) {
                const input = testNumbers[i];
                if (input.value) {
                    // Remove dashes for validation
                    const digitsOnly = input.value.replace(/\D/g, '');
                    if (!/^(?:\d{8}|\d{11})$/.test(digitsOnly)) {
                        alert(`شماره تست ${i + 1} باید 8 یا 11 رقم باشد (با فرمت صحیح)`);
                        input.focus();
                        return false;
                    }
                }
            }


            return true;
        }

        // Formats input as 8-digit (123-45678) or 11-digit (0912-345-6789)
        function formatPhoneNumber(input) {
            // Remove all non-digits
            let value = input.value.replace(/\D/g, '');

            // Apply mask based on length
            if (value.length <= 8) {
                // 8-digit format: 123-45678
                if (value.length > 3) {
                    value = value.substring(0, 3) + '-' + value.substring(3, 8);
                }
            } else if (value.length <= 11) {
                // 11-digit format: 0912-345-6789
                if (value.length > 4) {
                    value = value.substring(0, 4) + '-' + value.substring(4, 7) + '-' + value.substring(7, 11);
                }
            } else {
                // Trim excess digits
                value = value.substring(0, 11);
            }

            input.value = value;
        }

              
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <!-- Top Controls -->
            <div class="section">
                <div class="top-controls">
                    <div class="control-group">
                        <span class="label">1 - telFileName:</span>
                        <asp:FileUpload ID="FileUpload1" runat="server" CssClass="input" accept=".txt" />
                    </div>
                    <div class="control-group">
                        <span class="label">2 - haveInternalWave:</span>
                        <asp:CheckBox ID="chkInternalWave" runat="server" Checked="true" OnCheckedChanged="chkInternalWave_CheckedChanged" onchange="toggleWavPanel(this);" CssClass="aligned-checkbox" />
                    </div>
                    <div class="control-group">
                        <span class="label">3 - internalWaveNo:</span>
                        <asp:DropDownList ID="ddlInternalWaveNo" runat="server" CssClass="input"></asp:DropDownList>
                    </div>
                </div>
                <asp:Panel ID="pnlFullPathWavFileName" runat="server">
                    <span class="label">4 - fullPathWavFileName:</span>
                    <asp:FileUpload ID="wavFileUpload" runat="server" CssClass="input" accept=".wav" />
                </asp:Panel>
            </div>

            <!-- Time Control Section -->
            <div class="section groupbox">
                <div class="time-call-container">
                    <!-- Time Control Section -->
                    <div class="time-controls">
                        <strong>زمان‌های مجاز تماس</strong>
                        <div class="time-group">
                            <div class="time-label">5-زمان شروع:</div>
                            <div class="time-inputs">
                                <asp:TextBox ID="txtStartHour" runat="server" CssClass="input time-input" Text="8" TextMode="Number" min="8" max="23" Width="60px" />
                                <span class="time-separator">:</span>
                                <asp:TextBox ID="txtStartMinute" runat="server" CssClass="input time-input" Text="0" TextMode="Number" min="0" max="59" Width="60px" />
                            </div>
                        </div>
                        <div class="time-group">
                            <div class="time-label">6-زمان پایان:</div>
                            <div class="time-inputs">
                                <asp:TextBox ID="txtEndHour" runat="server" CssClass="input time-input" Text="21" TextMode="Number" min="8" max="21" Width="60px" />
                                <span class="time-separator">:</span>
                                <asp:TextBox ID="txtEndMinute" runat="server" CssClass="input time-input" Text="0" TextMode="Number" min="0" max="59" Width="60px" />
                            </div>
                        </div>
                    </div>



                            <!-- Vertical Separator -->
        <div class="vertical-separator"></div>

        <!-- Call Parameters Section -->
        <div class="call-parameters">
            <strong>پارامترهای تماس</strong>

             <div class="parameter">
               <span class="label">7 - callerId:</span>
               <asp:DropDownList ID="ddlCalleId" runat="server" CssClass="input" />
              </div>
            <div class="parameter">
                <span class="label">8 - desiredSendNo:</span>
                <asp:TextBox ID="txtDesiredSendNo" runat="server" CssClass="input" Text="1" TextMode="Number" />
            </div>
            <div class="parameter">
                <span class="label">9 - tresholdNo:</span>
                <asp:TextBox ID="txtTresholdNo" runat="server" CssClass="input" Text="1000" TextMode="Number" />
            </div>
            <div class="parameter">
                <span class="label">10 - testRingtime:</span>
                <asp:TextBox ID="txtTestRingtime" runat="server" CssClass="input" Text="13" TextMode="Number" />
            </div>
            <div class="parameter">
                <span class="label">11 - mainRingtime:</span>
                <asp:TextBox ID="txtMainRingtime" runat="server" CssClass="input" Text="45" TextMode="Number" />
            </div>
            <div class="parameter">
                <span class="label">12 - priority:</span>
                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="input">
                    <asp:ListItem Text="low" Value="3" Selected="True" />
                    <asp:ListItem Text="medium" Value="2" />
                    <asp:ListItem Text="high" Value="1" />
                </asp:DropDownList>
            </div>
        </div>




                    <!-- Vertical Separator -->
                    <div class="vertical-separator"></div>

                    <!-- Call Parameters Section -->
 <div class="call-parameters">
                        <strong>شماره های تست</strong>
                        <div class="parameter">
                            <span class="label">13-number1:</span>
                            <asp:TextBox ID="txtTestNumber1" runat="server" CssClass="input" MaxLength="14" oninput="formatPhoneNumber(this)" placeholder="8 or 11 digits" />
                        </div>
                        <div class="parameter">
                            <span class="label">14-number2:</span>
                            <asp:TextBox ID="txtTestNumber2" runat="server" CssClass="input" MaxLength="14" oninput="formatPhoneNumber(this)" placeholder="8 or 11 digits" />
                        </div>
                        <div class="parameter">
                            <span class="label">15-number3:</span>
                            <asp:TextBox ID="txtTestNumber3" runat="server" CssClass="input" MaxLength="14" oninput="formatPhoneNumber(this)" placeholder="8 or 11 digits" />
                        </div>
                    </div>
                </div>
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
                        <asp:BoundField DataField="totalTelInMainFile" HeaderText="curTotalTels" />
                        <asp:BoundField DataField="strCurSendingNo" HeaderText="strCurSendingNo" />
                        <asp:BoundField DataField="priority" HeaderText="priority" />
                    </Columns>
                </asp:GridView>
            </div>

            <!-- Project Information Section -->
            <div class="section groupbox">
                <strong style="direction: rtl; display: block;">نمایش اطلاعات پروژه های انجام شده</strong>
                <div style="max-height: 300px; overflow-y: auto; direction: rtl;">
                    <asp:GridView ID="gvDoneProjects" runat="server"
                        CssClass="project-info-grid"
                        AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="projectsId" HeaderText="شناسه پروژه" />
                            <asp:BoundField DataField="prjName" HeaderText="نام پروژه" />
                            <asp:BoundField DataField="startSendDate" HeaderText="تاریخ شروع" DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:BoundField DataField="endSendDate" HeaderText="تاریخ پایان" DataFormatString="{0:yyyy/MM/dd}" />
                            <asp:BoundField DataField="status" HeaderText="وضعیت" />
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
                  <asp:Button ID="btnSend" runat="server" Text="Send =>" 
    OnClientClick="return validateFileUploads()" 
    OnClick="btnSend_Click" />
                   <asp:Button ID="btnResume" runat="server" Text="Resume" OnClick="btnResume_Click" />
                    <asp:Button ID="btnPause" runat="server" Text="Pause"  OnClick="btnPause_Click"/>
                    <asp:Button ID="btnBreak" runat="server" Text="Break"  OnClick="btnBreak_Click"/>
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