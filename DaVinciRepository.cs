using DaVinci.Models.DaVinciAdmin;
using DaVinciAdminApi.Helper;
using DaVinciAdminApi.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DaVinciAdminApi.Repositories
{
    public class DaVinciRepository : IDaVinci
    {
        #region Global Variable
        private static string connStr;
        private static StringBuilder ConnStr = new StringBuilder();
        private static DataTable TempTable = new DataTable();
        private static DataSet TempDataSet = new DataSet();
        private static DataTable _tempDataTable = new DataTable();
        private static DataTable dt = new DataTable();
        private static int ReturnValue = 0;
        private static int TempReturnValue = 0;
        private static DataTable TempDataTable = new DataTable();
        private static Dictionary<string, string> Params = new Dictionary<string, string>();
        #endregion Global Variable

        #region AppData Folder
        #region AppData
        public void L_GetCompanyInfo(int TillId, out int CompanyId, out int CompanyContactId, out string CompanyName, out byte[] Logo, out byte[] Icon, out int IsActive, out string LocalCurrencyCode)
        {
            int t = 0;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("App.GetCompanyInfo", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Add("@TillId", SqlDbType.VarChar).Value = TillId;

                    try
                    {

                        if (sqlconn.State != ConnectionState.Open)
                        {
                            sqlconn.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            TempTable.Clear();
                            da.Fill(TempTable);
                        }
                        if (TempTable.Rows.Count > 0)
                        {
                            TillId = Convert.ToInt32(TempTable.Rows[0]["TillId"]);
                            CompanyId = Convert.ToInt32(TempTable.Rows[0]["CompanyId"]);
                            CompanyContactId = Convert.ToInt32(TempTable.Rows[0]["ContactId"].ToString());
                            CompanyName = TempTable.Rows[0]["CompanyName"].ToString();
                            LocalCurrencyCode = TempTable.Rows[0]["LocalCurrencyCode"].ToString();
                            if (!DBNull.Value.Equals(TempTable.Rows[0]["Logo"]))
                            {
                                Logo = ((byte[])TempTable.Rows[0]["Logo"]);
                            }
                            else
                            {
                                Logo = null;
                            }

                            if (!DBNull.Value.Equals(TempTable.Rows[0]["Icon"]))
                            {
                                Icon = ((byte[])TempTable.Rows[0]["Icon"]);
                            }
                            else
                            {
                                Icon = null;
                            }

                            IsActive = Convert.ToInt32(TempTable.Rows[0]["IsActive"]);
                        }
                        else
                        {
                            TillId = t;
                            CompanyId = 0;
                            CompanyContactId = 0;
                            CompanyName = "N/A";
                            Logo = null;
                            Icon = null;
                            IsActive = -2;
                            LocalCurrencyCode = "N/A";
                        }

                        if (sqlconn.State != ConnectionState.Closed)
                        {
                            sqlconn.Close();
                        }

                    }
                    catch (Exception)
                    {
                        TillId = t;
                        CompanyId = 0;
                        CompanyContactId = 0;
                        CompanyName = "N/A";
                        Logo = null;
                        Icon = null;
                        IsActive = -2;
                        LocalCurrencyCode = "N/A";
                    }

                }
            }
        }
        public DataTable L_GetAssistants()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("App.GetAssistants", sqlconn) { CommandType = CommandType.StoredProcedure })
                {

                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                    }

                    IDataReader rdr = sqlcmd.ExecuteReader();
                    TempTable = DataReader2DataTable(rdr);

                    if (sqlconn.State != ConnectionState.Closed)
                    {
                        sqlconn.Close();
                    }

                }
            }
            return TempTable;
        }
        public DataTable DataReader2DataTable(IDataReader dataReader)
        {
            using (DataTable schemaTable = dataReader.GetSchemaTable())
            {
                using (DataTable resultTable = new DataTable())
                {
                    foreach (DataRow dataRow in schemaTable.Rows)
                    {
                        using (DataColumn dataColumn = new DataColumn
                        {
                            ColumnName = dataRow["ColumnName"].ToString(),
                            DataType = Type.GetType(dataRow["DataType"].ToString()),
                            ReadOnly = (bool)dataRow["IsReadOnly"],
                            AutoIncrement = (bool)dataRow["IsAutoIncrement"],
                            Unique = (bool)dataRow["IsUnique"]
                        })
                        {
                            resultTable.Columns.Add(dataColumn);
                        }
                    }
                    while (dataReader.Read())
                    {
                        DataRow dataRow = resultTable.NewRow();
                        for (int i = 0; i < resultTable.Columns.Count - 1; i++)
                        {
                            dataRow[i] = dataReader[i];
                        }
                        resultTable.Rows.Add(dataRow);
                    }
                    return resultTable;
                }
            }
        }
        public Dictionary<string, string> L_GetParameters()
        {
            Params.Clear();
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("App.GetParameters", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataTable table = new DataTable())
                        {
                            if (sqlconn.State != ConnectionState.Open)
                            {
                                sqlconn.Open();
                            }

                            sqlda.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    Params.Add(table.Rows[i]["Parameter"].ToString(), table.Rows[i]["Value"].ToString());
                                }
                            }

                            if (sqlconn.State != ConnectionState.Closed)
                            {
                                sqlconn.Close();
                            }
                        }
                    }

                }
            }
            return Params;
        }
        public DataTable GetAllActiveTills()
        {
            DataTable d = new DataTable();
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Till.GetAllActive", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    using (SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataTable table = new DataTable())
                        {
                            if (sqlconn.State != ConnectionState.Open)
                            {
                                sqlconn.Open();
                            }

                            sqlda.Fill(table);

                            d = table;

                            if (sqlconn.State != ConnectionState.Closed)
                            {
                                sqlconn.Close();
                            }
                        }
                    }

                }
            }

            return d;
        }
        public bool UpdateTillImages(int userid, int tillid, int imgid, string imagename, string imagetype, decimal sizekb, string pixels, byte[] logo, byte[] ico, byte[] imgbutton)
        {
            bool isSucceeded = false;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Till.UpdateImages", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userid;
                    sqlcmd.Parameters.Add("@TillId", SqlDbType.Int).Value = tillid;
                    sqlcmd.Parameters.Add("@ImgId", SqlDbType.Int).Value = imgid;
                    sqlcmd.Parameters.Add("@ImgName", SqlDbType.NVarChar).Value = imagename;
                    sqlcmd.Parameters.Add("@ImgType", SqlDbType.VarChar).Value = imagetype;
                    sqlcmd.Parameters.Add("@SizeKB", SqlDbType.Decimal).Value = sizekb;
                    sqlcmd.Parameters.Add("@Pixels", SqlDbType.VarChar).Value = pixels;
                    sqlcmd.Parameters.Add("@ImgButton", SqlDbType.VarBinary).Value = imgbutton;
                    sqlcmd.Parameters.Add("@Logo", SqlDbType.VarBinary).Value = logo;
                    sqlcmd.Parameters.Add("@Icon", SqlDbType.VarBinary).Value = ico;

                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                    }
                    try
                    {
                        isSucceeded = sqlcmd.ExecuteNonQuery() > 0;
                    }
                    catch (Exception)
                    {
                        isSucceeded = false;
                    }


                    if (sqlconn.State != ConnectionState.Closed)
                    {
                        sqlconn.Close();
                    }

                }
            }

            return isSucceeded;
        }
        public DataTable UpdatePIDNumber(int userId, int tillid, string pID)
        {
            DataTable d = new DataTable();
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Till.UpdatePIDNumber", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlcmd.Parameters.Add("@TillId", SqlDbType.Int).Value = tillid;
                    sqlcmd.Parameters.Add("@PID", SqlDbType.NVarChar).Value = pID;
                    using (SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataTable table = new DataTable())
                        {
                            if (sqlconn.State != ConnectionState.Open)
                            {
                                sqlconn.Open();
                            }

                            sqlda.Fill(table);

                            d = table;

                            if (sqlconn.State != ConnectionState.Closed)
                            {
                                sqlconn.Close();
                            }
                        }
                    }

                }
            }

            return d;
        }
        #endregion

        #region VersionType
        public DataTable GetDisplayAllVersionWithVersionType(int UpdateVersionTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("App.DisplayAllVersionWithVersionType", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UpdateVersionTypeId", SqlDbType.Int).Value = UpdateVersionTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllVersionDetailsWithUpdateVersionId(int UpdateVersionId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("App.DisplayAllVersionDetailsWithUpdateVersionId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UpdateVersionId", SqlDbType.Int).Value = UpdateVersionId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region AppSettingsFolder
        #region Skins

        public string L_GetUserSkin(int UserId)
        {
            string SkinName = String.Empty;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("App.GetUserSkin", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        TempTable.Clear();
                        da.Fill(TempTable);
                    }

                    //IDataReader rdr = sqlcmd.ExecuteReader();
                    //TempTable = Database.DataReader2DataTable(rdr);

                    if (TempTable.Rows.Count > 0)
                    {
                        SkinName = TempTable.Rows[0][0].ToString();
                    }
                    if (sqlconn.State != ConnectionState.Closed)
                    {
                        sqlconn.Close();
                    }


                }
            }
            return SkinName;
        }

        public bool L_AddUserSkin(int UserId, string SkinName)
        {
            bool IsSaved = false;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("App.SetUserSkin", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                    sqlcmd.Parameters.Add("@SkinName", SqlDbType.VarChar).Value = SkinName;


                    if (sqlconn.State != ConnectionState.Open)
                    {
                        sqlconn.Open();
                    }

                    IsSaved = (sqlcmd.ExecuteNonQuery() > 0);

                    if (sqlconn.State != ConnectionState.Closed)
                    {
                        sqlconn.Close();
                    }


                }
            }
            return IsSaved;
        }

        #endregion

        #endregion

        #region BalanceSheet Folder

        #region AccountTransactionHistory
        public DataTable GetDisplayAccountTransactionHistoryByDateAndTill(int TillId, DateTime TransactionDateTime)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayAccountTransactionHistoryByDateAndTill", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@TransactionDateTime", SqlDbType.DateTime).Value = TransactionDateTime;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region CompanyDetails
        public DataSet GetDisplayCompanyDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayCompanyDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetDisplayCompanyDetailsNotAll()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayCompanyDetailsNotAll", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetSummaryReportSalePerformanceAnalysis(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSummaryReportSalePerformanceAnalysis", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public bool DisplayAddUpdateBalancesSheet(int TillID, DateTime AccountingDate)
        {
            bool IsPassed = false;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayAddUpdateBalancesSheet", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillID", SqlDbType.Int).Value = TillID;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        IsPassed = sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

            return IsPassed;
        }



        public DataSet DisplayAddUpdateSalesHeadCount(DataTable SalesHeadCount, int LocationId, int CompanyId, int UpdatedBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayAddUpdateSalesHeadCount", sqlconn)
                    { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SalesHeadCount", SqlDbType.Structured).Value = SalesHeadCount;
                        sqlCmd.Parameters.Add("@LocationId", SqlDbType.Int).Value = LocationId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = UpdatedBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }

                        return TempDataSet;
                    }
                }
                catch (Exception ex)
                {
                    return TempDataSet;
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region CompanyDetailsBrand
        public DataSet DisplayCompanyDetailsForBrand()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayCompanyDetailsForBrand", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet DisplayLocationDetailsForBrand(int companyParentId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayLocationDetailsForBrand", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = companyParentId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet DisplayBrandDetailsForLocation(int companyParentId, int companyId, DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayBrandDetailsForLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = companyParentId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetStockQtyandPricesCompanyDetails(string BrandId, int CompanyId, DateTime FromDate, DateTime Todate, int ParentCompanyId)
        {
            //string connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockQtyandPricesCompanyDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BrandId", SqlDbType.NVarChar).Value = BrandId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = Todate;
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = ParentCompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable ds = new DataTable())
                            {
                                da.Fill(ds);
                                TempDataTable = ds;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region MoneyTransferHistory

        public DataSet GetDisplayAllCurrentMoneyTransferDetails(int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayAllCurrentMoneyTransferDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetDisplayReportForSelectTransferSpecification(int CashTransferId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.DisplayReportForSelectTransferSpecificationWithAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CashTransferId", SqlDbType.Int).Value = CashTransferId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region CashMgmtFolder

        #region AccountingDate
        //public  DataTable GetDisplayDefaultAccountTransactionDetails(int TillId)
        //{
        //    using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
        //    {
        //        try
        //        {
        //            using (SqlCommand sqlCmd = new SqlCommand("Account.VerifyAccountingDateOpenOrClose", sqlconn) { CommandType = CommandType.StoredProcedure })
        //            {
        //                sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
        //                if (sqlconn.State == ConnectionState.Closed)
        //                {
        //                    sqlconn.Open();
        //                }
        //                TempDataSet = new DataSet();
        //                using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
        //                {
        //                    using (DataSet ds = new DataSet())
        //                    {
        //                        da.Fill(ds);
        //                        TempTable = ds.Tables[0];
        //                    }
        //                }
        //                return TempTable;
        //            }
        //        }
        //        finally
        //        {

        //            if (sqlconn.State == ConnectionState.Open)
        //            {
        //                sqlconn.Close();
        //            }
        //        }
        //    }
        //}


        public DataTable GetCloseCurrentAccountingDate(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.CloseCurrentAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetOpenNewAccountingDate(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.OpenNewAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region  AccountTransaction
        public DataSet GetDisplayDefaultAccountTransactionDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayDefaultAccountTransactionDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAccountTransactionHistory(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayAccountTransactionHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable GetAddNewAccountTransaction(int TillId, int UserId, decimal CurrencyAmount, string TransactionDesc, int CurrencyId, int AccountId, int TransactionTypesId, int DenominationTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.AddNewAccountTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CurrencyAmount", SqlDbType.Decimal).Value = CurrencyAmount;
                        sqlCmd.Parameters.Add("@TransactionDesc", SqlDbType.VarChar).Value = TransactionDesc;
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@AccountId", SqlDbType.Int).Value = AccountId;
                        sqlCmd.Parameters.Add("@TransactionTypesId", SqlDbType.Int).Value = TransactionTypesId;
                        sqlCmd.Parameters.Add("@DenominationTypeId", SqlDbType.Int).Value = DenominationTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region CashTransfer
        public DataTable GetDisplayTillCashTransfers(DateTime FromDate, DateTime ToDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.GetCashTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayReloadTillCashTransfers(DateTime RunningDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.ReloadUpdateCashTransfers_RunByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@RunningDate", SqlDbType.Date).Value = RunningDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Currency
        public DataTable DisplayActiveCurrency()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayActiveCurrency", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplauUpdateCurrencyIcon(int CurrencyId, byte[] CurrencyIcon)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplauUpdateCurrencyIcon", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@CurrencyIcon", SqlDbType.VarBinary).Value = CurrencyIcon;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region EditAccountTransaction
        public DataTable GetDisplayAccountTransactionHistoryByAccountTransId(long AccountTransId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayAccountTransactionHistoryByAccountTransId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@AccountTransId", SqlDbType.BigInt).Value = AccountTransId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayEditTransactionHistory(long AccountTransId, int AccountId, int DenominationValueTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.EditTransactionHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@AccountTransId", SqlDbType.BigInt).Value = AccountTransId;
                        sqlCmd.Parameters.Add("@AccountId", SqlDbType.Int).Value = AccountId;
                        sqlCmd.Parameters.Add("@DenominationValueTypeId", SqlDbType.Int).Value = DenominationValueTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region EditCashTransferSpeicification
        public DataSet GetHODisplayCashTransferForTill(int TillId, DateTime CashTransferDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.HODisplayCashTransferForTill", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CashTransferDate", SqlDbType.Date).Value = CashTransferDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetHODisplayCloseAccountingDate(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.HODisplayCloseAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetHODenominationTotalValueInUSD(int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.HODenominationTotalValueInUSD", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        //public  DataTable GetDisplayDenominationValueTypes()
        //{
        //    using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
        //    {
        //        try
        //        {
        //            using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayDenominationValueTypes", sqlconn) { CommandType = CommandType.StoredProcedure })
        //            {
        //                if (sqlconn.State == ConnectionState.Closed)
        //                {
        //                    sqlconn.Open();
        //                }
        //                TempDataSet = new DataSet();
        //                using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
        //                {
        //                    using (DataSet ds = new DataSet())
        //                    {
        //                        da.Fill(ds);
        //                        TempTable = ds.Tables[0];
        //                    }
        //                }
        //                return TempTable;
        //            }
        //        }
        //        finally
        //        {

        //            if (sqlconn.State == ConnectionState.Open)
        //            {
        //                sqlconn.Close();
        //            }
        //        }
        //    }
        //}


        public DataTable GetHODisplayDenominationTypeDetails(int DenominationValueTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.HODisplayDenominationTypeDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DenominationValueTypeId", SqlDbType.Int).Value = DenominationValueTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetHODisplayDenominationTypeDetails(int TillId, int DenominationTypeId, DateTime AccountingDate, int CashTransferId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.HODisplayDenominationTotalValues", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@DenominationTypeId", SqlDbType.Int).Value = DenominationTypeId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        sqlCmd.Parameters.Add("@CashTransferId", SqlDbType.Int).Value = CashTransferId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetHODisplayDenominationTypeDetails4Drawer(int TillId, int DenominationTypeId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.HODisplayDenominationTotalValues4Drawer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@DenominationTypeId", SqlDbType.Int).Value = DenominationTypeId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetUpdateCashTransfersWithCashSpecification(DataTable CashTransfersWithCashSpecification, int UserId, int CashTransferId,
                                    int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.UpdateCashTransfersWithCashSpecification", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CashTransfersWithCashSpecification", SqlDbType.Structured).Value = CashTransfersWithCashSpecification;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CashTranferId", SqlDbType.Int).Value = CashTransferId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetUpdateCashTransfersWithCashSpecification4Drawer(DataTable CashTransfersWithCashSpecification, int UserId,
                                   int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.UpdateCashTransfersWithCashSpecification4Drawer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CashTransfersWithCashSpecification", SqlDbType.Structured).Value = CashTransfersWithCashSpecification;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetHOUpdateAccountTransaction(int TillId, int UserId, decimal CurrencyAmount, string TransactionDesc, int CurrencyId, int AccountId, int TransactionTypesId,
                int DenominationTypeId, long AccountTransId, DateTime AccountingDate, int EditTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.HOUpdateAccountTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CurrencyAmount", SqlDbType.Decimal).Value = CurrencyAmount;
                        sqlCmd.Parameters.Add("@TransactionDesc", SqlDbType.VarChar).Value = TransactionDesc;
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@AccountId", SqlDbType.Int).Value = AccountId;
                        sqlCmd.Parameters.Add("@TransactionTypesId", SqlDbType.Int).Value = TransactionTypesId;
                        sqlCmd.Parameters.Add("@DenominationTypeId", SqlDbType.Int).Value = DenominationTypeId;
                        sqlCmd.Parameters.Add("@AccountTransId", SqlDbType.BigInt).Value = AccountTransId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        sqlCmd.Parameters.Add("@EditTypeId", SqlDbType.Int).Value = EditTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable GetCurrentCashTransferSpecificationChanged(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.CurrentCashTransferSpecificationChanged", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ExchangeRate
        //public  DataTable GetDisplayCompanyDetails()
        //{
        //    using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
        //    {
        //        try
        //        {
        //            using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayCompanyDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
        //            {
        //                if (sqlconn.State == ConnectionState.Closed)
        //                {
        //                    sqlconn.Open();
        //                }
        //                TempTable = new DataTable();
        //                using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
        //                {
        //                    using (DataTable dt = new DataTable())
        //                    {
        //                        da.Fill(dt);
        //                        TempTable = dt;
        //                    }
        //                }
        //                return TempTable;
        //            }
        //        }
        //        finally
        //        {

        //            if (sqlconn.State == ConnectionState.Open)
        //            {
        //                sqlconn.Close();
        //            }
        //        }
        //    }
        //}

        public DataTable GetDisplayCompanyExchangeRateForEveryLocation(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayCompanyExchangeRateForEveryLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayCompanyDetailsWithAll()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayCompanyDetailsWithAll", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayExchangeRateHistory(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayExchangeRateHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayCompanyExchangeRateDetails(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.DisplayCompanyExchangeRateDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetUpdateCompanyExchangeRate(int CompanyExchangeRatesId, int CurrencyId, int CompanyId, int UserId, decimal CurrencyRate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Currency.UpdateCompanyExchangeRate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyExchangeRatesId", SqlDbType.Int).Value = CompanyExchangeRatesId;
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@CurrencyRate", SqlDbType.Decimal).Value = CurrencyRate;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Common Folder
        #region Country
        public DataTable GetCountries()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("General.GetCountries", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "Countries";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Locations
        public DataTable GetLocations()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("General.GetLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "Locations";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region MaritalStatus
        public DataTable GetMaritalStatuses()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("General.GetMaritalStatus", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "MaritalStatuses";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion 

        #region Inventory Folder

        #region Inventory

        public int L_CreateNewInventory(int CompanyId, int StockLocationId, int UserId, string Note)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.New", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@Note", SqlDbType.VarChar).Value = Note;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        SqlParameter retval = sqlCmd.Parameters.Add("@b", SqlDbType.Int);
                        retval.Direction = ParameterDirection.ReturnValue;
                        sqlCmd.ExecuteNonQuery();
                        return (int)sqlCmd.Parameters["@b"].Value;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetInventoryByLocId(int CompanyId, int StockLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetInventoryByLocId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@LocationId", SqlDbType.Int).Value = StockLocationId;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetInventoryItemsById(int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetInventoryItemsById", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_NewCount(int InventoryId, int UserId, string Barcode, decimal Qty, int SectionId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.NewCount", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Qty;
                        sqlCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = SectionId;
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_UpdateCountedQty(int InventoryId, int UserId, string Barcode, decimal Qty)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.UpdateCountedQty", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Qty;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable UpdateCountedQty_Troubleshooting(int InventoryId, int UserId, int ItemId, decimal Qty)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.UpdateCountedQty_Troubleshooting", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Qty;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void MakeWriteOff(int CompanyId, int StockLocId, string Comment, int UserId, DataTable Items2Writeoff, int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("WriteOff.MakeWriteOff", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocId;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@Items2Writeoff", SqlDbType.Structured).Value = Items2Writeoff;


                        sqlCmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_NewCount_Bulk(int InventoryId, int UserId, DataTable dt)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.NewCount_Bulk", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CountedItems", SqlDbType.Structured).Value = dt;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("CountedItems"))
                            {
                                da.Fill(data);
                                TempTable = data;
                            }
                        }
                        TempTable.TableName = "CountedItems";
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region InventoryWithAddStock

        public int L_DisplayJobUpdateInventory(int InventoryId, int IndividualItemId, bool IsLastItem, int SectionId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.UpdateInventoryWithInvidualItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        sqlCmd.Parameters.Add("@IndividualItemId", SqlDbType.Int).Value = IndividualItemId;
                        sqlCmd.Parameters.Add("@IsLastItem", SqlDbType.Bit).Value = IsLastItem;
                        sqlCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = SectionId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        SqlParameter retval = sqlCmd.Parameters.Add("@ReturnItemId", SqlDbType.Int);
                        retval.Direction = ParameterDirection.ReturnValue;
                        sqlCmd.ExecuteNonQuery();
                        return (int)sqlCmd.Parameters["@ReturnItemId"].Value;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Financial Report Folder

        #region DailyCashTransaction
        public DataSet GetDisplayAccountCashTransactions(DateTime FromDate, DateTime ToDate, int CurrencyId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.GetCashTransactions", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayGetSalesUsingCompanyDate(int CompanyId, DateTime TransactedOn)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesUsingCompanynDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@TransactedOn", SqlDbType.Date).Value = TransactedOn;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayGetTransactionsUsingCompanynDate(int CompanyId, DateTime TransactedOn)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.GetTransactionsUsingCompanynDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@TransactedOn", SqlDbType.Date).Value = TransactedOn;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayStockDisplayDailyTransferredValues(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayDailyTransferredValues", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region StockMovements

        public DataSet GetDisplayFinancialStockMovements(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayFinancialStockMovements", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayFinancialItemFlow(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayFinancialItemFlow", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayFinancialPurchasedItems(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayFinancialPurchasedItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        #endregion

        #region TransactionPerDay

        public DataTable GetDisplayGetPurchasedQtyPerDay(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetPurchasedQtyPerDay", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplaySoldQtyPerDay(int StockLocationId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSoldQtyPerDay", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region TransfersBalancesInOut

        public DataSet GetDisplayGetTransfersBalances(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetTransfersBalances", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayGetTransfersBalancesReloadDate(DateTime ReloadDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.UpdateReloadTransfersBalances_RunByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ReloadDate", SqlDbType.Date).Value = ReloadDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion
        #endregion

        #region Gift Certificate Folder

        #region GiftCertificateCancel
        public DataSet GetDisplayCouponAmountOptionsDetails(int TillId, string GiftBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.GiftCertificatesDetailsWithBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@GiftBarcode", SqlDbType.VarChar).Value = GiftBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetCancelCouponDetailsWithReturnAmount(DataTable CouponPaymentDetails, int UserId, decimal ReturnAmount, string CancelComment, int GiftCertificatesDetailId, DataTable SalesPaymentCashAmount, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.CancelCouponDetailsWithReturnAmount", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CouponPaymentDetails", SqlDbType.Structured).Value = CouponPaymentDetails;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@ReturnAmount", SqlDbType.Decimal).Value = ReturnAmount;
                        sqlCmd.Parameters.Add("@CancelComment", SqlDbType.VarChar).Value = CancelComment;
                        sqlCmd.Parameters.Add("@GiftCertificatesDetailId", SqlDbType.Int).Value = GiftCertificatesDetailId;
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = SalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region GiftCertificateCancelByHO
        public DataTable GetDisplayCouponAmountOptionsDetails(string GiftCertificateBarcode, DateTime FromDate, DateTime ToDate, bool IsBarcodeSearch)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.GiftCertificatesDetailsWithBarcodeForHO", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@GiftCertificateBarcode", SqlDbType.VarChar).Value = GiftCertificateBarcode;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@IsBarcodeSearch", SqlDbType.Bit).Value = IsBarcodeSearch;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetCancelGiftCertificatesDetailsWithBarcodeForHO(int CouponId, int GiftCertificatesDetailId, int CouponCancelledBy, string CancelledComment)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.CancelGiftCertificatesDetailsWithBarcodeForHO", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CouponId", SqlDbType.Int).Value = CouponId;
                        sqlCmd.Parameters.Add("@GiftCertificatesDetailId", SqlDbType.Int).Value = GiftCertificatesDetailId;
                        sqlCmd.Parameters.Add("@CouponCancelledBy", SqlDbType.Int).Value = CouponCancelledBy;
                        sqlCmd.Parameters.Add("@CancelledComment", SqlDbType.VarChar).Value = CancelledComment;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayCoupons4Cancellation(string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayCoupons4Cancellation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void CancelCouponTypeOne(int couponid, string canceltext, int userid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.CancelCouponTypeOne", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CouponId", SqlDbType.Int).Value = couponid;
                        sqlCmd.Parameters.Add("@CancelledComment", SqlDbType.VarChar).Value = canceltext;
                        sqlCmd.Parameters.Add("@CouponCancelledBy", SqlDbType.Int).Value = userid;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region GiftCertificatePayment
        public DataSet GetDisplayGiftCertificatePendingPaymentDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayGiftCertificatePendingPaymentDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddUpdateCouponAmountOptions(DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, int GiftCertificatesDetailId, int TillId, int UsedId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.AddGiftCertificatesPendingPaymentTransactionDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = SalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@SalesReturnCashAmount", SqlDbType.Structured).Value = SalesReturnCashAmount;
                        sqlCmd.Parameters.Add("@GiftCertificatesDetailId", SqlDbType.Int).Value = GiftCertificatesDetailId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@UsedId", SqlDbType.Int).Value = UsedId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region GiftCerts
        public DataSet GetDisplayCouponAmountOptionsDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayCouponAmountOptionsDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddUpdateCouponAmountOptions(int CurrencyId, int PickId, decimal PickAmount, bool IsActive, string Descr, int ParentCompanyId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.AddUpdateCouponAmountOptions", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@PickId", SqlDbType.Int).Value = PickId;
                        sqlCmd.Parameters.Add("@PickAmount", SqlDbType.Decimal).Value = PickAmount;
                        sqlCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        sqlCmd.Parameters.Add("@Descr", SqlDbType.VarChar).Value = Descr;
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = ParentCompanyId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region NewGiftCertificate
        public DataSet GetDisplayGiftCertificatesPickAmountDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayGiftCertificatesPickAmountDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetAddGiftCertificatesWithDetailsReports(int TillId, int CustomerId, int UserId, decimal TotalAmountInUSD, DataTable GiftCertificateDetails, DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, string CustomerName)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.AddGiftCertificatesWithDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TotalAmountInUSD", SqlDbType.Decimal).Value = TotalAmountInUSD;
                        sqlCmd.Parameters.Add("@GiftCertificateDetails", SqlDbType.Structured).Value = GiftCertificateDetails;
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = SalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@SalesReturnCashAmount", SqlDbType.Structured).Value = SalesReturnCashAmount;
                        sqlCmd.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = CustomerName;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddGiftCertificatesWithDetails(int CurrencyId, int PickId, decimal PickAmount, bool IsActive, string Descr)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.AddGiftCertificatesWithDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        sqlCmd.Parameters.Add("@PickId", SqlDbType.Int).Value = PickId;
                        sqlCmd.Parameters.Add("@PickAmount", SqlDbType.Decimal).Value = PickAmount;
                        sqlCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        sqlCmd.Parameters.Add("@Descr", SqlDbType.VarChar).Value = Descr;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region RevalidateCoupons
        public DataTable GetDisplayRevalidateCouponDetails(int TillId, string CouponBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayRevalidateCouponDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CouponBarcode", SqlDbType.VarChar).Value = CouponBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetUpdateRevalidateCouponDetails(int CouponId, int TillId, string CouponBarcode, int ApprovedBy, string ReValidComment)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.UpdateRevalidateCouponDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CouponId", SqlDbType.Int).Value = CouponId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CouponBarcode", SqlDbType.VarChar).Value = CouponBarcode;
                        sqlCmd.Parameters.Add("@ApprovedBy", SqlDbType.Int).Value = ApprovedBy;
                        sqlCmd.Parameters.Add("@ReValidComment", SqlDbType.VarChar).Value = ReValidComment;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region UsedGiftCertificate
        public DataSet GetDisplayGiftCertificateCouponBarcodeUsedDetails(string GiftCertificateBarcode, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayGiftCertificateCouponBarcodeUsedDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@GiftBarcode", SqlDbType.VarChar).Value = GiftCertificateBarcode;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayCouponBarcodeUsedDetails(string GiftCertificateBarcode, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayGiftCertificateCouponBarcodeUsedDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@GiftCertificateBarcode", SqlDbType.VarChar).Value = GiftCertificateBarcode;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion

        #region HRM Folder

        #region UserDiscount
        public DataTable GetDisplayAllEmployeeDiscountLimits()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayAllEmployeeDiscountLimits", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddUpdateEmployeeDiscountLimits(int UserId, decimal PercentageDiscount, int UpdatedBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.AddUpdateEmployeeDiscountLimits", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@PercentageDiscount", SqlDbType.Decimal).Value = PercentageDiscount;
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = UpdatedBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Coupen Folder

        #region CouponBarcodeDetails

        public DataSet GetDisplayAllCouponBarcodePaymentDetails(string CouponBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayCusomterItemsDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CouponBarcode", SqlDbType.VarChar).Value = CouponBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayAddUpdateReactiveCoupon(int CouponId, int RevalidateBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayAddUpdateReactiveCoupon", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CouponId", SqlDbType.Int).Value = CouponId;
                        sqlCmd.Parameters.Add("@RevalidateBy", SqlDbType.Int).Value = RevalidateBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region CouponHistory
        public DataTable DisplayAllActiveLocationForCouponHistory()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayAllActiveLocationForCouponHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayCouponHistoryWithPeriodLocation(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayCouponHistoryWithPeriodLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region CouponNearlyExpiration
        public DataTable GetDisplayAllCouponAndGiftNearlyExpiration(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayAllCouponAndGiftNearlyExpiration", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllCompanyParentsDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayAllCompanyParentsDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayUpdateExtendDaysPerCoupon(int ExtendCouponDays, int CouponId, int ApprovedBy, string ReValidComment, string CouponBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.UpdateExtendDaysPerCoupon", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ExtendCouponDays", SqlDbType.Int).Value = ExtendCouponDays;
                        sqlCmd.Parameters.Add("@CouponId", SqlDbType.Int).Value = CouponId;
                        sqlCmd.Parameters.Add("@ApprovedBy", SqlDbType.Int).Value = ApprovedBy;
                        sqlCmd.Parameters.Add("@ReValidComment", SqlDbType.VarChar).Value = ReValidComment;
                        sqlCmd.Parameters.Add("@CouponBarcode", SqlDbType.VarChar).Value = CouponBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Promo

        public DataTable GetDisplayDefaultAccountTransactionDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.VerifyAccountingDateOpenOrClose", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region PromoWithExpiration
        public DataTable GetDisplayAllPromoWithExpiration(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Promo.DisplayAllPromoWithExpiration", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllPromoWithExpirationItems(int PromoId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Promo.DisplayAllPromoWithExpirationItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = PromoId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddUpdatePromoWithExpirationDays(int PromoId, int PromoDays, int CreatedBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Promo.AddUpdatePromoWithExpirationDays", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PromoId", SqlDbType.Int).Value = PromoId;
                        sqlCmd.Parameters.Add("@PromoDays", SqlDbType.Int).Value = PromoDays;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Denomination Folder

        #region BalanceReport
        /// <summary>
        ///  method name "GetDisplayCashFigureSpecificationReportDetails" changed to "Get_DisplayCashFigureSpecificationReportDetails".
        ///  so please verify before use this method (Because same method different SP for those method so avoid conflict issue name changed).
        ///  Check SP and then use this Method.
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        public DataSet Get_DisplayCashFigureSpecificationReportDetails(int UserId, int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayCashFigureSpecificationReportDetailsWithAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayCashFigureAndAccountTransactionBalanceSheetReport(int UserId, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayCashFigureAndAccountTransactionBalanceSheetReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region BalanceTransactionReport
        public DataSet GetDisplayCashFigureSpecificationReportDetails(int UserId, int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayCashFigureAndAccountBalanceSheetWithDateReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayBalanceDetailWithAccountingDate(int UserId, int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.GetBalanceDetailWithAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayDailyTransactionOverview(int UserId, int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayDailyTransactionOverview", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CurrentDate", SqlDbType.Date).Value = AccountingDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplaySaleBalanceSheets(int TillId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleBalanceSheets", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.CommandTimeout = 500;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDailySalesReportWithAccountingDatePerTill(int UserId, int TillId, DateTime AccountingDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DailySalesReportWithAccountingDatePerTill", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@AccountingDate", SqlDbType.Date).Value = AccountingDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Denomination
        public DataTable GetDisplayDenominationValueTypes()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayDenominationValueTypes", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayDenominationTotalValues(int TillId, int DenominationTypeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayDenominationTotalValues", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@DenominationTypeId", SqlDbType.Int).Value = DenominationTypeId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetUpdateTillDenominationStatistics(int TillId, DataTable DenominationStatisticsDetails)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.UpdateTillDenominationStatistics", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@DenominationStatisticsDetails", SqlDbType.Structured).Value = DenominationStatisticsDetails;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDenominationTotalValueInUSD(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DenominationTotalValueInUSD", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public List<DenominationType> GetDisplayDenominationTypeDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Denomination.DisplayDenominationTypeDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        List<DenominationType> denominationTypeList = new List<DenominationType>();
                        for (int denomTypeCount = 0; denomTypeCount < TempTable.Rows.Count; denomTypeCount++)
                        {
                            DenominationType denomType = new DenominationType();
                            denomType.CurrencyCode = TempTable.Rows[denomTypeCount]["Code"].ToString();
                            denomType.CurrencyId = Convert.ToInt32(TempTable.Rows[denomTypeCount]["CurrencyId"].ToString());
                            denomType.DenominationTypeId = Convert.ToInt32(TempTable.Rows[denomTypeCount]["DenominationTypeId"].ToString());
                            denomType.DenominationTypeName = TempTable.Rows[denomTypeCount]["DenominationType"].ToString();
                            denomType.DenominationValueTypeId = Convert.ToInt32(TempTable.Rows[denomTypeCount]["DenominationValueTypeId"].ToString());
                            denomType.DenominationValueCode = TempTable.Rows[denomTypeCount]["DenominationValueCode"].ToString();
                            denomType.DenominationValueType = TempTable.Rows[denomTypeCount]["DenominationValueType"].ToString();
                            denominationTypeList.Add(denomType);
                        }

                        return denominationTypeList;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion
        #endregion

        #region Purchases Folder

        #region AddUpdatePurchase

        public DataSet GetPurchasesByDate(DateTime PurchasedOnFrom, DateTime PurchasedOnTo)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetPurchasesByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = PurchasedOnFrom;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = PurchasedOnTo;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayAddUpdateInformation(int PurchaseId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayAddUpdateCategoryTypesModelBrandForPurchase", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetUpdatePurchaseDetailsWithItems(DataTable PurchaseItemsWithDetails, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int PurchaseId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.UpdatePurchaseDetailsWithItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseItemsWithDetails", SqlDbType.Structured).Value = PurchaseItemsWithDetails;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Decimal).Value = TillId;
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        sqlCmd.Parameters.Add("@InvoiceNumber", SqlDbType.VarChar).Value = InvoiceNumber;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@MainDiscount", SqlDbType.Decimal).Value = MainDiscount;
                        sqlCmd.Parameters.Add("@PurchasedOn", SqlDbType.DateTime).Value = PurchasedOn;
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetPurchasedItemsByPurchaseId(int PurchaseId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetPurchasedItemsByPurchaseId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void UpdateQtynCp4PurchasedItems(List<AddUpdatePurchase> l, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.UpdateQtynCp4PurchasedItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        for (int i = 0; i < l.Count; i++)
                        {
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Parameters.Add("@PurchasedItemId", SqlDbType.Int).Value = l[i].PurchaseItemId;
                            sqlCmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = l[i].Qty;
                            sqlCmd.Parameters.Add("@PurchasePrice", SqlDbType.Decimal).Value = l[i].Price;
                            sqlCmd.Parameters.Add("@ChangedBy", SqlDbType.Int).Value = UserId;

                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }


        }

        public DataSet ItemsWithoutCostprices()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetItemsWhichHasNoCostPrice", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet ItemsWithoutCostprices_ByPurchases()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetItemsWhichHasNoCostPrice_ByPurchases", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayReportPurchaseDetailsWithItems(int PurchaseId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.DisplayReportPurchaseDetailsWithItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool ReceiptNumberExists(string ReceiptNumber)
        {
            bool ReceiptNumberExists = false;

            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.ReceiptNumberExists", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable d = new DataTable())
                            {
                                da.Fill(d);
                                ReceiptNumberExists = Convert.ToInt32(d.Rows[0][0].ToString()) == 1;
                            }
                        }
                        return ReceiptNumberExists;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region NewPurchase
        public DataSet GetDisplaySupplierContactPerson()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Supplier.DisplaySupplierContactPerson", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAddPurchaseDetailsWithItems(int SupplierId, int TransactedBy, int CompanyId, string ReceiptNumber, string Comment, DataTable PurchaseItemsDetails)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.AddPurchaseDetailsWithItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@PurchaseItemsDetails", SqlDbType.Structured).Value = PurchaseItemsDetails;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayManualAddedItems()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.DisplayManualAddedItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Purchase
        public DataSet L_GetDisplayCategoryTypesModelBrandForPurchase()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayCategoryTypesModelBrandForPurchase", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetDiaplayPurchaseItemsWithModelId(int ItemId, decimal Quantity, decimal PurchasePrice, string InvoiceNumber, string ReceiptNumber)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DiaplayPurchaseItemsWithModelId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = Quantity;
                        sqlCmd.Parameters.Add("@PurchasePrice", SqlDbType.Decimal).Value = PurchasePrice;
                        sqlCmd.Parameters.Add("@InvoiceNumber", SqlDbType.VarChar).Value = InvoiceNumber;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAddUpdatePurchaseDetailsWithItems(DataTable PurchaseItemsWithDetails, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int CompanyId, int StockLocationId, string Container)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.AddUpdatePurchaseDetailsWithItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseItemsWithDetails", SqlDbType.Structured).Value = PurchaseItemsWithDetails;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Decimal).Value = TillId;
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        sqlCmd.Parameters.Add("@InvoiceNumber", SqlDbType.VarChar).Value = InvoiceNumber;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@MainDiscount", SqlDbType.Decimal).Value = MainDiscount;
                        sqlCmd.Parameters.Add("@PurchasedOn", SqlDbType.DateTime).Value = PurchasedOn;

                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@Container", SqlDbType.NVarChar).Value = Container;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllReceipts()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetAllReceipts", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetDateAndSupplierById(int PurchaseId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetDateAndSupplierById", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetBranches()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetBranches", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetStockLocations()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetLocations4Purchase()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetLocations4Purchase", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetSuppliers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Supplier.GetSuppliers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetInfoById(int PurchaseId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetInfoById", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void UpdateHeaderInfo(int SupplierId, int UserId, int CompanyId, int StockLocationId, string ReceiptNumber, string Comment, int PurchaseId, DateTime PurchasedOn, string Container)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.UpdateHeaderInfo", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        sqlCmd.Parameters.Add("@PurchasedOn", SqlDbType.Date).Value = PurchasedOn;
                        sqlCmd.Parameters.Add("@Container", SqlDbType.VarChar).Value = Container;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAllActiveItems()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetAllActiveItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable("GetAllActiveItems"))
                            {
                                da.Fill(dt);
                                TempTable = dt;
                                TempTable.TableName = "AllActiveItems";
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemByManBarcode(string ManBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetItemByManBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = ManBarcode;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable("GetItemByManBarcode"))
                            {
                                da.Fill(dt);
                                TempTable = dt;
                                TempTable.TableName = "ItemByManBarcode";
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemById(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetItemById", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable("GetItemById"))
                            {
                                da.Fill(dt);
                                TempTable = dt;
                                TempTable.TableName = "ItemById";
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void AddItems2ExistingPurchase(List<Purchase> pl)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.AddItems2ExistingPurchase", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        for (int i = 0; i < pl.Count; i++)
                        {
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = pl[i].ItemId;
                            sqlCmd.Parameters.Add("@PurchaseQty", SqlDbType.Decimal).Value = pl[i].PurchaseQty;
                            sqlCmd.Parameters.Add("@PurchasePrice", SqlDbType.Decimal).Value = pl[i].PurchasePrice;
                            sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = pl[i].PurchaseId;
                            sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = pl[i].CreatedBy;
                            sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = pl[i].Barcode;

                            sqlCmd.ExecuteNonQuery();
                        }

                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool IsBarcodeAlreadyInPurchase(int PurchaseId, string Barcode)
        {
            bool BarcodeExists = false;

            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.IsBarcodeAlreadyInPurchase", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = PurchaseId;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;


                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable("GetItemById"))
                            {
                                da.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    BarcodeExists = Convert.ToInt32(dt.Rows[0]["BarcodeExists"]) == 1;
                                }
                            }
                        }
                        return BarcodeExists;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemsByPurchaseId(int _purchaseid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetItemsByPurchaseId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = _purchaseid;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void AddNewPurchaseUsingXLsheet(DataTable Barcodes, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int CompanyId, int StockLocationId, string Container)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.AddNewPurchaseUsingXLsheet", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 900 })
                    {
                        sqlCmd.Parameters.Add("@Barcodes", SqlDbType.Structured).Value = Barcodes;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        sqlCmd.Parameters.Add("@InvoiceNumber", SqlDbType.VarChar).Value = InvoiceNumber;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@MainDiscount", SqlDbType.Decimal).Value = MainDiscount;
                        sqlCmd.Parameters.Add("@PurchasedOn", SqlDbType.DateTime).Value = PurchasedOn;

                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@Container", SqlDbType.VarChar).Value = Container;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void EditPurchaseUsingXLsheet(DataTable Barcodes, int _purchaseid, int _userid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {


                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.EditPurchaseUsingXLsheet", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcodes", SqlDbType.Structured).Value = Barcodes;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = _userid;
                        sqlCmd.Parameters.Add("@PurchaseId", SqlDbType.Int).Value = _purchaseid;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetNonMatchBarcodes(DataTable ExcelTable, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.GetNonMatchBarcodes", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcodes", SqlDbType.Structured).Value = ExcelTable;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion

        #region CreditNotes Fodler

        # region NewCreditNotes
        public DataSet DisplaySaleReturnsHistoryItems(DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Sale].[DisplaySaleReturnsHistoryItems]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@Todate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet CreditNoteDistributorsDetails(DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[CreditNoteDistributorsDetails]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@Todate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet CreditNoteDistributorsHistory(DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[CreditNoteDistributorsHistory]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@Todate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayAllCreditNoteDistributors()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("CreditNote.DisplayAllCreditNoteDistributors", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayAddUpdateCommissionPercentage(int DistributorId, String DistributorName, bool IsActive, string Address, string Phone,
                            int CreatedBy, DataTable CommissionPercentage, string EMail)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("CreditNote.DisplayAddUpdateCommissionPercentage", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        sqlCmd.Parameters.Add("@DistributorName", SqlDbType.VarChar).Value = DistributorName;
                        sqlCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;
                        sqlCmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = Address;
                        sqlCmd.Parameters.Add("@Phone", SqlDbType.VarChar).Value = Phone;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@CommissionPercentage", SqlDbType.Structured).Value = CommissionPercentage;
                        sqlCmd.Parameters.Add("@EMail", SqlDbType.VarChar).Value = EMail;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet DisplayAllCreditNoteDistributorsWithDistributorId(int DistributorId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("CreditNote.DisplayAllCreditNoteDistributorsWithDistributorId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayDistributorsWIthPeriod(DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayDistributorsWIthPeriod]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@Todate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayDistributorsCreditPaymentsPeriod(int DistributorId, DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayDistributorsCreditPaymentsPeriod]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@Todate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayOutstandingCreditNoteDistributors(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayOutstandingCreditNoteDistributors]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayCreditNoteReceiptDetails(int DistributorId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayCreditNoteReceiptDetails]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayOutstandingCreditNoteDistributorsUpdateDiscount(int CompanyId, DateTime FromDate, DateTime ToDate, int DistributorId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayOutstandingCreditNoteDistributorsUpdateDiscount]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayOutstandingCreditNoteDistributorsWithPeriod(int CompanyId, DateTime FromDate, DateTime ToDate, int DistributorId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[DisplayOutstandingCreditNoteDistributorsWithPeriod]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@DistributorId", SqlDbType.Int).Value = DistributorId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable UpdateCreditNoteDistributorsDiscounts(DataTable dtDiscounts)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[CreditNote].[UpdateCreditNoteDistributorsDiscount]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CNDiscountPercentages", SqlDbType.Structured).Value = dtDiscounts;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet.Tables[0];
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion

        #region Employee Folder

        #region Employee 

        public DataTable GetDisplayAllActiveEmployeeRoles()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Employee.DisplayAllActiveEmployeeRoles", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllEmployeeByRolesId(int RoleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Employee.DisplayAllEmployeeByRolesId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = RoleId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetAddUpdateEmployeeDiscountLimits(int UpdatedBy, int RoleId, int EmployeeId, bool IsRole, decimal PercentageDiscount)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Employee.AddUpdateEmployeeDiscountLimits", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = UpdatedBy;
                        sqlCmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = EmployeeId;
                        sqlCmd.Parameters.Add("@IsRole", SqlDbType.Bit).Value = IsRole;
                        sqlCmd.Parameters.Add("@PercentageDiscount", SqlDbType.Decimal).Value = PercentageDiscount;
                        sqlCmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = RoleId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAllActiveUsers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllActiveUsers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateDiscountLimits(decimal discount, int userid, DataTable users)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.UpdateDiscountLimits", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal).Value = discount;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userid;
                        sqlCmd.Parameters.Add("@users", SqlDbType.Structured).Value = users;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDiscountLimitHistory(int UserID)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetDiscountLimitHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserID;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Features Folder
        #region NewTeamViewer
        public DataTable GetDisplayAllDetailsTeamViewerInformation()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("General.DisplayAllDetailsTeamViewerInformation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetAddUpdateTeamViewerId(int TillId, string TeamViewerId, string TeamViewerPassword, int CreatedBy, int CreatedTillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("General.AddUpdateTeamViewerId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@TeamViewerId", SqlDbType.VarChar).Value = TeamViewerId;
                        sqlCmd.Parameters.Add("@TeamViewerPassword", SqlDbType.VarChar).Value = TeamViewerPassword;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@CreatedTillId", SqlDbType.Int).Value = CreatedTillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region MailAndAlert Folder

        #region MailAndAlert 
        public DataTable GetDisplayAllTillDetailsForReceiver(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.DisplayAllTillDetailsForReceiver", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllSentMailDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.DisplayAllSentMailDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayAllInboxMail(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.DisplayAllInboxMail", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetTotalNoOfUnreadMessage(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.TotalNoOfUnreadMessage", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetAddNewMailInformationWithTill(string MailSubject, string MailBody, int SenderUserId, int MailTillId, string MailerMacAddress, DataTable ReceiverTillDetails)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.AddNewMailInformationWithTill", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@MailSubject", SqlDbType.NVarChar).Value = MailSubject;
                        sqlCmd.Parameters.Add("@MailBody", SqlDbType.NVarChar).Value = MailBody;
                        sqlCmd.Parameters.Add("@SenderUserId", SqlDbType.Int).Value = SenderUserId;
                        sqlCmd.Parameters.Add("@MailTillId", SqlDbType.Int).Value = MailTillId;
                        sqlCmd.Parameters.Add("@MailerMacAddress", SqlDbType.NVarChar).Value = MailerMacAddress;
                        sqlCmd.Parameters.Add("@ReceiverTillDetails", SqlDbType.Structured).Value = ReceiverTillDetails;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempReturnValue = sqlCmd.ExecuteNonQuery();
                        return TempReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetUpdateReadMessage(int TillId, int MailId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.UpdateReadMessage", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.NVarChar).Value = TillId;
                        sqlCmd.Parameters.Add("@MailId", SqlDbType.NVarChar).Value = MailId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempReturnValue = sqlCmd.ExecuteNonQuery();
                        return TempReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Parameter Folder
        #region Parameter

        public DataTable GetDisplayAppParemters(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("App.DisplayAppParemters", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int GetUpdateAppParemters(string Value, int UserId, int Id)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("App.UpdateAppParemters", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = Value;
                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAllLocationsTills()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.GetAllLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int SetLocTillImg(Byte[] Img, int TillLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("App.SetLocTillImg", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Img", SqlDbType.VarBinary).Value = Img;
                        sqlCmd.Parameters.Add("@TillLocationId", SqlDbType.Int).Value = TillLocationId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        #endregion
        #endregion

        #region Reports Folder

        #region Sales Folder(In Reports Folder)
        #region BrandProductSaleOnCredit
        public DataSet GetBrandProductSaleOnCreditDetails(DateTime BeginDate, DateTime EndDate, int Department)
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_VERKOOP_PRODUCTEN_TOTAAL_MERK_REKENING", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = BeginDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = EndDate;
                        sqlCmd.Parameters.Add("@AFD", SqlDbType.Int).Value = Department;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }

                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }

        }

        public DataSet DisplayDepartment()
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand("SP_Afdelingen", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter sqlDapter = new SqlDataAdapter(sqlCommand))
                        {
                            using (DataSet dataSet = new DataSet())
                            {
                                TempDataSet = dataSet;
                                sqlDapter.Fill(TempDataSet);
                            }
                            return TempDataSet;
                        }
                    }
                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        #endregion

        #region CustomersRemainingPayments
        public DataTable GetCustomerRemainingPayments()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_OPENSTAANDE_REKENING", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "SP_OPENSTAANDE_REKENING";
                        return TempTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetCustomerRemainingPaymentByCustomerId(Int64 CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_RPT_OPENSTAANDE_VERKOPEN_KLANTNew", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@KlantID", SqlDbType.BigInt).Value = CustomerId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region MarginReport
        public DataTable GetAllParentLocations()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetAllParents", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerItem(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerItem", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerReceipt(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerReceipt", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerDate(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerDate", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerShop", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerItemPerReceipt(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerItemPerReceipt", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerShopClerk(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerShopClerk", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetSalesPerItemPerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerItemPerShop", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerDatePerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerDatePerShop", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerCustomer(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerCustomer", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerCashier(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerCashier", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesPerClerk(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesPerClerk", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 200 })
                    {
                        sqlCmd.Parameters.Add("@IsPeriodSearch", SqlDbType.Int).Value = IsPeriodSearch;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Jaar", SqlDbType.Int).Value = Jaar;
                        sqlCmd.Parameters.Add("@Maand", SqlDbType.Int).Value = Maand;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        _tempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                _tempDataTable = dt;
                            }
                        }
                        return _tempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ModelProductSaleOnCredit
        public DataSet GetModelProductSaleOnCreditDetails(DateTime BeginDate, DateTime EndDate, int Department)
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_VERKOOP_PRODUCTEN_TOTAAL_MODEL_REKENING", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = BeginDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = EndDate;
                        sqlCmd.Parameters.Add("@AFD", SqlDbType.Int).Value = Department;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }

                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }

        }
        #endregion

        #region MonthlySales4Lacoste
        public DataTable LacosteMenTextile(int YearNumber)
        {
            DataTable MenTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteMenTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            MenTextiles = dt;
                        }
                    }
                }
            }

            return MenTextiles;
        }

        public DataTable LacosteWomenTextile(int YearNumber)
        {
            DataTable WomenTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteWomenTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            WomenTextiles = dt;
                        }
                    }
                }
            }

            return WomenTextiles;
        }

        public DataTable LacosteKidsTextile(int YearNumber)
        {
            DataTable KidsTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteKidsTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            KidsTextiles = dt;
                        }
                    }
                }
            }

            return KidsTextiles;
        }

        public DataTable LacosteAccesoriesTextile(int YearNumber)
        {
            DataTable AccesoriesTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteAccesoriesTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            AccesoriesTextiles = dt;
                        }
                    }
                }
            }

            return AccesoriesTextiles;
        }

        public DataTable LacosteFootwearsTextile(int YearNumber)
        {
            DataTable FootwearsTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteFootwearsTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            FootwearsTextiles = dt;
                        }
                    }
                }
            }

            return FootwearsTextiles;
        }

        public DataTable LacosteTotalSalesFootwearsnTextile(int YearNumber)
        {
            DataTable SalesTotalTextiles = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppSettings.DaVinciConnectionStringForDW))
            {
                using (SqlCommand cmd = new SqlCommand("Sale.LacosteTotalSalesFootwearsnTextile", conn))
                {
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Year", SqlDbType.Int).Value = YearNumber;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            SalesTotalTextiles = dt;
                        }
                    }
                }
            }

            return SalesTotalTextiles;
        }
        #endregion

        #region ProductAnalysis

        public DataSet GetProductAnalysis(int ProductId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_ProdBreakeven_Analysis", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Prod_id", SqlDbType.Int).Value = ProductId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemOverView(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetOverView", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetPurchasesSalesReturnsExportsStockCorrectionsTransfers(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetPurchasesSalesReturnsExportsStockCorrectionsTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetAvgCostPriceCalcPlusStockPerLoc(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetAvgCostPriceCalcPlusStockPerLoc", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplaySalesPerShopByItemCategory(DateTime bd, DateTime ed, int CompanyId,
            int GroupId, int SubGroupId, int TypeId, int ColorId, int BrandId, int ModelId, int SizeId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesPerShopByItemCategory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = bd;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ed;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = GroupId;
                        sqlCmd.Parameters.Add("@SubGroupId", SqlDbType.Int).Value = SubGroupId;
                        sqlCmd.Parameters.Add("@TypeId", SqlDbType.Int).Value = TypeId;
                        sqlCmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = ColorId;
                        sqlCmd.Parameters.Add("@BrandId", SqlDbType.Int).Value = BrandId;
                        sqlCmd.Parameters.Add("@ModelId", SqlDbType.Int).Value = ModelId;
                        sqlCmd.Parameters.Add("@SizeId", SqlDbType.Int).Value = SizeId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        #endregion

        #region PurchaseSaleAnalysis
        public DataSet GetPurchaseSaleAnalysis()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Inkoop_Verkoop_Analyse", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region PurchaseSaleCalculation
        public DataSet GetStillToSaleInformation(int ProductID)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Prod_Breakeven_Analysis", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Prod_id", SqlDbType.Int).Value = ProductID;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region QuantityProductSaleOnCredit

        public DataSet GetQuantityproductSaleOnCredit(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_VERKOOP_PRODUKTEN_AANTAL_REKENING", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = ToDate;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }

        #endregion

        #region RevenueOverAPeriod
        public DataSet GetRevenueDataOverAPeriod(DateTime BeginDate, DateTime EndDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Omzet_Inkoop_Rekening_Winst", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = BeginDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = EndDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalePerDepartmentPerBrand
        public DataSet GetSalesPerDepartmentPerBrandDetails(DateTime BeginDate, DateTime EndDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Verkoop_Produkten_afdeling", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = BeginDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = EndDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesRep
        public DataTable LacosteSalesReport(DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.LacosteReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable RevenueBySaleDate(DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.RevenueBySaleDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataTable = dt.Tables[0];
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet RevenueAndProfitBySaleDate(DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.RevenueAndProfitBySaleDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetSalesByCustomerId(int CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllByCustomer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetSalesByBarcode(string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllByBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetSalesByDate(DateTime BD, DateTime ED, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        public DataTable GetCreditSales(DateTime BD, DateTime ED, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetCreditSales", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayDailyTransactionOverviewOverPeriod(DateTime bd, DateTime ed, int parentcompanyid, int companyid, int tillid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Account.DisplayDailyTransactionOverviewOverPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();

                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = bd;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ed;

                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = parentcompanyid;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyid;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = tillid;

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesSerachByBarcode
        public DataSet GetDisplaySearchSaleByItemBarcode(string ItemBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySearchSaleByItemBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemsBarcode", SqlDbType.VarChar).Value = ItemBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesSummary

        public DataTable GetBalancePaymentsPerReceipt(Int64 saleid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetBalancePaymentsOfReceipt", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = saleid;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet DisplaySalesSummaryReport(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSummaryReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataSet DisplayMontlySales(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesOverPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SizeProductSaleOnCredit
        public DataSet GetSizeProductSaleOnCredit(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_VERKOOP_PRODUCTEN_TOTAAL_MAAT_REKENING", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = ToDate;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }
        }
        #endregion

        #region ViewItemSaleOnPeriod
        public DataSet GetSoldItemsDetailsInPeriod(DateTime BeginDate, DateTime ToDate)
        {
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Omzet_Inkoop_Rekening_Winst", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.DateTime).Value = BeginDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.DateTime).Value = ToDate;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }

                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Close();
                    }
                }
            }

        }
        #endregion

        #endregion

        #region Stock Folder(in Report Folder)

        #region Exports

        public DataTable L_NewExport(int CustomerId, int UserId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.NewExport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId; ;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("Export"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "Export";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetExports(int CustomerId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetExports", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("Exports"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "Exports";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetExportedItems(int ExportId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetExportedItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ExportId", SqlDbType.Int).Value = ExportId;

                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("ExportedItems"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "ExportedItems";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_AddExportItem(int ExportId, int UserId, int Qty, string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AddExportItem", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ExportId", SqlDbType.Int).Value = ExportId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = Qty;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;

                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("ExportedItem"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "ExportedItem";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetExportReport(int ExportId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetExportReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ExportId", SqlDbType.Int).Value = ExportId;
                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("ExportedItem"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "ExportedItem";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable AddUpdateNewExportWithExcel(int ExportId, int UserId, DataTable dTableBarcodeQuantity, int ExportCompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AddUpdateExcelExportItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ExportId", SqlDbType.Int).Value = ExportId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId; ;
                        sqlCmd.Parameters.Add("@ExcelExportItems", SqlDbType.Structured).Value = dTableBarcodeQuantity;
                        sqlCmd.Parameters.Add("@ExportCompanyId", SqlDbType.Int).Value = ExportCompanyId;
                        dt = new DataTable();

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable data = new DataTable("Export"))
                            {
                                da.Fill(data);
                                dt = data;
                            }
                        }
                        dt.TableName = "Export";
                        return dt;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region ProductStockPerDepartment
        public DataSet GetProductStockPerDepartment()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_Produkt_Voorraad_Inkoop_Afdeling_For_Application", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesItemStockPerDepartment
        public DataSet GetSalesItemStockPerDepartment()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("SP_SaleItem_Voorraad_Inkoop_Afdeling_For_Application", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockRep

        public DataSet ItemsStockPerCompany()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.PerCompany", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dt = new DataSet())
                            {
                                da.Fill(dt);
                                TempDataSet = dt;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllParentLocations()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetAllParents", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetCurrentStockList(int CompId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockList", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetCurrentStockList2(int CompId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockQtyList2", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetCompany()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockQtyList2", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

        }

        public DataSet L_GetItemFlow(int ItemId, string CompanyCode, int CompanyParentId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetItemFlow", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompanyParentId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayStockItemsFlowForAllWithItemBarcodeForHO(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayAllLocationStockItemsFlowWithItemBarcodeForHO", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId(int ItemId, int ParentCompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayAllLocationStockItemsFlowWithItemBarcodeForHOCompanyId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = ParentCompanyId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetCurrentStockQtyList(int CompId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockQtyList", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetItemListOfNegativeQtyForCompany(int CompId, DateTime TransactDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetNegStockQuantityPerCompany", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompId;
                        sqlCmd.Parameters.Add("@TransactionDate", SqlDbType.Date).Value = TransactDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetReportOfNegativeQtyForCompany(int CompId, DateTime TransactDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetNegStockQuantityPerCompany_Pivot", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompId;
                        sqlCmd.Parameters.Add("@TransactionDate", SqlDbType.DateTime).Value = TransactDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetItems2ReorderPerCompany(int companyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetItems2ReorderPerCompany2", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = companyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetStockCompany()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Stock].[GetStockCompany]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }

                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

        }

        public DataSet L_GetStockValue(int companyID, string Date)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {

                    using (SqlCommand sqlCmd = new SqlCommand("[Stock].[GetStockDetails]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        sqlCmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyID;
                        sqlCmd.Parameters.Add("@Date", SqlDbType.VarChar).Value = Date;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }

                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

        }

        public DataTable L_GetStockByStockLocId(int StockLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockByStockLocId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_CurrentStockQtyList2ByItemId(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockQtyList2ByItemId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetCurrentNegStockList2(int companyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentNegStockQtyList2", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int L_GetItemIdbyBarcode(string Barcode)
        {
            int ItemId = 0;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetItemIdbyBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    if (Int32.TryParse(dt.Rows[0][0].ToString(), out ItemId))
                                    {

                                    }
                                }
                                else
                                {
                                    ItemId = 0;
                                }
                            }
                        }
                        return ItemId;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetTillIdByStockLocationId(int StockLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetTillIdByStockLocationId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetStockLocByCompanyId(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetStockLocByCompanyId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetScannedItemsByStockLocId(int StockLocId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetScannedItemsByStockLocId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetInventoriesByStockLocId(int CompanyId, int StockLocId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetInventoriesByStockLocId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetInventoryTroubleshootingItemsById(int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetInventoryTroubleshootingItemsById", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemsByInventoryId_Diff(int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetItemsByInventoryId_Diff", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public string GetInventoryNote(int StockLocId, int CompanyId)
        {
            string Note = "N/A";

            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetNote", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocId;

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    Note = dt.Rows[0][0].ToString();
                                }
                            }
                        }
                        return Note;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemsByInventoryId(int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.GetItemsByInventoryId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetWriteOffHistory(DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("WriteOff.GetHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetWriteOffReport(int WriteOffId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("WriteOff.GetWriteOffReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@WriteOffId", SqlDbType.Int).Value = WriteOffId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public object GetStockHistory(int StockLocationId, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayItemAllItemsDetails(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayAllItemsDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;

                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_Reshuffle(int companyId, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayReshuffle", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable L_CurrentStockQtyList2ByRefCode(string RefCode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockQtyList2ByRefCode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@RefCode", SqlDbType.VarChar).Value = RefCode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetItemMutations(int StockLocationId, int ItemId, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetItemMutations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetHistoryByPeriod(int StockLocationId, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetHistoryByPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetAllStockLocations()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetAllLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetStockItemTransactionDetailPerDay(int StockLocationId, int ItemId, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockItemTransactionDetailPerDay", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetLocationsUsingTransferBarcode(string _transfer_barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetLocationsUsingTransferBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@transfer_barcode", SqlDbType.VarChar).Value = _transfer_barcode;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateTransferStockLocations(string _transfer_barcode, int stockloc_from, int stockloc_to)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.UpdateTransferStockLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@transfer_barcode", SqlDbType.VarChar).Value = _transfer_barcode;
                        sqlCmd.Parameters.Add("@from_id", SqlDbType.Int).Value = stockloc_from;
                        sqlCmd.Parameters.Add("@to_id", SqlDbType.Int).Value = stockloc_to;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateTransferredItem(string _transfer_barcode, string _old_barcode, string _new_barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.UpdateTransferredItem", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@transfer_barcode", SqlDbType.VarChar).Value = _transfer_barcode;
                        sqlCmd.Parameters.Add("@item_barcode_old", SqlDbType.VarChar).Value = _old_barcode;
                        sqlCmd.Parameters.Add("@item_barcode_new", SqlDbType.VarChar).Value = _new_barcode;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateTransferredItemQty(string _transfer_barcode, string barcode, decimal qty)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.UpdateTransferredItemQty", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@transfer_barcode", SqlDbType.VarChar).Value = _transfer_barcode;
                        sqlCmd.Parameters.Add("@barcode", SqlDbType.VarChar).Value = barcode;
                        sqlCmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = qty;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        //method name same so name changed
        public DataTable Get_Shops(int ParentCompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetDetailUsingParentId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = ParentCompanyId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetTills(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Till.GetDetailUsingCompanyId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool ReceiveAll(int transferid, int userId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AutoReceiveTransfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = transferid;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetBrandsBasedonStockItem()
        {
            //string connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
            using (SqlConnection con = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Stock.GetBrandsByStockItem", con) { CommandType = CommandType.StoredProcedure })
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                    }
                    return TempDataTable;
                }

                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }

        public DataTable L_GetCompanyDetailsStockItem()
        {
            // string connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
            using (SqlConnection con = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("Stock.GetCompanyDetailsforstock", con) { CommandType = CommandType.StoredProcedure })
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempDataTable = dt;
                            }
                        }
                    }
                    return TempDataTable;
                }

                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }

        public DataTable GetCurrentStoctQtyAndPrice(string BrandId, int CompanyId, DateTime FromDate, DateTime Todate)
        {
            //string connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockQtyandPrices", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BrandId", SqlDbType.NVarChar).Value = BrandId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = Todate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable ds = new DataTable())
                            {
                                da.Fill(ds);
                                TempDataTable = ds;
                            }
                        }
                        return TempDataTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #endregion
        #endregion

        #region Revenue Folder

        #region ItemStockByBarcode

        public DataTable L_GetDisplayAllActiveItemsWithBarcode()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayAllActiveItemsWithBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayStockItemDetailsDiffLocationWithItemId(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayStockItemDetailsDiffLocationWithItemId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region OutStandingSaleForCompany
        public DataSet GetDisplayOutStandingSaleForCompany(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayOutStandingSaleForCompany", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region OutstandingSalesPerCustomer
        public DataSet GetDisplayOutstandingSalesPerCustomer(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayOutstandingSalesPerCustomer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetDisplayReportDetailsOutstandingSalesPerCustomer(int CompanyId, int CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayReportDetailsOutstandingSalesPerCustomer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetCreditSalesPaymentHistory(DateTime FromDate, DateTime ToDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetCreditPayments", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        //TempDataSet = new DataSet();
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayAccountsReceivable(int CompanyId, DateTime TransactionDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayAccountsReceivable", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@TransactionDate", SqlDbType.Date).Value = TransactionDate;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplaySubsidiaryLedger(int customerid, int companyid, DateTime TransactionDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySubsidiaryLedger", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = customerid;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyid;
                        sqlCmd.Parameters.Add("@TransactionDate", SqlDbType.Date).Value = TransactionDate;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable ManualUpdateCashCreditCostsRunByDate(DateTime RefreshDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.ManualUpdateCashCreditCostsRunByDate", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1000 })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@RefreshDate", SqlDbType.Date).Value = RefreshDate;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ProductAnalyse
        public DataSet DisplayProductAnalysis(int ItemId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayProductAnalysis", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                catch (Exception ex)
                {
                    return TempDataSet;
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region PurchaseSaleAnalysis
        public DataTable GetPurchasesSales(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.GetPurchasesSales", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ReplenishmentReport
        public DataTable DisplayMangoStockPerHoursWithPeriod(DateTime CurrentDate, DateTime FromTime, DateTime ToTime, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayMangoStockPerHoursWithPeriod", sqlconn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 40000 })
                    {
                        sqlCmd.Parameters.Add("@CurrentDate", SqlDbType.Date).Value = CurrentDate;
                        sqlCmd.Parameters.Add("@FromTime", SqlDbType.Time).Value = FromTime.TimeOfDay;
                        sqlCmd.Parameters.Add("@ToTime", SqlDbType.Time).Value = ToTime.TimeOfDay;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayActiveMangoLocation()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayActiveMangoLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Revenues
        public DataTable GetDisplayPivotReportRevenueByDate(DateTime FromDate, DateTime ToDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayPivotReportRevenueByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayPivotReportRevenueByLocation(DateTime FromDate, DateTime ToDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayPivotReportRevenueByLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetRevenuesByCompanyByPeriodPerClerk(DateTime BD, DateTime ED, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetRevenuesByCompanyByPeriodPerClerk", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetRevenuesByCompanyByPeriodPerCustomer(DateTime BD, DateTime ED, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetRevenuesByCompanyByPeriodPerCustomer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetRevenuesByCompanyByPeriodPerClerk4ExcelReport(DateTime BD, DateTime ED, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetRevenuesByCompanyByPeriodPerClerk4ExcelReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;


                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSalesByCustomer(int CustomerId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Customer.GetSales", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayPivotReportNetSalesByLocation(DateTime BD, DateTime ED, int CurrencyId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayPivotReportNetSalesByLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CurrencyId", SqlDbType.Int).Value = CurrencyId;
                        //sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region RevenuesByItem
        public DataTable GetDisplayAllItemsDetailsWithReportByPeriod(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayRevenuesPerItem", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SaleGetSalesByDate
        public DataTable GetDisplaySaleGetSalesByDate(DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSalesByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetSalesReprintSalesReceiptForHOAllLocation(long SaleId, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.SalesReprintSalesReceiptForHOAllLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetSalesReprintSalesReceiptForHOAllLocation_GC(int AccountTypeId, long SaleId, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.SalesReprintSalesReceiptForHOAllLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@AccountTypeId", SqlDbType.BigInt).Value = AccountTypeId;
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetGetAllByItemByBarcode(string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllByItem", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetGetAllByItemByIMEI(string IMEI)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllByIMEI", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@IMEI", SqlDbType.VarChar).Value = IMEI;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesAnalysis
        public DataTable DisplaySalesAnalysis(DateTime BD, DateTime ED, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesAnalysis", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesPerClerk
        public DataTable GetDisplaySaleClerkCommissionDetails(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleClerkCommissionDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplaySaleClerkItemsCommissionDetails(int CompanyId, int ClerkId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleClerkItemsCommissionDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ClerkId", SqlDbType.Int).Value = ClerkId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SalesPerCustomer

        public DataTable GetBalancePayments(int CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetBalancePayments", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayAllCompanyDetailsForReport()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.DisplayAllCompanyDetailsForReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetCustomersByCompany(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetCustomersByCompany", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetBalancePerCustomer(int CompanyId, int CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesBalancePerCustomer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SoldItemsByItem
        public DataTable GetSoldItemsByItemProperties(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSoldItemsByItemProperties", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetSaleNotes(DateTime bd, DateTime ed, int itemid, int parent_companyid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetComment4Discounts", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@ParentCompanyId", SqlDbType.Int).Value = parent_companyid;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = bd;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ed;
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = itemid;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SoldItemsPerProduct
        public DataTable GetCompanies(int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetCompanies", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetDisplayCompanySoldItemsPerProduct(int CompanyId, DateTime FromDate, DateTime ToDate, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplaySoldItemsPerProduct", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region SoldSaleItemsByItemProperties
        public DataTable GetSoldSaleItemsByItemProperties(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSoldSaleItemsByItemProperties", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetSoldItemsByPropertiesPerCompany(int CompanyId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetSoldItemsByItemPropertiesPerCompany", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable L_GetCurrentStockListOfSaleItems(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CurrentStockListOfSaleItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion


        #endregion

        #region Sales Folder

        #region Sales

        public DataSet L_GetDisplaySalesDefaultInformation(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Employee.DisplaySalesDefaultInformation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplayReprintSalesDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayReprintSalesDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplaySalesReturnHistory(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesReturnHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplayReprintAllSaleItemWithPaymentDetails(string ReceiptNumber)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayReprintAllSaleItemWithPaymentDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplayPendingPaymentCustomerBarcode(int TillId, string SalesBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayPendingPaymentCustomerBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SalesBarcode", SqlDbType.VarChar).Value = SalesBarcode;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplayPendingPaymentCustomerReceipt(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayPendingPaymentCustomerReceipt", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetReturnItemsWithReturnMoneyDetails(DataTable DataTableSaleReturnedItems, DataTable DataTableSalesPaymentCashAmount, string Comment, int UserId, int ReturnedByTillId, string ReceiptNumber, decimal TotalReturnMoneyAmount)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.ReturnItemsWithReturnMoneyDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SaleReturnedItems", SqlDbType.Structured).Value = DataTableSaleReturnedItems;
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = DataTableSalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@ReturnedByTillId", SqlDbType.Int).Value = ReturnedByTillId;
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@TotalReturnMoneyAmount", SqlDbType.Decimal).Value = TotalReturnMoneyAmount;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetReturnDisplayCustomerItemsDetail(string Barcode, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.ReturnDisplayCustomerItemsDetail", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplaySaleHistoryOfCurrentAccountingDate(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleHistoryOfCurrentAccountingDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetReturnItemsForCouponDetails(int ReturnedByTillId, int UserId, string Barcode, string Comment, DataTable SaleReturnedItems, decimal CouponAmount)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.ReturnItemsForCouponDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ReturnedByTillId", SqlDbType.Int).Value = ReturnedByTillId;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@SaleReturnedItems", SqlDbType.Structured).Value = SaleReturnedItems;
                        sqlCmd.Parameters.Add("@CouponAmount", SqlDbType.Decimal).Value = CouponAmount;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetDisplaySaleHistoryOfCurrentAccountingDateForCancel(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleHistoryOfCurrentAccountingDateForCancel", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetAddPendingPaymentWithPaymentTransaction(DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, int TillId, int PrincipalId, string SalesBarcode, decimal PaidAmountInUSD, decimal ReturnAmountInUSD)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.AddPendingPaymentWithPaymentTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = SalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@SalesReturnCashAmount", SqlDbType.Structured).Value = SalesReturnCashAmount;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = PrincipalId;
                        sqlCmd.Parameters.Add("@SalesBarcode", SqlDbType.VarChar).Value = SalesBarcode;
                        sqlCmd.Parameters.Add("@PaidAmountInUSD", SqlDbType.Decimal).Value = PaidAmountInUSD;
                        sqlCmd.Parameters.Add("@ReturnAmountInUSD", SqlDbType.Decimal).Value = ReturnAmountInUSD;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetAddSaleWithSaleItemsDetailsWithPaymentTransaction(int TillId, int CustomerId, DataTable dTableSalesPerson, int PrincipalId, decimal Discount, string Comment, decimal Total,
                string TransactedBy, DataTable SaleItemsDetails, DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, DataTable CouponPaymentDetails, int SaleTypeId,
            string NewCustomerName, string NewCustomerAddress, string NewCustomerPhone)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.AddSaleWithSaleItemsDetailsWithPaymentTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SaleTypeId", SqlDbType.Int).Value = SaleTypeId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        sqlCmd.Parameters.Add("@SalesPersonDetails", SqlDbType.Structured).Value = dTableSalesPerson;
                        sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = PrincipalId;
                        sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal).Value = Discount;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@Total", SqlDbType.Decimal).Value = Total;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.VarChar).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@SaleItemsDetails", SqlDbType.Structured).Value = SaleItemsDetails;
                        sqlCmd.Parameters.Add("@SalesPaymentCashAmount", SqlDbType.Structured).Value = SalesPaymentCashAmount;
                        sqlCmd.Parameters.Add("@SalesReturnCashAmount", SqlDbType.Structured).Value = SalesReturnCashAmount;
                        sqlCmd.Parameters.Add("@CouponPaymentDetails", SqlDbType.Structured).Value = CouponPaymentDetails;

                        sqlCmd.Parameters.Add("@NewCustomerName", SqlDbType.VarChar).Value = NewCustomerName;
                        sqlCmd.Parameters.Add("@NewCustomerAddress", SqlDbType.VarChar).Value = NewCustomerAddress;
                        sqlCmd.Parameters.Add("@NewCustomerPhone", SqlDbType.VarChar).Value = NewCustomerPhone;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetDisplaySalesItemsDetailsWithBarcode(string ItemBarcode, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesItemsDetailsWithBarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemBarcode", SqlDbType.VarChar).Value = ItemBarcode;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetCancelSaleAndContraAllTransaction(string ReceiptNumber, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.CancelSaleAndContraAllTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ReceiptNumber", SqlDbType.VarChar).Value = ReceiptNumber;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetDisplayCouponsScanDetails(string Barcode, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Coupon.DisplayCouponsScanDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetSalePaymentDetailsInDifferentCurrency(int TillId, decimal PaymentAmountInUSD)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.SalePaymentDetailsInDifferentCurrency", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@PaymentAmountInUSD", SqlDbType.Decimal).Value = PaymentAmountInUSD;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet MonthlySales4Lacoste(int Year)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.MonthlySales4Lacoste", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Year", SqlDbType.Int).Value = Year;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region SalesCreditDiscount
        public DataTable GetCustomer4DiscountUpdate(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetCustomer4DiscountUpdate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayAllPendingSalesReceipts(int CustomerId, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayAllPendingSalesReceipts", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable GetAllSoldItems4DiscountUpdate(long SaleId, int TillId, int CustomerId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetAllSoldItems4DiscountUpdate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustomerId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayUpdateItemsDiscount(DataTable dTableUpdateItemDiscount, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayUpdateItemsDiscount", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SalesUpdateItemsDiscount", SqlDbType.Structured).Value = dTableUpdateItemDiscount;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayUpdateSoldItemsDiscount(DataTable dTableUpdateItemDiscount, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayUpdateSoldItemsDiscount", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SalesUpdateItemsDiscount", SqlDbType.Structured).Value = dTableUpdateItemDiscount;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable GetItemsBySaleId(long SaleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetItemsPricesBySaleId(long SaleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetItemsTotalPrice", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateDiscounts(int UserId, DataTable dt)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.UpdateDiscounts", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@Items4DiscountUpdate", SqlDbType.Structured).Value = dt;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool UpdateReciptDiscounts(long SaleId, decimal Discount, int UserID)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Sale].[UpdateReciptDiscounts]", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal).Value = Discount;
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserID;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        return sqlCmd.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int isEligible(long SaleId)
        {
            int returnval = 0;
            using (SqlConnection sqlConn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("[Sale].[CheckSaleId]", sqlConn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        if (sqlConn.State == ConnectionState.Closed)
                        {
                            sqlConn.Open();
                        }
                        returnval = (int)sqlCmd.ExecuteScalar();
                        return Convert.ToInt32(returnval);
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
                finally
                {
                    if (sqlConn.State == ConnectionState.Open)
                    {
                        sqlConn.Dispose();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region SalesTax Folder
        #region SalesTaxSummary
        public DataTable DisplaySalesTaxReportSummaryInPeriod(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesTaxReportSummaryInPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplaySalesTaxRevenuesCompanyLocation()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesTaxRevenuesCompanyLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxRevenuesCompanyLocation";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable DisplaySalesTaxRevenuesInPeriod(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySalesTaxRevenuesInPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxRevenuesInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region StockFolder

        #region Checker
        public DataTable StockChecker(int CompanyId, DataTable scanqty)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.Checker", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ScanQty", SqlDbType.Structured).Value = scanqty;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable ShuffleChecker(int companyId, DataTable dataTable)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.ShuffleChecker", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                        sqlCmd.Parameters.Add("@ToTransferQty", SqlDbType.Structured).Value = dataTable;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetTransfersUsingBarcodes(int CompanyId, DataTable Barcodes, DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetTransfersUsingBarcodes", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Barcodes", SqlDbType.Structured).Value = Barcodes;
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public string NewTransferXL(int UserId, int CompanyId, string Shop, DataTable barcodeqty)
        {
            string transfernumber = "";
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CreateTransferUsingXLsheet", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ShopCode", SqlDbType.VarChar).Value = Shop;
                        sqlCmd.Parameters.Add("@BarcodeQty", SqlDbType.Structured).Value = barcodeqty;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }

                        if (TempTable.Rows.Count > 0)
                        {
                            transfernumber = TempTable.Rows[0][0].ToString();
                        }
                        else
                        { transfernumber = ""; }
                        return transfernumber;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

        }
        #endregion

        #region CompanyPromoCurrentStockList
        public DataTable DisplayGetCompanyParents()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetCompanyParents", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayStockGetStockTransferReport(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CompanyPromoCurrentStockList", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayPromoAddNewPromoItems(DataTable CompanyPromoItems, string Code, int CompanyId, decimal Discount, int CreatedBy, DateTime PromoDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Promo.AddNewPromoItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyPromoItems", SqlDbType.Structured).Value = CompanyPromoItems;
                        sqlCmd.Parameters.Add("@Code", SqlDbType.VarChar).Value = Code;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Discount", SqlDbType.Decimal).Value = Discount;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@PromoDate", SqlDbType.Date).Value = PromoDate;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region DefectInventory
        public DataTable DisplayItemDescriptionWithStockItems(int CompanyId, string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayItemDescriptionWithStockItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayAddUpdateStockDefectItems(DataTable ManualDefectReturnItems, int StockLocationId, int CreatedBy, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayAddUpdateStockDefectItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ManualDefectReturnItems", SqlDbType.Structured).Value = ManualDefectReturnItems;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet DisplayDefectInventoryHistoryPeriod(DateTime FromDate, DateTime ToDate, int StockLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayDefectInventoryHistoryPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = StockLocationId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplayDefectInventoryItems()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayDefectInventoryItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayAllActiveStockLocation()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayAllActiveStockLocation", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayAllActiveStockLocationAll()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayAllActiveStockLocationAll", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region IncompleteTransfer
        public DataTable DisplayIncompletedTransfers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.IncompletedTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ReprintStockTransferReport

        public DataTable L_DisplayStockTransferReport(int StoreTransferId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockTransferReport", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = StoreTransferId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetStockTransferCheckList(int StoreTransferId, int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetStockTransferCheckList", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = StoreTransferId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        #endregion

        #region SaleGetSalesByDate
        public DataTable GetDisplayShopStockDetails(DateTime BD, DateTime ED)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.SaleGetSalesByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = BD;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ED;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetSaleForSalesReprintSalesReceipt(long SaleId, int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.SalesReprintSalesReceipt", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SaleId", SqlDbType.BigInt).Value = SaleId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region Section
        public DataTable DisplayAddUpdateSectionDetails(string SectionNumber, string SectionName, int CreatedBy, string Remarks)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("WriteOff.DisplayAddUpdateSectionDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@SectionNumber", SqlDbType.VarChar).Value = SectionNumber;
                        sqlCmd.Parameters.Add("@SectionName", SqlDbType.VarChar).Value = SectionName;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = Remarks;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayInventoryDeleteBySection(int UserId, int SectionId, int InventoryId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.DisplayInventoryDeleteBySection", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@SectionId", SqlDbType.Int).Value = SectionId;
                        sqlCmd.Parameters.Add("@InventoryId", SqlDbType.Int).Value = InventoryId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region ShopStock
        public DataTable GetDisplayShopStockDetails(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayShopStockDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockCurrentStockDetails
        public DataTable GetDisplayItemIdWithItembarcode(string Barcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Item.DisplayItemIdWithItembarcode", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Barcode", SqlDbType.VarChar).Value = Barcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataSet GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId(int ItemId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.ItemFlowForEveryLocationWithCompanyNameByItemId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.VarChar).Value = ItemId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockSoldItemsForCompanyAndLocationId
        public DataTable GetDisplayItemIdWithItembarcode(string CompanyCode, int CompanyParentId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.TotalSoldItemsForCompanyAndLocationId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = CompanyCode;
                        sqlCmd.Parameters.Add("@CompanyParentId", SqlDbType.Int).Value = CompanyParentId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockTransfer

        public DataTable L_GetAddUpdateReceiveItemsAndStockTransaction(int TillId, int TransactedBy, string TransferNumber, string Batchcode, decimal RequestedQuantity)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AddUpdateReceiveItemsAndStockTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@Batchcode", SqlDbType.VarChar).Value = Batchcode;
                        sqlCmd.Parameters.Add("@TransferNumber", SqlDbType.VarChar).Value = TransferNumber;
                        sqlCmd.Parameters.Add("@RequestedQuantity", SqlDbType.Decimal).Value = RequestedQuantity;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_ReceiveBadStockTransfer(int StockLocationId, int TransactedBy, string TransferNumber, string Batchcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.ReceiveBadStockTransfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@Batchcode", SqlDbType.VarChar).Value = Batchcode;
                        sqlCmd.Parameters.Add("@TransferNumber", SqlDbType.VarChar).Value = TransferNumber;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetTransferByTransferNo(int StockLocationId, string TransferNumber)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetTransferByTransferNo", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@TransferNumber", SqlDbType.VarChar).Value = TransferNumber;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetBadStockTransferByTransferNo(int StockLocationId, string TransferNumber)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetBadStockTransferByTransferNo", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@TransferNumber", SqlDbType.VarChar).Value = TransferNumber;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet L_GetStockLocations(int StockLocationId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetStockLocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetItemByBarcode4Transfer(string Batchcode, int StockLocationId, decimal TotalQuantity, decimal RequestQuantity)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetItemByBarcode4Transfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Batchcode", SqlDbType.VarChar).Value = Batchcode;
                        sqlCmd.Parameters.Add("@StockLocationId", SqlDbType.Int).Value = StockLocationId;
                        sqlCmd.Parameters.Add("@TotalQuantity", SqlDbType.Decimal).Value = TotalQuantity;
                        sqlCmd.Parameters.Add("@RequestQuantity", SqlDbType.Decimal).Value = RequestQuantity;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_CreateStockTransfer(DataTable StockTransferItemsDetails, int TransactedBy, int FromStockLocId, int ToStockLocId, int TillId, string Comment, int NoOfBoxes)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CreateTransfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockTransferItemsDetails", SqlDbType.Structured).Value = StockTransferItemsDetails;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@FromStockLocId", SqlDbType.Int).Value = FromStockLocId;
                        sqlCmd.Parameters.Add("@ToStockLocId", SqlDbType.Int).Value = ToStockLocId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@NoOfBoxes", SqlDbType.Int).Value = NoOfBoxes;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable CreateTransferXLsheet(DataTable ExcelTable, int TransactedBy, int FromStockLocId, int ToStockLocId, int TillId, string Comment, int noofbox)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CreateTransferXLsheet", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@ItemsToTransfer", SqlDbType.Structured).Value = ExcelTable;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        sqlCmd.Parameters.Add("@FromStockLocId", SqlDbType.Int).Value = FromStockLocId;
                        sqlCmd.Parameters.Add("@ToStockLocId", SqlDbType.Int).Value = ToStockLocId;
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = Comment;
                        sqlCmd.Parameters.Add("@NoOfBoxes", SqlDbType.Int).Value = noofbox;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockTransferCancel

        public DataSet L_GetPendingTransfers(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetPendingTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_CancelStockTransfer(int CompanyId, int StoreTransferId, int TransactedBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CancelStockTransfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = StoreTransferId;
                        sqlCmd.Parameters.Add("@TransactedBy", SqlDbType.Int).Value = TransactedBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }





        public DataTable L_DefectStockTransfer(string StockTransferBarcode)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.MakeTransferDefect", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StockTransferBarcode", SqlDbType.VarChar).Value = StockTransferBarcode;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockTransferHistory
        public DataSet GetDisplayCompanyWithLocationAndAddress(int TillId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayStockTransferDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable GetDisplayStockTransferHistoryDetails(int StoreTransferId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayStockTransferHistoryDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = StoreTransferId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region StockTransferReport


        public DataSet L_GetTransfersByDate(DateTime FromDate, DateTime ToDate)
        {
            TempDataSet = new DataSet();
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetTransfersByDate", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@BD", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ED", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                catch
                {
                    return TempDataSet;
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataSet GetAllOpenTransfers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetAllOpenTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataSet GetAllCancelledTransfers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetAllCancelledTransfers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempDataSet = dSet;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void AddItemsIntoTransfer(DataTable dt, int _transferid)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AddItemsIntoTransfer", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.Add("@items", SqlDbType.Structured).Value = dt;
                        sqlCmd.Parameters.Add("@transferid", SqlDbType.Int).Value = _transferid;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.ExecuteNonQuery();
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region  StockTransferRequestsHistory


        public DataSet L_GetDisplayAllCompanyDetailsForReport(int TillId, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayStockTransferRequestsHistory", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetDisplayCancelStockTransferRequests(int TransferRequestsId, int CancelledBy, string CancelComments)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.CancelStockTranfserRequestedItems", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TransferRequestsId", SqlDbType.Int).Value = TransferRequestsId;
                        sqlCmd.Parameters.Add("@CancelledBy", SqlDbType.Int).Value = CancelledBy;
                        sqlCmd.Parameters.Add("@CancelComments", SqlDbType.VarChar).Value = CancelComments;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region TransferRequest



        public DataTable L_GetDisplayCompanyStockItemsDetailsWithCompanyId(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.DisplayCompanyStockItemsDetailsWithCompanyId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAddTransferRequestItemsDetailsWithCompanyId(DataTable TransferRequestItemsDetails, int FromCompanyId, int ToCompanyId, int CreatedBy, string Comments)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.AddTransferRequestItemsDetailsWithCompanyId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TransferRequestItemsDetails", SqlDbType.Structured).Value = TransferRequestItemsDetails;
                        sqlCmd.Parameters.Add("@FromCompanyId", SqlDbType.Int).Value = FromCompanyId;
                        sqlCmd.Parameters.Add("@ToCompanyId", SqlDbType.Int).Value = ToCompanyId;
                        sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = CreatedBy;
                        sqlCmd.Parameters.Add("@Comments", SqlDbType.NVarChar).Value = Comments;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region TransfersBarcodeByStoreTransfer


        public DataTable L_GetDisplayShopStockDetails(int StoreTransferId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Stock.GetTransfersBarcodeByStoreTransferId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@StoreTransferId", SqlDbType.Int).Value = StoreTransferId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempTable = ds.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region  WriteOff
        public DataTable GetDisplayCompanyWithAddress(int TillId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.DisplayCompanyWithAddress", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@TillId", SqlDbType.Int).Value = TillId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }



        public DataTable GetDisplayInventoryDateByHO(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.DisplayInventoryDateByHO", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable GetDisplayInventoryForWritesOffItemsDetails(int CompanyId, DateTime? InventoryDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.DisplayInventoryForWritesOffItemsDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@InventoryDate", SqlDbType.Date).Value = InventoryDate;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                TempTable = dSet.Tables[0];
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public int GetWriteoffUpdateAndStockTransaction(int CompanyId, int ItemId, int EnteredBy, decimal StockQuantity, int PurchaseItemId, string Batch, decimal CurrentStockQty, decimal CountedQty)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Inventory.WriteoffUpdateAndStockTransaction", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = ItemId;
                        sqlCmd.Parameters.Add("@EnteredBy", SqlDbType.Int).Value = EnteredBy;
                        sqlCmd.Parameters.Add("@StockQuantity", SqlDbType.Decimal).Value = StockQuantity;
                        sqlCmd.Parameters.Add("@PurchaseItemId", SqlDbType.Int).Value = PurchaseItemId;
                        sqlCmd.Parameters.Add("@Batch", SqlDbType.VarChar).Value = Batch;
                        sqlCmd.Parameters.Add("@CurrentStockQty", SqlDbType.Decimal).Value = CurrentStockQty;
                        sqlCmd.Parameters.Add("@CountedQty", SqlDbType.Decimal).Value = CountedQty;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        ReturnValue = sqlCmd.ExecuteNonQuery();
                        return ReturnValue;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion




        #endregion

        #region SupplierPurchase Folder
        #region SupplierPurchase
        public DataTable DisplayPurchaseHistoryWithPeriod(DateTime FromDate, DateTime ToDate, int SupplierId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.DisplayPurchaseHistoryWithPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = SupplierId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable DisplaySaleAndReceiptFromCustomerWithPeriod(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleAndReceiptFromCustomerWithPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayReturnAndReceiptFromCustomerWithPeriod(DateTime FromDate, DateTime ToDate, int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayReturnAndReceiptFromCustomerWithPeriod", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = FromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = ToDate;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplayAllActiveSupplier()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Purchase.DisplayAllActiveSupplier", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable DisplaySaleReceiptCompanyDetails()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplaySaleReceiptCompanyDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "DisplaySalesTaxReportSummaryInPeriod";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion
        #endregion

        #region UserSecurityFolder

        #region Log
        public bool L_In(string Login, string Pwd, bool UseUserId4Login, out string Msg, out int Rv)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Sec.UserLogin", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Add("@Login", SqlDbType.VarChar).Value = Login;
                    sqlcmd.Parameters.Add("@UserPwd", SqlDbType.VarChar).Value = Pwd;
                    sqlcmd.Parameters.Add("@UseUserId4Login", SqlDbType.Bit).Value = UseUserId4Login;

                    try
                    {

                        if (sqlconn.State != ConnectionState.Open)
                        {
                            sqlconn.Open();
                        }

                        SqlParameter RetVal = sqlcmd.Parameters.Add("@returnValue", SqlDbType.Int);
                        RetVal.Direction = ParameterDirection.ReturnValue;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }


                        sqlcmd.ExecuteNonQuery();
                        Rv = Convert.ToInt32(RetVal.Value);
                        if (Convert.ToInt32(RetVal.Value) == 1)
                        {
                            Msg = "Login succeeded";
                            return true;
                        }
                        else if (Convert.ToInt32(RetVal.Value) == -2)
                        {
                            Msg = String.Format("The entered User ID: {0} is invalid", Login);
                            return false;
                        }
                        else if (Convert.ToInt32(RetVal.Value) == -3)
                        {
                            Msg = String.Format("The entered Username: {0} is invalid", Login);
                            return false;
                        }
                        else if (Convert.ToInt32(RetVal.Value) == -4)
                        {
                            Msg = "The entered password is invalid";
                            return false;
                        }
                        else if (Convert.ToInt32(RetVal.Value) == -5)
                        {
                            Msg = String.Format("User: {0} is not active anymore in the system", Login);
                            return false;
                        }
                        else
                        {
                            Msg = "Login failed due to an unknown error";
                            return false;
                        }

                    }
                    catch (Exception err)
                    {
                        Rv = -1;
                        Msg = String.Format("Login failed due to the following error:\r\n{0}", err.Message);
                        return false;
                    }
                    finally
                    {
                        if (sqlconn.State != ConnectionState.Closed)
                        {
                            sqlconn.Close();
                        }
                    }

                }
            }
        }

        public void L_GetUserCredentials(string Login, bool UseUserId4Login, out string FullName, out int ContactId, out int UserID, out string UserName, out bool IsActive, out bool IsUserMale)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("Sec.GetUserCredentials", sqlconn) { CommandType = CommandType.StoredProcedure })
                {
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.Add("@Login", SqlDbType.VarChar).Value = Login;
                    sqlcmd.Parameters.Add("@UseUserId4Login", SqlDbType.Bit).Value = UseUserId4Login;

                    try
                    {

                        if (sqlconn.State != ConnectionState.Open)
                        {
                            sqlconn.Open();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            TempTable.Clear();
                            da.Fill(TempTable);
                        }
                        if (TempTable.Rows.Count > 0)
                        {
                            FullName = TempTable.Rows[0]["FullName"].ToString();
                            UserName = TempTable.Rows[0]["UserName"].ToString();
                            UserID = Convert.ToInt32(TempTable.Rows[0]["UserID"].ToString());
                            ContactId = Convert.ToInt32(TempTable.Rows[0]["ContactId"].ToString());
                            IsActive = Convert.ToBoolean(TempTable.Rows[0]["IsActive"].ToString());
                            IsUserMale = Convert.ToBoolean(TempTable.Rows[0]["IsUserMale"].ToString());
                        }
                        else
                        {
                            FullName = "N/A";
                            UserName = "N/A";
                            UserID = -1;
                            ContactId = -1;
                            IsActive = IsUserMale = false;
                        }

                        if (sqlconn.State != ConnectionState.Closed)
                        {
                            sqlconn.Close();
                        }

                    }
                    catch (Exception)
                    {
                        FullName = "N/A";
                        UserName = "N/A";
                        UserID = -1;
                        ContactId = -1;
                        IsActive = IsUserMale = false;
                    }

                }
            }
        }

        #endregion

        #region UserRight
        public DataTable L_GetUiObjects()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetUiObjects", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "UiObjects";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetUiObjectsByMainId(int MainObjectId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetUiObjectsByMainId", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@MainObjectId", SqlDbType.Int).Value = MainObjectId;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }

                        TempTable.TableName = "UiObjectsByMainId";
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int L_AddUpdateUiObject(int ObjectId, int ObjectMainId, string ObjectName, string ObjectText)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.AddUpdateUiObject", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = ObjectId;
                        sqlCmd.Parameters.Add("@MainId", SqlDbType.Int).Value = ObjectMainId;
                        sqlCmd.Parameters.Add("@ObjectName", SqlDbType.VarChar).Value = ObjectName;
                        sqlCmd.Parameters.Add("@ObjectText", SqlDbType.VarChar).Value = ObjectText;

                        SqlParameter RetVal = sqlCmd.Parameters.Add("@returnValue", SqlDbType.Int);
                        RetVal.Direction = ParameterDirection.ReturnValue;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.ExecuteNonQuery();

                        return Convert.ToInt32(RetVal.Value);
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int L_AddUser(int UserID, string UserName, string Description, string Password, int ContactId, bool IsActive)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.AddUser", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                        sqlCmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
                        sqlCmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = Password;
                        sqlCmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                        sqlCmd.Parameters.Add("@ContactId", SqlDbType.Int).Value = ContactId;
                        sqlCmd.Parameters.Add("@IsActive", SqlDbType.Int).Value = IsActive;

                        SqlParameter RetVal = sqlCmd.Parameters.Add("@returnValue", SqlDbType.Int);
                        RetVal.Direction = ParameterDirection.ReturnValue;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.ExecuteNonQuery();

                        return Convert.ToInt32(RetVal.Value);
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void L_CatUiObjects(int MainId, List<UserPrivilege> Ids)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.CatUiObjects", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        for (int i = 0; i < Ids.Count; i++)
                        {
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = Ids[i].Id;
                            sqlCmd.Parameters.Add("@MainId", SqlDbType.Int).Value = MainId;

                            sqlCmd.ExecuteNonQuery();
                        }

                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool L_RemoveUiObject(int ObjectId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.RemoveUiObject", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = ObjectId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        return (sqlCmd.ExecuteNonQuery() > 0);
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool L_RemoveSubFromMainObject(int ObjectId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.RemoveSubFromMainObject", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = ObjectId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        return (sqlCmd.ExecuteNonQuery() > 0);
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllUsers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllUsers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "AllUsers";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetEmployees()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetEmployees", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "Employees";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllActiveRoles()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllActiveRoles", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "AllActiveRoles";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllRoles()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllRoles", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "AllRoles";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public List<UserPrivilege> L_GetUiObjectsInHierarchyOrder()
        {
            List<UserPrivilege> pl = new List<UserPrivilege>();
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetUiObjectsInHierarchyOrder", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        //IDataReader rdr = sqlCmd.ExecuteReader();
                        //TempTable = Database.DataReader2DataTable(rdr);
                        if (TempTable.Rows.Count > 0)
                        {
                            TempTable.Clear();
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            da.Fill(TempTable);
                        }

                        TempTable.TableName = "UiObjectsInHierarchyOrder";
                        for (int i = 0; i < TempTable.Rows.Count; i++)
                        {
                            UserPrivilege p = new UserPrivilege();
                            p.Id = Convert.ToInt32(TempTable.Rows[i]["Id"]);
                            p.ObjectName = TempTable.Rows[i]["ObjectName"].ToString();
                            p.ObjectText = TempTable.Rows[i]["ObjectText"].ToString();
                            p.MainId = Convert.ToInt32(TempTable.Rows[i]["MainId"]);
                            p.UiObjectLevel = Convert.ToInt32(TempTable.Rows[i]["UiObjectLevel"]);
                            p.SetSel = Convert.ToBoolean(TempTable.Rows[i]["SetSel"]);
                            pl.Add(p);
                        }

                        return pl;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable L_GetPrincipalRights(int PrincipalId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetPrincipalRights", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = PrincipalId;
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "PrincipalRights";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool L_ChangeUserRights(List<UserPrivilege> EmptyLs, List<UserPrivilege> GrantLs, List<UserPrivilege> DenyLs, bool IsUser)
        {
            try
            {

                using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
                {
                    try
                    {
                        for (int i = 0; i < EmptyLs.Count; i++)
                        {
                            if (IsUser)
                            {
                                UpdateUserRights(EmptyLs[i], sqlconn);
                            }
                            else
                            {
                                UpdateRoleRights(EmptyLs[i], sqlconn);
                            }
                        }

                        for (int i = 0; i < GrantLs.Count; i++)
                        {
                            if (IsUser)
                            {
                                UpdateUserRights(GrantLs[i], sqlconn);
                            }
                            else
                            {
                                UpdateRoleRights(GrantLs[i], sqlconn);
                            }
                        }

                        for (int i = 0; i < DenyLs.Count; i++)
                        {
                            if (IsUser)
                            {
                                UpdateUserRights(DenyLs[i], sqlconn);
                            }
                            else
                            {
                                UpdateRoleRights(DenyLs[i], sqlconn);
                            }
                        }
                    }
                    finally
                    {
                        if (sqlconn.State == ConnectionState.Open)
                        {
                            sqlconn.Close();
                        }
                    }
                }


            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool UpdateUserRights(UserPrivilege ur, SqlConnection sqlconn)
        {

            using (SqlCommand sqlCmd = new SqlCommand("Sec.ChangeUserRights", sqlconn) { CommandType = CommandType.StoredProcedure })
            {
                sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = ur.PrincipalId;
                sqlCmd.Parameters.Add("@OperationId", SqlDbType.Int).Value = ur.OperationId;
                sqlCmd.Parameters.Add("@ObjectId", SqlDbType.Int).Value = ur.ObjectId;
                sqlCmd.Parameters.Add("@PermissionId", SqlDbType.Int).Value = ur.PermissionId;

                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

                return (sqlCmd.ExecuteNonQuery() > 0);
            }

        }

        public bool UpdateRoleRights(UserPrivilege ur, SqlConnection sqlconn)
        {

            using (SqlCommand sqlCmd = new SqlCommand("Sec.ChangeGroupRights", sqlconn) { CommandType = CommandType.StoredProcedure })
            {
                sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = ur.PrincipalId;
                sqlCmd.Parameters.Add("@OperationId", SqlDbType.Int).Value = ur.OperationId;
                sqlCmd.Parameters.Add("@ObjectId", SqlDbType.Int).Value = ur.ObjectId;
                sqlCmd.Parameters.Add("@PermissionId", SqlDbType.Int).Value = ur.PermissionId;

                if (sqlconn.State == ConnectionState.Closed)
                {
                    sqlconn.Open();
                }

                return (sqlCmd.ExecuteNonQuery() > 0);
            }

        }

        public bool L_AutoAddUiObjects(DataTable UiObjects)
        {
            bool IsOK = false;
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.AutoAddUiObjects", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@UiObjects", SqlDbType.Structured).Value = UiObjects;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        IsOK = sqlCmd.ExecuteNonQuery() > 0;

                        return IsOK;
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }


        public DataTable L_GetAllUsersObjects(int UserId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllUsersObjects", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@Id", SqlDbType.Int).Value = UserId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "AllUsersObjects";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public int L_AddRole(int PrincipalId, string Description, bool IsActive)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.AddRole", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = PrincipalId;
                        sqlCmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = Description;
                        sqlCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = IsActive;

                        SqlParameter RetVal = sqlCmd.Parameters.Add("@returnValue", SqlDbType.Int);
                        RetVal.Direction = ParameterDirection.ReturnValue;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.ExecuteNonQuery();

                        return Convert.ToInt32(RetVal.Value);
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetUsersInRole(int RoleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetUsersInRole", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = RoleId;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "UsersInRole";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public DataTable L_GetAllActiveUsers()
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.GetAllActiveUsers", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "AllActiveUsers";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void L_AddUserToRole(List<int> pl, int RoleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.AddUserToRole", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        for (int i = 0; i < pl.Count; i++)
                        {
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = pl[i];
                            sqlCmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = RoleId;
                            sqlCmd.ExecuteNonQuery();
                        }

                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public void L_RemoveUserFromRole(List<int> pl, int RoleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.RemoveUserFromRole", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        for (int i = 0; i < pl.Count; i++)
                        {
                            sqlCmd.Parameters.Clear();
                            sqlCmd.Parameters.Add("@PrincipalId", SqlDbType.Int).Value = pl[i];
                            sqlCmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = RoleId;
                            sqlCmd.ExecuteNonQuery();
                        }
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }

        public bool L_ChangeUserPassword(int UserId, string OldPwd, string NewPwd)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.ChangeUserPassword", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                        sqlCmd.Parameters.Add("@OldPwd", SqlDbType.VarChar).Value = OldPwd;
                        sqlCmd.Parameters.Add("@NewPwd", SqlDbType.VarChar).Value = NewPwd;

                        SqlParameter RetVal = sqlCmd.Parameters.Add("@returnValue", SqlDbType.Int);
                        RetVal.Direction = ParameterDirection.ReturnValue;

                        sqlCmd.ExecuteNonQuery();
                        if (Convert.ToInt32(RetVal.Value) == 1)
                        {
                            return true;
                        }
                        else if (Convert.ToInt32(RetVal.Value) == -1)
                        {
                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                finally
                {
                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }

        }


        public DataTable PrincipleUpdateCopyUserRights(int FromPrincipleId, int ToPrincipleId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sec.PrincipleUpdateCopyUserRights", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        sqlCmd.Parameters.Add("@FromPrincipleId", SqlDbType.Int).Value = FromPrincipleId;
                        sqlCmd.Parameters.Add("@ToPrincipleId", SqlDbType.Int).Value = ToPrincipleId;

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "PrincipleUpdateCopyUserRights";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion

      

        #region ConnectioString
        public  string CreateSqlConnectionString(string Server, string Database, string UserID, string Password)
        {
            ConnStr.Clear();
            ConnStr.Append("Data Source="); ConnStr.Append(Server);
            ConnStr.Append(";Initial Catalog="); ConnStr.Append(Database);
            ConnStr.Append(";User Id="); ConnStr.Append(UserID);
            ConnStr.Append(";Password="); ConnStr.Append(Password); ConnStr.Append(";");
            return ConnStr.ToString();
        }

        public void CreateSqlConnectionString(int CountryId)
        {
            //DataService.Database.CreateSqlConnectionString(@"SAMEER-PC\SQLSERVER2k12", "DSDB", "sa", "YsecIT2008");
            //DataService.Database.CreateSqlConnectionString(@"192.168.248.245,41433", "DSDB", "DSADBd3v", "India$$");
            //connStr =  DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456"); // Server Publish

            #region LocalConnection
            if (CountryId == 1)
            {
                connStr = CreateSqlConnectionString(@"10.200.10.16", "DSDB", "retaildb", "YsecIT@@");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.190", "DSDB", "appuser", "@ppUs3R");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.211", "DSDB", "sa", "YsecIT2008");
                //connStr = DataService.Database.CreateSqlConnectionString(@"SHIV29\MSSQL16", "DSDB", "sa", "123456");
                //DataService.Database.GetCountryConnectionString(@"192.168.248.238", "DSDB", "appuser", "123456");
            }
            else if (CountryId == 2)
            {
                connStr = CreateSqlConnectionString(@"10.200.10.16", "DSDB", "retaildb", "YsecIT@@");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.190", "DSDB", "appuser", "@ppUs3R");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.211", "DSDB", "sa", "YsecIT2008");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.190", "DSDB", "appuser", "@ppUs3R");
                //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.238", "DSDB", "appuser", "123456");
                //DataService.Database.GetCountryConnectionString(@"192.168.248.236\SQLSERVER", "DSDB", "appuser", "123456");
            }

            // Vijay - PC
            //if (CountryId == 1)
            //{
            //    connStr = DataService.Database.CreateSqlConnectionString(@"SUPPORT-PC", "DSDB", "sa", "123456");
            //    DataService.Database.GetCountryConnectionString(@"SUPPORT-PC", "DSDB", "sa", "123456");
            //}
            //else if (CountryId == 2)
            //{
            //    connStr = DataService.Database.CreateSqlConnectionString(@"SUPPORT-PC", "DSDB", "sa", "123456");
            //    DataService.Database.GetCountryConnectionString(@"SUPPORT-PC", "DSDB", "sa", "123456");
            //}
            // End
            #endregion

            #region Serverconnection

            //if (CountryId == 1)
            //{
            //    connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.32", "DSDB", "ysecit", "y68t2k8@sr"); // Suriname connection
            //    //connStr = DataService.Database.CreateSqlConnectionString(@"davdb", "DSDB", "ysecit", "y68t2k8@sr"); // Suriname connection
            //    try
            //    {
            //        //DataService.Database.GetCountryConnectionString(@"db9", "DSDB", "appuser", "@ppUs3R");
            //        DataService.Database.GetCountryConnectionString(@"192.168.247.230", "DSDB", "appuser", "@ppUs3R");
            //    }
            //    catch
            //    {

            //    }//    //if (PingHost("db9"))
            //    //{
            //    //    DataService.Database.GetCountryConnectionString(@"192.168.10.252", "DSDB", "sa", "YsecIT2008");
            //    //}//    //else if (PingHost("db9,11433"))
            //    //{
            //    //    DataService.Database.GetCountryConnectionString(@"192.168.10.252:11433", "DSDB", "sa", "YsecIT2008");
            //    //}
            //}
            //else if (CountryId == 2)
            //{
            //    try
            //    {
            //        DataService.Database.GetCountryConnectionString(@"192.168.247.32", "DSDB", "ysecit", "y68t2k8@sr");
            //        //DataService.Database.GetCountryConnectionString(@"davdb", "DSDB", "ysecit", "y68t2k8@sr");
            //    }
            //    catch
            //    {

            //    }

            //    //connStr = DataService.Database.CreateSqlConnectionString(@"db9", "DSDB", "appuser", "@ppUs3R"); // Curacao connection

            //    connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.230", "DSDB", "appuser", "@ppUs3R");

            //    // connStr = DataService.Database.CreateSqlConnectionString(@"db9,11433", "DSDB", "appuser", "@ppUs3R"); // Curacao connection
            //    //if (PingHost("db9"))
            //    //{
            //    //    connStr = DataService.Database.CreateSqlConnectionString(@"192.168.10.252", "DSDB", "sa", "YsecIT2008"); // Curacao connection
            //    //}
            //    //else if (PingHost("db9,11433"))
            //    //{
            //    //    connStr = DataService.Database.CreateSqlConnectionString(@"192.168.10.252:11433", "DSDB", "sa", "YsecIT2008"); // Curacao connection
            //    //}
            //}

            #endregion


            //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.237\SQL", "DSDB", "appuser", "123456");
            //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.248.237\SQL", "DSDB", "appuser", "123456");
            //DataService.Database.CreateSqlConnectionString(@"dv.ysecit.com", "DSDB", "sa", "D@^!nc!");//DataService.Database.CreateSqlConnectionString(@"dj-pc\denali", "DSDB", "sa", "ysecit");
            //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.32", "DSDB", "ysecit", "y68t2k8@sr");
            //DataService.Database.CreateSqlConnectionString(@"192.168.247.46", "DSDB", "admin", "YsecIT2012");
            //connStr = DataService.Database.CreateSqlConnectionString(@"192.168.247.235", "DSDB", "sa", "YsecIT2008");

            // New Server Publish
            //DataService.Database.CreateSqlConnectionString(@"dj", "DSDB", "sa", "P@$$4SQLadmin"); 
            //DataService.Database.CreateSqlConnectionString(@"192.168.247.46", "DSDB", "ysecit", "y68t2k8@sr");
        }



        #endregion

        //New added
        #region New added

        #region GetCompanyForlocations
        public  DataTable GetCompanyForlocations()
        {
            DataTable TempTable = new DataTable();

            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Company.GetCompanyForlocations", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }

                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                da.Fill(dt);
                                TempTable = dt;
                            }
                        }
                        TempTable.TableName = "Companies";

                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region AddLocationEntry
        public  DataSet AddLocationEntry(int companyId, string locationCode, string locationName, string address, string fullName, string phone, string moblie, string fax,
           string email, string website, string note, int countryId, bool isMainWarehouse, bool isMainBank, int createdBy, bool isOutlet)
        {
            DataSet dsAddLocationEntry = new DataSet();
            DataTable dtAddLocationEntry = new DataTable();

            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {

                    using (SqlCommand sqlcmd = new SqlCommand("Company.NewLocationRegistration", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlcmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
                        sqlcmd.Parameters.Add("@LocationCode", SqlDbType.VarChar).Value = locationCode;
                        sqlcmd.Parameters.Add("@Locationname", SqlDbType.VarChar).Value = locationName;
                        sqlcmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = address;
                        sqlcmd.Parameters.Add("@FullName", SqlDbType.VarChar).Value = fullName;
                        sqlcmd.Parameters.Add("@Phone", SqlDbType.VarChar).Value = phone;
                        sqlcmd.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = moblie;
                        sqlcmd.Parameters.Add("@Fax", SqlDbType.VarChar).Value = fax;
                        sqlcmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                        sqlcmd.Parameters.Add("@Website", SqlDbType.VarChar).Value = website;
                        sqlcmd.Parameters.Add("@Note", SqlDbType.VarChar).Value = note;
                        sqlcmd.Parameters.Add("@countryId", SqlDbType.Int).Value = countryId;
                        sqlcmd.Parameters.Add("@IsMainWarehouse", SqlDbType.Bit).Value = isMainWarehouse;
                        sqlcmd.Parameters.Add("@IsMainBank", SqlDbType.Bit).Value = isMainBank;
                        sqlcmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = createdBy;
                        sqlcmd.Parameters.Add("@IsOutlet", SqlDbType.Bit).Value = isOutlet;

                        if (sqlconn.State != ConnectionState.Open)
                        {
                            sqlconn.Open();
                        }
                        dtAddLocationEntry = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            using (DataSet dSet = new DataSet())
                            {
                                da.Fill(dSet);
                                dsAddLocationEntry = dSet;
                            }
                        }
                        return dsAddLocationEntry;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region AddUpdateCashbackAmount
        public  DataSet AddUpdateCashbackAmount(int CashbackAmountId, int CompanyId, decimal CashbackBracketsUpTo, decimal CashbackAmount, bool IsEnabled, int UpdatedBy)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.AddUpdateCashbackAmount", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CashbackAmountId", SqlDbType.Int).Value = CashbackAmountId;
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@CashbackBracketsUpTo", SqlDbType.Int).Value = CashbackBracketsUpTo;
                        sqlCmd.Parameters.Add("@CashbackAmount", SqlDbType.Decimal).Value = CashbackAmount;
                        sqlCmd.Parameters.Add("@IsEnabled", SqlDbType.Bit).Value = IsEnabled;
                        sqlCmd.Parameters.Add("@UpdatedBy", SqlDbType.Int).Value = UpdatedBy;
                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempDataSet = new DataSet();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataSet ds = new DataSet())
                            {
                                da.Fill(ds);
                                TempDataSet = ds;
                            }
                        }
                        return TempDataSet;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region GetCashbackDetails
        public  DataTable GetCashbackDetails(int CompanyId)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.GetCashbackDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dtCashbackDetails = new DataTable())
                            {
                                da.Fill(dtCashbackDetails);
                                TempTable = dtCashbackDetails;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #region GetCashbackVoucherDetails
        public  DataTable GetCashbackVoucherDetails(int CompanyId, DateTime fromDate, DateTime toDate)
        {
            using (SqlConnection sqlconn = new SqlConnection(AppSettings.DaVinciConnectionString))
            {
                try
                {
                    using (SqlCommand sqlCmd = new SqlCommand("Sale.DisplayCashbackDetails", sqlconn) { CommandType = CommandType.StoredProcedure })
                    {
                        sqlCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                        sqlCmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                        sqlCmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;

                        if (sqlconn.State == ConnectionState.Closed)
                        {
                            sqlconn.Open();
                        }
                        TempTable = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            using (DataTable dtCashbackDetails = new DataTable())
                            {
                                da.Fill(dtCashbackDetails);
                                TempTable = dtCashbackDetails;
                            }
                        }
                        return TempTable;
                    }
                }
                finally
                {

                    if (sqlconn.State == ConnectionState.Open)
                    {
                        sqlconn.Close();
                    }
                }
            }
        }
        #endregion

        #endregion
    }

}


