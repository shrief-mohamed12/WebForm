using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebForm
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Register a client script block for the AJAX request
            ScriptManager.RegisterStartupScript(this, GetType(), "ScrollScript",
                @"<script type='text/javascript'>
                    var pageIndex = 1;
                    var pageSize = 10;
                    var loading = false;
                    function loadMoreData() {
                        if (loading) return;
                        loading = true;
                        $('#loading').show();
                        $.ajax({
                            type: 'POST',
                            url: 'Default.aspx/LoadData',
                            data: JSON.stringify({ pageIndex: pageIndex, pageSize: pageSize }),
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (response) {
                                if (response.d) {
                                    $('#GridView1 tbody').append(response.d);
                                    pageIndex++;
                                }
                                $('#loading').hide();
                                loading = false;
                            },
                            error: function (error) {
                                console.log(error);
                                $('#loading').hide();
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
                </script>", false);
        }

        [WebMethod]
        public static string LoadData(int pageIndex, int pageSize)
        {
            DataTable dt = GetDataFromDatabase(pageIndex, pageSize);
            return ConvertDataTableToJsonString(dt);
        }

        private static DataTable GetDataFromDatabase(int pageIndex, int pageSize)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WebTest;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Grades ORDER BY GradeId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                int offset = (pageIndex - 1) * pageSize;

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    DataTable dataTable = new DataTable();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    return dataTable;
                }
            }
        }

        private static string ConvertDataTableToJsonString(DataTable dataTable)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>> rows = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();

            foreach (DataRow dr in dataTable.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> row = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            return serializer.Serialize(rows);
        }
    }
}
