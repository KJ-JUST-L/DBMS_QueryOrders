using System.Data;
using Microsoft.Data.SqlClient;

namespace DBMS_QueryOrders
{
    public partial class DBMS_QueryOrders : Form
    {
        public DBMS_QueryOrders()
        {
            InitializeComponent();
        }

        SqlConnection conn;
        SqlCommand cmd;
        SqlDataAdapter da;

        public void connect()
        {
            string server = @".\SQLEXPRESS";
            string db = "northwind";
            string strConn = string.Format(@"Data Source={0};Initial Catalog={1};Integrated Security=True ;Encrypt=False", server, db);
            conn = new SqlConnection(strConn);
            conn.Open();
        }
        public void disconnect()
        {
            conn.Close();
        }

        private void showData(string sql, DataGridView dgv)
        {
            da = new SqlDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dgv.DataSource = ds.Tables[0];
            dgvOrders.DataSource = ds.Tables[0];
        }

        private void DBMS_QueryOrders_Load(object sender, EventArgs e)
        {
            connect();
            string sqlQuery = "SELECT o.OrderID, o.OrderDate, FORMAT(o.RequiredDate, 'dd-MM-yyyy'), s.CompanyName AS ShipperCompany,  CONCAT(e.TitleOfCourtesy, e.FirstName, ' ', e.LastName) as employeeName, c.CompanyName AS CustomerCompany,  c.Phone,  SUM(od.Quantity * od.UnitPrice * (1 - od.Discount)) AS totalCash FROM [Order Details] od JOIN Orders o ON o.OrderID = od.OrderID  JOIN Shippers s ON s.ShipperID = o.ShipVia  JOIN Employees e ON e.EmployeeID = o.EmployeeID  JOIN Customers c ON c.CustomerID = o.CustomerID GROUP BY  o.OrderID,  o.OrderDate, o.RequiredDate,  s.CompanyName, CONCAT(e.TitleOfCourtesy, e.FirstName, ' ', e.LastName), c.CompanyName, c.Phone";
            showData(sqlQuery, dgvOrders);
        }

        private void dgvOrders_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                int id = Convert.ToInt32(dgvOrders.CurrentRow.Cells[0].Value);
                //MessageBox.Show(id.ToString(), "รหัสที่เลือก");
                string sqlQuery = "SELECT p.ProductID, p.ProductName, od.Quantity, od.UnitPrice, (od.Discount * 100) AS Discount, CONVERT(decimal(10, 2), od.Quantity * od.UnitPrice * (1 - od.Discount)) AS totalSellPrice, od.Quantity * od.UnitPrice * od.Discount AS totalDiscount, CONVERT(decimal(10, 2), (od.Quantity * od.UnitPrice * (1 - od.Discount)) - od.Quantity * od.UnitPrice * od.Discount) AS totalExtraCharge FROM [Order Details] od JOIN Orders o ON o.OrderID = od.OrderID JOIN Products p ON p.ProductID = od.ProductID WHERE o.OrderID = @id";
                cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dgvDetails.DataSource = ds.Tables[0];
            }
        }
    }
}
