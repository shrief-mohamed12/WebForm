<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebForm._Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Scroll Pagination</title>
    <style>
        .gridview {
            width: 100%;
            overflow-x: auto;
            display: block;
        }

        .gridview th, .gridview td {
            white-space: nowrap;
        }

        #loading {
            text-align: center;
            display: none;
        }
    </style>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var pageIndex = 1;
            var pageSize = 10;
            var loading = false;

            function loadMoreData() {
                if (loading) return;
                loading = true;
                $("#loading").show();
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/LoadData",
                    data: JSON.stringify({ pageIndex: pageIndex, pageSize: pageSize }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d) {
                            $('#GridView1 tbody').append(response.d);
                            pageIndex++;
                        }
                        $("#loading").hide();
                        loading = false;
                    },
                    error: function (error) {
                        console.log(error);
                        $("#loading").hide();
                        loading = false;
                    }
                });
            }

            $(window).scroll(function () {
                if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100) {
                    loadMoreData();
                }
            });

            loadMoreData(); // Initial load
        });
    </script>
</head>
<body>
    <form id="form2" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="gridview">
                    <Columns>
                        <asp:BoundField DataField="Grade" HeaderText="Grade" />
                        <asp:BoundField DataField="GradeName" HeaderText="Grade Name" />
                        <asp:BoundField DataField="GradeGroupName" HeaderText="Grade Group Name" />
                        <asp:BoundField DataField="GradeCons" HeaderText="Grade Cons" />
                        <asp:BoundField DataField="GradeNameCons" HeaderText="Grade Name Cons" />
                    </Columns>
                </asp:GridView>
                <div id="loading">Loading...</div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
