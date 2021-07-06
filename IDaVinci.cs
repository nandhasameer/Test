using DaVinci.Models.DaVinciAdmin;
using System;
using System.Collections.Generic;
using System.Data;


namespace DaVinciAdminApi.Repositories.Interface
{
    public interface IDaVinci
    {
        #region AppData Folder
        #region AppData
        void L_GetCompanyInfo(int TillId, out int CompanyId, out int CompanyContactId, out string CompanyName, out byte[] Logo, out byte[] Icon, out int IsActive, out string LocalCurrencyCode);
        DataTable L_GetAssistants();
        public Dictionary<string, string> L_GetParameters();
        public DataTable GetAllActiveTills();
        bool UpdateTillImages(int userid, int tillid, int imgid, string imagename, string imagetype, decimal sizekb, string pixels, byte[] logo, byte[] ico, byte[] imgbutton);
        DataTable UpdatePIDNumber(int userId, int tillid, string pID);
        #endregion

        #region VersionType
        DataTable GetDisplayAllVersionWithVersionType(int UpdateVersionTypeId);
        DataTable GetDisplayAllVersionDetailsWithUpdateVersionId(int UpdateVersionId);
        #endregion

        #endregion

        #region AppSettingsFolder
        #region Skins
        string L_GetUserSkin(int UserId);
        bool L_AddUserSkin(int UserId, string SkinName);
        #endregion

        #endregion

        #region BalanceSheet Folder

        #region AccountTransactionHistory
        DataTable GetDisplayAccountTransactionHistoryByDateAndTill(int TillId, DateTime TransactionDateTime);
        #endregion

        #region CompanyDetails
        DataSet GetDisplayCompanyDetails();
        DataSet GetDisplayCompanyDetailsNotAll();
        DataSet GetSummaryReportSalePerformanceAnalysis(DateTime FromDate, DateTime ToDate, int CompanyId);
        bool DisplayAddUpdateBalancesSheet(int TillID, DateTime AccountingDate);
        DataSet DisplayAddUpdateSalesHeadCount(DataTable SalesHeadCount, int LocationId, int CompanyId, int UpdatedBy);
        #endregion

        #region CompanyDetailsBrand
        DataSet DisplayCompanyDetailsForBrand();
        DataSet DisplayLocationDetailsForBrand(int companyParentId);
        DataSet DisplayBrandDetailsForLocation(int companyParentId, int companyId, DateTime fromDate, DateTime toDate);
        DataTable GetStockQtyandPricesCompanyDetails(string BrandId, int CompanyId, DateTime FromDate, DateTime Todate, int ParentCompanyId);
        #endregion

        #region MoneyTransferHistory
        DataSet GetDisplayAllCurrentMoneyTransferDetails(int TillId, DateTime AccountingDate);
        DataSet GetDisplayReportForSelectTransferSpecification(int CashTransferId, int UserId);
        #endregion

        #endregion

        #region CashMgmtFolder

        #region AccountingDate
        // DataTable GetDisplayDefaultAccountTransactionDetails(int TillId);
        DataTable GetCloseCurrentAccountingDate(int TillId);
        DataTable GetOpenNewAccountingDate(int TillId);
        #endregion

        #region  AccountTransaction
        DataSet GetDisplayDefaultAccountTransactionDetails();
        DataTable GetDisplayAccountTransactionHistory(int TillId);
        DataTable GetAddNewAccountTransaction(int TillId, int UserId, decimal CurrencyAmount, string TransactionDesc, int CurrencyId, int AccountId, int TransactionTypesId, int DenominationTypeId);
        #endregion

        #region CashTransfer
        DataTable GetDisplayTillCashTransfers(DateTime FromDate, DateTime ToDate, int UserId);
        DataTable GetDisplayReloadTillCashTransfers(DateTime RunningDate, int UserId);
        #endregion

        #region Currency
        DataTable DisplayActiveCurrency();
        DataTable DisplauUpdateCurrencyIcon(int CurrencyId, byte[] CurrencyIcon);
        #endregion

        #region EditAccountTransaction
        DataTable GetDisplayAccountTransactionHistoryByAccountTransId(long AccountTransId);
        DataTable GetDisplayEditTransactionHistory(long AccountTransId, int AccountId, int DenominationValueTypeId);
        #endregion

        #region EditCashTransferSpeicification
        DataSet GetHODisplayCashTransferForTill(int TillId, DateTime CashTransferDate);
        DataTable GetHODisplayCloseAccountingDate(int TillId);
        DataTable GetHODenominationTotalValueInUSD(int TillId, DateTime AccountingDate);
        // DataTable GetDisplayDenominationValueTypes();
        DataTable GetHODisplayDenominationTypeDetails(int DenominationValueTypeId);
        DataTable GetHODisplayDenominationTypeDetails(int TillId, int DenominationTypeId, DateTime AccountingDate, int CashTransferId);
        DataTable GetHODisplayDenominationTypeDetails4Drawer(int TillId, int DenominationTypeId, DateTime AccountingDate);
        DataTable GetUpdateCashTransfersWithCashSpecification(DataTable CashTransfersWithCashSpecification, int UserId, int CashTransferId,
                                   int TillId, DateTime AccountingDate);
        DataTable GetUpdateCashTransfersWithCashSpecification4Drawer(DataTable CashTransfersWithCashSpecification, int UserId,
                                   int TillId, DateTime AccountingDate);
        DataTable GetHOUpdateAccountTransaction(int TillId, int UserId, decimal CurrencyAmount, string TransactionDesc, int CurrencyId, int AccountId, int TransactionTypesId,
               int DenominationTypeId, long AccountTransId, DateTime AccountingDate, int EditTypeId);
        DataTable GetCurrentCashTransferSpecificationChanged(DateTime FromDate, DateTime ToDate);
        #endregion

        #region ExchangeRate
        //  DataTable GetDisplayCompanyDetails();
        DataTable GetDisplayCompanyExchangeRateForEveryLocation(int TillId);
        DataTable GetDisplayCompanyDetailsWithAll();
        DataTable GetDisplayExchangeRateHistory(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable GetDisplayCompanyExchangeRateDetails(int CompanyId);
        int GetUpdateCompanyExchangeRate(int CompanyExchangeRatesId, int CurrencyId, int CompanyId, int UserId, decimal CurrencyRate);

        #endregion
        #endregion

        #region Common Folder
        #region Country
        DataTable GetCountries();
        #endregion

        #region Locations
        DataTable GetLocations();
        #endregion

        #region MaritalStatus
        DataTable GetMaritalStatuses();
        #endregion
        #endregion

        #region Inventory Folder

        #region Inventory
        int L_CreateNewInventory(int CompanyId, int StockLocationId, int UserId, string Note);
        DataTable L_GetInventoryByLocId(int CompanyId, int StockLocationId);
        DataTable L_GetInventoryItemsById(int InventoryId);
        DataTable L_NewCount(int InventoryId, int UserId, string Barcode, decimal Qty, int SectionId);
        DataTable L_UpdateCountedQty(int InventoryId, int UserId, string Barcode, decimal Qty);
        DataTable UpdateCountedQty_Troubleshooting(int InventoryId, int UserId, int ItemId, decimal Qty);
        void MakeWriteOff(int CompanyId, int StockLocId, string Comment, int UserId, DataTable Items2Writeoff, int InventoryId);
        DataTable L_NewCount_Bulk(int InventoryId, int UserId, DataTable dt);
        #endregion

        #region InventoryWithAddStock
        int L_DisplayJobUpdateInventory(int InventoryId, int IndividualItemId, bool IsLastItem, int SectionId);
        #endregion

        #endregion

        #region Financial Report Folder

        #region DailyCashTransaction

        DataSet GetDisplayAccountCashTransactions(DateTime FromDate, DateTime ToDate, int CurrencyId, int UserId);
        DataSet DisplayGetSalesUsingCompanyDate(int CompanyId, DateTime TransactedOn);
        DataSet DisplayGetTransactionsUsingCompanynDate(int CompanyId, DateTime TransactedOn);
        DataSet DisplayStockDisplayDailyTransferredValues(DateTime FromDate, DateTime ToDate);
        #endregion

        #region StockMovements
        DataSet GetDisplayFinancialStockMovements(DateTime FromDate, DateTime ToDate);
        DataSet GetDisplayFinancialItemFlow(int ItemId);
        DataSet GetDisplayFinancialPurchasedItems(DateTime FromDate, DateTime ToDate);


        #endregion

        #region TransactionPerDay
        DataTable GetDisplayGetPurchasedQtyPerDay(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable GetDisplaySoldQtyPerDay(int StockLocationId, DateTime FromDate, DateTime ToDate);
        #endregion

        #region TransfersBalancesInOut
        DataSet GetDisplayGetTransfersBalances(DateTime FromDate, DateTime ToDate);
        DataTable GetDisplayGetTransfersBalancesReloadDate(DateTime ReloadDate);
        #endregion
        #endregion

        #region Gift Certificate Folder

        #region GiftCertificateCancel
        DataSet GetDisplayCouponAmountOptionsDetails(int TillId, string GiftBarcode);
        DataTable GetCancelCouponDetailsWithReturnAmount(DataTable CouponPaymentDetails, int UserId, decimal ReturnAmount, string CancelComment, int GiftCertificatesDetailId,
                   DataTable SalesPaymentCashAmount, int TillId);
        #endregion

        #region GiftCertificateCancelByHO
        DataTable GetDisplayCouponAmountOptionsDetails(string GiftCertificateBarcode, DateTime FromDate, DateTime ToDate, bool IsBarcodeSearch);
        DataTable GetCancelGiftCertificatesDetailsWithBarcodeForHO(int CouponId, int GiftCertificatesDetailId, int CouponCancelledBy, string CancelledComment);
        DataTable DisplayCoupons4Cancellation(string Barcode);
        void CancelCouponTypeOne(int couponid, string canceltext, int userid);
        #endregion

        #region GiftCertificatePayment
        DataSet GetDisplayGiftCertificatePendingPaymentDetails(int TillId);
        DataTable GetAddUpdateCouponAmountOptions(DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, int GiftCertificatesDetailId, int TillId, int UsedId);
        #endregion

        #region GiftCerts
        DataSet GetDisplayCouponAmountOptionsDetails();
        DataTable GetAddUpdateCouponAmountOptions(int CurrencyId, int PickId, decimal PickAmount, bool IsActive, string Descr, int ParentCompanyId, int CompanyId);
        #endregion

        #region NewGiftCertificate
        DataSet GetDisplayGiftCertificatesPickAmountDetails(int TillId);
        DataSet GetAddGiftCertificatesWithDetailsReports(int TillId, int CustomerId, int UserId, decimal TotalAmountInUSD, DataTable GiftCertificateDetails, DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, string CustomerName);
        DataTable GetAddGiftCertificatesWithDetails(int CurrencyId, int PickId, decimal PickAmount, bool IsActive, string Descr);
        #endregion

        #region RevalidateCoupons
        DataTable GetDisplayRevalidateCouponDetails(int TillId, string CouponBarcode);
        DataTable GetUpdateRevalidateCouponDetails(int CouponId, int TillId, string CouponBarcode, int ApprovedBy, string ReValidComment);
        #endregion

        #region UsedGiftCertificate
        DataSet GetDisplayGiftCertificateCouponBarcodeUsedDetails(string GiftCertificateBarcode, int TillId);
        DataSet GetDisplayCouponBarcodeUsedDetails(string GiftCertificateBarcode, int TillId);
        #endregion


        #endregion

        #region HRM Folder

        #region UserDiscount
        DataTable GetDisplayAllEmployeeDiscountLimits();
        DataTable GetAddUpdateEmployeeDiscountLimits(int UserId, decimal PercentageDiscount, int UpdatedBy);
        #endregion
        #endregion

        #region Coupen Folder

        #region CouponBarcodeDetails
        DataSet GetDisplayAllCouponBarcodePaymentDetails(string CouponBarcode);
        DataTable DisplayAddUpdateReactiveCoupon(int CouponId, int RevalidateBy);
        #endregion

        #region CouponHistory
        DataTable DisplayAllActiveLocationForCouponHistory();
        DataTable DisplayCouponHistoryWithPeriodLocation(int CompanyId, DateTime FromDate, DateTime ToDate);
        #endregion

        #region CouponNearlyExpiration
        DataTable GetDisplayAllCouponAndGiftNearlyExpiration(int CompanyId);
        DataTable GetDisplayAllCompanyParentsDetails();
        DataTable GetDisplayUpdateExtendDaysPerCoupon(int ExtendCouponDays, int CouponId, int ApprovedBy, string ReValidComment, string CouponBarcode);
        #endregion

        #region Promo
        DataTable GetDisplayDefaultAccountTransactionDetails(int TillId);
        #endregion

        #region PromoWithExpiration
        DataTable GetDisplayAllPromoWithExpiration(int CompanyId);
        DataTable GetDisplayAllPromoWithExpirationItems(int PromoId);
        DataTable GetAddUpdatePromoWithExpirationDays(int PromoId, int PromoDays, int CreatedBy);
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
        DataSet Get_DisplayCashFigureSpecificationReportDetails(int UserId, int TillId, DateTime AccountingDate);
        DataSet GetDisplayCashFigureAndAccountTransactionBalanceSheetReport(int UserId, int TillId);
        #endregion

        #region BalanceTransactionReport
        DataSet GetDisplayCashFigureSpecificationReportDetails(int UserId, int TillId, DateTime AccountingDate);
        DataSet GetDisplayBalanceDetailWithAccountingDate(int UserId, int TillId, DateTime AccountingDate);
        DataSet GetDisplayDailyTransactionOverview(int UserId, int TillId, DateTime AccountingDate);
        DataSet DisplaySaleBalanceSheets(int TillId, DateTime FromDate, DateTime ToDate);
        DataSet GetDailySalesReportWithAccountingDatePerTill(int UserId, int TillId, DateTime AccountingDate);

        #endregion

        #region Denomination

        DataTable GetDisplayDenominationValueTypes();
        DataTable GetDisplayDenominationTotalValues(int TillId, int DenominationTypeId);
        DataTable GetUpdateTillDenominationStatistics(int TillId, DataTable DenominationStatisticsDetails);
        DataTable GetDenominationTotalValueInUSD(int TillId);
        List<DenominationType> GetDisplayDenominationTypeDetails();


        #endregion


        #endregion

        #region Purchases Folder

        #region AddUpdatePurchase
        DataSet GetPurchasesByDate(DateTime PurchasedOnFrom, DateTime PurchasedOnTo);
        DataSet GetDisplayAddUpdateInformation(int PurchaseId);
        DataTable GetUpdatePurchaseDetailsWithItems(DataTable PurchaseItemsWithDetails, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int PurchaseId);
        DataTable GetPurchasedItemsByPurchaseId(int PurchaseId);
        void UpdateQtynCp4PurchasedItems(List<AddUpdatePurchase> l, int UserId);
        DataSet ItemsWithoutCostprices();
        DataSet ItemsWithoutCostprices_ByPurchases();
        DataTable GetDisplayReportPurchaseDetailsWithItems(int PurchaseId, int UserId);
        bool ReceiptNumberExists(string ReceiptNumber);
        #endregion

        #region NewPurchase
        DataSet GetDisplaySupplierContactPerson();
        DataTable GetAddPurchaseDetailsWithItems(int SupplierId, int TransactedBy, int CompanyId, string ReceiptNumber, string Comment, DataTable PurchaseItemsDetails);
        DataTable GetDisplayManualAddedItems();
        #endregion

        #region Purchase
        DataSet L_GetDisplayCategoryTypesModelBrandForPurchase();
        DataTable L_GetDiaplayPurchaseItemsWithModelId(int ItemId, decimal Quantity, decimal PurchasePrice, string InvoiceNumber, string ReceiptNumber);
        DataTable L_GetAddUpdatePurchaseDetailsWithItems(DataTable PurchaseItemsWithDetails, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int CompanyId, int StockLocationId, string Container);
        DataTable L_GetAllReceipts();
        DataTable L_GetDateAndSupplierById(int PurchaseId);
        DataTable L_GetBranches();
        DataTable L_GetStockLocations();
        DataTable L_GetLocations4Purchase();
        DataTable L_GetSuppliers();
        DataTable GetInfoById(int PurchaseId);
        void UpdateHeaderInfo(int SupplierId, int UserId, int CompanyId, int StockLocationId, string ReceiptNumber, string Comment, int PurchaseId, DateTime PurchasedOn, string Container);
        DataTable GetAllActiveItems();
        DataTable GetItemByManBarcode(string ManBarcode);
        DataTable GetItemById(int ItemId);
        void AddItems2ExistingPurchase(List<Purchase> pl);
        bool IsBarcodeAlreadyInPurchase(int PurchaseId, string Barcode);
        DataTable GetItemsByPurchaseId(int _purchaseid);
        void AddNewPurchaseUsingXLsheet(DataTable Barcodes, int UserId, int TillId, int SupplierId, string InvoiceNumber, string ReceiptNumber, string Comment, decimal MainDiscount, DateTime PurchasedOn, int CompanyId, int StockLocationId, string Container);
        void EditPurchaseUsingXLsheet(DataTable Barcodes, int _purchaseid, int _userid);
        DataTable GetNonMatchBarcodes(DataTable ExcelTable, int CompanyId);
        #endregion

        #endregion

        #region CreditNotes Fodler

        #region NewCreditNotes
        DataSet DisplaySaleReturnsHistoryItems(DateTime FromDate, DateTime Todate);
        DataSet CreditNoteDistributorsDetails(DateTime FromDate, DateTime Todate);
        DataSet CreditNoteDistributorsHistory(DateTime FromDate, DateTime Todate);
        DataSet DisplayAllCreditNoteDistributors();
        DataSet DisplayAddUpdateCommissionPercentage(int DistributorId, String DistributorName, bool IsActive, string Address, string Phone, int CreatedBy, DataTable CommissionPercentage, string EMail);
        DataSet DisplayAllCreditNoteDistributorsWithDistributorId(int DistributorId);
        DataTable DisplayDistributorsWIthPeriod(DateTime FromDate, DateTime Todate);
        DataTable DisplayDistributorsCreditPaymentsPeriod(int DistributorId, DateTime FromDate, DateTime Todate);
        DataTable DisplayOutstandingCreditNoteDistributors(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable DisplayCreditNoteReceiptDetails(int DistributorId);
        DataTable DisplayOutstandingCreditNoteDistributorsUpdateDiscount(int CompanyId, DateTime FromDate, DateTime ToDate, int DistributorId);
        DataTable DisplayOutstandingCreditNoteDistributorsWithPeriod(int CompanyId, DateTime FromDate, DateTime ToDate, int DistributorId);
        DataTable UpdateCreditNoteDistributorsDiscounts(DataTable dtDiscounts);
        #endregion

        #endregion

        #region Employee Folder

        #region Employee 

        DataTable GetDisplayAllActiveEmployeeRoles();
        DataTable GetDisplayAllEmployeeByRolesId(int RoleId);
        int GetAddUpdateEmployeeDiscountLimits(int UpdatedBy, int RoleId, int EmployeeId, bool IsRole, decimal PercentageDiscount);
        DataTable GetAllActiveUsers();
        bool UpdateDiscountLimits(decimal discount, int userid, DataTable users);
        DataTable GetDiscountLimitHistory(int UserID);
        #endregion

        #endregion

        #region Features Folder

        #region NewTeamViewer
        DataTable GetDisplayAllDetailsTeamViewerInformation();
        int GetAddUpdateTeamViewerId(int TillId, string TeamViewerId, string TeamViewerPassword, int CreatedBy, int CreatedTillId);

        #endregion
        #endregion

        #region MailAndAlert Folder

        #region MailAndAlert 
        DataTable GetDisplayAllTillDetailsForReceiver(int TillId);
        DataTable GetDisplayAllSentMailDetails(int TillId);
        DataTable GetDisplayAllInboxMail(int TillId);
        DataTable GetTotalNoOfUnreadMessage(int TillId);
        int GetAddNewMailInformationWithTill(string MailSubject, string MailBody, int SenderUserId, int MailTillId, string MailerMacAddress, DataTable ReceiverTillDetails);
        int GetUpdateReadMessage(int TillId, int MailId, int UserId);
        #endregion
        #endregion

        #region Parameter Folder

        #region Parameter
        DataTable GetDisplayAppParemters(int TillId);
        int GetUpdateAppParemters(string Value, int UserId, int Id);
        DataTable GetAllLocationsTills();
        int SetLocTillImg(Byte[] Img, int TillLocationId);
        #endregion
        #endregion

        #region Reports Folder

        #region  Sales Folder(In Reports Folder)

        #region BrandProductSaleOnCredit
        DataSet GetBrandProductSaleOnCreditDetails(DateTime BeginDate, DateTime EndDate, int Department);
        DataSet DisplayDepartment();
        #endregion

        #region CustomersRemainingPayments
        DataTable GetCustomerRemainingPayments();
        DataSet GetCustomerRemainingPaymentByCustomerId(Int64 CustomerId);
        #endregion

        #region MarginReport
        DataTable GetAllParentLocations();
        DataTable GetSalesPerItem(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerReceipt(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerDate(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerItemPerReceipt(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerShopClerk(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerItemPerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerDatePerShop(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerCustomer(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerCashier(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        DataTable GetSalesPerClerk(int IsPeriodSearch, int CompanyId, int Jaar, int Maand, DateTime BD, DateTime ED);
        #endregion

        #region ModelProductSaleOnCredit
        DataSet GetModelProductSaleOnCreditDetails(DateTime BeginDate, DateTime EndDate, int Department);
        #endregion

        #region MonthlySales4Lacoste
        DataTable LacosteMenTextile(int YearNumber);
        DataTable LacosteWomenTextile(int YearNumber);
        DataTable LacosteKidsTextile(int YearNumber);
        DataTable LacosteAccesoriesTextile(int YearNumber);
        DataTable LacosteFootwearsTextile(int YearNumber);
        DataTable LacosteTotalSalesFootwearsnTextile(int YearNumber);
        #endregion

        #region ProductAnalysis
        DataSet GetProductAnalysis(int ProductId);
        DataTable GetItemOverView(int ItemId);
        DataSet GetPurchasesSalesReturnsExportsStockCorrectionsTransfers(int ItemId);
        DataSet GetAvgCostPriceCalcPlusStockPerLoc(int ItemId);
        DataTable DisplaySalesPerShopByItemCategory(DateTime bd, DateTime ed, int CompanyId,
           int GroupId, int SubGroupId, int TypeId, int ColorId, int BrandId, int ModelId, int SizeId);
        #endregion

        #region PurchaseSaleAnalysis
        DataSet GetPurchaseSaleAnalysis();
        #endregion

        #region PurchaseSaleCalculation
        DataSet GetStillToSaleInformation(int ProductID);
        #endregion

        #region QuantityProductSaleOnCredit
        DataSet GetQuantityproductSaleOnCredit(DateTime FromDate, DateTime ToDate);
        #endregion

        #region RevenueOverAPeriod
        DataSet GetRevenueDataOverAPeriod(DateTime BeginDate, DateTime EndDate);
        #endregion

        #region SalePerDepartmentPerBrand
        DataSet GetSalesPerDepartmentPerBrandDetails(DateTime BeginDate, DateTime EndDate);
        #endregion   

        #region SalesRep
        DataTable LacosteSalesReport(DateTime BD, DateTime ED);
        DataTable RevenueBySaleDate(DateTime BD, DateTime ED);
        DataSet RevenueAndProfitBySaleDate(DateTime BD, DateTime ED);
        DataSet GetSalesByCustomerId(int CustomerId);
        DataSet GetSalesByBarcode(string Barcode);
        DataSet GetSalesByDate(DateTime BD, DateTime ED, int CompanyId);
        DataTable GetCreditSales(DateTime BD, DateTime ED, int UserId);
        DataTable DisplayDailyTransactionOverviewOverPeriod(DateTime bd, DateTime ed, int parentcompanyid, int companyid, int tillid);
        #endregion

        #region SalesSerachByBarcode
        DataSet GetDisplaySearchSaleByItemBarcode(string ItemBarcode);
        #endregion

        #region SalesSummary
        DataTable GetBalancePaymentsPerReceipt(Int64 saleid);
        DataSet DisplaySalesSummaryReport(DateTime FromDate, DateTime ToDate, int CompanyId);
        DataSet DisplayMontlySales(DateTime FromDate, DateTime ToDate, int CompanyId);
        #endregion

        #region SizeProductSaleOnCredit
        DataSet GetSizeProductSaleOnCredit(DateTime FromDate, DateTime ToDate);
        #endregion

        #region ViewItemSaleOnPeriod
        DataSet GetSoldItemsDetailsInPeriod(DateTime BeginDate, DateTime ToDate);
        #endregion

        #endregion

        #region Stock Folder(in Report Folder)

        #region Exports
        DataTable L_NewExport(int CustomerId, int UserId, int CompanyId);
        DataTable L_GetExports(int CustomerId, int CompanyId);
        DataTable L_GetExportedItems(int ExportId);
        DataTable L_AddExportItem(int ExportId, int UserId, int Qty, string Barcode);
        DataTable L_GetExportReport(int ExportId);
        DataTable AddUpdateNewExportWithExcel(int ExportId, int UserId, DataTable dTableBarcodeQuantity, int ExportCompanyId);
        #endregion

        #region ProductStockPerDepartment
        DataSet GetProductStockPerDepartment();
        #endregion

        #region SalesItemStockPerDepartment
        DataSet GetSalesItemStockPerDepartment();
        #endregion

        #region StockRep
        DataSet ItemsStockPerCompany();
        DataTable L_GetAllParentLocations();
        DataSet GetCurrentStockList(int CompId);
        DataSet L_GetCurrentStockList2(int CompId);
        DataSet GetCompany();
        DataSet L_GetItemFlow(int ItemId, string CompanyCode, int CompanyParentId);
        DataSet GetDisplayStockItemsFlowForAllWithItemBarcodeForHO(int ItemId);
        DataSet GetDisplayStockItemsFlowForAllWithItemBarcodeForHOCompanyId(int ItemId, int ParentCompanyId);
        DataSet GetCurrentStockQtyList(int CompId);
        DataSet GetItemListOfNegativeQtyForCompany(int CompId, DateTime TransactDate);
        DataSet GetReportOfNegativeQtyForCompany(int CompId, DateTime TransactDate);
        DataSet GetItems2ReorderPerCompany(int companyId);
        DataTable L_GetStockCompany();
        DataSet L_GetStockValue(int companyID, string Date);
        DataTable L_GetStockByStockLocId(int StockLocationId);
        DataTable L_CurrentStockQtyList2ByItemId(int ItemId);
        DataSet L_GetCurrentNegStockList2(int companyId);
        int L_GetItemIdbyBarcode(string Barcode);
        DataTable L_GetTillIdByStockLocationId(int StockLocationId);
        DataTable L_GetStockLocByCompanyId(int CompanyId);
        DataTable L_GetScannedItemsByStockLocId(int StockLocId, int CompanyId);
        DataTable GetInventoriesByStockLocId(int CompanyId, int StockLocId);
        DataTable GetInventoryTroubleshootingItemsById(int InventoryId);
        DataTable GetItemsByInventoryId_Diff(int InventoryId);
        string GetInventoryNote(int StockLocId, int CompanyId);
        DataTable GetItemsByInventoryId(int InventoryId);
        DataSet GetWriteOffHistory(DateTime BD, DateTime ED);
        DataTable GetWriteOffReport(int WriteOffId);
        object GetStockHistory(int StockLocationId, DateTime BD, DateTime ED);
        DataTable GetDisplayItemAllItemsDetails(int ItemId);
        DataTable L_Reshuffle(int companyId, DateTime BD, DateTime ED);
        DataTable L_CurrentStockQtyList2ByRefCode(string RefCode);
        DataSet GetItemMutations(int StockLocationId, int ItemId, DateTime BD, DateTime ED);
        DataTable GetHistoryByPeriod(int StockLocationId, DateTime BD, DateTime ED);
        DataTable GetAllStockLocations();
        DataTable GetStockItemTransactionDetailPerDay(int StockLocationId, int ItemId, DateTime BD, DateTime ED);
        DataTable GetLocationsUsingTransferBarcode(string _transfer_barcode);
        bool UpdateTransferStockLocations(string _transfer_barcode, int stockloc_from, int stockloc_to);
        bool UpdateTransferredItem(string _transfer_barcode, string _old_barcode, string _new_barcode);
        bool UpdateTransferredItemQty(string _transfer_barcode, string barcode, decimal qty);
        DataTable Get_Shops(int ParentCompanyId);
        DataTable GetTills(int CompanyId);
        bool ReceiveAll(int transferid, int userId);
        DataTable L_GetBrandsBasedonStockItem();
        DataTable L_GetCompanyDetailsStockItem();
        DataTable GetCurrentStoctQtyAndPrice(string BrandId, int CompanyId, DateTime FromDate, DateTime Todate);
        #endregion

        #endregion

        #endregion

        #region Revenue Folder

        #region ItemStockByBarcode
        DataTable L_GetDisplayAllActiveItemsWithBarcode();
        DataTable GetDisplayStockItemDetailsDiffLocationWithItemId(int ItemId);
        #endregion

        #region OutStandingSaleForCompany
        DataSet GetDisplayOutStandingSaleForCompany(int CompanyId);
        #endregion

        #region OutstandingSalesPerCustomer
        DataSet GetDisplayOutstandingSalesPerCustomer(int CompanyId);
        DataSet GetDisplayReportDetailsOutstandingSalesPerCustomer(int CompanyId, int CustomerId);
        DataTable GetCreditSalesPaymentHistory(DateTime FromDate, DateTime ToDate, int UserId);
        DataTable DisplayAccountsReceivable(int CompanyId, DateTime TransactionDate);
        DataTable DisplaySubsidiaryLedger(int customerid, int companyid, DateTime TransactionDate);
        DataTable ManualUpdateCashCreditCostsRunByDate(DateTime RefreshDate);
        #endregion

        #region ProductAnalyse
        DataSet DisplayProductAnalysis(int ItemId, int CompanyId);
        #endregion

        #region PurchaseSaleAnalysis
        DataTable GetPurchasesSales(int CompanyId);
        #endregion

        #region ReplenishmentReport
        DataTable DisplayMangoStockPerHoursWithPeriod(DateTime CurrentDate, DateTime FromTime, DateTime ToTime, int CompanyId);
        DataTable DisplayActiveMangoLocation();
        #endregion

        #region Revenues
        DataTable GetDisplayPivotReportRevenueByDate(DateTime FromDate, DateTime ToDate, int UserId);
        DataTable GetDisplayPivotReportRevenueByLocation(DateTime FromDate, DateTime ToDate, int UserId);
        DataSet GetRevenuesByCompanyByPeriodPerClerk(DateTime BD, DateTime ED, int CompanyId);
        DataTable GetRevenuesByCompanyByPeriodPerCustomer(DateTime BD, DateTime ED, int CompanyId);
        DataTable GetRevenuesByCompanyByPeriodPerClerk4ExcelReport(DateTime BD, DateTime ED, int CompanyId);
        DataTable GetSalesByCustomer(int CustomerId, int CompanyId);
        DataTable GetDisplayPivotReportNetSalesByLocation(DateTime BD, DateTime ED, int CurrencyId, int UserId);
        #endregion

        #region RevenuesByItem
        DataTable GetDisplayAllItemsDetailsWithReportByPeriod(DateTime FromDate, DateTime ToDate, int CompanyId);
        #endregion

        #region SaleGetSalesByDate
        DataTable GetDisplaySaleGetSalesByDate(DateTime FromDate, DateTime ToDate);
        DataSet GetSalesReprintSalesReceiptForHOAllLocation(long SaleId, int TillId);
        DataSet GetSalesReprintSalesReceiptForHOAllLocation_GC(int AccountTypeId, long SaleId, int TillId);
        DataSet GetGetAllByItemByBarcode(string Barcode);
        DataSet GetGetAllByItemByIMEI(string IMEI);
        #endregion

        #region SalesAnalysis
        DataTable DisplaySalesAnalysis(DateTime BD, DateTime ED, int CompanyId);
        #endregion

        #region SalesPerClerk
        DataTable GetDisplaySaleClerkCommissionDetails(int CompanyId);
        DataTable GetDisplaySaleClerkItemsCommissionDetails(int CompanyId, int ClerkId);
        #endregion

        #region SalesPerCustomer
        DataTable GetBalancePayments(int CustomerId);
        DataTable GetDisplayAllCompanyDetailsForReport();
        DataTable GetCustomersByCompany(int CompanyId);
        DataSet GetBalancePerCustomer(int CompanyId, int CustomerId);
        #endregion

        #region SoldItemsByItem
        DataTable GetSoldItemsByItemProperties(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable GetSaleNotes(DateTime bd, DateTime ed, int itemid, int parent_companyid);
        #endregion

        #region SoldItemsPerProduct
        DataTable GetCompanies(int UserId);
        DataSet GetDisplayCompanySoldItemsPerProduct(int CompanyId, DateTime FromDate, DateTime ToDate, int UserId);
        #endregion

        #region SoldSaleItemsByItemProperties
        DataTable GetSoldSaleItemsByItemProperties(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable GetSoldItemsByPropertiesPerCompany(int CompanyId, DateTime FromDate, DateTime ToDate);
        DataTable L_GetCurrentStockListOfSaleItems(int CompanyId);
        #endregion

        #endregion

        #region Sales Folder

        #region Sales
        DataSet L_GetDisplaySalesDefaultInformation(int CompanyId);
        DataSet L_GetDisplayReprintSalesDetails(int TillId);
        DataSet L_GetDisplaySalesReturnHistory(int TillId);
        DataSet L_GetDisplayReprintAllSaleItemWithPaymentDetails(string ReceiptNumber);
        DataSet L_GetDisplayPendingPaymentCustomerBarcode(int TillId, string SalesBarcode);
        DataSet L_GetDisplayPendingPaymentCustomerReceipt(int TillId);
        DataSet L_GetReturnItemsWithReturnMoneyDetails(DataTable DataTableSaleReturnedItems, DataTable DataTableSalesPaymentCashAmount, string Comment, int UserId, int ReturnedByTillId, string ReceiptNumber, decimal TotalReturnMoneyAmount);
        DataSet L_GetReturnDisplayCustomerItemsDetail(string Barcode, int TillId);
        DataSet L_GetDisplaySaleHistoryOfCurrentAccountingDate(int TillId);
        DataSet L_GetReturnItemsForCouponDetails(int ReturnedByTillId, int UserId, string Barcode, string Comment, DataTable SaleReturnedItems, decimal CouponAmount);
        DataSet L_GetDisplaySaleHistoryOfCurrentAccountingDateForCancel(int TillId);
        DataSet L_GetAddPendingPaymentWithPaymentTransaction(DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, int TillId, int PrincipalId, string SalesBarcode, decimal PaidAmountInUSD, decimal ReturnAmountInUSD);
        DataSet L_GetAddSaleWithSaleItemsDetailsWithPaymentTransaction(int TillId, int CustomerId, DataTable dTableSalesPerson, int PrincipalId, decimal Discount, string Comment, decimal Total,
           string TransactedBy, DataTable SaleItemsDetails, DataTable SalesPaymentCashAmount, DataTable SalesReturnCashAmount, DataTable CouponPaymentDetails, int SaleTypeId,
           string NewCustomerName, string NewCustomerAddress, string NewCustomerPhone);
        DataTable L_GetDisplaySalesItemsDetailsWithBarcode(string ItemBarcode, int CompanyId);
        DataTable L_GetCancelSaleAndContraAllTransaction(string ReceiptNumber, int UserId);
        DataTable L_GetDisplayCouponsScanDetails(string Barcode, int TillId);
        DataSet L_GetSalePaymentDetailsInDifferentCurrency(int TillId, decimal PaymentAmountInUSD);
        DataSet MonthlySales4Lacoste(int Year);
        #endregion

        #region SalesCreditDiscount
        DataTable GetCustomer4DiscountUpdate(int CompanyId);
        DataTable DisplayAllPendingSalesReceipts(int CustomerId, int CompanyId);
        DataTable GetAllSoldItems4DiscountUpdate(long SaleId, int TillId, int CustomerId);
        DataTable GetDisplayUpdateItemsDiscount(DataTable dTableUpdateItemDiscount, int UserId);
        DataTable GetDisplayUpdateSoldItemsDiscount(DataTable dTableUpdateItemDiscount, int UserId);
        DataTable GetItemsBySaleId(long SaleId);
        DataTable GetItemsPricesBySaleId(long SaleId);
        bool UpdateDiscounts(int UserId, DataTable dt);
        bool UpdateReciptDiscounts(long SaleId, decimal Discount, int UserID);
        int isEligible(long SaleId);
        #endregion
        #endregion

        #region SalesTax
        #region SalesTaxSummary
        DataTable DisplaySalesTaxReportSummaryInPeriod(DateTime FromDate, DateTime ToDate, int CompanyId);
        DataTable DisplaySalesTaxRevenuesCompanyLocation();
        DataTable DisplaySalesTaxRevenuesInPeriod(DateTime FromDate, DateTime ToDate, int CompanyId);
        #endregion
        #endregion

        #region StockFolder

        #region Checker
        DataTable StockChecker(int CompanyId, DataTable scanqty);
        DataTable ShuffleChecker(int companyId, DataTable dataTable);
        DataTable GetTransfersUsingBarcodes(int CompanyId, DataTable Barcodes, DateTime BD, DateTime ED);
        string NewTransferXL(int UserId, int CompanyId, string Shop, DataTable barcodeqty);

        #endregion

        #region CompanyPromoCurrentStockList
        DataTable DisplayGetCompanyParents();
        DataTable DisplayStockGetStockTransferReport(int CompanyId);
        DataTable DisplayPromoAddNewPromoItems(DataTable CompanyPromoItems, string Code, int CompanyId, decimal Discount, int CreatedBy, DateTime PromoDate);
        #endregion

        #region DefectInventory
        DataTable DisplayItemDescriptionWithStockItems(int CompanyId, string Barcode);
        DataTable DisplayAddUpdateStockDefectItems(DataTable ManualDefectReturnItems, int StockLocationId, int CreatedBy, int TillId);
        DataSet DisplayDefectInventoryHistoryPeriod(DateTime FromDate, DateTime ToDate, int StockLocationId);
        DataTable DisplayDefectInventoryItems();
        DataTable DisplayAllActiveStockLocation();
        DataTable DisplayAllActiveStockLocationAll();
        #endregion

        #region IncompleteTransfer
        DataTable DisplayIncompletedTransfers();
        #endregion

        #region ReprintStockTransferReport
        DataTable L_DisplayStockTransferReport(int StoreTransferId);
        DataTable L_GetStockTransferCheckList(int StoreTransferId, int UserId);
        #endregion

        #region SaleGetSalesByDate
        DataTable GetDisplayShopStockDetails(DateTime BD, DateTime ED);
        DataSet GetSaleForSalesReprintSalesReceipt(long SaleId, int TillId);
        #endregion

        #region Section
        DataTable DisplayAddUpdateSectionDetails(string SectionNumber, string SectionName, int CreatedBy, string Remarks);
        DataTable DisplayInventoryDeleteBySection(int UserId, int SectionId, int InventoryId);
        #endregion

        #region ShopStock
        DataTable GetDisplayShopStockDetails(int TillId);
        #endregion

        #region StockCurrentStockDetails
        DataTable GetDisplayItemIdWithItembarcode(string Barcode);
        DataSet GetDisplayItemFlowForEveryLocationWithCompanyNameByItemId(int ItemId);

        #endregion

        #region StockSoldItemsForCompanyAndLocationId
        DataTable GetDisplayItemIdWithItembarcode(string CompanyCode, int CompanyParentId, DateTime FromDate, DateTime ToDate);
        #endregion

        #region StockTransfer
        DataTable L_GetAddUpdateReceiveItemsAndStockTransaction(int TillId, int TransactedBy, string TransferNumber, string Batchcode, decimal RequestedQuantity);
        DataTable L_ReceiveBadStockTransfer(int StockLocationId, int TransactedBy, string TransferNumber, string Batchcode);
        DataTable L_GetTransferByTransferNo(int StockLocationId, string TransferNumber);
        DataTable L_GetBadStockTransferByTransferNo(int StockLocationId, string TransferNumber);
        DataSet L_GetStockLocations(int StockLocationId);
        DataTable L_GetItemByBarcode4Transfer(string Batchcode, int StockLocationId, decimal TotalQuantity, decimal RequestQuantity);
        DataTable L_CreateStockTransfer(DataTable StockTransferItemsDetails, int TransactedBy, int FromStockLocId, int ToStockLocId, int TillId, string Comment, int NoOfBoxes);
        DataTable CreateTransferXLsheet(DataTable ExcelTable, int TransactedBy, int FromStockLocId, int ToStockLocId, int TillId, string Comment, int noofbox);
        #endregion

        #region StockTransferCancel
        DataSet L_GetPendingTransfers(int CompanyId);
        DataTable L_CancelStockTransfer(int CompanyId, int StoreTransferId, int TransactedBy);
        DataTable L_DefectStockTransfer(string StockTransferBarcode);
        #endregion

        #region StockTransferHistory
        DataSet GetDisplayCompanyWithLocationAndAddress(int TillId, DateTime FromDate, DateTime ToDate);
        DataTable GetDisplayStockTransferHistoryDetails(int StoreTransferId);
        #endregion

        #region StockTransferReport
        DataSet L_GetTransfersByDate(DateTime FromDate, DateTime ToDate);
        DataSet GetAllOpenTransfers();
        DataSet GetAllCancelledTransfers();
        void AddItemsIntoTransfer(DataTable dt, int _transferid);
        #endregion

        #region  StockTransferRequestsHistory
        DataSet L_GetDisplayAllCompanyDetailsForReport(int TillId, DateTime FromDate, DateTime ToDate);
        DataTable L_GetDisplayCancelStockTransferRequests(int TransferRequestsId, int CancelledBy, string CancelComments);
        #endregion

        #region TransferRequest
        DataTable L_GetDisplayCompanyStockItemsDetailsWithCompanyId(int CompanyId);
        DataTable L_GetAddTransferRequestItemsDetailsWithCompanyId(DataTable TransferRequestItemsDetails, int FromCompanyId, int ToCompanyId, int CreatedBy, string Comments);
        #endregion

        #region TransfersBarcodeByStoreTransfer
        DataTable L_GetDisplayShopStockDetails(int StoreTransferId);
        #endregion

        #region  WriteOff
        DataTable GetDisplayCompanyWithAddress(int TillId);
        DataTable GetDisplayInventoryDateByHO(int CompanyId);
        DataTable GetDisplayInventoryForWritesOffItemsDetails(int CompanyId, DateTime? InventoryDate);
        int GetWriteoffUpdateAndStockTransaction(int CompanyId, int ItemId, int EnteredBy, decimal StockQuantity, int PurchaseItemId, string Batch, decimal CurrentStockQty, decimal CountedQty);

        #endregion

        #endregion

        #region SupplierPurchase Folder
        #region SupplierPurchase
        DataTable DisplayPurchaseHistoryWithPeriod(DateTime FromDate, DateTime ToDate, int SupplierId);
        DataTable DisplaySaleAndReceiptFromCustomerWithPeriod(DateTime FromDate, DateTime ToDate, int CompanyId);
        DataTable DisplayReturnAndReceiptFromCustomerWithPeriod(DateTime FromDate, DateTime ToDate, int CompanyId);
        DataTable DisplayAllActiveSupplier();
        DataTable DisplaySaleReceiptCompanyDetails();
        #endregion
        #endregion

        #region UserSecurity

        #region Log
        public bool L_In(string Login, string Pwd, bool UseUserId4Login, out string Msg, out int Rv);
        public void L_GetUserCredentials(string Login, bool UseUserId4Login, out string FullName, out int ContactId, out int UserID, out string UserName, out bool IsActive, out bool IsUserMale);
        #endregion

        #region UserRight
        DataTable L_GetUiObjects();
        DataTable L_GetUiObjectsByMainId(int MainObjectId);
        int L_AddUpdateUiObject(int ObjectId, int ObjectMainId, string ObjectName, string ObjectText);
        int L_AddUser(int UserID, string UserName, string Description, string Password, int ContactId, bool IsActive);
        void L_CatUiObjects(int MainId, List<UserPrivilege> Ids);
        bool L_RemoveUiObject(int ObjectId);
        bool L_RemoveSubFromMainObject(int ObjectId);
        DataTable L_GetAllUsers();
        DataTable L_GetEmployees();
        DataTable L_GetAllActiveRoles();
        DataTable L_GetAllRoles();
        List<UserPrivilege> L_GetUiObjectsInHierarchyOrder();
        DataTable L_GetPrincipalRights(int PrincipalId);
        bool L_ChangeUserRights(List<UserPrivilege> EmptyLs, List<UserPrivilege> GrantLs, List<UserPrivilege> DenyLs, bool IsUser);
        bool L_AutoAddUiObjects(DataTable UiObjects);
        DataTable L_GetAllUsersObjects(int UserId);
        int L_AddRole(int PrincipalId, string Description, bool IsActive);
        DataTable L_GetUsersInRole(int RoleId);
        DataTable L_GetAllActiveUsers();
        void L_AddUserToRole(List<int> pl, int RoleId);
        void L_RemoveUserFromRole(List<int> pl, int RoleId);
        bool L_ChangeUserPassword(int UserId, string OldPwd, string NewPwd);
        DataTable PrincipleUpdateCopyUserRights(int FromPrincipleId, int ToPrincipleId);
        #endregion

        #endregion

        #region ConnectioString
        string CreateSqlConnectionString(string Server, string Database, string UserID, string Password);
        void CreateSqlConnectionString(int CountryId);
        #endregion

        #region NewAdded
        DataTable GetCompanyForlocations();
        DataSet AddLocationEntry(int companyId, string locationCode, string locationName, string address, string fullName, string phone, string moblie, string fax,
           string email, string website, string note, int countryId, bool isMainWarehouse, bool isMainBank, int createdBy, bool isOutlet);
        DataSet AddUpdateCashbackAmount(int CashbackAmountId, int CompanyId, decimal CashbackBracketsUpTo, decimal CashbackAmount, bool IsEnabled, int UpdatedBy);
        DataTable GetCashbackDetails(int CompanyId);
        DataTable GetCashbackVoucherDetails(int CompanyId, DateTime fromDate, DateTime toDate);
        #endregion
    }
}
