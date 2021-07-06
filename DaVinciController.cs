using DaVinci.Models.DaVinciAdmin;
using DaVinciAdminApi.Helper;
using DaVinciAdminApi.Repositories;
using DaVinciAdminApi.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DaVinciAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaVinciController : ControllerBase
    {
        static readonly IDaVinci _dalDaVinci = new DaVinciRepository();
        public static SqlConnection _sqlConnection;
        private static string connStr;
        private readonly ILogger<DaVinciController> _logger;
        private readonly AppSettings _appSettings;
        public DaVinciController(ILogger<DaVinciController> logger , IOptions<AppSettings> option)
        {
            _logger = logger;
            _appSettings = option.Value;

        }

        #region AppData Folder
        #region AppData
        [Route("GetCompanyInfo")]
        [HttpPost]
        public IActionResult GetCompanyInfo(Dictionary<string, object> Param)
        {
            try
            {
                 int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                 int CompanyId = 0;
                 int  CompanyContactId = 0;
                 string CompanyName = "";
                 byte[]  Logo = null;
                 byte[]  Icon = null;
                 int  IsActive = 0;
                 string LocalCurrencyCode = "";

                _dalDaVinci.L_GetCompanyInfo(TillId, out CompanyId, out CompanyContactId, out CompanyName, out Logo, out Icon, out IsActive, out LocalCurrencyCode);
                GetCompanyInfo companyInfo = new GetCompanyInfo();
                companyInfo.CompanyId = CompanyId;
                companyInfo.CompanyContactId = CompanyContactId;
                companyInfo.CompanyName = CompanyName;
                companyInfo.Logo = Logo;
                companyInfo.Icon = Icon;
                companyInfo.IsActive = IsActive;
                companyInfo.LocalCurrencyCode = LocalCurrencyCode;
                return Ok(companyInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCompanyInfo.");
            }
            return NotFound();
        }
        [Route("GetAssistants")]
        [HttpGet]
        public IActionResult GetAssistants()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAssistants();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAssistants.");
            }
            return NotFound();
        }


        [Route("GetParameters")]
        [HttpGet]
        public IActionResult GetParameters()
        {
            try
            {
                Dictionary<string, string> result = _dalDaVinci.L_GetParameters();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetParameters.");
            }
            return NotFound();
        }
        [Route("GetAllActiveTills")]
        [HttpGet]
        public IActionResult GetAllActiveTills()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetAllActiveTills();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllActiveTills.");
            }
            return NotFound();
        }
        [Route("UpdateTillImages")]
        [HttpPost]
        public IActionResult UpdateTillImages(Dictionary<string, object> Param)
        {
            try
            {
                int userid = Int32.Parse(Convert.ToString(Param["userid"]));
                int tillid = Int32.Parse(Convert.ToString(Param["tillid"]));
                int imgid = Int32.Parse(Convert.ToString(Param["imgid"]));
                string imagename = (Convert.ToString(Param["imagename"]));
                string imagetype = (Convert.ToString(Param["imagetype"]));
                decimal sizekb = Convert.ToDecimal(Convert.ToString(Param["sizekb"]));
                string pixels = (Convert.ToString(Param["pixels"]));
                byte[] logo = Convert.FromBase64String(Convert.ToString(Param["logo"]));
                byte[] ico = Convert.FromBase64String(Convert.ToString(Param["ico"]));
                byte[] imgbutton = Convert.FromBase64String(Convert.ToString(Param["imgbutton"]));


                bool result = _dalDaVinci.UpdateTillImages(userid, tillid, imgid, imagename, imagetype, sizekb, pixels, logo, ico, imgbutton);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateTillImages.");
            }
            return NotFound();
        }
        [Route("UpdatePIDNumber")]
        [HttpPost]
        public IActionResult UpdatePIDNumber(Dictionary<string, object> Param)
        {
            try
            {
                int userId = Int32.Parse(Convert.ToString(Param["userId"]));
                int tillid = Int32.Parse(Convert.ToString(Param["tillid"]));
                string pID = (Convert.ToString(Param["pID"]));
                DataTable dtResult = _dalDaVinci.UpdatePIDNumber(userId, tillid, pID);
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateTillImages.");
            }
            return NotFound();
        }


        #endregion

        #region VersionType
        [Route("GetDisplayAllVersionWithVersionType")]
        [HttpGet]
        public IActionResult GetDisplayAllVersionWithVersionType(int UpdateVersionTypeId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayAllVersionWithVersionType(UpdateVersionTypeId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllVersionWithVersionType.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllVersionDetailsWithUpdateVersionId")]
        [HttpGet]
        public IActionResult GetDisplayAllVersionDetailsWithUpdateVersionId(int UpdateVersionTypeId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayAllVersionDetailsWithUpdateVersionId(UpdateVersionTypeId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllVersionDetailsWithUpdateVersionId.");
            }
            return NotFound();
        }

        #endregion

        #endregion

        #region AppSettingsFolder
        #region Skins
        [Route("GetUserSkin")]
        [HttpGet]
        public IActionResult GetUserSkin(int UserId)
        {
            try
            {
                string result = _dalDaVinci.L_GetUserSkin(UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetUserSkin.");
            }
            return NotFound();
        }

        [Route("AddUserSkin")]
        [HttpPost]
        public IActionResult AddUserSkin(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string SkinName = (Convert.ToString(Param["SkinName"]));
                bool result = _dalDaVinci.L_AddUserSkin(UserId, SkinName);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_AddUserSkin.");
            }
            return NotFound();
        }


        #endregion

        #endregion

        #region BalanceSheet Folder

        #region AccountTransactionHistory
        [Route("GetDisplayAccountTransactionHistoryByDateAndTill")]
        [HttpPost]
        public IActionResult GetDisplayAccountTransactionHistoryByDateAndTill(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime TransactionDateTime = Convert.ToDateTime(Param["TransactionDateTime"].ToString());
                DataTable dtResult = _dalDaVinci.GetDisplayAccountTransactionHistoryByDateAndTill(TillId, TransactionDateTime);

                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAccountTransactionHistoryByDateAndTill.");
            }
            return NotFound();
        }


        #endregion

        #region CompanyDetails
        [Route("GetDisplayCompanyDetails")]
        [HttpGet]
        public IActionResult GetDisplayCompanyDetails()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetDisplayCompanyDetails();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayCompanyDetailsNotAll")]
        [HttpGet]
        public IActionResult GetDisplayCompanyDetailsNotAll()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetDisplayCompanyDetailsNotAll();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyDetailsNotAll.");
            }
            return NotFound();
        }

        [Route("GetSummaryReportSalePerformanceAnalysis")]
        [HttpPost]
        public IActionResult GetSummaryReportSalePerformanceAnalysis(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataSet dsResult = _dalDaVinci.GetSummaryReportSalePerformanceAnalysis(FromDate, ToDate, CompanyId);

                return Ok(dsResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSummaryReportSalePerformanceAnalysis.");
            }
            return NotFound();
        }

        [Route("DisplayAddUpdateBalancesSheet")]
        [HttpPost]
        public IActionResult DisplayAddUpdateBalancesSheet(Dictionary<string, object> Param)
        {
            try
            {
                int TillID = Int32.Parse(Convert.ToString(Param["TillID"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                bool result = _dalDaVinci.DisplayAddUpdateBalancesSheet(TillID, AccountingDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateBalancesSheet.");
            }
            return NotFound();
        }

        [Route("DisplayAddUpdateSalesHeadCount")]
        [HttpPost]
        public IActionResult DisplayAddUpdateSalesHeadCount(Dictionary<string, object> Param)
        {
            try
            {
                DataTable SalesHeadCount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesHeadCount"].ToString(), typeof(DataTable));
                int LocationId = Int32.Parse(Convert.ToString(Param["LocationId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int UpdatedBy = Int32.Parse(Convert.ToString(Param["UpdatedBy"]));
                DataSet dsResult = _dalDaVinci.DisplayAddUpdateSalesHeadCount(SalesHeadCount, LocationId, CompanyId, UpdatedBy);

                return Ok(dsResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateSalesHeadCount.");
            }
            return NotFound();
        }

        #endregion

        #region CompanyDetailsBrand
        [Route("DisplayCompanyDetailsForBrand")]
        [HttpGet]
        public IActionResult DisplayCompanyDetailsForBrand()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.DisplayCompanyDetailsForBrand();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayCompanyDetailsForBrand.");
            }
            return NotFound();
        }

        [Route("DisplayLocationDetailsForBrand")]
        [HttpGet]
        public IActionResult DisplayLocationDetailsForBrand(int companyParentId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.DisplayLocationDetailsForBrand(companyParentId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayLocationDetailsForBrand.");
            }
            return NotFound();
        }

        [Route("DisplayBrandDetailsForLocation")]
        [HttpPost]
        public IActionResult DisplayBrandDetailsForLocation(Dictionary<string, object> Param)
        {
            try
            {

                int companyParentId = Int32.Parse(Convert.ToString(Param["companyParentId"]));
                int companyId = Int32.Parse(Convert.ToString(Param["companyId"]));
                DateTime fromDate = Convert.ToDateTime(Param["fromDate"].ToString());
                DateTime toDate = Convert.ToDateTime(Param["toDate"].ToString());
                DataSet dsResult = _dalDaVinci.DisplayBrandDetailsForLocation(companyParentId, companyId, fromDate, toDate);

                return Ok(dsResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayBrandDetailsForLocation.");
            }
            return NotFound();
        }

        [Route("GetStockQtyandPricesCompanyDetails")]
        [HttpPost]
        public IActionResult GetStockQtyandPricesCompanyDetails(Dictionary<string, object> Param)
        {
            try
            {
                string BrandId = (Convert.ToString(Param["BrandId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                int ParentCompanyId = Int32.Parse(Convert.ToString(Param["ParentCompanyId"]));
                DataTable dtResult = _dalDaVinci.GetStockQtyandPricesCompanyDetails(BrandId, CompanyId, FromDate, Todate, ParentCompanyId);

                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockQtyandPricesCompanyDetails.");
            }
            return NotFound();
        }

        #endregion

        #region MoneyTransferHistory
        [Route("GetDisplayAllCurrentMoneyTransferDetails")]
        [HttpPost]
        public IActionResult GetDisplayAllCurrentMoneyTransferDetails(Dictionary<string, object> Param)
        {
            try
            {

                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet dsResult = _dalDaVinci.GetDisplayAllCurrentMoneyTransferDetails(TillId, AccountingDate);

                return Ok(dsResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCurrentMoneyTransferDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayReportForSelectTransferSpecification")]
        [HttpPost]
        public IActionResult GetDisplayReportForSelectTransferSpecification(Dictionary<string, object> Param)
        {
            try
            {
                int CashTransferId = Int32.Parse(Convert.ToString(Param["CashTransferId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataSet dsResult = _dalDaVinci.GetDisplayReportForSelectTransferSpecification(CashTransferId, UserId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReportForSelectTransferSpecification.");
            }
            return NotFound();
        }


        #endregion

        #endregion

        #region CashMgmtFolder

        #region AccountingDate
        // DataTable GetDisplayDefaultAccountTransactionDetails(int TillId);
        //[Route("GetDisplayDefaultAccountTransactionDetails")]
        //[HttpGet]
        //public IActionResult GetDisplayDefaultAccountTransactionDetails(int TillId)
        //{
        //    try
        //    {

        //        DataTable result = _dalDaVinci.GetDisplayDefaultAccountTransactionDetails(TillId);

        //        return Ok(result);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDefaultAccountTransactionDetails.");
        //    }
        //    return NotFound();
        //}

        [Route("GetCloseCurrentAccountingDate")]
        [HttpGet]
        public IActionResult GetCloseCurrentAccountingDate(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetCloseCurrentAccountingDate(TillId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCloseCurrentAccountingDate.");
            }
            return NotFound();
        }

        [Route("GetOpenNewAccountingDate")]
        [HttpGet]
        public IActionResult GetOpenNewAccountingDate(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetOpenNewAccountingDate(TillId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetOpenNewAccountingDate.");
            }
            return NotFound();
        }

        #endregion

        #region  AccountTransaction

        [Route("Get_DisplayDefaultAccountTransactionDetails")]
        [HttpGet]
        public IActionResult Get_DisplayDefaultAccountTransactionDetails()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetDisplayDefaultAccountTransactionDetails();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDefaultAccountTransactionDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayAccountTransactionHistory")]
        [HttpGet]
        public IActionResult GetDisplayAccountTransactionHistory(int TillId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayAccountTransactionHistory(TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAccountTransactionHistory.");
            }
            return NotFound();
        }

        #endregion

        #region AccountTransactionHistory
        [Route("GetAddNewAccountTransaction")]
        [HttpPost]
        public IActionResult GetAddNewAccountTransaction(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal CurrencyAmount = Convert.ToDecimal(Convert.ToString(Param["CurrencyAmount"]));
                string TransactionDesc = (Convert.ToString(Param["TransactionDesc"]));
                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int AccountId = Int32.Parse(Convert.ToString(Param["AccountId"]));
                int TransactionTypesId = Int32.Parse(Convert.ToString(Param["TransactionTypesId"]));
                int DenominationTypeId = Int32.Parse(Convert.ToString(Param["DenominationTypeId"]));
                DataTable dtResult = _dalDaVinci.GetAddNewAccountTransaction(TillId, UserId, CurrencyAmount, TransactionDesc, CurrencyId, AccountId, TransactionTypesId, DenominationTypeId);

                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddNewAccountTransaction.");
            }
            return NotFound();
        }

        #endregion

        #region CashTransfer

        [Route("GetDisplayTillCashTransfers")]
        [HttpPost]
        public IActionResult GetDisplayTillCashTransfers(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dtResult = _dalDaVinci.GetDisplayTillCashTransfers(FromDate, ToDate, UserId);

                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayTillCashTransfers.");
            }
            return NotFound();
        }


        [Route("GetDisplayReloadTillCashTransfers")]
        [HttpPost]
        public IActionResult GetDisplayReloadTillCashTransfers(Dictionary<string, object> Param)
        {
            try
            {

                DateTime RunningDate = Convert.ToDateTime(Param["RunningDate"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dtResult = _dalDaVinci.GetDisplayReloadTillCashTransfers(RunningDate, UserId);
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReloadTillCashTransfers.");
            }
            return NotFound();
        }

        #endregion

        #region Currency

        [Route("DisplayActiveCurrency")]
        [HttpGet]
        public IActionResult DisplayActiveCurrency()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayActiveCurrency();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayActiveCurrency.");
            }
            return NotFound();
        }

        [Route("DisplauUpdateCurrencyIcon")]
        [HttpPost]
        public IActionResult DisplauUpdateCurrencyIcon(Dictionary<string, object> Param)
        {
            try
            {


                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                byte[] CurrencyIcon = Convert.FromBase64String(Convert.ToString(Param["CurrencyIcon"]));
                DataTable dtResult = _dalDaVinci.DisplauUpdateCurrencyIcon(CurrencyId, CurrencyIcon);
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplauUpdateCurrencyIcon.");
            }
            return NotFound();
        }

        #endregion

        #region EditAccountTransaction

        [Route("GetDisplayAccountTransactionHistoryByAccountTransId")]
        [HttpGet]
        public IActionResult GetDisplayAccountTransactionHistoryByAccountTransId(long AccountTransId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayAccountTransactionHistoryByAccountTransId(AccountTransId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAccountTransactionHistoryByAccountTransId.");
            }
            return NotFound();
        }
        [Route("GetDisplayEditTransactionHistory")]
        [HttpPost]
        public IActionResult GetDisplayEditTransactionHistory(Dictionary<string, object> Param)
        {
            try
            {

                long AccountTransId = Int64.Parse(Convert.ToString(Param["AccountTransId"]));
                int AccountId = Int32.Parse(Convert.ToString(Param["AccountId"]));
                int DenominationValueTypeId = Int32.Parse(Convert.ToString(Param["DenominationValueTypeId"]));
                DataTable dtResult = _dalDaVinci.GetDisplayEditTransactionHistory(AccountTransId, AccountId, DenominationValueTypeId);
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayEditTransactionHistory.");
            }
            return NotFound();
        }

        #endregion

        #region EditCashTransferSpeicification
        [Route("GetHODisplayCashTransferForTill")]
        [HttpPost]
        public IActionResult GetHODisplayCashTransferForTill(Dictionary<string, object> Param)
        {
            try
            {
                int TillID = Int32.Parse(Convert.ToString(Param["TillID"]));
                DateTime CashTransferDate = Convert.ToDateTime(Param["CashTransferDate"].ToString());
                DataSet dsResult = _dalDaVinci.GetHODisplayCashTransferForTill(TillID, CashTransferDate);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODisplayCashTransferForTill.");
            }
            return NotFound();
        }

        [Route("GetHODisplayCloseAccountingDate")]
        [HttpGet]
        public IActionResult GetHODisplayCloseAccountingDate(int TillId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetHODisplayCloseAccountingDate(TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODisplayCloseAccountingDate.");
            }
            return NotFound();
        }

        [Route("GetHODenominationTotalValueInUSD")]
        [HttpPost]
        public IActionResult GetHODenominationTotalValueInUSD(Dictionary<string, object> Param)
        {
            try
            {
                int TillID = Int32.Parse(Convert.ToString(Param["TillID"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetHODenominationTotalValueInUSD(TillID, AccountingDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODenominationTotalValueInUSD.");
            }
            return NotFound();
        }


        //[Route("GetDisplayDenominationValueTypes")]
        //[HttpGet]
        //public IActionResult GetDisplayDenominationValueTypes()
        //{
        //    try
        //    {
        //        DataTable dtResult = _dalDaVinci.GetDisplayDenominationValueTypes();
        //        return Ok(dtResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDenominationValueTypes.");
        //    }
        //    return NotFound();
        //}

        [Route("GetHODisplayDenominationTypeDetails")]
        [HttpGet]
        public IActionResult GetHODisplayDenominationTypeDetails(int DenominationValueTypeId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetHODisplayDenominationTypeDetails(DenominationValueTypeId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODisplayDenominationTypeDetails.");
            }
            return NotFound();
        }


        [Route("GetHODisplayDenominationTypeDetails")]
        [HttpPost]
        public IActionResult GetHODisplayDenominationTypeDetails(Dictionary<string, object> Param)
        {
            try
            {
                int TillID = Int32.Parse(Convert.ToString(Param["TillID"]));
                int DenominationTypeId = Int32.Parse(Convert.ToString(Param["DenominationTypeId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                int CashTransferId = Int32.Parse(Convert.ToString(Param["CashTransferId"]));
                DataTable dtResult = _dalDaVinci.GetHODisplayDenominationTypeDetails(TillID, DenominationTypeId, AccountingDate, CashTransferId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODisplayDenominationTypeDetails.");
            }
            return NotFound();
        }

        [Route("GetHODisplayDenominationTypeDetails4Drawer")]
        [HttpPost]
        public IActionResult GetHODisplayDenominationTypeDetails4Drawer(Dictionary<string, object> Param)
        {
            try
            {
                int TillID = Int32.Parse(Convert.ToString(Param["TillID"]));
                int DenominationTypeId = Int32.Parse(Convert.ToString(Param["DenominationTypeId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetHODisplayDenominationTypeDetails4Drawer(TillID, DenominationTypeId, AccountingDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHODisplayDenominationTypeDetails4Drawer.");
            }
            return NotFound();
        }



        [Route("GetUpdateCashTransfersWithCashSpecification")]
        [HttpPost]
        public IActionResult GetUpdateCashTransfersWithCashSpecification(Dictionary<string, object> Param)
        {
            try
            {
                DataTable CashTransfersWithCashSpecification = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CashTransfersWithCashSpecification"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int CashTransferId = Int32.Parse(Convert.ToString(Param["CashTransferId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetUpdateCashTransfersWithCashSpecification(CashTransfersWithCashSpecification, UserId, CashTransferId,
                                    TillId, AccountingDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateCashTransfersWithCashSpecification.");
            }
            return NotFound();
        }



        [Route("GetUpdateCashTransfersWithCashSpecification4Drawer")]
        [HttpPost]
        public IActionResult GetUpdateCashTransfersWithCashSpecification4Drawer(Dictionary<string, object> Param)
        {
            try
            {
                DataTable CashTransfersWithCashSpecification = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CashTransfersWithCashSpecification"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetUpdateCashTransfersWithCashSpecification4Drawer(CashTransfersWithCashSpecification, UserId,
                                    TillId, AccountingDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateCashTransfersWithCashSpecification4Drawer.");
            }
            return NotFound();
        }

        [Route("GetHOUpdateAccountTransaction")]
        [HttpPost]
        public IActionResult GetHOUpdateAccountTransaction(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal CurrencyAmount = Convert.ToDecimal(Convert.ToString(Param["CurrencyAmount"]));
                string TransactionDesc = (Convert.ToString(Param["TransactionDesc"]));
                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int AccountId = Int32.Parse(Convert.ToString(Param["AccountId"]));
                int TransactionTypesId = Int32.Parse(Convert.ToString(Param["TransactionTypesId"]));
                int DenominationTypeId = Int32.Parse(Convert.ToString(Param["DenominationTypeId"]));
                long AccountTransId = Int64.Parse(Convert.ToString(Param["AccountTransId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());
                int EditTypeId = Int32.Parse(Convert.ToString(Param["EditTypeId"]));
                DataTable dtResult = _dalDaVinci.GetHOUpdateAccountTransaction(TillId, UserId, CurrencyAmount, TransactionDesc, CurrencyId, AccountId, TransactionTypesId,
                                                                                DenominationTypeId, AccountTransId, AccountingDate, EditTypeId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHOUpdateAccountTransaction.");
            }
            return NotFound();
        }
        [Route("GetCurrentCashTransferSpecificationChanged")]
        [HttpPost]
        public IActionResult GetCurrentCashTransferSpecificationChanged(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetCurrentCashTransferSpecificationChanged(FromDate, ToDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentCashTransferSpecificationChanged.");
            }
            return NotFound();
        }

        #endregion

        #region ExchangeRate

        //[Route("GetDisplayCompanyDetails")]
        //[HttpGet]
        //public IActionResult GetDisplayCompanyDetails()
        //{
        //    try
        //    {
        //        DataTable dtResult = _dalDaVinci.GetDisplayCompanyDetails();
        //        return Ok(dtResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyDetails.");
        //    }
        //    return NotFound();
        //}
        [Route("GetDisplayCompanyExchangeRateForEveryLocation")]
        [HttpGet]
        public IActionResult GetDisplayCompanyExchangeRateForEveryLocation(int TillId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayCompanyExchangeRateForEveryLocation(TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyExchangeRateForEveryLocation.");
            }
            return NotFound();
        }


        [Route("GetDisplayCompanyDetailsWithAll")]
        [HttpGet]
        public IActionResult GetDisplayCompanyDetailsWithAll()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayCompanyDetailsWithAll();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyDetailsWithAll.");
            }
            return NotFound();
        }

        [Route("GetDisplayExchangeRateHistory")]
        [HttpPost]
        public IActionResult GetDisplayExchangeRateHistory(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetDisplayExchangeRateHistory(CompanyId, FromDate, ToDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayExchangeRateHistory.");
            }
            return NotFound();
        }


        [Route("GetDisplayCompanyExchangeRateDetails")]
        [HttpGet]
        public IActionResult GetDisplayCompanyExchangeRateDetails(int CompanyId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayCompanyExchangeRateDetails(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyExchangeRateDetails.");
            }
            return NotFound();
        }

        [Route("GetUpdateCompanyExchangeRate")]
        [HttpPost]
        public IActionResult GetUpdateCompanyExchangeRate(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyExchangeRatesId = Int32.Parse(Convert.ToString(Param["CompanyExchangeRatesId"]));
                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal CurrencyRate = Convert.ToDecimal(Convert.ToString(Param["CurrencyRate"]));
                int result = _dalDaVinci.GetUpdateCompanyExchangeRate(CompanyExchangeRatesId, CurrencyId, CompanyId, UserId, CurrencyRate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateCompanyExchangeRate.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region Common Folder
        #region Country
        [Route("GetCountries")]
        [HttpGet]
        public IActionResult GetCountries()
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetCountries();
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCountries.");
            }
            return NotFound();
        }

        #endregion

        #region Locations
        [Route("GetLocations")]
        [HttpGet]
        public IActionResult GetLocations()
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetLocations();
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetLocations.");
            }
            return NotFound();
        }

        #endregion

        #region MaritalStatus
        [Route("GetMaritalStatuses")]
        [HttpGet]
        public IActionResult GetMaritalStatuses()
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetMaritalStatuses();
                return Ok(dtResult);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetMaritalStatuses.");
            }
            return NotFound();
        }


        #endregion
        #endregion    

        #region Inventory Folder

        #region Inventory

        [Route("CreateNewInventory")]
        [HttpPost]
        public IActionResult CreateNewInventory(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string Note = (Convert.ToString(Param["Note"]));
                int result = _dalDaVinci.L_CreateNewInventory(CompanyId, StockLocationId, UserId, Note);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreateNewInventory.");
            }
            return NotFound();
        }


        [Route("GetInventoryByLocId")]
        [HttpPost]
        public IActionResult GetInventoryByLocId(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));

                DataTable result = _dalDaVinci.L_GetInventoryByLocId(CompanyId, StockLocationId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInventoryByLocId.");
            }
            return NotFound();
        }

        [Route("GetInventoryItemsById")]
        [HttpPost]
        public IActionResult GetInventoryItemsById(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                DataTable result = _dalDaVinci.L_GetInventoryItemsById(InventoryId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInventoryItemsById.");
            }
            return NotFound();
        }

        [Route("NewCount")]
        [HttpPost]
        public IActionResult NewCount(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));
                decimal Qty = Convert.ToDecimal(Convert.ToString(Param["Qty"]));
                int SectionId = Int32.Parse(Convert.ToString(Param["SectionId"]));
                DataTable result = _dalDaVinci.L_NewCount(InventoryId, UserId, Barcode, Qty, SectionId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - NewCount.");
            }
            return NotFound();
        }

        [Route("UpdateCountedQty")]
        [HttpPost]
        public IActionResult UpdateCountedQty(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));
                decimal Qty = Convert.ToDecimal(Convert.ToString(Param["Qty"]));

                DataTable result = _dalDaVinci.L_UpdateCountedQty(InventoryId, UserId, Barcode, Qty);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateCountedQty.");
            }
            return NotFound();
        }

        [Route("UpdateCountedQty_Troubleshooting")]
        [HttpPost]
        public IActionResult UpdateCountedQty_Troubleshooting(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                decimal Qty = Convert.ToDecimal(Convert.ToString(Param["Qty"]));

                DataTable result = _dalDaVinci.UpdateCountedQty_Troubleshooting(InventoryId, UserId, ItemId, Qty);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateCountedQty_Troubleshooting.");
            }
            return NotFound();
        }

        [Route("MakeWriteOff")]
        [HttpPost]
        public EmptyResult MakeWriteOff(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocId = Int32.Parse(Convert.ToString(Param["StockLocId"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable Items2Writeoff = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["Items2Writeoff"].ToString(), typeof(DataTable));
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                _dalDaVinci.MakeWriteOff(CompanyId, StockLocId, Comment, UserId, Items2Writeoff, InventoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - MakeWriteOff.");
            }
            return new EmptyResult();
        }

        [Route("NewCount_Bulk")]
        [HttpPost]
        public IActionResult NewCount_Bulk(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dt"].ToString(), typeof(DataTable));

                DataTable result = _dalDaVinci.L_NewCount_Bulk(InventoryId, UserId, dt);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - NewCount_Bulk.");
            }
            return NotFound();
        }



        #endregion


        #region InventoryWithAddStock


        [Route("DisplayJobUpdateInventory")]
        [HttpPost]
        public IActionResult DisplayJobUpdateInventory(Dictionary<string, object> Param)
        {
            try
            {
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                bool IsLastItem = Convert.ToBoolean(Param["IsLastItem"].ToString());
                int SectionId = Int32.Parse(Convert.ToString(Param["SectionId"]));

                int result = _dalDaVinci.L_DisplayJobUpdateInventory(InventoryId, UserId, IsLastItem, SectionId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_DisplayJobUpdateInventory.");
            }
            return NotFound();
        }


        #endregion

        #endregion

        #region Financial ReportFolder

        #region DailyCashTransaction

        [Route("GetDisplayAccountCashTransactions")]
        [HttpPost]
        public IActionResult GetDisplayAccountCashTransactions(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataSet result = _dalDaVinci.GetDisplayAccountCashTransactions(FromDate, ToDate, CurrencyId, UserId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAccountCashTransactions.");
            }
            return NotFound();
        }

        [Route("DisplayGetSalesUsingCompanyDate")]
        [HttpPost]
        public IActionResult DisplayGetSalesUsingCompanyDate(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime TransactedOn = Convert.ToDateTime(Param["TransactedOn"].ToString());
                DataSet result = _dalDaVinci.DisplayGetSalesUsingCompanyDate(CompanyId, TransactedOn);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayGetSalesUsingCompanyDate.");
            }
            return NotFound();
        }

        [Route("DisplayGetTransactionsUsingCompanynDate")]
        [HttpPost]
        public IActionResult DisplayGetTransactionsUsingCompanynDate(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime TransactedOn = Convert.ToDateTime(Param["TransactedOn"].ToString());
                DataSet result = _dalDaVinci.DisplayGetTransactionsUsingCompanynDate(CompanyId, TransactedOn);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayGetTransactionsUsingCompanynDate.");
            }
            return NotFound();
        }

        [Route("DisplayStockDisplayDailyTransferredValues")]
        [HttpPost]
        public IActionResult DisplayStockDisplayDailyTransferredValues(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet result = _dalDaVinci.DisplayStockDisplayDailyTransferredValues(FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayStockDisplayDailyTransferredValues.");
            }
            return NotFound();
        }

        #endregion

        #region StockMovements

        [Route("GetDisplayFinancialStockMovements")]
        [HttpPost]
        public IActionResult GetDisplayFinancialStockMovements(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet result = _dalDaVinci.GetDisplayFinancialStockMovements(FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayFinancialStockMovements.");
            }
            return NotFound();
        }
        [Route("GetDisplayFinancialItemFlow")]
        [HttpPost]
        public IActionResult GetDisplayFinancialItemFlow(Dictionary<string, object> Param)
        {
            try
            {
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                DataSet result = _dalDaVinci.GetDisplayFinancialItemFlow(ItemId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayFinancialItemFlow.");
            }
            return NotFound();
        }
        [Route("GetDisplayFinancialPurchasedItems")]
        [HttpPost]
        public IActionResult GetDisplayFinancialPurchasedItems(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet result = _dalDaVinci.GetDisplayFinancialPurchasedItems(FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayFinancialPurchasedItems.");
            }
            return NotFound();
        }

        #endregion

        #region TransactionPerDay

        [Route("GetDisplayGetPurchasedQtyPerDay")]
        [HttpPost]
        public IActionResult GetDisplayGetPurchasedQtyPerDay(Dictionary<string, object> Param)
        {
            try
            {
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable result = _dalDaVinci.GetDisplayGetPurchasedQtyPerDay(ItemId, FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGetPurchasedQtyPerDay.");
            }
            return NotFound();
        }
        [Route("GetDisplaySoldQtyPerDay")]
        [HttpPost]
        public IActionResult GetDisplaySoldQtyPerDay(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable result = _dalDaVinci.GetDisplaySoldQtyPerDay(StockLocationId, FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySoldQtyPerDay.");
            }
            return NotFound();
        }


        #endregion

        #region TransfersBalancesInOut

        [Route("GetDisplayGetTransfersBalances")]
        [HttpPost]
        public IActionResult GetDisplayGetTransfersBalances(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet result = _dalDaVinci.GetDisplayGetTransfersBalances(FromDate, ToDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGetTransfersBalances.");
            }
            return NotFound();
        }
        [Route("GetDisplayGetTransfersBalancesReloadDate")]
        [HttpPost]
        public IActionResult GetDisplayGetTransfersBalancesReloadDate(Dictionary<string, object> Param)
        {
            try
            {

                DateTime ReloadDate = Convert.ToDateTime(Param["ReloadDate"].ToString());

                DataTable result = _dalDaVinci.GetDisplayGetTransfersBalancesReloadDate(ReloadDate);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGetTransfersBalancesReloadDate.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region Gift Certificate Folder

        #region GiftCertificateCancel

        [Route("GetDisplayCouponAmountOptionsDetails")]
        [HttpPost]
        public IActionResult GetDisplayCouponAmountOptionsDetails(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string GiftBarcode = (Convert.ToString(Param["GiftBarcode"]));

                DataSet result = _dalDaVinci.GetDisplayCouponAmountOptionsDetails(TillId, GiftBarcode);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCouponAmountOptionsDetails.");
            }
            return NotFound();
        }


        [Route("GetCancelCouponDetailsWithReturnAmount")]
        [HttpPost]
        public IActionResult GetCancelCouponDetailsWithReturnAmount(Dictionary<string, object> Param)
        {
            try
            {

                DataTable CouponPaymentDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CouponPaymentDetails"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal ReturnAmount = Convert.ToDecimal(Convert.ToString(Param["ReturnAmount"]));
                string CancelComment = (Convert.ToString(Param["CancelComment"]));
                int GiftCertificatesDetailId = Int32.Parse(Convert.ToString(Param["GiftCertificatesDetailId"]));
                DataTable SalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesPaymentCashAmount"].ToString(), typeof(DataTable));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataTable result = _dalDaVinci.GetCancelCouponDetailsWithReturnAmount(CouponPaymentDetails, UserId, ReturnAmount, CancelComment, GiftCertificatesDetailId, SalesPaymentCashAmount, TillId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCancelCouponDetailsWithReturnAmount.");
            }
            return NotFound();
        }

        #endregion

        #region GiftCertificateCancelByHO

        [Route("Get_DisplayCouponAmountOptionsDetails")]
        [HttpPost]
        public IActionResult Get_DisplayCouponAmountOptionsDetails(Dictionary<string, object> Param)
        {
            try
            {

                string GiftCertificateBarcode = (Convert.ToString(Param["GiftCertificateBarcode"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                bool IsBarcodeSearch = Convert.ToBoolean(Param["IsBarcodeSearch"].ToString());

                DataTable result = _dalDaVinci.GetDisplayCouponAmountOptionsDetails(GiftCertificateBarcode, FromDate, ToDate, IsBarcodeSearch);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCouponAmountOptionsDetails.");
            }
            return NotFound();
        }


        [Route("GetCancelGiftCertificatesDetailsWithBarcodeForHO")]
        [HttpPost]
        public IActionResult GetCancelGiftCertificatesDetailsWithBarcodeForHO(Dictionary<string, object> Param)
        {
            try
            {


                int CouponId = Int32.Parse(Convert.ToString(Param["CouponId"]));
                int GiftCertificatesDetailId = Int32.Parse(Convert.ToString(Param["GiftCertificatesDetailId"]));
                int CouponCancelledBy = Int32.Parse(Convert.ToString(Param["CouponCancelledBy"]));
                string CancelledComment = (Convert.ToString(Param["CancelledComment"]));

                DataTable result = _dalDaVinci.GetCancelGiftCertificatesDetailsWithBarcodeForHO(CouponId, GiftCertificatesDetailId, CouponCancelledBy, CancelledComment);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCancelGiftCertificatesDetailsWithBarcodeForHO.");
            }
            return NotFound();
        }


        [Route("DisplayCoupons4Cancellation")]
        [HttpGet]
        public IActionResult DisplayCoupons4Cancellation(string Barcode)
        {
            try
            {

                DataTable result = _dalDaVinci.DisplayCoupons4Cancellation(Barcode);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayCoupons4Cancellation.");
            }
            return NotFound();
        }

        [Route("CancelCouponTypeOne")]
        [HttpPost]
        public EmptyResult CancelCouponTypeOne(Dictionary<string, object> Param)
        {
            try
            {
                int couponid = Int32.Parse(Convert.ToString(Param["couponid"]));
                string canceltext = (Convert.ToString(Param["canceltext"]));
                int userid = Int32.Parse(Convert.ToString(Param["userid"]));
                _dalDaVinci.CancelCouponTypeOne(couponid, canceltext, userid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CancelCouponTypeOne.");
            }
            return new EmptyResult();
        }

        #endregion

        #region GiftCertificatePayment


        [Route("GetDisplayGiftCertificatePendingPaymentDetails")]
        [HttpGet]
        public IActionResult GetDisplayGiftCertificatePendingPaymentDetails(int TillId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplayGiftCertificatePendingPaymentDetails(TillId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGiftCertificatePendingPaymentDetails.");
            }
            return NotFound();
        }


        //one more method "GetAddUpdateCouponAmountOptions" changed to   to "Get_AddUpdateCouponAmountOptions" because same method with same parameter coming twice so controller action method become conflict. 
        [Route("GetAddUpdateCouponAmountOptions")]
        [HttpPost]
        public IActionResult GetAddUpdateCouponAmountOptions(Dictionary<string, object> Param)
        {
            try
            {

                DataTable SalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesPaymentCashAmount"].ToString(), typeof(DataTable));
                DataTable SalesReturnCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesReturnCashAmount"].ToString(), typeof(DataTable));
                int GiftCertificatesDetailId = Int32.Parse(Convert.ToString(Param["GiftCertificatesDetailId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int UsedId = Int32.Parse(Convert.ToString(Param["UsedId"]));
                DataTable result = _dalDaVinci.GetAddUpdateCouponAmountOptions(SalesPaymentCashAmount, SalesReturnCashAmount, GiftCertificatesDetailId, TillId, UsedId);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateCouponAmountOptions.");
            }
            return NotFound();
        }



        #endregion

        #region GiftCerts

        [Route("GetDisplayCouponAmountOptionsDetails")]
        [HttpGet]
        public IActionResult GetDisplayCouponAmountOptionsDetails()
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplayCouponAmountOptionsDetails();

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCouponAmountOptionsDetails.");
            }
            return NotFound();
        }

        //changing original method name "GetAddUpdateCouponAmountOptions"   to "Get_AddUpdateCouponAmountOptions" because same method with same parameter coming twice so controller action method become conflict. 
        [Route("Get_AddUpdateCouponAmountOptions")]
        [HttpPost]
        public IActionResult Get_AddUpdateCouponAmountOptions(Dictionary<string, object> Param)
        {
            try
            {

                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int PickId = Int32.Parse(Convert.ToString(Param["PickId"]));
                decimal PickAmount = Convert.ToDecimal(Convert.ToString(Param["PickAmount"]));
                bool IsActive = Convert.ToBoolean(Param["IsActive"].ToString());
                string Descr = (Convert.ToString(Param["Descr"]));
                int ParentCompanyId = Int32.Parse(Convert.ToString(Param["ParentCompanyId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));

                DataTable result = _dalDaVinci.GetAddUpdateCouponAmountOptions(CurrencyId, PickId, PickAmount, IsActive, Descr, ParentCompanyId, CompanyId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateCouponAmountOptions.");
            }
            return NotFound();
        }


        #endregion

        #region NewGiftCertificate

        [Route("GetDisplayGiftCertificatesPickAmountDetails")]
        [HttpGet]
        public IActionResult GetDisplayGiftCertificatesPickAmountDetails(int TillId)
        {
            try
            {
                DataSet result = _dalDaVinci.GetDisplayGiftCertificatesPickAmountDetails(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGiftCertificatesPickAmountDetails.");
            }
            return NotFound();
        }

        [Route("GetAddGiftCertificatesWithDetailsReports")]
        [HttpPost]
        public IActionResult GetAddGiftCertificatesWithDetailsReports(Dictionary<string, object> Param)
        {
            try
            {

                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal TotalAmountInUSD = Convert.ToDecimal(Convert.ToString(Param["TotalAmountInUSD"]));
                DataTable GiftCertificateDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["GiftCertificateDetails"].ToString(), typeof(DataTable));
                DataTable SalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesPaymentCashAmount"].ToString(), typeof(DataTable));
                DataTable SalesReturnCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesReturnCashAmount"].ToString(), typeof(DataTable));
                string CustomerName = (Convert.ToString(Param["CustomerName"]));

                DataSet result = _dalDaVinci.GetAddGiftCertificatesWithDetailsReports(TillId, CustomerId, UserId, TotalAmountInUSD, GiftCertificateDetails, SalesPaymentCashAmount, SalesReturnCashAmount, CustomerName);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddGiftCertificatesWithDetailsReports.");
            }
            return NotFound();
        }
        [Route("GetAddGiftCertificatesWithDetails")]
        [HttpPost]
        public IActionResult GetAddGiftCertificatesWithDetails(Dictionary<string, object> Param)
        {
            try
            {

                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int PickId = Int32.Parse(Convert.ToString(Param["PickId"]));
                decimal PickAmount = Convert.ToDecimal(Convert.ToString(Param["PickAmount"]));
                bool IsActive = Convert.ToBoolean(Param["IsActive"].ToString());
                string Descr = (Convert.ToString(Param["Descr"]));

                string Barcode = (Convert.ToString(Param["Barcode"]));

                DataTable result = _dalDaVinci.GetAddGiftCertificatesWithDetails(CurrencyId, PickId, PickAmount, IsActive, Descr);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddGiftCertificatesWithDetails.");
            }
            return NotFound();
        }

        #endregion

        #region RevalidateCoupons

        [Route("GetDisplayRevalidateCouponDetails")]
        [HttpPost]
        public IActionResult GetDisplayRevalidateCouponDetails(Dictionary<string, object> Param)
        {
            try
            {

                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string CouponBarcode = (Convert.ToString(Param["CouponBarcode"]));
                DataTable result = _dalDaVinci.GetDisplayRevalidateCouponDetails(TillId, CouponBarcode);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayRevalidateCouponDetails.");
            }
            return NotFound();
        }

        [Route("GetUpdateRevalidateCouponDetails")]
        [HttpPost]
        public IActionResult GetUpdateRevalidateCouponDetails(Dictionary<string, object> Param)
        {
            try
            {

                int CouponId = Int32.Parse(Convert.ToString(Param["CouponId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string CouponBarcode = (Convert.ToString(Param["CouponBarcode"]));
                int ApprovedBy = Int32.Parse(Convert.ToString(Param["ApprovedBy"]));
                string ReValidComment = (Convert.ToString(Param["ReValidComment"]));

                DataTable result = _dalDaVinci.GetUpdateRevalidateCouponDetails(CouponId, TillId, CouponBarcode, ApprovedBy, ReValidComment);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateRevalidateCouponDetails.");
            }
            return NotFound();
        }

        #endregion

        #region UsedGiftCertificate

        [Route("GetDisplayGiftCertificateCouponBarcodeUsedDetails")]
        [HttpPost]
        public IActionResult GetDisplayGiftCertificateCouponBarcodeUsedDetails(Dictionary<string, object> Param)
        {
            try
            {
                string GiftCertificateBarcode = (Convert.ToString(Param["GiftCertificateBarcode"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataSet result = _dalDaVinci.GetDisplayGiftCertificateCouponBarcodeUsedDetails(GiftCertificateBarcode, TillId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayGiftCertificateCouponBarcodeUsedDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayCouponBarcodeUsedDetails")]
        [HttpPost]
        public IActionResult GetDisplayCouponBarcodeUsedDetails(Dictionary<string, object> Param)
        {
            try
            {

                string GiftCertificateBarcode = (Convert.ToString(Param["GiftCertificateBarcode"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));

                DataSet result = _dalDaVinci.GetDisplayCouponBarcodeUsedDetails(GiftCertificateBarcode, TillId);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCouponBarcodeUsedDetails.");
            }
            return NotFound();
        }


        #endregion


        #endregion

        #region HRM Folder

        #region UserDiscount
        [Route("GetDisplayAllEmployeeDiscountLimits")]
        [HttpGet]
        public IActionResult GetDisplayAllEmployeeDiscountLimits()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllEmployeeDiscountLimits();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllEmployeeDiscountLimits.");
            }
            return NotFound();
        }


        /// <summary>
        ///  below one more method  name "GetAddUpdateEmployeeDiscountLimits" changed to "Get_AddUpdateEmployeeDiscountLimits".
        ///  so please verify before use this Controller method (Because Controller having same Dictionary  parameter but repository method having different parameter based on requirement for   different different SP ).
        ///  Check SP and then use this Method.
        ///  Avoid Conflict issue changed Method name
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>

        [Route("GetAddUpdateEmployeeDiscountLimits")]
        [HttpPost]
        public IActionResult GetAddUpdateEmployeeDiscountLimits(Dictionary<string, object> Param)
        {
            try
            {


                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                decimal PercentageDiscount = Convert.ToDecimal(Convert.ToString(Param["PercentageDiscount"]));
                int UpdatedBy = Int32.Parse(Convert.ToString(Param["UpdatedBy"]));

                DataTable result = _dalDaVinci.GetAddUpdateEmployeeDiscountLimits(UserId, PercentageDiscount, UpdatedBy);

                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateEmployeeDiscountLimits.");
            }
            return NotFound();
        }


        #endregion
        #endregion

        #region Coupen Folder

        #region CouponBarcodeDetails


        [Route("GetDisplayAllCouponBarcodePaymentDetails")]
        [HttpGet]
        public IActionResult GetDisplayAllCouponBarcodePaymentDetails(string CouponBarcode)
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplayAllCouponBarcodePaymentDetails(CouponBarcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCouponBarcodePaymentDetails.");
            }
            return NotFound();
        }


        [Route("DisplayAddUpdateReactiveCoupon")]
        [HttpPost]
        public IActionResult DisplayAddUpdateReactiveCoupon(Dictionary<string, object> Param)
        {
            try
            {
                int CouponId = Int32.Parse(Convert.ToString(Param["CouponId"]));
                int RevalidateBy = Int32.Parse(Convert.ToString(Param["RevalidateBy"]));
                DataTable result = _dalDaVinci.DisplayAddUpdateReactiveCoupon(CouponId, RevalidateBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateReactiveCoupon.");
            }
            return NotFound();
        }

        #endregion

        #region CouponHistory

        [Route("DisplayAllActiveLocationForCouponHistory")]
        [HttpGet]
        public IActionResult DisplayAllActiveLocationForCouponHistory()
        {
            try
            {
                DataTable result = _dalDaVinci.DisplayAllActiveLocationForCouponHistory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllActiveLocationForCouponHistory.");
            }
            return NotFound();
        }


        [Route("DisplayCouponHistoryWithPeriodLocation")]
        [HttpPost]
        public IActionResult DisplayCouponHistoryWithPeriodLocation(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable result = _dalDaVinci.DisplayCouponHistoryWithPeriodLocation(CompanyId, FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayCouponHistoryWithPeriodLocation.");
            }
            return NotFound();
        }

        #endregion

        #region CouponNearlyExpiration



        [Route("GetDisplayAllCouponAndGiftNearlyExpiration")]
        [HttpGet]
        public IActionResult GetDisplayAllCouponAndGiftNearlyExpiration(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllCouponAndGiftNearlyExpiration(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCouponAndGiftNearlyExpiration.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllCompanyParentsDetails")]
        [HttpGet]
        public IActionResult GetDisplayAllCompanyParentsDetails()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllCompanyParentsDetails();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCompanyParentsDetails.");
            }
            return NotFound();
        }


        [Route("GetDisplayUpdateExtendDaysPerCoupon")]
        [HttpPost]
        public IActionResult GetDisplayUpdateExtendDaysPerCoupon(Dictionary<string, object> Param)
        {
            try
            {
                int ExtendCouponDays = Int32.Parse(Convert.ToString(Param["ExtendCouponDays"]));
                int CouponId = Int32.Parse(Convert.ToString(Param["CouponId"]));
                int ApprovedBy = Int32.Parse(Convert.ToString(Param["ApprovedBy"]));
                string ReValidComment = (Convert.ToString(Param["ReValidComment"]));
                string CouponBarcode = (Convert.ToString(Param["CouponBarcode"]));
                DataTable result = _dalDaVinci.GetDisplayUpdateExtendDaysPerCoupon(ExtendCouponDays, CouponId, ApprovedBy, ReValidComment, CouponBarcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayUpdateExtendDaysPerCoupon.");
            }
            return NotFound();
        }


        #endregion

        #region Promo

        [Route("GetDisplayDefaultAccountTransactionDetails")]
        [HttpGet]
        public IActionResult GetDisplayDefaultAccountTransactionDetails(int TillId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayDefaultAccountTransactionDetails(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDefaultAccountTransactionDetails.");
            }
            return NotFound();
        }
        #endregion

        #region PromoWithExpiration

        [Route("GetDisplayAllPromoWithExpiration")]
        [HttpGet]
        public IActionResult GetDisplayAllPromoWithExpiration(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllPromoWithExpiration(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllPromoWithExpiration.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllPromoWithExpirationItems")]
        [HttpGet]
        public IActionResult GetDisplayAllPromoWithExpirationItems(int PromoId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllPromoWithExpirationItems(PromoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllPromoWithExpirationItems.");
            }
            return NotFound();
        }


        [Route("GetAddUpdatePromoWithExpirationDays")]
        [HttpPost]
        public IActionResult GetAddUpdatePromoWithExpirationDays(Dictionary<string, object> Param)
        {
            try
            {

                int PromoId = Int32.Parse(Convert.ToString(Param["PromoId"]));
                int PromoDays = Int32.Parse(Convert.ToString(Param["PromoDays"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                DataTable result = _dalDaVinci.GetAddUpdatePromoWithExpirationDays(PromoId, PromoDays, CreatedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdatePromoWithExpirationDays.");
            }
            return NotFound();
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

        [Route("Get_DisplayCashFigureSpecificationReportDetails")]
        [HttpPost]
        public IActionResult Get_DisplayCashFigureSpecificationReportDetails(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet result = _dalDaVinci.Get_DisplayCashFigureSpecificationReportDetails(UserId, TillId, AccountingDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - Get_DisplayCashFigureSpecificationReportDetails.");
            }
            return NotFound();
        }


        [Route("GetDisplayCashFigureAndAccountTransactionBalanceSheetReport")]
        [HttpPost]
        public IActionResult GetDisplayCashFigureAndAccountTransactionBalanceSheetReport(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataSet result = _dalDaVinci.GetDisplayCashFigureAndAccountTransactionBalanceSheetReport(UserId, TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCashFigureAndAccountTransactionBalanceSheetReport.");
            }
            return NotFound();
        }

        #endregion

        #region BalanceTransactionReport

        [Route("GetDisplayCashFigureSpecificationReportDetails")]
        [HttpPost]
        public IActionResult GetDisplayCashFigureSpecificationReportDetails(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet result = _dalDaVinci.GetDisplayCashFigureSpecificationReportDetails(UserId, TillId, AccountingDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCashFigureSpecificationReportDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayBalanceDetailWithAccountingDate")]
        [HttpPost]
        public IActionResult GetDisplayBalanceDetailWithAccountingDate(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet result = _dalDaVinci.GetDisplayBalanceDetailWithAccountingDate(UserId, TillId, AccountingDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayBalanceDetailWithAccountingDate.");
            }
            return NotFound();
        }

        [Route("GetDisplayDailyTransactionOverview")]
        [HttpPost]
        public IActionResult GetDisplayDailyTransactionOverview(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet result = _dalDaVinci.GetDisplayDailyTransactionOverview(UserId, TillId, AccountingDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDailyTransactionOverview.");
            }
            return NotFound();
        }


        [Route("DisplaySaleBalanceSheets")]
        [HttpPost]
        public IActionResult DisplaySaleBalanceSheets(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataSet result = _dalDaVinci.DisplaySaleBalanceSheets(TillId, FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySaleBalanceSheets.");
            }
            return NotFound();
        }
        [Route("GetDailySalesReportWithAccountingDatePerTill")]
        [HttpPost]
        public IActionResult GetDailySalesReportWithAccountingDatePerTill(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime AccountingDate = Convert.ToDateTime(Param["AccountingDate"].ToString());

                DataSet result = _dalDaVinci.GetDailySalesReportWithAccountingDatePerTill(UserId, TillId, AccountingDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDailySalesReportWithAccountingDatePerTill.");
            }
            return NotFound();
        }




        #endregion

        #region Denomination

        [Route("GetDisplayDenominationValueTypes")]
        [HttpGet]
        public IActionResult GetDisplayDenominationValueTypes()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayDenominationValueTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDenominationValueTypes.");
            }
            return NotFound();
        }

        [Route("GetDisplayDenominationTotalValues")]
        [HttpPost]
        public IActionResult GetDisplayDenominationTotalValues(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int DenominationTypeId = Int32.Parse(Convert.ToString(Param["DenominationTypeId"]));
                DataTable result = _dalDaVinci.GetDisplayDenominationTotalValues(TillId, DenominationTypeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDenominationTotalValues.");
            }
            return NotFound();
        }

        [Route("GetUpdateTillDenominationStatistics")]
        [HttpPost]
        public IActionResult GetUpdateTillDenominationStatistics(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataTable DenominationStatisticsDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["DenominationStatisticsDetails"].ToString(), typeof(DataTable));
                DataTable result = _dalDaVinci.GetUpdateTillDenominationStatistics(TillId, DenominationStatisticsDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateTillDenominationStatistics.");
            }
            return NotFound();
        }

        [Route("GetDenominationTotalValueInUSD")]
        [HttpGet]
        public IActionResult GetDenominationTotalValueInUSD(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDenominationTotalValueInUSD(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDenominationTotalValueInUSD.");
            }
            return NotFound();
        }

        [Route("GetDisplayDenominationTypeDetails")]
        [HttpGet]
        public IActionResult GetDisplayDenominationTypeDetails()
        {
            try
            {

                List<DenominationType> _denominationResult = _dalDaVinci.GetDisplayDenominationTypeDetails();
                return Ok(_denominationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayDenominationTypeDetails.");
            }
            return NotFound();
        }
        #endregion


        #endregion

        #region Purchases Folder

        #region AddUpdatePurchase

        [Route("GetPurchasesByDate")]
        [HttpPost]
        public IActionResult GetPurchasesByDate(Dictionary<string, object> Param)
        {
            try
            {

                DateTime PurchasedOnFrom = Convert.ToDateTime(Param["PurchasedOnFrom"].ToString());
                DateTime PurchasedOnTo = Convert.ToDateTime(Param["PurchasedOnTo"].ToString());
                DataSet result = _dalDaVinci.GetPurchasesByDate(PurchasedOnFrom, PurchasedOnTo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchasesByDate.");
            }
            return NotFound();
        }

        [Route("GetDisplayAddUpdateInformation")]
        [HttpGet]
        public IActionResult GetDisplayAddUpdateInformation(int PurchaseId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplayAddUpdateInformation(PurchaseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAddUpdateInformation.");
            }
            return NotFound();
        }

        [Route("GetUpdatePurchaseDetailsWithItems")]
        [HttpPost]
        public IActionResult GetUpdatePurchaseDetailsWithItems(Dictionary<string, object> Param)
        {
            try
            {
                DataTable PurchaseItemsWithDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["PurchaseItemsWithDetails"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                string InvoiceNumber = (Convert.ToString(Param["InvoiceNumber"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                decimal MainDiscount = Convert.ToDecimal(Convert.ToString(Param["MainDiscount"]));
                DateTime PurchasedOn = Convert.ToDateTime(Param["PurchasedOn"].ToString());
                int PurchaseId = Int32.Parse(Convert.ToString(Param["PurchaseId"]));
                DataTable result = _dalDaVinci.GetUpdatePurchaseDetailsWithItems(PurchaseItemsWithDetails, UserId, TillId, SupplierId, InvoiceNumber, ReceiptNumber, Comment, MainDiscount, PurchasedOn, PurchaseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdatePurchaseDetailsWithItems.");
            }
            return NotFound();
        }

        [Route("GetPurchasedItemsByPurchaseId")]
        [HttpGet]
        public IActionResult GetPurchasedItemsByPurchaseId(int PurchaseId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetPurchasedItemsByPurchaseId(PurchaseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchasedItemsByPurchaseId.");
            }
            return NotFound();
        }

        [Route("UpdateQtynCp4PurchasedItems")]
        [HttpPost]
        public EmptyResult UpdateQtynCp4PurchasedItems(Dictionary<string, object> Param)
        {
            try
            {
                List<AddUpdatePurchase> l = (List<AddUpdatePurchase>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["l"].ToString(), typeof(List<AddUpdatePurchase>));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                _dalDaVinci.UpdateQtynCp4PurchasedItems(l, UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateQtynCp4PurchasedItems.");
            }
            return new EmptyResult();
        }

        [Route("ItemsWithoutCostprices")]
        [HttpGet]
        public IActionResult ItemsWithoutCostprices()
        {
            try
            {
                DataSet result = _dalDaVinci.ItemsWithoutCostprices();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ItemsWithoutCostprices.");
            }
            return NotFound();
        }

        [Route("ItemsWithoutCostprices_ByPurchases")]
        [HttpGet]
        public IActionResult ItemsWithoutCostprices_ByPurchases()
        {
            try
            {
                DataSet result = _dalDaVinci.ItemsWithoutCostprices_ByPurchases();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ItemsWithoutCostprices_ByPurchases.");
            }
            return NotFound();
        }

        [Route("GetDisplayReportPurchaseDetailsWithItems")]
        [HttpPost]
        public IActionResult GetDisplayReportPurchaseDetailsWithItems(Dictionary<string, object> Param)
        {
            try
            {
                int PurchaseId = Int32.Parse(Convert.ToString(Param["PurchaseId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetDisplayReportPurchaseDetailsWithItems(PurchaseId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReportPurchaseDetailsWithItems.");
            }
            return NotFound();
        }

        [Route("ReceiptNumberExists")]
        [HttpPost]
        public IActionResult ReceiptNumberExists(string ReceiptNumber)
        {
            try
            {
                bool result = _dalDaVinci.ReceiptNumberExists(ReceiptNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ReceiptNumberExists.");
            }
            return NotFound();
        }

        #endregion

        #region NewPurchase

        [Route("GetDisplaySupplierContactPerson")]
        [HttpGet]
        public IActionResult GetDisplaySupplierContactPerson()
        {
            try
            {
                DataSet result = _dalDaVinci.GetDisplaySupplierContactPerson();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySupplierContactPerson.");
            }
            return NotFound();
        }

        [Route("GetAddPurchaseDetailsWithItems")]
        [HttpPost]
        public IActionResult GetAddPurchaseDetailsWithItems(Dictionary<string, object> Param)
        {
            try
            {

                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                DataTable PurchaseItemsDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["PurchaseItemsDetails"].ToString(), typeof(DataTable));
                DataTable result = _dalDaVinci.GetAddPurchaseDetailsWithItems(SupplierId, TransactedBy, CompanyId, ReceiptNumber, Comment, PurchaseItemsDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddPurchaseDetailsWithItems.");
            }
            return NotFound();
        }

        [Route("GetDisplayManualAddedItems")]
        [HttpGet]
        public IActionResult GetDisplayManualAddedItems()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayManualAddedItems();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayManualAddedItems.");
            }
            return NotFound();
        }

        #endregion

        #region Purchase

        [Route("GetDisplayCategoryTypesModelBrandForPurchase")]
        [HttpGet]
        public IActionResult GetDisplayCategoryTypesModelBrandForPurchase()
        {
            try
            {
                DataSet result = _dalDaVinci.L_GetDisplayCategoryTypesModelBrandForPurchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCategoryTypesModelBrandForPurchase.");
            }
            return NotFound();
        }

        [Route("GetDiaplayPurchaseItemsWithModelId")]
        [HttpPost]
        public IActionResult GetDiaplayPurchaseItemsWithModelId(Dictionary<string, object> Param)
        {
            try
            {

                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                decimal Quantity = Convert.ToDecimal(Convert.ToString(Param["Quantity"]));
                decimal PurchasePrice = Convert.ToDecimal(Convert.ToString(Param["PurchasePrice"]));
                string InvoiceNumber = (Convert.ToString(Param["InvoiceNumber"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));

                DataTable result = _dalDaVinci.L_GetDiaplayPurchaseItemsWithModelId(ItemId, Quantity, PurchasePrice, InvoiceNumber, ReceiptNumber); ;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDiaplayPurchaseItemsWithModelId.");
            }
            return NotFound();
        }

        [Route("GetAddUpdatePurchaseDetailsWithItems")]
        [HttpPost]
        public IActionResult GetAddUpdatePurchaseDetailsWithItems(Dictionary<string, object> Param)
        {
            try
            {
                DataTable PurchaseItemsWithDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["PurchaseItemsWithDetails"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                string InvoiceNumber = (Convert.ToString(Param["InvoiceNumber"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                decimal MainDiscount = Convert.ToDecimal(Convert.ToString(Param["MainDiscount"]));
                DateTime PurchasedOn = Convert.ToDateTime(Param["PurchasedOn"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                string Container = (Convert.ToString(Param["Container"]));
                DataTable result = _dalDaVinci.L_GetAddUpdatePurchaseDetailsWithItems(PurchaseItemsWithDetails, UserId, TillId, SupplierId, InvoiceNumber, ReceiptNumber, Comment, MainDiscount, PurchasedOn, CompanyId, StockLocationId, Container);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdatePurchaseDetailsWithItems.");
            }
            return NotFound();
        }

        [Route("GetAllReceipts")]
        [HttpGet]
        public IActionResult GetAllReceipts()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetAllReceipts();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllReceipts.");
            }
            return NotFound();
        }

        [Route("GetDateAndSupplierById")]
        [HttpGet]
        public IActionResult GetDateAndSupplierById(int PurchaseId)
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetDateAndSupplierById(PurchaseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDateAndSupplierById.");
            }
            return NotFound();
        }
        [Route("GetBranches")]
        [HttpGet]
        public IActionResult GetBranches()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetBranches();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBranches.");
            }
            return NotFound();
        }


        /// <summary>
        ///(in stock transfer region) there also  controller method with this name "GetStockLocations"  with different parameter.
        ///so  there avoid conflict issue  name changed "GetStockLocations" to "L_GetStockLocations"
        /// please before use this method verify once 
        /// </summary>

        [Route("GetStockLocations")]
        [HttpGet]
        public IActionResult GetStockLocations()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetStockLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockLocations.");
            }
            return NotFound();
        }

        [Route("GetLocations4Purchase")]
        [HttpGet]
        public IActionResult GetLocations4Purchase()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetLocations4Purchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetLocations4Purchase.");
            }
            return NotFound();
        }


        [Route("GetSuppliers")]
     //   [CacheFilter(TimeDuration = 100)]
        [HttpGet]
        public IActionResult GetSuppliers()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetSuppliers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSuppliers.");
            }
            return NotFound();
        }

        [Route("GetInfoById")]
        [HttpGet]
        public IActionResult GetInfoById(int PurchaseId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetInfoById(PurchaseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInfoById.");
            }
            return NotFound();
        }

        [Route("UpdateHeaderInfo")]
        [HttpPost]
        public EmptyResult UpdateHeaderInfo(Dictionary<string, object> Param)
        {
            try
            {
                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                int PurchaseId = Int32.Parse(Convert.ToString(Param["PurchaseId"]));
                DateTime PurchasedOn = Convert.ToDateTime(Param["PurchasedOn"].ToString());
                string Container = (Convert.ToString(Param["Container"]));
                _dalDaVinci.UpdateHeaderInfo(SupplierId, UserId, CompanyId, StockLocationId, ReceiptNumber, Comment, PurchaseId, PurchasedOn, Container);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateHeaderInfo.");
            }
            return new EmptyResult();
        }

        [Route("GetAllActiveItems")]
        [HttpGet]
        public IActionResult GetAllActiveItems()
        {
            try
            {
                DataTable result = _dalDaVinci.GetAllActiveItems();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllActiveItems.");
            }
            return NotFound();
        }

        [Route("GetItemByManBarcode")]
        [HttpGet]
        public IActionResult GetItemByManBarcode(string ManBarcode)
        {
            try
            {

                DataTable result = _dalDaVinci.GetItemByManBarcode(ManBarcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemByManBarcode.");
            }
            return NotFound();
        }

        [Route("GetItemById")]
        [HttpGet]
        public IActionResult GetItemById(int ItemId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetItemById(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemById.");
            }
            return NotFound();
        }
        [Route("AddItems2ExistingPurchase")]
        [HttpPost]
        public EmptyResult AddItems2ExistingPurchase(Dictionary<string, object> Param)
        {
            try
            {
                List<Purchase> pl = (List<Purchase>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["pl"].ToString(), typeof(List<AddUpdatePurchase>));
                _dalDaVinci.AddItems2ExistingPurchase(pl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddItems2ExistingPurchase.");
            }
            return new EmptyResult();
        }
        [Route("IsBarcodeAlreadyInPurchase")]
        [HttpPost]
        public IActionResult IsBarcodeAlreadyInPurchase(Dictionary<string, object> Param)
        {
            try
            {
                int PurchaseId = Int32.Parse(Convert.ToString(Param["PurchaseId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));
                bool result = _dalDaVinci.IsBarcodeAlreadyInPurchase(PurchaseId, Barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - IsBarcodeAlreadyInPurchase.");
            }
            return NotFound();
        }
        [Route("GetItemsByPurchaseId")]
        [HttpGet]
        public IActionResult GetItemsByPurchaseId(int _purchaseid)
        {
            try
            {

                DataTable result = _dalDaVinci.GetItemsByPurchaseId(_purchaseid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemsByPurchaseId.");
            }
            return NotFound();
        }
        [Route("AddNewPurchaseUsingXLsheet")]
        [HttpPost]
        public EmptyResult AddNewPurchaseUsingXLsheet(Dictionary<string, object> Param)
        {
            try
            {
                DataTable Barcodes = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["Barcodes"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                string InvoiceNumber = (Convert.ToString(Param["InvoiceNumber"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                decimal MainDiscount = Convert.ToDecimal(Convert.ToString(Param["MainDiscount"]));
                int PurchaseId = Int32.Parse(Convert.ToString(Param["PurchaseId"]));
                DateTime PurchasedOn = Convert.ToDateTime(Param["PurchasedOn"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                string Container = (Convert.ToString(Param["Container"]));
                _dalDaVinci.AddNewPurchaseUsingXLsheet(Barcodes, UserId, TillId, SupplierId, InvoiceNumber, ReceiptNumber, Comment, MainDiscount, PurchasedOn, CompanyId, StockLocationId, Container);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddNewPurchaseUsingXLsheet.");
            }
            return new EmptyResult();
        }
        [Route("EditPurchaseUsingXLsheet")]
        [HttpPost]
        public EmptyResult EditPurchaseUsingXLsheet(Dictionary<string, object> Param)
        {
            try
            {
                DataTable Barcodes = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["Barcodes"].ToString(), typeof(DataTable));
                int _purchaseid = Int32.Parse(Convert.ToString(Param["_purchaseid"]));
                int _userid = Int32.Parse(Convert.ToString(Param["_userid"]));
                _dalDaVinci.EditPurchaseUsingXLsheet(Barcodes, _purchaseid, _userid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - EditPurchaseUsingXLsheet.");
            }
            return new EmptyResult();
        }
        [Route("GetNonMatchBarcodes")]
        [HttpPost]
        public IActionResult GetNonMatchBarcodes(Dictionary<string, object> Param)
        {
            try
            {
                DataTable ExcelTable = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["ExcelTable"].ToString(), typeof(DataTable));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.GetNonMatchBarcodes(ExcelTable, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetNonMatchBarcodes.");
            }
            return NotFound();
        }


        #endregion

        #endregion

        #region CreditNotes Fodler

        #region NewCreditNotes

        [Route("DisplaySaleReturnsHistoryItems")]
        [HttpPost]
        public IActionResult DisplaySaleReturnsHistoryItems(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataSet result = _dalDaVinci.DisplaySaleReturnsHistoryItems(FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySaleReturnsHistoryItems.");
            }
            return NotFound();
        }

        [Route("CreditNoteDistributorsDetails")]
        [HttpPost]
        public IActionResult CreditNoteDistributorsDetails(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataSet result = _dalDaVinci.CreditNoteDistributorsDetails(FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreditNoteDistributorsDetails.");
            }
            return NotFound();
        }
        [Route("CreditNoteDistributorsHistory")]
        [HttpPost]
        public IActionResult CreditNoteDistributorsHistory(Dictionary<string, object> Param)
        {
            try
            {

                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataSet result = _dalDaVinci.CreditNoteDistributorsHistory(FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreditNoteDistributorsHistory.");
            }
            return NotFound();
        }
        [Route("DisplayAllCreditNoteDistributors")]
        [HttpGet]
        public IActionResult DisplayAllCreditNoteDistributors()
        {
            try
            {
                DataSet result = _dalDaVinci.DisplayAllCreditNoteDistributors();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllCreditNoteDistributors.");
            }
            return NotFound();
        }

        [Route("DisplayAddUpdateCommissionPercentage")]
        [HttpPost]
        public IActionResult DisplayAddUpdateCommissionPercentage(Dictionary<string, object> Param)
        {
            try
            {
                int DistributorId = Int32.Parse(Convert.ToString(Param["DistributorId"]));
                string DistributorName = (Convert.ToString(Param["DistributorName"]));
                bool IsActive = Convert.ToBoolean(Param["IsActive"].ToString());
                string Address = (Convert.ToString(Param["Address"]));
                string Phone = (Convert.ToString(Param["Phone"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                DataTable CommissionPercentage = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CommissionPercentage"].ToString(), typeof(DataTable));
                string EMail = (Convert.ToString(Param["EMail"]));
                DataSet result = _dalDaVinci.DisplayAddUpdateCommissionPercentage(DistributorId, DistributorName, IsActive, Address, Phone, CreatedBy, CommissionPercentage, EMail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateCommissionPercentage.");
            }
            return NotFound();
        }
        [Route("DisplayAllCreditNoteDistributorsWithDistributorId")]
        [HttpGet]
        public IActionResult DisplayAllCreditNoteDistributorsWithDistributorId(int DistributorId)
        {
            try
            {
                DataSet result = _dalDaVinci.DisplayAllCreditNoteDistributorsWithDistributorId(DistributorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllCreditNoteDistributorsWithDistributorId.");
            }
            return NotFound();
        }

        [Route("DisplayDistributorsWIthPeriod")]
        [HttpPost]
        public IActionResult DisplayDistributorsWIthPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataTable result = _dalDaVinci.DisplayDistributorsWIthPeriod(FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDistributorsWIthPeriod.");
            }
            return NotFound();
        }

        [Route("DisplayDistributorsCreditPaymentsPeriod")]
        [HttpPost]
        public IActionResult DisplayDistributorsCreditPaymentsPeriod(Dictionary<string, object> Param)
        {
            try
            {
                int DistributorId = Int32.Parse(Convert.ToString(Param["DistributorId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataTable result = _dalDaVinci.DisplayDistributorsCreditPaymentsPeriod(DistributorId, FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDistributorsCreditPaymentsPeriod.");
            }
            return NotFound();
        }

        [Route("DisplayOutstandingCreditNoteDistributors")]
        [HttpPost]
        public IActionResult DisplayOutstandingCreditNoteDistributors(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable result = _dalDaVinci.DisplayOutstandingCreditNoteDistributors(CompanyId, FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayOutstandingCreditNoteDistributors.");
            }
            return NotFound();
        }


        [Route("DisplayCreditNoteReceiptDetails")]
        [HttpGet]
        public IActionResult DisplayCreditNoteReceiptDetails(int DistributorId)
        {
            try
            {
                DataTable result = _dalDaVinci.DisplayCreditNoteReceiptDetails(DistributorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayCreditNoteReceiptDetails.");
            }
            return NotFound();
        }

        [Route("DisplayOutstandingCreditNoteDistributorsUpdateDiscount")]
        [HttpPost]
        public IActionResult DisplayOutstandingCreditNoteDistributorsUpdateDiscount(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                int DistributorId = Int32.Parse(Convert.ToString(Param["DistributorId"]));
                DataTable result = _dalDaVinci.DisplayOutstandingCreditNoteDistributorsUpdateDiscount(CompanyId, FromDate, Todate, DistributorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayOutstandingCreditNoteDistributorsUpdateDiscount.");
            }
            return NotFound();
        }

        [Route("DisplayOutstandingCreditNoteDistributorsWithPeriod")]
        [HttpPost]
        public IActionResult DisplayOutstandingCreditNoteDistributorsWithPeriod(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                int DistributorId = Int32.Parse(Convert.ToString(Param["DistributorId"]));
                DataTable result = _dalDaVinci.DisplayOutstandingCreditNoteDistributorsWithPeriod(CompanyId, FromDate, Todate, DistributorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayOutstandingCreditNoteDistributorsWithPeriod.");
            }
            return NotFound();
        }

        [Route("UpdateCreditNoteDistributorsDiscounts")]
        [HttpPost]
        public IActionResult UpdateCreditNoteDistributorsDiscounts(DataTable dtDiscounts)
        {
            try
            {
                DataTable result = _dalDaVinci.UpdateCreditNoteDistributorsDiscounts(dtDiscounts);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateCreditNoteDistributorsDiscounts.");
            }
            return NotFound();
        }


        #endregion

        #endregion

        #region Employee Folder

        #region Employee 

        [Route("GetDisplayAllActiveEmployeeRoles")]
        [HttpGet]
        public IActionResult GetDisplayAllActiveEmployeeRoles()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllActiveEmployeeRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllActiveEmployeeRoles.");
            }
            return NotFound();
        }
        [Route("GetDisplayAllEmployeeByRolesId")]
        [HttpGet]
        public IActionResult GetDisplayAllEmployeeByRolesId(int RoleId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAllEmployeeByRolesId(RoleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllEmployeeByRolesId.");
            }
            return NotFound();
        }

        /// <summary>
        ///  method name "GetAddUpdateEmployeeDiscountLimits" changed to "Get_AddUpdateEmployeeDiscountLimits".
        ///  so please verify before use this Controller method (Because Controller having same  Dictionary parameter but repository method having different parameter based on requirement for   different different SP ).
        ///  Check SP and then use this Method.
        ///  Avoid Conflict issue changed Method name
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        [Route("Get_AddUpdateEmployeeDiscountLimits")]
        [HttpPost]
        public IActionResult Get_AddUpdateEmployeeDiscountLimits(Dictionary<string, object> Param)
        {
            try
            {
                int UpdatedBy = Int32.Parse(Convert.ToString(Param["UpdatedBy"]));
                int RoleId = Int32.Parse(Convert.ToString(Param["RoleId"]));
                int EmployeeId = Int32.Parse(Convert.ToString(Param["EmployeeId"]));
                bool IsRole = Convert.ToBoolean(Param["IsRole"].ToString());
                decimal PercentageDiscount = Convert.ToDecimal(Convert.ToString(Param["PercentageDiscount"]));
                int result = _dalDaVinci.GetAddUpdateEmployeeDiscountLimits(UpdatedBy, RoleId, EmployeeId, IsRole, PercentageDiscount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateEmployeeDiscountLimits.");
            }
            return NotFound();
        }

        [Route("GetAllActiveUsers")]
        [HttpGet]
        public IActionResult GetAllActiveUsers()
        {
            try
            {
                DataTable result = _dalDaVinci.GetAllActiveUsers();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllActiveUsers.");
            }
            return NotFound();
        }

        [Route("UpdateDiscountLimits")]
        [HttpPost]
        public IActionResult UpdateDiscountLimits(Dictionary<string, object> Param)
        {
            try
            {
                decimal discount = Convert.ToDecimal(Convert.ToString(Param["discount"]));
                int userid = Int32.Parse(Convert.ToString(Param["userid"]));
                DataTable users = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["users"].ToString(), typeof(DataTable));
                bool result = _dalDaVinci.UpdateDiscountLimits(discount, userid, users);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateDiscountLimits.");
            }
            return NotFound();
        }
        [Route("GetDiscountLimitHistory")]
        [HttpGet]
        public IActionResult GetDiscountLimitHistory(int UserID)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDiscountLimitHistory(UserID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDiscountLimitHistory.");
            }
            return NotFound();
        }

        #endregion

        #endregion

        #region Features Folder

        #region NewTeamViewer
        [Route("GetDisplayAllDetailsTeamViewerInformation")]
        [HttpGet]
        public IActionResult GetDisplayAllDetailsTeamViewerInformation()
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayAllDetailsTeamViewerInformation();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllDetailsTeamViewerInformation.");
            }
            return NotFound();
        }
        [Route("GetAddUpdateTeamViewerId")]
        [HttpPost]
        public IActionResult GetAddUpdateTeamViewerId(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string TeamViewerId = (Convert.ToString(Param["TeamViewerId"]));
                string TeamViewerPassword = (Convert.ToString(Param["TeamViewerPassword"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                int CreatedTillId = Int32.Parse(Convert.ToString(Param["CreatedTillId"]));
                int result = _dalDaVinci.GetAddUpdateTeamViewerId(TillId, TeamViewerId, TeamViewerPassword, CreatedBy, CreatedTillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateTeamViewerId.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region MailAndAlert Folder

        #region MailAndAlert 

        /// <summary>
        ///  method name "GetDisplayAllTillDetailsForReceiver" changed to "Get_DisplayAllTillDetailsForReceiver".
        ///  so please verify before use this Controller method (Because Controller having same  Dictionary parameter but repository method having different parameter based on requirement for   different different SP ).
        ///  Check SP and then use this Method.
        ///  Avoid Conflict issue changed Method name
        /// </summary>
        /// <param name="Param"></param>
        /// <returns></returns>
        [Route("Get_DisplayAllTillDetailsForReceiver")]
        [HttpGet]
        public IActionResult Get_DisplayAllTillDetailsForReceiver(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAllTillDetailsForReceiver(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllTillDetailsForReceiver.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllSentMailDetails")]
        [HttpGet]
        public IActionResult GetDisplayAllSentMailDetails(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAllSentMailDetails(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllSentMailDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllInboxMail")]
        [HttpGet]
        public IActionResult GetDisplayAllInboxMail(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAllInboxMail(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllInboxMail.");
            }
            return NotFound();
        }

        [Route("GetTotalNoOfUnreadMessage")]
        [HttpGet]
        public IActionResult GetTotalNoOfUnreadMessage(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetTotalNoOfUnreadMessage(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetTotalNoOfUnreadMessage.");
            }
            return NotFound();
        }

        [Route("GetAddNewMailInformationWithTill")]
        [HttpPost]
        public IActionResult GetAddNewMailInformationWithTill(Dictionary<string, object> Param)
        {
            try
            {

                string MailSubject = (Convert.ToString(Param["MailSubject"]));
                string MailBody = (Convert.ToString(Param["MailBody"]));
                int SenderUserId = Int32.Parse(Convert.ToString(Param["SenderUserId"]));
                int MailTillId = Int32.Parse(Convert.ToString(Param["MailTillId"]));
                string MailerMacAddress = (Convert.ToString(Param["MailerMacAddress"]));
                DataTable ReceiverTillDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["ReceiverTillDetails"].ToString(), typeof(DataTable));
                int result = _dalDaVinci.GetAddNewMailInformationWithTill(MailSubject, MailBody, SenderUserId, MailTillId, MailerMacAddress, ReceiverTillDetails);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddNewMailInformationWithTill.");
            }
            return NotFound();
        }

        [Route("GetUpdateReadMessage")]
        [HttpPost]
        public IActionResult GetUpdateReadMessage(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int MailId = Int32.Parse(Convert.ToString(Param["MailId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int result = _dalDaVinci.GetUpdateReadMessage(TillId, MailId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateReadMessage.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region Parameter Folder

        #region Parameter

        [Route("GetDisplayAppParemters")]
        [HttpGet]
        public IActionResult GetDisplayAppParemters(int TillId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAppParemters(TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAppParemters.");
            }
            return NotFound();
        }

        [Route("GetUpdateAppParemters")]
        [HttpPost]
        public IActionResult GetUpdateAppParemters(Dictionary<string, object> Param)
        {
            try
            {
                string Value = (Convert.ToString(Param["Value"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int Id = Int32.Parse(Convert.ToString(Param["Id"]));
                int result = _dalDaVinci.GetUpdateAppParemters(Value, UserId, Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUpdateAppParemters.");
            }
            return NotFound();
        }



        [Route("GetAllLocationsTills")]
        [HttpGet]
        public IActionResult GetAllLocationsTills()
        {
            try
            {
                DataTable result = _dalDaVinci.GetAllLocationsTills();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllLocationsTills.");
            }
            return NotFound();
        }

        [Route("SetLocTillImg")]
        [HttpPost]
        public IActionResult SetLocTillImg(Dictionary<string, object> Param)
        {
            try
            {
                byte[] Img = Convert.FromBase64String(Convert.ToString(Param["Img"]));
                int TillLocationId = Int32.Parse(Convert.ToString(Param["TillLocationId"]));
                int result = _dalDaVinci.SetLocTillImg(Img, TillLocationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - SetLocTillImg.");
            }
            return NotFound();
        }


        #endregion
        #endregion

        #region Reports Folder

        #region Sales Folder(In Reports Folder)

        #region BrandProductSaleOnCredit

        [Route("GetBrandProductSaleOnCreditDetails")]
        [HttpPost]
        public IActionResult GetBrandProductSaleOnCreditDetails(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BeginDate = Convert.ToDateTime(Param["BeginDate"].ToString());
                DateTime EndDate = Convert.ToDateTime(Param["EndDate"].ToString());
                int Department = Int32.Parse(Convert.ToString(Param["Department"]));
                DataSet result = _dalDaVinci.GetBrandProductSaleOnCreditDetails(BeginDate, EndDate, Department);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBrandProductSaleOnCreditDetails.");
            }
            return NotFound();
        }

        [Route("DisplayDepartment")]
        [HttpGet]
        public IActionResult DisplayDepartment()
        {
            try
            {
                DataSet result = _dalDaVinci.DisplayDepartment();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDepartment.");
            }
            return NotFound();
        }


        #endregion

        #region CustomersRemainingPayments

        [Route("GetCustomerRemainingPayments")]
        [HttpGet]
        public IActionResult GetCustomerRemainingPayments()
        {
            try
            {
                DataTable result = _dalDaVinci.GetCustomerRemainingPayments();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCustomerRemainingPayments.");
            }
            return NotFound();
        }

        [Route("GetCustomerRemainingPaymentByCustomerId")]
        [HttpGet]
        public IActionResult GetCustomerRemainingPaymentByCustomerId(long CustomerId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetCustomerRemainingPaymentByCustomerId(CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCustomerRemainingPaymentByCustomerId.");
            }
            return NotFound();
        }

        #endregion

        #region MarginReport

        [Route("GetAllParentLocations")]
        [HttpGet]
        public IActionResult GetAllParentLocations()
        {
            try
            {
                DataTable result = _dalDaVinci.GetAllParentLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllParentLocations.");
            }
            return NotFound();
        }

        [Route("GetSalesPerItem")]
        [HttpPost]
        public IActionResult GetSalesPerItem(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerItem(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerItem.");
            }
            return NotFound();
        }

        [Route("GetSalesPerReceipt")]
        [HttpPost]
        public IActionResult GetSalesPerReceipt(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerReceipt(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerReceipt.");
            }
            return NotFound();
        }

        [Route("GetSalesPerDate")]
        [HttpPost]
        public IActionResult GetSalesPerDate(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerDate(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerDate.");
            }
            return NotFound();
        }

        [Route("GetSalesPerShop")]
        [HttpPost]
        public IActionResult GetSalesPerShop(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerShop(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerShop.");
            }
            return NotFound();
        }

        [Route("GetSalesPerItemPerReceipt")]
        [HttpPost]
        public IActionResult GetSalesPerItemPerReceipt(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerItemPerReceipt(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerItemPerReceipt.");
            }
            return NotFound();
        }

        [Route("GetSalesPerShopClerk")]
        [HttpPost]
        public IActionResult GetSalesPerShopClerk(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerShopClerk(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerShopClerk.");
            }
            return NotFound();
        }

        [Route("GetSalesPerItemPerShop")]
        [HttpPost]
        public IActionResult GetSalesPerItemPerShop(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerItemPerShop(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerItemPerShop.");
            }
            return NotFound();
        }

        [Route("GetSalesPerDatePerShop")]
        [HttpPost]
        public IActionResult GetSalesPerDatePerShop(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerDatePerShop(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerDatePerShop.");
            }
            return NotFound();
        }

        [Route("GetSalesPerCustomer")]
        [HttpPost]
        public IActionResult GetSalesPerCustomer(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerCustomer(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerCustomer.");
            }
            return NotFound();
        }

        [Route("GetSalesPerCashier")]
        [HttpPost]
        public IActionResult GetSalesPerCashier(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerCashier(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerCashier.");
            }
            return NotFound();
        }

        [Route("GetSalesPerClerk")]
        [HttpPost]
        public IActionResult GetSalesPerClerk(Dictionary<string, object> Param)
        {
            try
            {

                int IsPeriodSearch = Int32.Parse(Convert.ToString(Param["IsPeriodSearch"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int Jaar = Int32.Parse(Convert.ToString(Param["Jaar"]));
                int Maand = Int32.Parse(Convert.ToString(Param["Maand"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetSalesPerClerk(IsPeriodSearch, CompanyId, Jaar, Maand, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerClerk.");
            }
            return NotFound();
        }

        #endregion

        #region ModelProductSaleOnCredit

        [Route("GetModelProductSaleOnCreditDetails")]
        [HttpPost]
        public IActionResult GetModelProductSaleOnCreditDetails(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BeginDate = Convert.ToDateTime(Param["BeginDate"].ToString());
                DateTime EndDate = Convert.ToDateTime(Param["EndDate"].ToString());
                int Department = Int32.Parse(Convert.ToString(Param["Department"]));
                DataSet result = _dalDaVinci.GetModelProductSaleOnCreditDetails(BeginDate, EndDate, Department);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetModelProductSaleOnCreditDetails.");
            }
            return NotFound();
        }


        #endregion

        #region MonthlySales4Lacoste

        [Route("LacosteMenTextile")]
        [HttpGet]
        public IActionResult LacosteMenTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteMenTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteMenTextile.");
            }
            return NotFound();
        }

        [Route("LacosteWomenTextile")]
        [HttpGet]
        public IActionResult LacosteWomenTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteWomenTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteWomenTextile.");
            }
            return NotFound();
        }

        [Route("LacosteKidsTextile")]
        [HttpGet]
        public IActionResult LacosteKidsTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteKidsTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteKidsTextile.");
            }
            return NotFound();
        }

        [Route("LacosteAccesoriesTextile")]
        [HttpGet]
        public IActionResult LacosteAccesoriesTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteAccesoriesTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteAccesoriesTextile.");
            }
            return NotFound();
        }

        [Route("LacosteFootwearsTextile")]
        [HttpGet]
        public IActionResult LacosteFootwearsTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteFootwearsTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteFootwearsTextile.");
            }
            return NotFound();
        }

        [Route("LacosteTotalSalesFootwearsnTextile")]
        [HttpGet]
        public IActionResult LacosteTotalSalesFootwearsnTextile(int YearNumber)
        {
            try
            {

                DataTable result = _dalDaVinci.LacosteTotalSalesFootwearsnTextile(YearNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteTotalSalesFootwearsnTextile.");
            }
            return NotFound();
        }

        #endregion

        #region ProductAnalysis

        [Route("GetProductAnalysis")]
        [HttpGet]
        public IActionResult GetProductAnalysis(int ProductId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetProductAnalysis(ProductId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetProductAnalysis.");
            }
            return NotFound();
        }

        [Route("GetItemOverView")]
        [HttpGet]
        public IActionResult GetItemOverView(int ItemId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetItemOverView(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemOverView.");
            }
            return NotFound();
        }

        [Route("GetPurchasesSalesReturnsExportsStockCorrectionsTransfers")]
        [HttpGet]
        public IActionResult GetPurchasesSalesReturnsExportsStockCorrectionsTransfers(int ItemId)
        {
            try
            {
                DataSet result = _dalDaVinci.GetPurchasesSalesReturnsExportsStockCorrectionsTransfers(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchasesSalesReturnsExportsStockCorrectionsTransfers.");
            }
            return NotFound();
        }

        [Route("GetAvgCostPriceCalcPlusStockPerLoc")]
        [HttpGet]
        public IActionResult GetAvgCostPriceCalcPlusStockPerLoc(int ItemId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetAvgCostPriceCalcPlusStockPerLoc(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAvgCostPriceCalcPlusStockPerLoc.");
            }
            return NotFound();
        }

        [Route("DisplaySalesPerShopByItemCategory")]
        [HttpPost]
        public IActionResult DisplaySalesPerShopByItemCategory(Dictionary<string, object> Param)
        {
            try
            {
                DateTime bd = Convert.ToDateTime(Param["bd"].ToString());
                DateTime ed = Convert.ToDateTime(Param["ed"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int GroupId = Int32.Parse(Convert.ToString(Param["GroupId"]));
                int SubGroupId = Int32.Parse(Convert.ToString(Param["SubGroupId"]));
                int TypeId = Int32.Parse(Convert.ToString(Param["TypeId"]));
                int ColorId = Int32.Parse(Convert.ToString(Param["ColorId"]));
                int BrandId = Int32.Parse(Convert.ToString(Param["BrandId"]));
                int ModelId = Int32.Parse(Convert.ToString(Param["ModelId"]));
                int SizeId = Int32.Parse(Convert.ToString(Param["SizeId"]));
                DataTable result = _dalDaVinci.DisplaySalesPerShopByItemCategory(bd, ed, CompanyId, GroupId, SubGroupId, TypeId, ColorId, BrandId, ModelId, SizeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesPerShopByItemCategory.");
            }
            return NotFound();
        }

        #endregion

        #region PurchaseSaleAnalysis

        [Route("GetPurchaseSaleAnalysis")]
        [HttpGet]
        public IActionResult GetPurchaseSaleAnalysis()
        {
            try
            {
                DataSet result = _dalDaVinci.GetPurchaseSaleAnalysis();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchaseSaleAnalysis.");
            }
            return NotFound();
        }

        #endregion

        #region PurchaseSaleCalculation

        [Route("GetStillToSaleInformation")]
        [HttpGet]
        public IActionResult GetStillToSaleInformation(int ProductID)
        {
            try
            {

                DataSet result = _dalDaVinci.GetStillToSaleInformation(ProductID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStillToSaleInformation.");
            }
            return NotFound();
        }
        #endregion

        #region QuantityProductSaleOnCredit

        [Route("GetQuantityproductSaleOnCredit")]
        [HttpPost]
        public IActionResult GetQuantityproductSaleOnCredit(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataSet result = _dalDaVinci.GetQuantityproductSaleOnCredit(FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetQuantityproductSaleOnCredit.");
            }
            return NotFound();
        }

        #endregion

        #region RevenueOverAPeriod

        [Route("GetRevenueDataOverAPeriod")]
        [HttpPost]
        public IActionResult GetRevenueDataOverAPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BeginDate = Convert.ToDateTime(Param["BeginDate"].ToString());
                DateTime EndDate = Convert.ToDateTime(Param["EndDate"].ToString());

                DataSet result = _dalDaVinci.GetRevenueDataOverAPeriod(BeginDate, EndDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetRevenueDataOverAPeriod.");
            }
            return NotFound();
        }

        #endregion

        #region SalePerDepartmentPerBrand
        [Route("GetSalesPerDepartmentPerBrandDetails")]
        [HttpPost]
        public IActionResult GetSalesPerDepartmentPerBrandDetails(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BeginDate = Convert.ToDateTime(Param["BeginDate"].ToString());
                DateTime EndDate = Convert.ToDateTime(Param["EndDate"].ToString());

                DataSet result = _dalDaVinci.GetSalesPerDepartmentPerBrandDetails(BeginDate, EndDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesPerDepartmentPerBrandDetails.");
            }
            return NotFound();
        }

        #endregion

        #region SalesRep

        [Route("LacosteSalesReport")]
        [HttpPost]
        public IActionResult LacosteSalesReport(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());

                DataTable result = _dalDaVinci.LacosteSalesReport(BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - LacosteSalesReport.");
            }
            return NotFound();
        }

        [Route("RevenueBySaleDate")]
        [HttpPost]
        public IActionResult RevenueBySaleDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());

                DataTable result = _dalDaVinci.RevenueBySaleDate(BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - RevenueBySaleDate.");
            }
            return NotFound();
        }

        [Route("RevenueAndProfitBySaleDate")]
        [HttpPost]
        public IActionResult RevenueAndProfitBySaleDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());

                DataSet result = _dalDaVinci.RevenueAndProfitBySaleDate(BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - RevenueAndProfitBySaleDate.");
            }
            return NotFound();
        }

        [Route("GetSalesByCustomerId")]
        [HttpGet]
        public IActionResult GetSalesByCustomerId(int CustomerId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetSalesByCustomerId(CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesByCustomerId.");
            }
            return NotFound();
        }

        [Route("GetSalesByBarcode")]
        [HttpGet]
        public IActionResult GetSalesByBarcode(string Barcode)
        {
            try
            {

                DataSet result = _dalDaVinci.GetSalesByBarcode(Barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesByCustomerId.");
            }
            return NotFound();
        }

        [Route("GetSalesByDate")]
        [HttpPost]
        public IActionResult GetSalesByDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataSet result = _dalDaVinci.GetSalesByDate(BD, ED, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesByDate.");
            }
            return NotFound();
        }

        [Route("GetCreditSales")]
        [HttpPost]
        public IActionResult GetCreditSales(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetCreditSales(BD, ED, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCreditSales.");
            }
            return NotFound();
        }

        [Route("DisplayDailyTransactionOverviewOverPeriod")]
        [HttpPost]
        public IActionResult DisplayDailyTransactionOverviewOverPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime bd = Convert.ToDateTime(Param["bd"].ToString());
                DateTime ed = Convert.ToDateTime(Param["ed"].ToString());
                int parentcompanyid = Int32.Parse(Convert.ToString(Param["parentcompanyid"]));
                int companyid = Int32.Parse(Convert.ToString(Param["companyid"]));
                int tillid = Int32.Parse(Convert.ToString(Param["tillid"]));
                DataTable result = _dalDaVinci.DisplayDailyTransactionOverviewOverPeriod(bd, ed, parentcompanyid, companyid, tillid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDailyTransactionOverviewOverPeriod.");
            }
            return NotFound();
        }

        #endregion

        #region SalesSerachByBarcode

        [Route("GetDisplaySearchSaleByItemBarcode")]
        [HttpGet]
        public IActionResult GetDisplaySearchSaleByItemBarcode(string ItemBarcode)
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplaySearchSaleByItemBarcode(ItemBarcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySearchSaleByItemBarcode.");
            }
            return NotFound();
        }

        #endregion

        #region SalesSummary

        [Route("GetBalancePaymentsPerReceipt")]
        [HttpGet]
        public IActionResult GetBalancePaymentsPerReceipt(Int64 saleid)
        {
            try
            {
                DataTable result = _dalDaVinci.GetBalancePaymentsPerReceipt(saleid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBalancePaymentsPerReceipt.");
            }
            return NotFound();
        }

        [Route("DisplaySalesSummaryReport")]
        [HttpPost]
        public IActionResult DisplaySalesSummaryReport(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataSet result = _dalDaVinci.DisplaySalesSummaryReport(FromDate, ToDate, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesSummaryReport.");
            }
            return NotFound();
        }

        [Route("DisplayMontlySales")]
        [HttpPost]
        public IActionResult DisplayMontlySales(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataSet result = _dalDaVinci.DisplayMontlySales(FromDate, ToDate, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayMontlySales.");
            }
            return NotFound();
        }

        #endregion

        #region SizeProductSaleOnCredit
        [Route("GetSizeProductSaleOnCredit")]
        [HttpPost]
        public IActionResult GetSizeProductSaleOnCredit(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataSet result = _dalDaVinci.GetSizeProductSaleOnCredit(FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSizeProductSaleOnCredit.");
            }
            return NotFound();
        }

        #endregion

        #region ViewItemSaleOnPeriod

        [Route("GetSoldItemsDetailsInPeriod")]
        [HttpPost]
        public IActionResult GetSoldItemsDetailsInPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BeginDate = Convert.ToDateTime(Param["BeginDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataSet result = _dalDaVinci.GetSoldItemsDetailsInPeriod(BeginDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSoldItemsDetailsInPeriod.");
            }
            return NotFound();
        }

        #endregion

        #endregion

        #region Stock Folder(in Report Folder)

        #region Exports

        [Route("NewExport")]
        [HttpPost]
        public IActionResult NewExport(Dictionary<string, object> Param)
        {
            try
            {
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.L_NewExport(CustomerId, UserId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - NewExport.");
            }
            return NotFound();
        }

        [Route("GetExports")]
        [HttpPost]
        public IActionResult GetExports(Dictionary<string, object> Param)
        {
            try
            {
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.L_GetExports(CustomerId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetExports.");
            }
            return NotFound();
        }

        [Route("GetExportedItems")]
        [HttpGet]
        public IActionResult GetExportedItems(int ExportId)
        {
            try
            {

                DataTable result = _dalDaVinci.L_GetExportedItems(ExportId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetExportedItems.");
            }
            return NotFound();
        }

        [Route("AddExportItem")]
        [HttpPost]
        public IActionResult AddExportItem(Dictionary<string, object> Param)
        {
            try
            {
                int ExportId = Int32.Parse(Convert.ToString(Param["ExportId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int Qty = Int32.Parse(Convert.ToString(Param["Qty"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));
                DataTable result = _dalDaVinci.L_AddExportItem(ExportId, UserId, Qty, Barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddExportItem.");
            }
            return NotFound();
        }

        [Route("GetExportReport")]
        [HttpGet]
        public IActionResult GetExportReport(int ExportId)
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetExportReport(ExportId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetExportReport.");
            }
            return NotFound();
        }

        [Route("AddUpdateNewExportWithExcel")]
        [HttpPost]
        public IActionResult AddUpdateNewExportWithExcel(Dictionary<string, object> Param)
        {
            try
            {
                int ExportId = Int32.Parse(Convert.ToString(Param["ExportId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dTableBarcodeQuantity = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dTableBarcodeQuantity"].ToString(), typeof(DataTable));
                int ExportCompanyId = Int32.Parse(Convert.ToString(Param["ExportCompanyId"]));
                DataTable result = _dalDaVinci.AddUpdateNewExportWithExcel(ExportId, UserId, dTableBarcodeQuantity, ExportCompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddExportItem.");
            }
            return NotFound();
        }

        #endregion

        #region ProductStockPerDepartment
        [Route("GetProductStockPerDepartment")]
        [HttpGet]
        public IActionResult GetProductStockPerDepartment()
        {
            try
            {
                DataSet result = _dalDaVinci.GetProductStockPerDepartment();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetProductStockPerDepartment.");
            }
            return NotFound();
        }

        #endregion

        #region SalesItemStockPerDepartment
        [Route("GetSalesItemStockPerDepartment")]
        [HttpGet]
        public IActionResult GetSalesItemStockPerDepartment()
        {
            try
            {
                DataSet result = _dalDaVinci.GetSalesItemStockPerDepartment();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesItemStockPerDepartment.");
            }
            return NotFound();
        }

        #endregion

        #region StockRep
        [Route("ItemsStockPerCompany")]
        [HttpGet]
        public IActionResult ItemsStockPerCompany()
        {
            try
            {
                DataSet result = _dalDaVinci.ItemsStockPerCompany();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ItemsStockPerCompany.");
            }
            return NotFound();
        }

        [Route("L_GetAllParentLocations")]
        [HttpGet]
        public IActionResult L_GetAllParentLocations()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetAllParentLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetAllParentLocations.");
            }
            return NotFound();
        }

        [Route("GetCurrentStockList")]
        [HttpGet]
        public IActionResult GetCurrentStockList(int CompId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetCurrentStockList(CompId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentStockList.");
            }
            return NotFound();
        }

        [Route("GetCurrentStockList2")]
        [HttpGet]
        public IActionResult GetCurrentStockList2(int CompId)
        {
            try
            {

                DataSet result = _dalDaVinci.L_GetCurrentStockList2(CompId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentStockList2.");
            }
            return NotFound();
        }

        [Route("GetCompany")]
        [HttpGet]
        public IActionResult GetCompany()
        {
            try
            {
                DataSet result = _dalDaVinci.GetCompany();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCompany.");
            }
            return NotFound();
        }

        [Route("GetItemFlow")]
        [HttpPost]
        public IActionResult GetItemFlow(Dictionary<string, object> Param)
        {
            try
            {
                int CompId = Int32.Parse(Convert.ToString(Param["CompId"]));
                string CompanyCode = (Convert.ToString(Param["CompanyCode"]));
                int CompanyParentId = Int32.Parse(Convert.ToString(Param["CompanyParentId"]));
                DataSet result = _dalDaVinci.L_GetItemFlow(CompId, CompanyCode, CompanyParentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemFlow.");
            }
            return NotFound();
        }

        [Route("GetDisplayStockItemsFlowForAllWithItemBarcodeForHO")]
        [HttpGet]
        public IActionResult GetDisplayStockItemsFlowForAllWithItemBarcodeForHO(int ItemId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetDisplayStockItemsFlowForAllWithItemBarcodeForHO(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayStockItemsFlowForAllWithItemBarcodeForHO.");
            }
            return NotFound();
        }

        [Route("GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId")]
        [HttpPost]
        public IActionResult GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId(Dictionary<string, object> Param)
        {
            try
            {
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                int ParentCompanyId = Int32.Parse(Convert.ToString(Param["ParentCompanyId"]));
                DataSet result = _dalDaVinci.GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId(ItemId, ParentCompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId.");
            }
            return NotFound();
        }

        [Route("GetCurrentStockQtyList")]
        [HttpGet]
        public IActionResult GetCurrentStockQtyList(int CompId)
        {
            try
            {

                DataSet result = _dalDaVinci.GetCurrentStockQtyList(CompId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentStockQtyList.");
            }
            return NotFound();
        }

        [Route("GetItemListOfNegativeQtyForCompany")]
        [HttpPost]
        public IActionResult GetItemListOfNegativeQtyForCompany(Dictionary<string, object> Param)
        {
            try
            {
                int CompId = Int32.Parse(Convert.ToString(Param["CompId"]));
                DateTime TransactDate = Convert.ToDateTime(Param["TransactDate"].ToString());
                DataSet result = _dalDaVinci.GetItemListOfNegativeQtyForCompany(CompId, TransactDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemListOfNegativeQtyForCompany.");
            }
            return NotFound();
        }


        [Route("GetReportOfNegativeQtyForCompany")]
        [HttpPost]
        public IActionResult GetReportOfNegativeQtyForCompany(Dictionary<string, object> Param)
        {
            try
            {
                int CompId = Int32.Parse(Convert.ToString(Param["CompId"]));
                DateTime TransactDate = Convert.ToDateTime(Param["TransactDate"].ToString());
                DataSet result = _dalDaVinci.GetReportOfNegativeQtyForCompany(CompId, TransactDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetReportOfNegativeQtyForCompany.");
            }
            return NotFound();
        }

        [Route("GetItems2ReorderPerCompany")]
        [HttpGet]
        public IActionResult GetItems2ReorderPerCompany(int CompId)
        {
            try
            {
                DataSet result = _dalDaVinci.GetItems2ReorderPerCompany(CompId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItems2ReorderPerCompany.");
            }
            return NotFound();
        }

        [Route("GetStockCompany")]
        [HttpGet]
        public IActionResult GetStockCompany()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetStockCompany();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockCompany.");
            }
            return NotFound();
        }
        [Route("GetStockValue")]
        [HttpPost]
        public IActionResult GetStockValue(Dictionary<string, object> Param)
        {
            try
            {
                int companyID = Int32.Parse(Convert.ToString(Param["companyID"]));
                string Date = (Convert.ToString(Param["Date"]));

                DataSet result = _dalDaVinci.L_GetStockValue(companyID, Date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockValue.");
            }
            return NotFound();
        }

        [Route("GetStockByStockLocId")]
        [HttpGet]
        public IActionResult GetStockByStockLocId(int StockLocationId)
        {
            try
            {

                DataTable result = _dalDaVinci.L_GetStockByStockLocId(StockLocationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetStockByStockLocId.");
            }
            return NotFound();
        }


        [Route("CurrentStockQtyList2ByItemId")]
        [HttpGet]
        public IActionResult CurrentStockQtyList2ByItemId(int ItemId)
        {
            try
            {

                DataTable result = _dalDaVinci.L_CurrentStockQtyList2ByItemId(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CurrentStockQtyList2ByItemId.");
            }
            return NotFound();
        }

        [Route("GetCurrentNegStockList2")]
        [HttpGet]
        public IActionResult GetCurrentNegStockList2(int companyId)
        {
            try
            {

                DataSet result = _dalDaVinci.L_GetCurrentNegStockList2(companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentNegStockList2.");
            }
            return NotFound();
        }

        [Route("GetItemIdbyBarcode")]
        [HttpGet]
        public IActionResult GetItemIdbyBarcode(string Barcode)
        {
            try
            {

                int result = _dalDaVinci.L_GetItemIdbyBarcode(Barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemIdbyBarcode.");
            }
            return NotFound();
        }

        [Route("GetTillIdByStockLocationId")]
        [HttpGet]
        public IActionResult GetTillIdByStockLocationId(int StockLocationId)
        {
            try
            {

                DataTable result = _dalDaVinci.L_GetTillIdByStockLocationId(StockLocationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetTillIdByStockLocationId.");
            }
            return NotFound();
        }

        [Route("GetStockLocByCompanyId")]
        [HttpGet]
        public IActionResult GetStockLocByCompanyId(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetStockLocByCompanyId(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockLocByCompanyId.");
            }
            return NotFound();
        }

        [Route("GetScannedItemsByStockLocId")]
        [HttpPost]
        public IActionResult GetScannedItemsByStockLocId(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocId = Int32.Parse(Convert.ToString(Param["StockLocId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.L_GetScannedItemsByStockLocId(StockLocId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetScannedItemsByStockLocId.");
            }
            return NotFound();
        }

        [Route("GetInventoriesByStockLocId")]
        [HttpPost]
        public IActionResult GetInventoriesByStockLocId(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StockLocId = Int32.Parse(Convert.ToString(Param["StockLocId"]));
                DataTable result = _dalDaVinci.GetInventoriesByStockLocId(CompanyId, StockLocId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInventoriesByStockLocId.");
            }
            return NotFound();
        }

        [Route("GetInventoryTroubleshootingItemsById")]
        [HttpGet]
        public IActionResult GetInventoryTroubleshootingItemsById(int InventoryId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetInventoryTroubleshootingItemsById(InventoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInventoryTroubleshootingItemsById.");
            }
            return NotFound();
        }

        [Route("GetItemsByInventoryId_Diff")]
        [HttpGet]
        public IActionResult GetItemsByInventoryId_Diff(int InventoryId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetItemsByInventoryId_Diff(InventoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemsByInventoryId_Diff.");
            }
            return NotFound();
        }

        [Route("GetInventoryNote")]
        [HttpPost]
        public IActionResult GetInventoryNote(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocId = Int32.Parse(Convert.ToString(Param["StockLocId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                string result = _dalDaVinci.GetInventoryNote(StockLocId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetInventoryNote.");
            }
            return NotFound();
        }

        [Route("GetItemsByInventoryId")]
        [HttpGet]
        public IActionResult GetItemsByInventoryId(int InventoryId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetItemsByInventoryId(InventoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemsByInventoryId.");
            }
            return NotFound();
        }

        [Route("GetWriteOffHistory")]
        [HttpPost]
        public IActionResult GetWriteOffHistory(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataSet result = _dalDaVinci.GetWriteOffHistory(BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetWriteOffHistory.");
            }
            return NotFound();
        }

        [Route("GetWriteOffReport")]
        [HttpGet]
        public IActionResult GetWriteOffReport(int WriteOffId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetWriteOffReport(WriteOffId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetWriteOffReport.");
            }
            return NotFound();
        }

        [Route("GetStockHistory")]
        [HttpPost]
        public IActionResult GetStockHistory(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                object result = _dalDaVinci.GetStockHistory(StockLocationId, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockHistory.");
            }
            return NotFound();
        }

        [Route("GetDisplayItemAllItemsDetails")]
        [HttpGet]
        public IActionResult GetDisplayItemAllItemsDetails(int ItemId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayItemAllItemsDetails(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayItemAllItemsDetails.");
            }
            return NotFound();
        }

        [Route("Reshuffle")]
        [HttpPost]
        public IActionResult Reshuffle(Dictionary<string, object> Param)
        {
            try
            {
                int companyId = Int32.Parse(Convert.ToString(Param["companyId"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.L_Reshuffle(companyId, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_Reshuffle.");
            }
            return NotFound();
        }
        [Route("CurrentStockQtyList2ByRefCode")]
        [HttpGet]
        public IActionResult CurrentStockQtyList2ByRefCode(string RefCode)
        {
            try
            {
                DataTable result = _dalDaVinci.L_CurrentStockQtyList2ByRefCode(RefCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_CurrentStockQtyList2ByRefCode.");
            }
            return NotFound();
        }

        [Route("GetItemMutations")]
        [HttpPost]
        public IActionResult GetItemMutations(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataSet result = _dalDaVinci.GetItemMutations(StockLocationId, ItemId, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemMutations.");
            }
            return NotFound();
        }

        [Route("GetHistoryByPeriod")]
        [HttpPost]
        public IActionResult GetHistoryByPeriod(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetHistoryByPeriod(StockLocationId, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetHistoryByPeriod.");
            }
            return NotFound();
        }
        [Route("GetAllStockLocations")]
        [HttpGet]
        public IActionResult GetAllStockLocations()
        {
            try
            {
                DataTable result = _dalDaVinci.GetAllStockLocations();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllStockLocations.");
            }
            return NotFound();
        }
        [Route("GetStockItemTransactionDetailPerDay")]
        [HttpPost]
        public IActionResult GetStockItemTransactionDetailPerDay(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable result = _dalDaVinci.GetStockItemTransactionDetailPerDay(StockLocationId, ItemId, BD, ED);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockItemTransactionDetailPerDay.");
            }
            return NotFound();
        }

        [Route("GetLocationsUsingTransferBarcode")]
        [HttpGet]
        public IActionResult GetLocationsUsingTransferBarcode(string _transfer_barcode)
        {
            try
            {
                DataTable result = _dalDaVinci.GetLocationsUsingTransferBarcode(_transfer_barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetLocationsUsingTransferBarcode.");
            }
            return NotFound();
        }

        [Route("UpdateTransferStockLocations")]
        [HttpPost]
        public IActionResult UpdateTransferStockLocations(Dictionary<string, object> Param)
        {
            try
            {
                string _transfer_barcode = (Convert.ToString(Param["_transfer_barcode"]));
                int stockloc_from = Int32.Parse(Convert.ToString(Param["stockloc_from"]));
                int stockloc_to = Int32.Parse(Convert.ToString(Param["stockloc_to"]));
                bool result = _dalDaVinci.UpdateTransferStockLocations(_transfer_barcode, stockloc_from, stockloc_to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateTransferStockLocations.");
            }
            return NotFound();
        }

        [Route("UpdateTransferredItem")]
        [HttpPost]
        public IActionResult UpdateTransferredItem(Dictionary<string, object> Param)
        {
            try
            {
                string _transfer_barcode = (Convert.ToString(Param["_transfer_barcode"]));
                string _old_barcode = (Convert.ToString(Param["_old_barcode"]));
                string _new_barcode = (Convert.ToString(Param["_new_barcode"]));
                bool result = _dalDaVinci.UpdateTransferredItem(_transfer_barcode, _old_barcode, _new_barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateTransferredItem.");
            }
            return NotFound();
        }

        [Route("UpdateTransferredItemQty")]
        [HttpPost]
        public IActionResult UpdateTransferredItemQty(Dictionary<string, object> Param)
        {
            try
            {
                string _transfer_barcode = (Convert.ToString(Param["_transfer_barcode"]));
                string barcode = (Convert.ToString(Param["barcode"]));
                decimal qty = Convert.ToDecimal(Convert.ToString(Param["qty"]));
                bool result = _dalDaVinci.UpdateTransferredItemQty(_transfer_barcode, barcode, qty);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdaUpdateTransferredItemQtyteTransferredItem.");
            }
            return NotFound();
        }

        /// <summary>
        ///same  method name different parameter that is why avoid conflict issue   name changed "GetShops" to "Get_Shops"
        /// </summary>
        /// <param name="ParentCompanyId"></param>
        /// <returns></returns>
        [Route("Get_Shops")]
        [HttpGet]
        public IActionResult Get_Shops(int ParentCompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.Get_Shops(ParentCompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetShops.");
            }
            return NotFound();
        }


        [Route("GetTills")]
        [HttpGet]
        public IActionResult GetTills(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetTills(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetWriteOffReport.");
            }
            return NotFound();
        }

        [Route("ReceiveAll")]
        [HttpPost]
        public IActionResult ReceiveAll(Dictionary<string, object> Param)
        {
            try
            {
                int transferid = Int32.Parse(Convert.ToString(Param["transferid"]));
                int userId = Int32.Parse(Convert.ToString(Param["userId"]));
                bool result = _dalDaVinci.ReceiveAll(transferid, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ReceiveAll.");
            }
            return NotFound();
        }

        [Route("GetBrandsBasedonStockItem")]
        [HttpGet]
        public IActionResult GetBrandsBasedonStockItem()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetBrandsBasedonStockItem();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBrandsBasedonStockItem.");
            }
            return NotFound();
        }

        [Route("GetCompanyDetailsStockItem")]
        [HttpGet]
        public IActionResult GetCompanyDetailsStockItem()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetCompanyDetailsStockItem();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCompanyDetailsStockItem.");
            }
            return NotFound();
        }
        [Route("GetCurrentStoctQtyAndPrice")]
        [HttpPost]
        public IActionResult GetCurrentStoctQtyAndPrice(Dictionary<string, object> Param)
        {
            try
            {
                string BrandId = (Convert.ToString(Param["BrandId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime Todate = Convert.ToDateTime(Param["Todate"].ToString());
                DataTable result = _dalDaVinci.GetCurrentStoctQtyAndPrice(BrandId, CompanyId, FromDate, Todate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCurrentStoctQtyAndPrice.");
            }
            return NotFound();
        }

        #endregion

        #endregion
        #endregion

        #region Revenue Folder

        #region ItemStockByBarcode
        [Route("GetDisplayAllActiveItemsWithBarcode")]
        [HttpGet]
        public IActionResult GetDisplayAllActiveItemsWithBarcode()
        {
            try
            {
                DataTable result = _dalDaVinci.L_GetDisplayAllActiveItemsWithBarcode();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllActiveItemsWithBarcode.");
            }
            return NotFound();
        }
        [Route("GetDisplayStockItemDetailsDiffLocationWithItemId")]
        [HttpGet]
        public IActionResult GetDisplayStockItemDetailsDiffLocationWithItemId(int ItemId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplayStockItemDetailsDiffLocationWithItemId(ItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayStockItemDetailsDiffLocationWithItemId.");
            }
            return NotFound();
        }


        #endregion

        #region OutStandingSaleForCompany
        [Route("GetDisplayOutStandingSaleForCompany")]
        [HttpGet]
        public IActionResult GetDisplayOutStandingSaleForCompany(int CompanyId)
        {
            try
            {
                DataSet result = _dalDaVinci.GetDisplayOutStandingSaleForCompany(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayOutStandingSaleForCompany.");
            }
            return NotFound();
        }

        #endregion

        #region OutstandingSalesPerCustomer
        [Route("GetDisplayOutstandingSalesPerCustomer")]
        [HttpGet]
        public IActionResult GetDisplayOutstandingSalesPerCustomer(int CompanyId)
        {
            try
            {
                DataSet result = _dalDaVinci.GetDisplayOutstandingSalesPerCustomer(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayOutstandingSalesPerCustomer.");
            }
            return NotFound();
        }

        [Route("GetDisplayReportDetailsOutstandingSalesPerCustomer")]
        [HttpPost]
        public IActionResult GetDisplayReportDetailsOutstandingSalesPerCustomer(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                DataSet result = _dalDaVinci.GetDisplayReportDetailsOutstandingSalesPerCustomer(CompanyId, CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReportDetailsOutstandingSalesPerCustomer.");
            }
            return NotFound();
        }

        [Route("GetCreditSalesPaymentHistory")]
        [HttpPost]
        public IActionResult GetCreditSalesPaymentHistory(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Convert.ToString(Param["ToDate"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetCreditSalesPaymentHistory(FromDate, ToDate, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCreditSalesPaymentHistory.");
            }
            return NotFound();
        }

        [Route("DisplayAccountsReceivable")]
        [HttpPost]
        public IActionResult DisplayAccountsReceivable(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DateTime TransactionDate = Convert.ToDateTime(Param["TransactionDate"].ToString());
                DataTable result = _dalDaVinci.DisplayAccountsReceivable(CompanyId, TransactionDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAccountsReceivable.");
            }
            return NotFound();
        }

        [Route("DisplaySubsidiaryLedger")]
        [HttpPost]
        public IActionResult DisplaySubsidiaryLedger(Dictionary<string, object> Param)
        {
            try
            {
                int customerid = Int32.Parse(Convert.ToString(Param["customerid"]));
                int companyid = Int32.Parse(Convert.ToString(Param["companyid"]));
                DateTime TransactionDate = Convert.ToDateTime(Param["TransactionDate"].ToString());
                DataTable result = _dalDaVinci.DisplaySubsidiaryLedger(customerid, companyid, TransactionDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySubsidiaryLedger.");
            }
            return NotFound();
        }

        [Route("ManualUpdateCashCreditCostsRunByDate")]
        [HttpGet]
        public IActionResult ManualUpdateCashCreditCostsRunByDate(DateTime RefreshDate)
        {
            try
            {
                DataTable result = _dalDaVinci.ManualUpdateCashCreditCostsRunByDate(RefreshDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ManualUpdateCashCreditCostsRunByDate.");
            }
            return NotFound();
        }
        #endregion

        #region ProductAnalyse
        [Route("DisplayProductAnalysis")]
        [HttpPost]
        public IActionResult DisplayProductAnalysis(Dictionary<string, object> Param)
        {
            try
            {
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));

                DataSet result = _dalDaVinci.DisplayProductAnalysis(ItemId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayProductAnalysis.");
            }
            return NotFound();
        }
        #endregion

        #region PurchaseSaleAnalysis
        [Route("GetPurchasesSales")]
        [HttpGet]
        public IActionResult GetPurchasesSales(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetPurchasesSales(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchasesSales.");
            }
            return NotFound();
        }

        // [Route("GetPurchasesSales")]

        //[HttpGet]
        //public async Task<IActionResult> GetPurchasesSales(int CompanyId)
        //{
        //    try
        //    {
        //        DataTable result =  _dalDaVinci.GetPurchasesSales(CompanyId);
        //        await Task.WhenAll((IEnumerable<Task>)result);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPurchasesSales.");
        //    }
        //    return NotFound();
        //}



        #endregion

        #region ReplenishmentReport
        [Route("DisplayMangoStockPerHoursWithPeriod")]
        [HttpPost]
        public IActionResult DisplayMangoStockPerHoursWithPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime CurrentDate = Convert.ToDateTime(Param["CurrentDate"].ToString());
                DateTime FromTime = Convert.ToDateTime(Param["FromTime"].ToString());
                DateTime ToTime = Convert.ToDateTime(Param["ToTime"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));

                DataTable result = _dalDaVinci.DisplayMangoStockPerHoursWithPeriod(CurrentDate, FromTime, ToTime, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayMangoStockPerHoursWithPeriod.");
            }
            return NotFound();
        }

        [Route("DisplayActiveMangoLocation")]
        [HttpGet]
        public IActionResult DisplayActiveMangoLocation()
        {
            try
            {
                DataTable result = _dalDaVinci.DisplayActiveMangoLocation();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayActiveMangoLocation.");
            }
            return NotFound();
        }

        #endregion

        #region Revenues

        [Route("GetDisplayPivotReportRevenueByDate")]
        [HttpPost]
        public IActionResult GetDisplayPivotReportRevenueByDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetDisplayPivotReportRevenueByDate(FromDate, ToDate, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayPivotReportRevenueByDate.");
            }
            return NotFound();
        }

        [Route("GetDisplayPivotReportRevenueByLocation")]
        [HttpPost]
        public IActionResult GetDisplayPivotReportRevenueByLocation(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetDisplayPivotReportRevenueByLocation(FromDate, ToDate, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayPivotReportRevenueByLocation.");
            }
            return NotFound();
        }

        [Route("GetRevenuesByCompanyByPeriodPerClerk")]
        [HttpPost]
        public IActionResult GetRevenuesByCompanyByPeriodPerClerk(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataSet result = _dalDaVinci.GetRevenuesByCompanyByPeriodPerClerk(BD, ED, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetRevenuesByCompanyByPeriodPerClerk.");
            }
            return NotFound();
        }

        [Route("GetRevenuesByCompanyByPeriodPerCustomer")]
        [HttpPost]
        public IActionResult GetRevenuesByCompanyByPeriodPerCustomer(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.GetRevenuesByCompanyByPeriodPerCustomer(BD, ED, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetRevenuesByCompanyByPeriodPerCustomer.");
            }
            return NotFound();
        }

        [Route("GetRevenuesByCompanyByPeriodPerClerk4ExcelReport")]
        [HttpPost]
        public IActionResult GetRevenuesByCompanyByPeriodPerClerk4ExcelReport(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.GetRevenuesByCompanyByPeriodPerClerk4ExcelReport(BD, ED, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetRevenuesByCompanyByPeriodPerClerk4ExcelReport.");
            }
            return NotFound();
        }

        [Route("GetSalesByCustomer")]
        [HttpPost]
        public IActionResult GetSalesByCustomer(Dictionary<string, object> Param)
        {
            try
            {
                int CustomerId = Int32.Parse(Param["CustomerId"].ToString());
                int CompanyId = Int32.Parse(Param["CompanyId"].ToString());
                DataTable result = _dalDaVinci.GetSalesByCustomer(CustomerId, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesByCustomer.");
            }
            return NotFound();
        }

        [Route("GetDisplayPivotReportNetSalesByLocation")]
        [HttpPost]
        public IActionResult GetDisplayPivotReportNetSalesByLocation(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CurrencyId = Int32.Parse(Convert.ToString(Param["CurrencyId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable result = _dalDaVinci.GetDisplayPivotReportNetSalesByLocation(BD, ED, CurrencyId, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayPivotReportNetSalesByLocation.");
            }
            return NotFound();
        }

        #endregion

        #region RevenuesByItem
        [Route("GetDisplayAllItemsDetailsWithReportByPeriod")]
        [HttpPost]
        public IActionResult GetDisplayAllItemsDetailsWithReportByPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.GetDisplayAllItemsDetailsWithReportByPeriod(FromDate, ToDate, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllItemsDetailsWithReportByPeriod.");
            }
            return NotFound();
        }

        #endregion

        #region SaleGetSalesByDate
        [Route("GetDisplaySaleGetSalesByDate")]
        [HttpPost]
        public IActionResult GetDisplaySaleGetSalesByDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataTable result = _dalDaVinci.GetDisplaySaleGetSalesByDate(FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySaleGetSalesByDate.");
            }
            return NotFound();
        }

        [Route("GetSalesReprintSalesReceiptForHOAllLocation")]
        [HttpPost]
        public IActionResult GetSalesReprintSalesReceiptForHOAllLocation(Dictionary<string, object> Param)
        {
            try
            {
                long SaleId = Int64.Parse(Param["CustomerId"].ToString());
                int TillId = Int32.Parse(Param["TillId"].ToString());
                DataSet result = _dalDaVinci.GetSalesReprintSalesReceiptForHOAllLocation(SaleId, TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesReprintSalesReceiptForHOAllLocation.");
            }
            return NotFound();
        }


        [Route("GetSalesReprintSalesReceiptForHOAllLocation_GC")]
        [HttpPost]
        public IActionResult GetSalesReprintSalesReceiptForHOAllLocation_GC(Dictionary<string, object> Param)
        {
            try
            {
                int AccountTypeId = Int32.Parse(Param["AccountTypeId"].ToString());
                long SaleId = Int64.Parse(Param["CustomerId"].ToString());
                int TillId = Int32.Parse(Param["TillId"].ToString());

                DataSet result = _dalDaVinci.GetSalesReprintSalesReceiptForHOAllLocation_GC(AccountTypeId, SaleId, TillId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalesReprintSalesReceiptForHOAllLocation_GC.");
            }
            return NotFound();
        }

        [Route("GetGetAllByItemByBarcode")]
        [HttpGet]
        public IActionResult GetGetAllByItemByBarcode(string Barcode)
        {
            try
            {
                DataSet result = _dalDaVinci.GetGetAllByItemByBarcode(Barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetGetAllByItemByBarcode.");
            }
            return NotFound();
        }

        [Route("GetGetAllByItemByIMEI")]
        [HttpGet]
        public IActionResult GetGetAllByItemByIMEI(string IMEI)
        {
            try
            {
                DataSet result = _dalDaVinci.GetGetAllByItemByIMEI(IMEI);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetGetAllByItemByIMEI.");
            }
            return NotFound();
        }

        #endregion

        #region SalesAnalysis

        [Route("DisplaySalesAnalysis")]
        [HttpPost]
        public IActionResult DisplaySalesAnalysis(Dictionary<string, object> Param)
        {
            try
            {
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable result = _dalDaVinci.DisplaySalesAnalysis(BD, ED, CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesAnalysis.");
            }
            return NotFound();
        }

        #endregion

        #region SalesPerClerk
        [Route("GetDisplaySaleClerkCommissionDetails")]
        [HttpGet]
        public IActionResult GetDisplaySaleClerkCommissionDetails(int CompanyId)
        {
            try
            {
                DataTable result = _dalDaVinci.GetDisplaySaleClerkCommissionDetails(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySaleClerkCommissionDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplaySaleClerkItemsCommissionDetails")]
        [HttpPost]
        public IActionResult GetDisplaySaleClerkItemsCommissionDetails(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int ClerkId = Int32.Parse(Convert.ToString(Param["ClerkId"]));
                DataTable result = _dalDaVinci.GetDisplaySaleClerkItemsCommissionDetails(CompanyId, ClerkId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySaleClerkItemsCommissionDetails.");
            }
            return NotFound();
        }


        #endregion

        #region SalesPerCustomer
        [Route("GetBalancePayments")]
        [HttpGet]
        public IActionResult GetBalancePayments(int CustomerId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetBalancePayments(CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBalancePayments.");
            }
            return NotFound();
        }

        [Route("GetDisplayAllCompanyDetailsForReport")]
        [HttpGet]
        public IActionResult GetDisplayAllCompanyDetailsForReport()
        {
            try
            {

                DataTable result = _dalDaVinci.GetDisplayAllCompanyDetailsForReport();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCompanyDetailsForReport.");
            }
            return NotFound();
        }

        [Route("GetCustomersByCompany")]
        [HttpGet]
        public IActionResult GetCustomersByCompany(int CompanyId)
        {
            try
            {

                DataTable result = _dalDaVinci.GetCustomersByCompany(CompanyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCustomersByCompany.");
            }
            return NotFound();
        }


        [Route("GetBalancePerCustomer")]
        [HttpPost]
        public IActionResult GetBalancePerCustomer(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                DataSet result = _dalDaVinci.GetBalancePerCustomer(CompanyId, CustomerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBalancePerCustomer.");
            }
            return NotFound();
        }


        #endregion

        #region SoldItemsByItem


        [Route("GetSoldItemsByItemProperties")]
        [HttpPost]
        public IActionResult GetSoldItemsByItemProperties(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable result = _dalDaVinci.GetSoldItemsByItemProperties(CompanyId, FromDate, ToDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSoldItemsByItemProperties.");
            }
            return NotFound();
        }

        [Route("GetSaleNotes")]
        [HttpPost]
        public IActionResult GetSaleNotes(Dictionary<string, object> Param)
        {
            try
            {
                DateTime bd = Convert.ToDateTime(Param["bd"].ToString());
                DateTime ed = Convert.ToDateTime(Param["ed"].ToString());
                int itemid = Int32.Parse(Convert.ToString(Param["itemid"]));
                int parent_companyid = Int32.Parse(Convert.ToString(Param["parent_companyid"]));
                DataTable result = _dalDaVinci.GetSaleNotes(bd, ed, itemid, parent_companyid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSaleNotes.");
            }
            return NotFound();
        }

        #endregion

        #region SoldItemsPerProduct

        [Route("GetCompanies")]
        [HttpGet]
        public IActionResult GetCompanies(int UserId)
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetCompanies(UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCompanies.");
            }
            return NotFound();
        }

        [Route("GetDisplayCompanySoldItemsPerProduct")]
        [HttpPost]
        public IActionResult GetDisplayCompanySoldItemsPerProduct(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataSet result = _dalDaVinci.GetDisplayCompanySoldItemsPerProduct(CompanyId, FromDate, ToDate, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanySoldItemsPerProduct.");
            }
            return NotFound();
        }

        #endregion

        #region SoldSaleItemsByItemProperties
        [Route("GetSoldSaleItemsByItemProperties")]
        [HttpPost]
        public IActionResult GetSoldSaleItemsByItemProperties(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataTable dtResult = _dalDaVinci.GetSoldSaleItemsByItemProperties(CompanyId, FromDate, ToDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSoldSaleItemsByItemProperties.");
            }
            return NotFound();
        }

        [Route("GetSoldItemsByPropertiesPerCompany")]
        [HttpPost]
        public IActionResult GetSoldItemsByPropertiesPerCompany(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());

                DataTable dtResult = _dalDaVinci.GetSoldItemsByPropertiesPerCompany(CompanyId, FromDate, ToDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSoldItemsByPropertiesPerCompany.");
            }
            return NotFound();
        }

        [Route("GetCurrentStockListOfSaleItems")]
        [HttpGet]
        public IActionResult GetCurrentStockListOfSaleItems(int CompanyId)
        {
            try
            {

                DataTable dtResult = _dalDaVinci.L_GetCurrentStockListOfSaleItems(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetCurrentStockListOfSaleItems.");
            }
            return NotFound();
        }

        #endregion

        #endregion

        #region Sales Folder

        #region Sales

        [Route("GetDisplaySalesDefaultInformation")]
        [HttpGet]
        public IActionResult GetDisplaySalesDefaultInformation(int CompanyId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.L_GetDisplaySalesDefaultInformation(CompanyId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySalesDefaultInformation.");
            }
            return NotFound();
        }

        [Route("GetDisplayReprintSalesDetails")]
        [HttpGet]
        public IActionResult GetDisplayReprintSalesDetails(int TillId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.L_GetDisplayReprintSalesDetails(TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReprintSalesDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplaySalesReturnHistory")]
        [HttpGet]
        public IActionResult GetDisplaySalesReturnHistory(int TillId)
        {
            try
            {

                DataSet dsResult = _dalDaVinci.L_GetDisplaySalesReturnHistory(TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySalesReturnHistory.");
            }
            return NotFound();
        }

        [Route("GetDisplayReprintAllSaleItemWithPaymentDetails")]
        [HttpGet]
        public IActionResult GetDisplayReprintAllSaleItemWithPaymentDetails(string ReceiptNumber)
        {
            try
            {

                DataSet dsResult = _dalDaVinci.L_GetDisplayReprintAllSaleItemWithPaymentDetails(ReceiptNumber);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayReprintAllSaleItemWithPaymentDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplayPendingPaymentCustomerBarcode")]
        [HttpPost]
        public IActionResult GetDisplayPendingPaymentCustomerBarcode(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string SalesBarcode = (Convert.ToString(Param["SalesBarcode"]));
                DataSet dsResult = _dalDaVinci.L_GetDisplayPendingPaymentCustomerBarcode(TillId, SalesBarcode);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayPendingPaymentCustomerBarcode.");
            }
            return NotFound();
        }

        [Route("GetDisplayPendingPaymentCustomerReceipt")]
        [HttpGet]
        public IActionResult GetDisplayPendingPaymentCustomerReceipt(int TillId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.L_GetDisplayPendingPaymentCustomerReceipt(TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayPendingPaymentCustomerReceipt.");
            }
            return NotFound();
        }

        [Route("GetReturnItemsWithReturnMoneyDetails")]
        [HttpPost]
        public IActionResult GetReturnItemsWithReturnMoneyDetails(Dictionary<string, object> Param)
        {
            try
            {
                DataTable DataTableSaleReturnedItems = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["DataTableSaleReturnedItems"].ToString(), typeof(DataTable));
                DataTable DataTableSalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["DataTableSalesPaymentCashAmount"].ToString(), typeof(DataTable));
                string Comment = (Convert.ToString(Param["Comment"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int ReturnedByTillId = Int32.Parse(Convert.ToString(Param["ReturnedByTillId"]));
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                decimal TotalReturnMoneyAmount = Convert.ToDecimal(Convert.ToString(Param["TotalReturnMoneyAmount"]));
                DataSet dsResult = _dalDaVinci.L_GetReturnItemsWithReturnMoneyDetails(DataTableSaleReturnedItems, DataTableSalesPaymentCashAmount, Comment, UserId, ReturnedByTillId, ReceiptNumber, TotalReturnMoneyAmount);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetReturnItemsWithReturnMoneyDetails.");
            }
            return NotFound();
        }

        [Route("GetReturnDisplayCustomerItemsDetail")]
        [HttpPost]
        public IActionResult GetReturnDisplayCustomerItemsDetail(Dictionary<string, object> Param)
        {
            try
            {
                string Barcode = (Convert.ToString(Param["Barcode"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataSet dsResult = _dalDaVinci.L_GetReturnDisplayCustomerItemsDetail(Barcode, TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetReturnDisplayCustomerItemsDetail.");
            }
            return NotFound();
        }

        [Route("GetDisplaySaleHistoryOfCurrentAccountingDate")]
        [HttpGet]
        public IActionResult GetDisplaySaleHistoryOfCurrentAccountingDate(int TillId)
        {
            try
            {

                DataSet dsResult = _dalDaVinci.L_GetDisplaySaleHistoryOfCurrentAccountingDate(TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySaleHistoryOfCurrentAccountingDate.");
            }
            return NotFound();
        }

        [Route("GetReturnItemsForCouponDetails")]
        [HttpPost]
        public IActionResult GetReturnItemsForCouponDetails(Dictionary<string, object> Param)
        {
            try
            {
                int ReturnedByTillId = Int32.Parse(Convert.ToString(Param["ReturnedByTillId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                DataTable SaleReturnedItems = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SaleReturnedItems"].ToString(), typeof(DataTable));
                decimal CouponAmount = Convert.ToDecimal(Convert.ToString(Param["CouponAmount"]));
                DataSet dsResult = _dalDaVinci.L_GetReturnItemsForCouponDetails(ReturnedByTillId, UserId, Barcode, Comment, SaleReturnedItems, CouponAmount);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetReturnItemsForCouponDetails.");
            }
            return NotFound();
        }

        [Route("GetDisplaySaleHistoryOfCurrentAccountingDateForCancel")]
        [HttpGet]
        public IActionResult GetDisplaySaleHistoryOfCurrentAccountingDateForCancel(int TillId)
        {
            try
            {

                DataSet dsResult = _dalDaVinci.L_GetDisplaySaleHistoryOfCurrentAccountingDateForCancel(TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySaleHistoryOfCurrentAccountingDateForCancel.");
            }
            return NotFound();
        }

        [Route("GetAddPendingPaymentWithPaymentTransaction")]
        [HttpPost]
        public IActionResult GetAddPendingPaymentWithPaymentTransaction(Dictionary<string, object> Param)
        {
            try
            {
                DataTable SalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesPaymentCashAmount"].ToString(), typeof(DataTable));
                DataTable SalesReturnCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesReturnCashAmount"].ToString(), typeof(DataTable));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int PrincipalId = Int32.Parse(Convert.ToString(Param["PrincipalId"]));
                string SalesBarcode = (Convert.ToString(Param["SalesBarcode"]));
                decimal PaidAmountInUSD = Convert.ToDecimal(Convert.ToString(Param["PaidAmountInUSD"]));
                decimal ReturnAmountInUSD = Convert.ToDecimal(Convert.ToString(Param["ReturnAmountInUSD"]));
                DataSet dsResult = _dalDaVinci.L_GetAddPendingPaymentWithPaymentTransaction(SalesPaymentCashAmount, SalesReturnCashAmount, TillId, PrincipalId, SalesBarcode, PaidAmountInUSD, ReturnAmountInUSD);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddPendingPaymentWithPaymentTransaction.");
            }
            return NotFound();
        }

        [Route("GetAddSaleWithSaleItemsDetailsWithPaymentTransaction")]
        [HttpPost]
        public IActionResult GetAddSaleWithSaleItemsDetailsWithPaymentTransaction(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                DataTable dTableSalesPerson = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dTableSalesPerson"].ToString(), typeof(DataTable));
                int PrincipalId = Int32.Parse(Convert.ToString(Param["PrincipalId"]));
                decimal Discount = Convert.ToDecimal(Convert.ToString(Param["Discount"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                decimal Total = Convert.ToDecimal(Convert.ToString(Param["Total"]));
                string TransactedBy = (Convert.ToString(Param["TransactedBy"]));
                DataTable SaleItemsDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SaleItemsDetails"].ToString(), typeof(DataTable));
                DataTable SalesPaymentCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesPaymentCashAmount"].ToString(), typeof(DataTable));
                DataTable SalesReturnCashAmount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["SalesReturnCashAmount"].ToString(), typeof(DataTable));
                DataTable CouponPaymentDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CouponPaymentDetails"].ToString(), typeof(DataTable));
                int SaleTypeId = Int32.Parse(Convert.ToString(Param["SaleTypeId"]));
                string NewCustomerName = (Convert.ToString(Param["NewCustomerName"]));
                string NewCustomerAddress = (Convert.ToString(Param["NewCustomerAddress"]));
                string NewCustomerPhone = (Convert.ToString(Param["NewCustomerPhone"]));
                DataSet dsResult = _dalDaVinci.L_GetAddSaleWithSaleItemsDetailsWithPaymentTransaction(TillId, CustomerId, dTableSalesPerson, PrincipalId, Discount, Comment, Total,
                                                                                                     TransactedBy, SaleItemsDetails, SalesPaymentCashAmount, SalesReturnCashAmount, CouponPaymentDetails, SaleTypeId,
                                                                                                      NewCustomerName, NewCustomerAddress, NewCustomerPhone);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddSaleWithSaleItemsDetailsWithPaymentTransaction.");
            }
            return NotFound();
        }

        [Route("GetDisplaySalesItemsDetailsWithBarcode")]
        [HttpPost]
        public IActionResult GetDisplaySalesItemsDetailsWithBarcode(Dictionary<string, object> Param)
        {
            try
            {
                string ItemBarcode = (Convert.ToString(Param["ItemBarcode"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable dtResult = _dalDaVinci.L_GetDisplaySalesItemsDetailsWithBarcode(ItemBarcode, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplaySalesItemsDetailsWithBarcode.");
            }
            return NotFound();
        }

        [Route("GetCancelSaleAndContraAllTransaction")]
        [HttpPost]
        public IActionResult GetCancelSaleAndContraAllTransaction(Dictionary<string, object> Param)
        {
            try
            {
                string ReceiptNumber = (Convert.ToString(Param["ReceiptNumber"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dtResult = _dalDaVinci.L_GetCancelSaleAndContraAllTransaction(ReceiptNumber, UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCancelSaleAndContraAllTransaction.");
            }
            return NotFound();
        }

        [Route("GetDisplayCouponsScanDetails")]
        [HttpPost]
        public IActionResult GetDisplayCouponsScanDetails(Dictionary<string, object> Param)
        {
            try
            {
                string Barcode = (Convert.ToString(Param["Barcode"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataTable dtResult = _dalDaVinci.L_GetDisplayCouponsScanDetails(Barcode, TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCouponsScanDetails.");
            }
            return NotFound();
        }

        [Route("GetSalePaymentDetailsInDifferentCurrency")]
        [HttpPost]
        public IActionResult GetSalePaymentDetailsInDifferentCurrency(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                decimal PaymentAmountInUSD = Convert.ToDecimal(Convert.ToString(Param["PaymentAmountInUSD"]));
                DataSet dsResult = _dalDaVinci.L_GetSalePaymentDetailsInDifferentCurrency(TillId, PaymentAmountInUSD);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSalePaymentDetailsInDifferentCurrency.");
            }
            return NotFound();
        }

        [Route("MonthlySales4Lacoste")]
        [HttpGet]
        public IActionResult MonthlySales4Lacoste(int Year)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.MonthlySales4Lacoste(Year);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - MonthlySales4Lacoste.");
            }
            return NotFound();
        }


        #endregion

        #region SalesCreditDiscount

        [Route("GetCustomer4DiscountUpdate")]
        [HttpGet]
        public IActionResult GetCustomer4DiscountUpdate(int CompanyId)
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetCustomer4DiscountUpdate(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCustomer4DiscountUpdate.");
            }
            return NotFound();
        }

        [Route("DisplayAllPendingSalesReceipts")]
        [HttpPost]
        public IActionResult DisplayAllPendingSalesReceipts(Dictionary<string, object> Param)
        {
            try
            {
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable dtResult = _dalDaVinci.DisplayAllPendingSalesReceipts(CustomerId, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllPendingSalesReceipts.");
            }
            return NotFound();
        }

        [Route("GetAllSoldItems4DiscountUpdate")]
        [HttpPost]
        public IActionResult GetAllSoldItems4DiscountUpdate(Dictionary<string, object> Param)
        {
            try
            {
                long SaleId = Int64.Parse(Convert.ToString(Param["SaleId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int CustomerId = Int32.Parse(Convert.ToString(Param["CustomerId"]));

                DataTable dtResult = _dalDaVinci.GetAllSoldItems4DiscountUpdate(SaleId, TillId, CustomerId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllSoldItems4DiscountUpdate.");
            }
            return NotFound();
        }

        [Route("GetDisplayUpdateItemsDiscount")]
        [HttpPost]
        public IActionResult GetDisplayUpdateItemsDiscount(Dictionary<string, object> Param)
        {
            try
            {
                DataTable dTableUpdateItemDiscount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dTableUpdateItemDiscount"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dtResult = _dalDaVinci.GetDisplayUpdateItemsDiscount(dTableUpdateItemDiscount, UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayUpdateItemsDiscount.");
            }
            return NotFound();
        }

        [Route("GetDisplayUpdateSoldItemsDiscount")]
        [HttpPost]
        public IActionResult GetDisplayUpdateSoldItemsDiscount(Dictionary<string, object> Param)
        {
            try
            {
                DataTable dTableUpdateItemDiscount = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dTableUpdateItemDiscount"].ToString(), typeof(DataTable));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));

                DataTable dtResult = _dalDaVinci.GetDisplayUpdateSoldItemsDiscount(dTableUpdateItemDiscount, UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayUpdateSoldItemsDiscount.");
            }
            return NotFound();
        }

        [Route("GetItemsBySaleId")]
        [HttpPost]
        public IActionResult GetItemsBySaleId(Dictionary<string, object> Param)
        {
            try
            {
                long SaleId = Int64.Parse(Convert.ToString(Param["SaleId"]));
                DataTable dtResult = _dalDaVinci.GetItemsBySaleId(SaleId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemsBySaleId.");
            }
            return NotFound();
        }

        [Route("GetItemsPricesBySaleId")]
        [HttpGet]
        public IActionResult GetItemsPricesBySaleId(long SaleId)
        {
            try
            {

                DataTable dtResult = _dalDaVinci.GetItemsPricesBySaleId(SaleId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemsPricesBySaleId.");
            }
            return NotFound();
        }

        [Route("UpdateDiscounts")]
        [HttpPost]
        public IActionResult UpdateDiscounts(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dt"].ToString(), typeof(DataTable));
                bool result = _dalDaVinci.UpdateDiscounts(UserId, dt);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateDiscounts.");
            }
            return NotFound();
        }

        [Route("UpdateReciptDiscounts")]
        [HttpPost]
        public IActionResult UpdateReciptDiscounts(Dictionary<string, object> Param)
        {
            try
            {
                long SaleId = Int64.Parse(Convert.ToString(Param["SaleId"]));
                decimal Discount = Convert.ToDecimal(Convert.ToString(Param["Discount"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));

                bool result = _dalDaVinci.UpdateReciptDiscounts(SaleId, Discount, UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - UpdateReciptDiscounts.");
            }
            return NotFound();
        }

        [Route("isEligible")]
        [HttpGet]
        public IActionResult isEligible(long SaleId)
        {
            try
            {

                int result = _dalDaVinci.isEligible(SaleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - isEligible.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region SalesTax
        #region SalesTaxSummary
        [Route("DisplaySalesTaxReportSummaryInPeriod")]
        [HttpPost]
        public IActionResult DisplaySalesTaxReportSummaryInPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));

                DataTable dtResult = _dalDaVinci.DisplaySalesTaxReportSummaryInPeriod(FromDate, ToDate, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesTaxReportSummaryInPeriod.");
            }
            return NotFound();
        }

        [Route("DisplaySalesTaxRevenuesCompanyLocation")]
        [HttpGet]
        public IActionResult DisplaySalesTaxRevenuesCompanyLocation()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplaySalesTaxRevenuesCompanyLocation();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesTaxRevenuesCompanyLocation.");
            }
            return NotFound();
        }

        [Route("DisplaySalesTaxRevenuesInPeriod")]
        [HttpPost]
        public IActionResult DisplaySalesTaxRevenuesInPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));

                DataTable dtResult = _dalDaVinci.DisplaySalesTaxRevenuesInPeriod(FromDate, ToDate, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySalesTaxRevenuesInPeriod.");
            }
            return NotFound();
        }



        #endregion
        #endregion

        #region StockFolder

        #region Checker
        [Route("StockChecker")]
        [HttpPost]
        public IActionResult StockChecker(Dictionary<string, object> Param)
        {
            try
            {

                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable scanqty = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["scanqty"].ToString(), typeof(DataTable));
                DataTable dtResult = _dalDaVinci.StockChecker(CompanyId, scanqty);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - StockChecker.");
            }
            return NotFound();
        }

        [Route("ShuffleChecker")]
        [HttpPost]
        public IActionResult ShuffleChecker(Dictionary<string, object> Param)
        {
            try
            {

                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable dataTable = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dataTable"].ToString(), typeof(DataTable));
                DataTable dtResult = _dalDaVinci.ShuffleChecker(CompanyId, dataTable);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ShuffleChecker.");
            }
            return NotFound();
        }



        [Route("GetTransfersUsingBarcodes")]
        [HttpPost]
        public IActionResult GetTransfersUsingBarcodes(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable Barcodes = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["Barcodes"].ToString(), typeof(DataTable));
                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable dtResult = _dalDaVinci.GetTransfersUsingBarcodes(CompanyId, Barcodes, BD, ED);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetTransfersUsingBarcodes.");
            }
            return NotFound();
        }

        [Route("NewTransferXL")]
        [HttpPost]
        public IActionResult NewTransferXL(Dictionary<string, object> Param)
        {
            try
            {

                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                string Shop = (Convert.ToString(Param["Shop"]));
                DataTable barcodeqty = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["barcodeqty"].ToString(), typeof(DataTable));
                string strResult = _dalDaVinci.NewTransferXL(UserId, CompanyId, Shop, barcodeqty);
                return Ok(strResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - NewTransferXL.");
            }
            return NotFound();
        }



        #endregion

        #region CompanyPromoCurrentStockList
        [Route("DisplayGetCompanyParents")]
        [HttpGet]
        public IActionResult DisplayGetCompanyParents()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayGetCompanyParents();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayGetCompanyParents.");
            }
            return NotFound();
        }
        [Route("DisplayStockGetStockTransferReport")]
        [HttpGet]
        public IActionResult DisplayStockGetStockTransferReport(int CompanyId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayStockGetStockTransferReport(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayStockGetStockTransferReport.");
            }
            return NotFound();
        }

        [Route("DisplayPromoAddNewPromoItems")]
        [HttpPost]
        public IActionResult DisplayPromoAddNewPromoItems(Dictionary<string, object> Param)
        {
            try
            {
                DataTable CompanyPromoItems = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["CompanyPromoItems"].ToString(), typeof(DataTable));
                string Code = (Convert.ToString(Param["Code"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                decimal Discount = Convert.ToDecimal(Convert.ToString(Param["Discount"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                DateTime PromoDate = Convert.ToDateTime(Param["PromoDate"].ToString());

                DataTable dtResult = _dalDaVinci.DisplayPromoAddNewPromoItems(CompanyPromoItems, Code, CompanyId, Discount, CreatedBy, PromoDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayPromoAddNewPromoItems.");
            }
            return NotFound();
        }

        #endregion

        #region DefectInventory
        [Route("DisplayItemDescriptionWithStockItems")]
        [HttpPost]
        public IActionResult DisplayItemDescriptionWithStockItems(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                string Barcode = (Convert.ToString(Param["Barcode"]));

                DataTable dtResult = _dalDaVinci.DisplayItemDescriptionWithStockItems(CompanyId, Barcode);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayItemDescriptionWithStockItems.");
            }
            return NotFound();
        }

        [Route("DisplayAddUpdateStockDefectItems")]
        [HttpPost]
        public IActionResult DisplayAddUpdateStockDefectItems(Dictionary<string, object> Param)
        {
            try
            {
                DataTable ManualDefectReturnItems = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["ManualDefectReturnItems"].ToString(), typeof(DataTable));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataTable dtResult = _dalDaVinci.DisplayAddUpdateStockDefectItems(ManualDefectReturnItems, StockLocationId, CreatedBy, TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateStockDefectItems.");
            }
            return NotFound();
        }

        [Route("DisplayDefectInventoryHistoryPeriod")]
        [HttpPost]
        public IActionResult DisplayDefectInventoryHistoryPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));

                DataSet dsResult = _dalDaVinci.DisplayDefectInventoryHistoryPeriod(FromDate, ToDate, StockLocationId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDefectInventoryHistoryPeriod.");
            }
            return NotFound();
        }

        [Route("DisplayDefectInventoryItems")]
        [HttpGet]
        public IActionResult DisplayDefectInventoryItems()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayDefectInventoryItems();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayDefectInventoryItems.");
            }
            return NotFound();
        }
        [Route("DisplayAllActiveStockLocation")]
        [HttpGet]
        public IActionResult DisplayAllActiveStockLocation()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayAllActiveStockLocation();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllActiveStockLocation.");
            }
            return NotFound();
        }

        [Route("DisplayAllActiveStockLocationAll")]
        [HttpGet]
        public IActionResult DisplayAllActiveStockLocationAll()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayAllActiveStockLocationAll();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllActiveStockLocationAll.");
            }
            return NotFound();
        }

        #endregion

        #region IncompleteTransfer

        [Route("DisplayIncompletedTransfers")]
        [HttpGet]
        public IActionResult DisplayIncompletedTransfers()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayIncompletedTransfers();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayIncompletedTransfers.");
            }
            return NotFound();
        }


        #endregion

        #region ReprintStockTransferReport


        [Route("DisplayStockTransferReport")]
        [HttpGet]
        public IActionResult DisplayStockTransferReport(int StoreTransferId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_DisplayStockTransferReport(StoreTransferId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayStockTransferReport.");
            }
            return NotFound();
        }

        [Route("GetStockTransferCheckList")]
        [HttpPost]
        public IActionResult GetStockTransferCheckList(Dictionary<string, object> Param)
        {
            try
            {

                int StoreTransferId = Int32.Parse(Convert.ToString(Param["StoreTransferId"]));
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                DataTable dtResult = _dalDaVinci.L_GetStockTransferCheckList(StoreTransferId, UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetStockTransferCheckList.");
            }
            return NotFound();
        }

        #endregion

        #region SaleGetSalesByDate
        /// <summary>
        /// there are multiple controller method with this name "GetDisplayShopStockDetails" with different parameter.  
        /// please before use this method verify once.
        /// </summary>
        /// <param name="Dictionary"></param>
        /// <returns></returns>
        [Route("GetDisplayShopStockDetails")]
        [HttpPost]
        public IActionResult GetDisplayShopStockDetails(Dictionary<string, object> Param)
        {
            try
            {

                DateTime BD = Convert.ToDateTime(Param["BD"].ToString());
                DateTime ED = Convert.ToDateTime(Param["ED"].ToString());
                DataTable dtResult = _dalDaVinci.GetDisplayShopStockDetails(BD, ED);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayShopStockDetails.");
            }
            return NotFound();
        }

        [Route("GetSaleForSalesReprintSalesReceipt")]
        [HttpPost]
        public IActionResult GetSaleForSalesReprintSalesReceipt(Dictionary<string, object> Param)
        {
            try
            {

                long SaleId = Int64.Parse(Convert.ToString(Param["SaleId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DataSet dsResult = _dalDaVinci.GetSaleForSalesReprintSalesReceipt(SaleId, TillId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetSaleForSalesReprintSalesReceipt.");
            }
            return NotFound();
        }


        #endregion

        #region Section

        [Route("DisplayAddUpdateSectionDetails")]
        [HttpPost]
        public IActionResult DisplayAddUpdateSectionDetails(Dictionary<string, object> Param)
        {
            try
            {
                string SectionNumber = (Convert.ToString(Param["SectionNumber"]));
                string SectionName = (Convert.ToString(Param["SectionName"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                string Remarks = (Convert.ToString(Param["Remarks"]));
                DataTable dtResult = _dalDaVinci.DisplayAddUpdateSectionDetails(SectionNumber, SectionName, CreatedBy, Remarks);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAddUpdateSectionDetails.");
            }
            return NotFound();
        }


        [Route("DisplayInventoryDeleteBySection")]
        [HttpPost]
        public IActionResult DisplayInventoryDeleteBySection(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                int SectionId = Int32.Parse(Convert.ToString(Param["SectionId"]));
                int InventoryId = Int32.Parse(Convert.ToString(Param["InventoryId"]));
                DataTable dtResult = _dalDaVinci.DisplayInventoryDeleteBySection(UserId, SectionId, InventoryId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayInventoryDeleteBySection.");
            }
            return NotFound();
        }

        #endregion

        #region ShopStock

        /// <summary>
        /// there are multiple controller method with this name "GetDisplayShopStockDetails" with different parameter.  
        /// please before use this method verify once.
        /// </summary>
        /// <param name="TillId"></param>
        /// <returns></returns>
        [Route("GetDisplayShopStockDetails")]
        [HttpGet]
        public IActionResult GetDisplayShopStockDetails(int TillId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayShopStockDetails(TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayShopStockDetails.");
            }
            return NotFound();
        }


        #endregion

        #region StockCurrentStockDetails
        [Route("GetDisplayItemIdWithItembarcode")]
        [HttpGet]
        public IActionResult GetDisplayItemIdWithItembarcode(string Barcode)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayItemIdWithItembarcode(Barcode);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayItemIdWithItembarcode.");
            }
            return NotFound();
        }

        [Route("GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId")]
        [HttpGet]
        public IActionResult GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId(int ItemId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId(ItemId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId.");
            }
            return NotFound();
        }



        #endregion

        #region StockSoldItemsForCompanyAndLocationId
        [Route("GetDisplayItemIdWithItembarcode")]
        [HttpPost]
        public IActionResult GetDisplayItemIdWithItembarcode(Dictionary<string, object> Param)
        {
            try
            {
                string CompanyCode = (Convert.ToString(Param["CompanyCode"]));
                int CompanyParentId = Int32.Parse(Convert.ToString(Param["CompanyParentId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataTable dtResult = _dalDaVinci.GetDisplayItemIdWithItembarcode(CompanyCode, CompanyParentId, FromDate, ToDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayItemIdWithItembarcode.");
            }
            return NotFound();
        }


        #endregion

        #region StockTransfer

        #region StockSoldItemsForCompanyAndLocationId
        [Route("GetAddUpdateReceiveItemsAndStockTransaction")]
        [HttpPost]
        public IActionResult GetAddUpdateReceiveItemsAndStockTransaction(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                string TransferNumber = (Convert.ToString(Param["TransferNumber"]));
                string Batchcode = (Convert.ToString(Param["Batchcode"]));
                decimal RequestedQuantity = Convert.ToDecimal(Convert.ToString(Param["RequestedQuantity"]));
                DataTable dtResult = _dalDaVinci.L_GetAddUpdateReceiveItemsAndStockTransaction(TillId, TransactedBy, TransferNumber, Batchcode, RequestedQuantity);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddUpdateReceiveItemsAndStockTransaction.");
            }
            return NotFound();
        }

        [Route("GetTransferByTransferNo")]
        [HttpPost]
        public IActionResult GetTransferByTransferNo(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                string TransferNumber = (Convert.ToString(Param["TransferNumber"]));
                DataTable dtResult = _dalDaVinci.L_GetTransferByTransferNo(StockLocationId, TransferNumber);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetTransferByTransferNo.");
            }
            return NotFound();
        }



        [Route("ReceiveBadStockTransfer")]
        [HttpPost]
        public IActionResult ReceiveBadStockTransfer(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                string TransferNumber = (Convert.ToString(Param["TransferNumber"]));
                string Batchcode = (Convert.ToString(Param["Batchcode"]));
                var result = _dalDaVinci.L_ReceiveBadStockTransfer(StockLocationId, TransactedBy, TransferNumber, Batchcode);
                return Ok(result);
            }
            catch (Exception ex)
            {

                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ReceiveBadStockTransfer.");
            }
            return NotFound();

        }



        [Route("GetBadStockTransferByTransferNo")]
        [HttpPost]
        public IActionResult GetBadStockTransferByTransferNo(Dictionary<string, object> Param)
        {
            try
            {
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                string TransferNumber = (Convert.ToString(Param["TransferNumber"]));
                DataTable dtResult = _dalDaVinci.L_GetBadStockTransferByTransferNo(StockLocationId, TransferNumber);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetBadStockTransferByTransferNo.");
            }
            return NotFound();
        }

        /// <summary>
        /// there are multiple   controller method with this name "GetStockLocations"  with different parameter.
        ///so  here  avoid conflict issue  name changed "GetStockLocations" to "L_GetStockLocations"
        /// please before use this method verify once 
        /// </summary>
        /// <param name="StockLocationId"></param>
        /// <returns></returns>
        [Route("L_GetStockLocations")]
        [HttpGet]
        public IActionResult L_GetStockLocations(int StockLocationId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.L_GetStockLocations(StockLocationId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetStockLocations.");
            }
            return NotFound();
        }

        [Route("GetItemByBarcode4Transfer")]
        [HttpPost]
        public IActionResult GetItemByBarcode4Transfer(Dictionary<string, object> Param)
        {
            try
            {
                string Batchcode = (Convert.ToString(Param["Batchcode"]));
                int StockLocationId = Int32.Parse(Convert.ToString(Param["StockLocationId"]));
                decimal TotalQuantity = Convert.ToDecimal(Convert.ToString(Param["TotalQuantity"]));
                decimal RequestQuantity = Convert.ToDecimal(Convert.ToString(Param["RequestQuantity"]));
                DataTable dtResult = _dalDaVinci.L_GetItemByBarcode4Transfer(Batchcode, StockLocationId, TotalQuantity, RequestQuantity);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetItemByBarcode4Transfer.");
            }
            return NotFound();
        }

        [Route("CreateStockTransfer")]
        [HttpPost]
        public IActionResult CreateStockTransfer(Dictionary<string, object> Param)
        {
            try
            {
                DataTable StockTransferItemsDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["StockTransferItemsDetails"].ToString(), typeof(DataTable));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                int FromStockLocId = Int32.Parse(Convert.ToString(Param["FromStockLocId"]));
                int ToStockLocId = Int32.Parse(Convert.ToString(Param["ToStockLocId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                int NoOfBoxes = Int32.Parse(Convert.ToString(Param["NoOfBoxes"]));

                DataTable dtResult = _dalDaVinci.L_CreateStockTransfer(StockTransferItemsDetails, TransactedBy, FromStockLocId, ToStockLocId, TillId, Comment, NoOfBoxes);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreateStockTransfer.");
            }
            return NotFound();
        }

        [Route("CreateTransferXLsheet")]
        [HttpPost]
        public IActionResult CreateTransferXLsheet(Dictionary<string, object> Param)
        {
            try
            {
                DataTable ExcelTable = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["ExcelTable"].ToString(), typeof(DataTable));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                int FromStockLocId = Int32.Parse(Convert.ToString(Param["FromStockLocId"]));
                int ToStockLocId = Int32.Parse(Convert.ToString(Param["ToStockLocId"]));
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                string Comment = (Convert.ToString(Param["Comment"]));
                int noofbox = Int32.Parse(Convert.ToString(Param["noofbox"]));
                DataTable dtResult = _dalDaVinci.CreateTransferXLsheet(ExcelTable, TransactedBy, FromStockLocId, ToStockLocId, TillId, Comment, noofbox);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreateTransferXLsheet.");
            }
            return NotFound();
        }
        #endregion

        #region StockTransferCancel

        [Route("GetPendingTransfers")]
        [HttpGet]
        public IActionResult GetPendingTransfers(int CompanyId)
        {
            try
            {
                DataSet dsResult = _dalDaVinci.L_GetPendingTransfers(CompanyId);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetPendingTransfers.");
            }
            return NotFound();
        }

        [Route("CancelStockTransfer")]
        [HttpPost]
        public IActionResult CancelStockTransfer(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int StoreTransferId = Int32.Parse(Convert.ToString(Param["StoreTransferId"]));
                int TransactedBy = Int32.Parse(Convert.ToString(Param["TransactedBy"]));
                DataTable dtResult = _dalDaVinci.L_CancelStockTransfer(CompanyId, StoreTransferId, TransactedBy);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_CancelStockTransfer.");
            }
            return NotFound();
        }


        [Route("DefectStockTransfer")]
        [HttpGet]
        public IActionResult DefectStockTransfer(string StockTransferBarcode)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_DefectStockTransfer(StockTransferBarcode);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_DefectStockTransfer.");
            }
            return NotFound();
        }


        #endregion

        #region StockTransferHistory
        [Route("GetDisplayCompanyWithLocationAndAddress")]
        [HttpPost]
        public IActionResult GetDisplayCompanyWithLocationAndAddress(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet dsResult = _dalDaVinci.GetDisplayCompanyWithLocationAndAddress(TillId, FromDate, ToDate);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyWithLocationAndAddress.");
            }
            return NotFound();
        }


        [Route("GetDisplayStockTransferHistoryDetails")]
        [HttpGet]
        public IActionResult GetDisplayStockTransferHistoryDetails(int StoreTransferId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayStockTransferHistoryDetails(StoreTransferId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayStockTransferHistoryDetails.");
            }
            return NotFound();
        }

        #endregion

        #region StockTransferReport
        [Route("GetTransfersByDate")]
        [HttpPost]
        public IActionResult GetTransfersByDate(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet dsResult = _dalDaVinci.L_GetTransfersByDate(FromDate, ToDate);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetTransfersByDate.");
            }
            return NotFound();
        }


        [Route("GetAllOpenTransfers")]
        [HttpGet]
        public IActionResult GetAllOpenTransfers()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetAllOpenTransfers();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllOpenTransfers.");
            }
            return NotFound();
        }

        [Route("GetAllCancelledTransfers")]
        [HttpGet]
        public IActionResult GetAllCancelledTransfers()
        {
            try
            {
                DataSet dsResult = _dalDaVinci.GetAllCancelledTransfers();
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllCancelledTransfers.");
            }
            return NotFound();
        }

        [Route("AddItemsIntoTransfer")]
        [HttpPost]
        public EmptyResult AddItemsIntoTransfer(Dictionary<string, object> Param)
        {
            try
            {
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["dt"].ToString(), typeof(DataTable));
                int _transferid = Int32.Parse(Convert.ToString(Param["_transferid"]));
                _dalDaVinci.AddItemsIntoTransfer(dt, _transferid);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddItemsIntoTransfer.");
            }
            return new EmptyResult();
        }


        #endregion

        #region  StockTransferRequestsHistory
        [Route("GetDisplayAllCompanyDetailsForReport")]
        [HttpPost]
        public IActionResult GetDisplayAllCompanyDetailsForReport(Dictionary<string, object> Param)
        {
            try
            {
                int TillId = Int32.Parse(Convert.ToString(Param["TillId"]));
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                DataSet dsResult = _dalDaVinci.L_GetDisplayAllCompanyDetailsForReport(TillId, FromDate, ToDate);
                return Ok(dsResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayAllCompanyDetailsForReport.");
            }
            return NotFound();
        }

        [Route("GetDisplayCancelStockTransferRequests")]
        [HttpPost]
        public IActionResult GetDisplayCancelStockTransferRequests(Dictionary<string, object> Param)
        {
            try
            {
                int TransferRequestsId = Int32.Parse(Convert.ToString(Param["TransferRequestsId"]));
                int CancelledBy = Int32.Parse(Convert.ToString(Param["CancelledBy"]));
                string CancelComments = (Convert.ToString(Param["CancelComments"]));
                DataTable dtResult = _dalDaVinci.L_GetDisplayCancelStockTransferRequests(TransferRequestsId, CancelledBy, CancelComments);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetDisplayCancelStockTransferRequests.");
            }
            return NotFound();
        }

        #endregion

        #region TransferRequest
        [Route("GetDisplayCompanyStockItemsDetailsWithCompanyId")]
        [HttpGet]
        public IActionResult GetDisplayCompanyStockItemsDetailsWithCompanyId(int CompanyId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetDisplayCompanyStockItemsDetailsWithCompanyId(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetDisplayCompanyStockItemsDetailsWithCompanyId.");
            }
            return NotFound();
        }

        [Route("GetAddTransferRequestItemsDetailsWithCompanyId")]
        [HttpPost]
        public IActionResult GetAddTransferRequestItemsDetailsWithCompanyId(Dictionary<string, object> Param)
        {
            try
            {
                DataTable TransferRequestItemsDetails = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["TransferRequestItemsDetails"].ToString(), typeof(DataTable));
                int FromCompanyId = Int32.Parse(Convert.ToString(Param["FromCompanyId"]));
                int ToCompanyId = Int32.Parse(Convert.ToString(Param["ToCompanyId"]));
                int CreatedBy = Int32.Parse(Convert.ToString(Param["CreatedBy"]));
                string Comments = (Convert.ToString(Param["Comments"]));
                DataTable dtResult = _dalDaVinci.L_GetAddTransferRequestItemsDetailsWithCompanyId(TransferRequestItemsDetails, FromCompanyId, ToCompanyId, CreatedBy, Comments);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAddTransferRequestItemsDetailsWithCompanyId.");
            }
            return NotFound();
        }

        #endregion

        #region TransfersBarcodeByStoreTransfer
        /// <summary>
        /// there are multiple controller method with this name "GetDisplayShopStockDetails"  with different parameter.
        ///so avoid conflict issue name changed "GetDisplayShopStockDetails" to "L_GetDisplayShopStockDetails"
        /// please before use this method verify once 
        /// </summary>
        /// <param name="StoreTransferId"></param>
        /// <returns></returns>
        [Route("L_GetDisplayShopStockDetails")]
        [HttpGet]
        public IActionResult L_GetDisplayShopStockDetails(int StoreTransferId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetDisplayShopStockDetails(StoreTransferId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetDisplayShopStockDetails.");
            }
            return NotFound();
        }


        #endregion

        #region  WriteOff

        [Route("GetDisplayCompanyWithAddress")]
        [HttpGet]
        public IActionResult GetDisplayCompanyWithAddress(int TillId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayCompanyWithAddress(TillId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayCompanyWithAddress.");
            }
            return NotFound();
        }

        [Route("GetDisplayInventoryDateByHO")]
        [HttpGet]
        public IActionResult GetDisplayInventoryDateByHO(int CompanyId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetDisplayInventoryDateByHO(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayInventoryDateByHO.");
            }
            return NotFound();
        }

        [Route("GetDisplayInventoryForWritesOffItemsDetails")]
        [HttpPost]
        public IActionResult GetDisplayInventoryForWritesOffItemsDetails(Dictionary<string, object> Param)
        {
            try
            {

                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime InventoryDate = Convert.ToDateTime(Param["InventoryDate"].ToString());

                DataTable dtResult = _dalDaVinci.GetDisplayInventoryForWritesOffItemsDetails(CompanyId, InventoryDate);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetDisplayInventoryForWritesOffItemsDetails.");
            }
            return NotFound();
        }

        [Route("GetWriteoffUpdateAndStockTransaction")]
        [HttpPost]
        public IActionResult GetWriteoffUpdateAndStockTransaction(Dictionary<string, object> Param)
        {
            try
            {

                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                int ItemId = Int32.Parse(Convert.ToString(Param["ItemId"]));
                int EnteredBy = Int32.Parse(Convert.ToString(Param["EnteredBy"]));
                decimal StockQuantity = Convert.ToDecimal(Convert.ToString(Param["StockQuantity"]));
                int PurchaseItemId = Int32.Parse(Convert.ToString(Param["PurchaseItemId"]));
                string Batch = (Convert.ToString(Param["Batch"]));
                decimal CurrentStockQty = Convert.ToDecimal(Convert.ToString(Param["CurrentStockQty"]));
                decimal CountedQty = Convert.ToDecimal(Convert.ToString(Param["CountedQty"]));
                DateTime InventoryDate = Convert.ToDateTime(Param["InventoryDate"].ToString());

                int result = _dalDaVinci.GetWriteoffUpdateAndStockTransaction(CompanyId, ItemId, EnteredBy, StockQuantity, PurchaseItemId, Batch, CurrentStockQty, CountedQty);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetWriteoffUpdateAndStockTransaction.");
            }
            return NotFound();
        }

        #endregion

        #endregion


        #endregion

        #region SupplierPurchase Folder
        #region SupplierPurchase

        [Route("DisplayPurchaseHistoryWithPeriod")]
        [HttpPost]
        public IActionResult DisplayPurchaseHistoryWithPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int SupplierId = Int32.Parse(Convert.ToString(Param["SupplierId"]));
                DataTable dtResult = _dalDaVinci.DisplayPurchaseHistoryWithPeriod(FromDate, ToDate, SupplierId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayPurchaseHistoryWithPeriod.");
            }
            return NotFound();
        }

        [Route("DisplaySaleAndReceiptFromCustomerWithPeriod")]
        [HttpPost]
        public IActionResult DisplaySaleAndReceiptFromCustomerWithPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable dtResult = _dalDaVinci.DisplaySaleAndReceiptFromCustomerWithPeriod(FromDate, ToDate, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySaleAndReceiptFromCustomerWithPeriod.");
            }
            return NotFound();
        }

        [Route("DisplayReturnAndReceiptFromCustomerWithPeriod")]
        [HttpPost]
        public IActionResult DisplayReturnAndReceiptFromCustomerWithPeriod(Dictionary<string, object> Param)
        {
            try
            {
                DateTime FromDate = Convert.ToDateTime(Param["FromDate"].ToString());
                DateTime ToDate = Convert.ToDateTime(Param["ToDate"].ToString());
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DataTable dtResult = _dalDaVinci.DisplayReturnAndReceiptFromCustomerWithPeriod(FromDate, ToDate, CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayReturnAndReceiptFromCustomerWithPeriod.");
            }
            return NotFound();
        }


        [Route("DisplayAllActiveSupplier")]
        [HttpGet]
        public IActionResult DisplayAllActiveSupplier()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplayAllActiveSupplier();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplayAllActiveSupplier.");
            }
            return NotFound();
        }

        [Route("DisplaySaleReceiptCompanyDetails")]
        [HttpGet]
        public IActionResult DisplaySaleReceiptCompanyDetails()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.DisplaySaleReceiptCompanyDetails();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - DisplaySaleReceiptCompanyDetails.");
            }
            return NotFound();
        }

        #endregion
        #endregion

        #region UserSecurity


        #region Log
        [Route("In")]
        [HttpPost]
        public IActionResult In(Dictionary<string, object> Param)
        {
            try
            {
                string Login = (Convert.ToString(Param["Login"]));
                string Pwd = (Convert.ToString(Param["Pwd"]));
                bool UseUserId4Login = Convert.ToBoolean(Param["UseUserId4Login"].ToString());              
                string Msg = string.Empty;
                int Rv = -1;
                LoginResponse loginResponse = new LoginResponse();
                loginResponse.isLogin = _dalDaVinci.L_In(Login, Pwd, UseUserId4Login, out Msg, out Rv);
                loginResponse.Rv = Rv;
                loginResponse.Msg = Msg;
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_In.");
            }
            return NotFound();
        }

        [Route("GetUserCredentials")]
        [HttpPost]
        public IActionResult GetUserCredentials(Dictionary<string, object> Param)
        {
            try
            {
                string Login = (Convert.ToString(Param["Login"]));
                bool UseUserId4Login = Convert.ToBoolean(Param["UseUserId4Login"].ToString());
                string FullName = "";
                int ContactId = 0;
                int UserID = 0;
                string UserName = "";
                bool IsActive = false;
                bool IsUserMale = false;

                _dalDaVinci.L_GetUserCredentials(Login, UseUserId4Login, out FullName, out ContactId, out UserID, out UserName, out IsActive, out IsUserMale);
                GetUserCredentials getUserCredentials = new GetUserCredentials();
                getUserCredentials.FullName = FullName;
                getUserCredentials.ContactId = ContactId;
                getUserCredentials.UserID = UserID;
                getUserCredentials.UserName = UserName;
                getUserCredentials.IsActive = IsActive;
                getUserCredentials.IsUserMale = IsUserMale;
                return Ok(getUserCredentials);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetUserCredentials.");
            }
            return NotFound();
        }

        #endregion

        #region UserRight

        [Route("GetUiObjects")]
        [HttpGet]
        public IActionResult GetUiObjects()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetUiObjects();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUiObjects.");
            }
            return NotFound();
        }


        [Route("GetUiObjectsByMainId")]
        [HttpGet]
        public IActionResult GetUiObjectsByMainId(int MainObjectId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetUiObjectsByMainId(MainObjectId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUiObjectsByMainId.");
            }
            return NotFound();
        }

        [Route("AddUpdateUiObject")]
        [HttpPost]
        public IActionResult AddUpdateUiObject(Dictionary<string, object> Param)
        {
            try
            {
                int ObjectId = Int32.Parse(Convert.ToString(Param["ObjectId"]));
                int ObjectMainId = Int32.Parse(Convert.ToString(Param["ObjectMainId"]));
                string ObjectName = (Convert.ToString(Param["ObjectName"]));
                string ObjectText = (Convert.ToString(Param["ObjectText"]));
                int result = _dalDaVinci.L_AddUpdateUiObject(ObjectId, ObjectMainId, ObjectName, ObjectText);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddUpdateUiObject.");
            }
            return NotFound();
        }

        [Route("AddUser")]
        [HttpPost]
        public IActionResult AddUser(Dictionary<string, object> Param)
        {
            try
            {
                int UserID = Int32.Parse(Convert.ToString(Param["UserID"]));
                string UserName = (Convert.ToString(Param["UserName"]));
                string Description = (Convert.ToString(Param["Description"]));
                string Password = (Convert.ToString(Param["Password"]));
                int ContactId = Int32.Parse(Convert.ToString(Param["ContactId"]));
                bool IsActive = Convert.ToBoolean(Param["IsActive"].ToString());
                int result = _dalDaVinci.L_AddUser(UserID, UserName, Description, Password, ContactId, IsActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddUser.");
            }
            return NotFound();
        }
        [Route("CatUiObjects")]
        [HttpPost]
        public EmptyResult CatUiObjects(Dictionary<string, object> Param)
        {
            try
            {

                int MainId = Int32.Parse(Convert.ToString(Param["MainId"]));
                List<UserPrivilege> Ids = (List<UserPrivilege>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["Ids"].ToString(), typeof(List<UserPrivilege>));
                _dalDaVinci.L_CatUiObjects(MainId, Ids);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CatUiObjects.");
            }
            return new EmptyResult();
        }

        [Route("RemoveUiObject")]
        [HttpGet]
        public IActionResult RemoveUiObject(int ObjectId)
        {
            try
            {
                bool result = _dalDaVinci.L_RemoveUiObject(ObjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - RemoveUiObject.");
            }
            return NotFound();
        }

        [Route("RemoveSubFromMainObject")]
        [HttpGet]
        public IActionResult RemoveSubFromMainObject(int ObjectId)
        {
            try
            {
                bool result = _dalDaVinci.L_RemoveSubFromMainObject(ObjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - RemoveSubFromMainObject.");
            }
            return NotFound();
        }

        [Route("GetAllUsers")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAllUsers();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllUsers.");
            }
            return NotFound();
        }


        [Route("GetEmployees")]
        [HttpGet]
        public IActionResult GetEmployees()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetEmployees();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetEmployees.");
            }
            return NotFound();
        }

        [Route("GetAllActiveRoles")]
        [HttpGet]
        public IActionResult GetAllActiveRoles()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAllActiveRoles();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllActiveRoles.");
            }
            return NotFound();
        }

        [Route("GetAllRoles")]
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAllRoles();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllRoles.");
            }
            return NotFound();
        }


        [Route("GetUiObjectsInHierarchyOrder")]
        [HttpGet]
        public IActionResult GetUiObjectsInHierarchyOrder()
        {
            try
            {
                List<UserPrivilege> _listResult = _dalDaVinci.L_GetUiObjectsInHierarchyOrder();
                return Ok(_listResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUiObjectsInHierarchyOrder.");
            }
            return NotFound();
        }


        [Route("GetPrincipalRights")]
        [HttpGet]
        public IActionResult GetPrincipalRights(int PrincipalId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetPrincipalRights(PrincipalId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetPrincipalRights.");
            }
            return NotFound();
        }

        [Route("ChangeUserRights")]
        [HttpPost]
        public IActionResult ChangeUserRights(Dictionary<string, object> Param)
        {
            try
            {
                List<UserPrivilege> EmptyLs = (List<UserPrivilege>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["EmptyLs"].ToString(), typeof(List<UserPrivilege>));
                List<UserPrivilege> GrantLs = (List<UserPrivilege>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["GrantLs"].ToString(), typeof(List<UserPrivilege>));
                List<UserPrivilege> DenyLs = (List<UserPrivilege>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["DenyLs"].ToString(), typeof(List<UserPrivilege>));
                bool IsUser = Convert.ToBoolean(Param["IsUser"].ToString());
                bool result = _dalDaVinci.L_ChangeUserRights(EmptyLs, GrantLs, DenyLs, IsUser);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ChangeUserRights.");
            }
            return NotFound();
        }

        [Route("AutoAddUiObjects")]
        [HttpPost]
        public IActionResult AutoAddUiObjects(DataTable UiObjects)
        {
            try
            {
                bool result = _dalDaVinci.L_AutoAddUiObjects(UiObjects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AutoAddUiObjects.");
            }
            return NotFound();
        }

        [Route("GetAllUsersObjects")]
        [HttpGet]
        public IActionResult GetAllUsersObjects(int UserId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAllUsersObjects(UserId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetAllUsersObjects.");
            }
            return NotFound();
        }

        [Route("AddRole")]
        [HttpPost]
        public IActionResult AddRole(Dictionary<string, object> Param)
        {
            try
            {
                int PrincipalId = Int32.Parse(Convert.ToString(Param["PrincipalId"]));
                string Description = (Convert.ToString(Param["Description"]));
                bool IsActive = Convert.ToBoolean(Param["IsActive"].ToString());
                int result = _dalDaVinci.L_AddRole(PrincipalId, Description, IsActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddRole.");
            }
            return NotFound();
        }

        [Route("GetUsersInRole")]
        [HttpGet]
        public IActionResult GetUsersInRole(int RoleId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetUsersInRole(RoleId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetUsersInRole.");
            }
            return NotFound();
        }

        [Route("L_GetAllActiveUsers")]
        [HttpGet]
        public IActionResult L_GetAllActiveUsers()
        {
            try
            {
                DataTable dtResult = _dalDaVinci.L_GetAllActiveUsers();
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - L_GetAllActiveUsers.");
            }
            return NotFound();
        }

        [Route("AddUserToRole")]
        [HttpPost]
        public EmptyResult AddUserToRole(Dictionary<string, object> Param)
        {
            try
            {
                List<int> pl = (List<int>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["pl"].ToString(), typeof(List<int>));
                int RoleId = Int32.Parse(Convert.ToString(Param["RoleId"]));
                _dalDaVinci.L_AddUserToRole(pl, RoleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddUserToRole.");
            }
            return new EmptyResult();
        }
        [Route("RemoveUserFromRole")]
        [HttpPost]
        public EmptyResult RemoveUserFromRole(Dictionary<string, object> Param)
        {
            try
            {
                List<int> pl = (List<int>)Newtonsoft.Json.JsonConvert.DeserializeObject(Param["pl"].ToString(), typeof(List<int>));
                int RoleId = Int32.Parse(Convert.ToString(Param["RoleId"]));
                _dalDaVinci.L_RemoveUserFromRole(pl, RoleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - RemoveUserFromRole.");
            }
            return new EmptyResult();
        }

        [Route("ChangeUserPassword")]
        [HttpPost]
        public IActionResult ChangeUserPassword(Dictionary<string, object> Param)
        {
            try
            {
                int UserId = Int32.Parse(Convert.ToString(Param["UserId"]));
                string OldPwd = (Convert.ToString(Param["OldPwd"]));
                string NewPwd = (Convert.ToString(Param["NewPwd"]));
                bool result = _dalDaVinci.L_ChangeUserPassword(UserId, OldPwd, NewPwd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - ChangeUserPassword.");
            }
            return NotFound();
        }

        [Route("PrincipleUpdateCopyUserRights")]
        [HttpPost]
        public IActionResult PrincipleUpdateCopyUserRights(Dictionary<string, object> Param)
        {
            try
            {
                int FromPrincipleId = Int32.Parse(Convert.ToString(Param["FromPrincipleId"]));
                int ToPrincipleId = Int32.Parse(Convert.ToString(Param["ToPrincipleId"]));
                DataTable dtResult = _dalDaVinci.PrincipleUpdateCopyUserRights(FromPrincipleId, ToPrincipleId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - PrincipleUpdateCopyUserRights.");
            }
            return NotFound();
        }

        #endregion

        #endregion

       

        #region ConnectioString

        [Route("CreateSqlConnectionString")]
        [HttpPost]
        public IActionResult CreateSqlConnectionString(Dictionary<string, object> Param)
        {
            try
            {
                string Server = (Convert.ToString(Param["Server"]));
                string Database = (Convert.ToString(Param["Database"]));
                string UserID = (Convert.ToString(Param["UserID"]));
                string Password = (Convert.ToString(Param["Password"]));
                string strResult = _dalDaVinci.CreateSqlConnectionString(Server,  Database,  UserID,  Password);
                return Ok(strResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - CreateSqlConnectionString.");
            }
            return NotFound();
        }

        //[Route("CreateSqlConnectionString")]
        //[HttpGet]
        //public EmptyResult CreateSqlConnectionString(int CountryId)
        //{
        //    try
        //    {
               
        //        _dalDaVinci.CreateSqlConnectionString(CountryId);
                
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.ToString());
        //        Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - PrincipleUpdateCopyUserRights.");
        //    }
        //    return new EmptyResult();
        //}


        #endregion

        #region New_Added
        //new added
        [Route("GetCashbackVoucherDetails")]
        [HttpPost]
        public IActionResult GetCashbackVoucherDetails(Dictionary<string, object> Param)
        {
            try
            {
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                DateTime fromDate = Convert.ToDateTime(Param["fromDate"].ToString());
                DateTime toDate = Convert.ToDateTime(Param["toDate"].ToString());             
                DataTable dTableCashbackDetails = _dalDaVinci.GetCashbackVoucherDetails(CompanyId, fromDate, toDate);
                return Ok(dTableCashbackDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCashbackVoucherDetails.");
            }
            return NotFound();
        }

        [Route("GetCashbackDetails")]
        [HttpGet]
        public IActionResult GetCashbackDetails(int CompanyId)
        {
            try
            {
                DataTable dtResult = _dalDaVinci.GetCashbackDetails(CompanyId);
                return Ok(dtResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCashbackDetails.");
            }
            return NotFound();
        }


        [Route("AddUpdateCashbackAmount")]
        [HttpPost]
        public IActionResult AddUpdateCashbackAmount(Dictionary<string, object> Param)
        {
            try
            {

                int CashbackAmountId = Int32.Parse(Convert.ToString(Param["CashbackAmountId"]));
                int CompanyId = Int32.Parse(Convert.ToString(Param["CompanyId"]));
                decimal CashbackBracketsUpTo = Convert.ToDecimal(Convert.ToString(Param["CashbackBracketsUpTo"]));
                decimal CashbackAmount = Convert.ToDecimal(Convert.ToString(Param["CashbackAmount"]));
                bool IsEnabled = Convert.ToBoolean(Convert.ToString(Param["IsEnabled"]));
                int UpdatedBy = Int32.Parse(Convert.ToString(Param["UpdatedBy"]));

                DataSet dsAddUpdateCashbackAmount = _dalDaVinci.AddUpdateCashbackAmount(CashbackAmountId, CompanyId, CashbackBracketsUpTo, CashbackAmount, IsEnabled, UpdatedBy);
                return Ok(dsAddUpdateCashbackAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddUpdateCashbackAmount.");
            }
            return NotFound();
        }


        [Route("AddLocationEntry")]
        [HttpPost]
        public IActionResult AddLocationEntry(Dictionary<string, object> Param)
        {
            try
            {

                int companyId = Int32.Parse(Convert.ToString(Param["companyId"]));
                string locationCode = (Convert.ToString(Param["locationCode"]));
                string locationName = (Convert.ToString(Param["locationName"]));
                string address = (Convert.ToString(Param["address"]));
                string fullName = (Convert.ToString(Param["fullName"]));
                string phone = (Convert.ToString(Param["phone"]));
                string moblie = (Convert.ToString(Param["moblie"]));
                string fax = (Convert.ToString(Param["fax"]));
                string email = (Convert.ToString(Param["email"]));
                string website = (Convert.ToString(Param["website"]));
                string note = (Convert.ToString(Param["note"]));
                int countryId = Int32.Parse(Convert.ToString(Param["countryId"]));
                bool isMainWarehouse = Convert.ToBoolean(Convert.ToString(Param["isMainWarehouse"]));
                bool isMainBank = Convert.ToBoolean(Convert.ToString(Param["isMainBank"]));
                int createdBy = Int32.Parse(Convert.ToString(Param["createdBy"]));
                bool isOutlet = Convert.ToBoolean(Convert.ToString(Param["isOutlet"]));

               

                DataSet dsAddLocationEntry = _dalDaVinci.AddLocationEntry(companyId, locationCode, locationName, address, fullName, phone , moblie , fax , email , website , note , countryId , isMainWarehouse , isMainBank , createdBy , isOutlet);
                return Ok(dsAddLocationEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - AddLocationEntry.");
            }
            return NotFound();
        }


        [Route("GetCompanyForlocations")]
        [HttpGet]
        public IActionResult GetCompanyForlocations()
        {
            try
            {
                DataTable dtGetCompanyForlocations = _dalDaVinci.GetCompanyForlocations();
                return Ok(dtGetCompanyForlocations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - GetCompanyForlocations.");
            }
            return NotFound();
        }



        #endregion





        [Route("CreateSqlConnectionString")]
        [HttpGet]
        public IActionResult CreateSqlConnectionString(int CountryId)
        {
            int countryId = CountryId;
            try
            {
                if (countryId == 1)
                {
                    connStr = AppSettings.DaVinciConnectionString;
                }
                else if (countryId == 2)
                {
                    connStr = AppSettings.DaVinciConnectionStringForDW;
                }
                // connStr = _dalDaVinci.CreateSqlConnectionString(CountryId);
                return Ok(connStr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - PrincipleUpdateCopyUserRights.");
            }
            return new EmptyResult();
        }

        [Route("CheckDbConnection")]
        [HttpGet]
        public bool CheckDbConnection()
        {
            connStr = AppSettings.DaVinciConnectionString;


            try
            {
                using (var connection = new SqlConnection(connStr))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                Utility.WriteToFile(ex, "ERROR: DaVinciAdminApi - Error in DB connection on CheckDBConnection.");
                return false; // any error is considered as db connection error for now
            }
        }
        

    }
}
