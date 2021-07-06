using Microsoft.Extensions.Options;
using OnlineSlots.Api.Data.Interface;
using OnlineSlots.Api.Helpers;
using OnlineSlots.Models.Casino;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OnlineSlots.Api.Data.Repositories
{
    public class CasinoRepository : ICasinoRepository
    {
        private readonly AppSettings _appSettings;

        public CasinoRepository(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public IEnumerable<Casinos> CasinoHomePageSlide(int CasinoID)
        {
            List<Casinos> _Casinos = new List<Casinos>();
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@CasinoID", CasinoID);
            param.Add(sqlParameter);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinoAndHotelsHederBanner]", param);

            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"].ToString()),
                        ImagePath = Convert.ToString(dt.Rows[i]["ImagePath"].ToString())
                    });
                }
            }
            return _Casinos;
        }

        public Tuple<List<Casinos>, List<CasinoContents>> GetCasinoContactDetails()
        {
            List<Casinos> _Casinos = new List<Casinos>();
            List<CasinoContents> _CasinoContact = new List<CasinoContents>();
            var ds = SqlHelper.ExecuteStoredProcedureWithDataSet(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinoContactDetails]", null);

            if (SqlHelper.HasTables(ds))
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(ds.Tables[0].Rows[i]["CasinoID"].ToString()),
                        ImagePath = Convert.ToString(ds.Tables[0].Rows[i]["LogoImagePathForMenu"].ToString()),
                    });
                }
            }

            if (SqlHelper.HasTables(ds))
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    _CasinoContact.Add(new CasinoContents
                    {
                        CasinoID = Convert.ToInt32(ds.Tables[0].Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(ds.Tables[0].Rows[i]["CasinoName"].ToString()),
                        ContentCategoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["ContentCategoryID"].ToString()),
                        ContentCategoryName = Convert.ToString(ds.Tables[0].Rows[i]["ContentCategoryName"].ToString()),
                        ContentData = Convert.ToString(ds.Tables[0].Rows[i]["ContentData"].ToString()),
                    });
                }
            }
            return Tuple.Create(_Casinos, _CasinoContact);
        }

        public IEnumerable<CasinoContents> GetCasinoContentDetails(int CasinoID)
        {
            List<CasinoContents> _Casinos = new List<CasinoContents>();
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@CasinoID", CasinoID);
            param.Add(sqlParameter);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinoContentDetails]", param);
            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _Casinos.Add(new CasinoContents
                    {
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"].ToString()),
                        ContentCategoryID = Convert.ToInt32(dt.Rows[i]["ContentCategoryID"].ToString()),
                        ContentCategoryName = Convert.ToString(dt.Rows[i]["ContentCategoryName"].ToString()),
                        ContentData = Convert.ToString(dt.Rows[i]["ContentData"].ToString()),
                    });
                }
            }
            return _Casinos;
        }

        public IEnumerable<Casinos> GetCasinoHPRestaurantSlide(int CasinoID)
        {
            List<Casinos> _Casinos = new List<Casinos>();
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@CasinoID", CasinoID);
            param.Add(sqlParameter);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinoHPRestaurantSlide]", param);
            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"].ToString()),
                        ImagePath = Convert.ToString(dt.Rows[i]["ImagePath"].ToString())
                    });


                }
            }
            return _Casinos;
        }

        public IEnumerable<Casinos> GetCasinosHotelsForMenu()
        {
            List<Casinos> _Casinos = new List<Casinos>();
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinosHotelsForMenu]", null);
            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"].ToString()),
                        ImagePath = Convert.ToString(dt.Rows[i]["LogoImagePathForMenu"].ToString()),
                        IsReservationViewRequired = Convert.ToBoolean(dt.Rows[i]["IsReservationViewRequired"]),
                        IsRestaurantViewRequired = Convert.ToBoolean(dt.Rows[i]["IsRestaurantViewRequired"])
                    });
                }
            }
            return _Casinos;
        }

        public JackpotCurrency GetJackpotCurrency(int ClientId)
        {
            JackpotCurrency _JackpotCurrency = new JackpotCurrency();
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@ClientId", ClientId);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[OnlineSlots].[GetJackpotCurrencyDetails]", param);
            if (SqlHelper.HasRows(dt))
            {

                _JackpotCurrency.MainCurrencyRate = Convert.ToDecimal(dt.Rows[0]["MainCurrencyRate"].ToString());
                _JackpotCurrency.ConversionCurrencyRate = Convert.ToDecimal(dt.Rows[0]["ConversionCurrencyRate"].ToString());
                _JackpotCurrency.CurrencyCode = Convert.ToString(dt.Rows[0]["CurrencyCode"].ToString());

            }
            return _JackpotCurrency;
        }

        public IEnumerable<NewsDetails> GetNewsDetails()
        {
            List<NewsDetails> _News = new List<NewsDetails>();
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetNewsDetails]", null);

            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _News.Add(new NewsDetails
                    {
                        NewsID = Convert.ToInt32(dt.Rows[i]["NewsID"].ToString()),
                        NewsHeader = Convert.ToString(dt.Rows[i]["NewsHeader"].ToString()),
                        NewsDescription = Convert.ToString(dt.Rows[i]["NewsDescription"].ToString()),
                        NewsImagePath = Convert.ToString(dt.Rows[i]["NewsImagePath"].ToString()),
                        NewsPostedDate = Convert.ToDateTime(dt.Rows[i]["PostedDate"]),
                        NewsPostedBy = Convert.ToString(dt.Rows[i]["UserName"].ToString()),
                        NewsPostedByID = Convert.ToInt32(dt.Rows[i]["PostedBy"].ToString()),
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoId"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"].ToString()),
                        IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]),
                    });

                }
            }
            return _News;
        }

        public string GetSportsBettingClientURL(int ClientId)
        {
            string ClentURL = string.Empty;
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@ClientId", ClientId);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[OnlineSlots].[Casino_GetClientURL]", param);

            if (SqlHelper.HasRows(dt))
            {
                ClentURL = Convert.ToString(dt.Rows[0]["ParameterValue"].ToString());
            }
            return ClentURL;
        }


        public IEnumerable<Casinos> GetCasinos()
        {
            List<Casinos> _Casinos = new List<Casinos>();
            var ds = SqlHelper.ExecuteStoredProcedureWithDataSet(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[PGPromo_GetCasinos]", null);
            var dtGetCasino = ds.Tables[0];
            if (SqlHelper.HasTables(ds))
            {
                for (int i = 0; i < dtGetCasino.Rows.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(ds.Tables[0].Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(ds.Tables[0].Rows[i]["CasinoName"].ToString()),
                    });
                }
            }
            return _Casinos;

        }

        public IEnumerable<CasinoGallery> GetCasinoGallery(int CasinoID)
        {
            List<CasinoGallery> _Gallery = new List<CasinoGallery>();
            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@CasinoID", CasinoID);
            param.Add(sqlParameter);
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetGallery]", param);
            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _Gallery.Add(new CasinoGallery
                    {
                        AlbumID = Convert.ToInt32(dt.Rows[i]["AlbumID"].ToString()),
                        CasinoID = Convert.ToInt32(dt.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dt.Rows[i]["CasinoName"]),
                        AlbumHeader = Convert.ToString(dt.Rows[i]["AlbumHeader"].ToString()),
                        AlbumDescription = Convert.ToString(dt.Rows[i]["AlbumDescription"].ToString()),
                        ImageDescription = Convert.ToString(dt.Rows[i]["ImageDescription"]),
                        ImageName = Convert.ToString(dt.Rows[i]["ImageName"].ToString()),
                        ImagePathMain = Convert.ToString(dt.Rows[i]["ImagePathMain"].ToString()),
                        IsActive = Convert.ToBoolean(dt.Rows[i]["IsActive"]),
                    });

                }

            }
            return _Gallery;

        }


        public IEnumerable<JobPostings> GetJobPostingsDetails()
        {
            List<JobPostings> _JobPostings = new List<JobPostings>();
            Casinos obj = new Casinos();
            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetJobPostingsDetails]", null);

            if (SqlHelper.HasRows(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    _JobPostings.Add(new JobPostings
                    {

                        JobPostingID = Convert.ToInt32(dt.Rows[i]["JobPostingID"].ToString()),
                        JobHeader = Convert.ToString(dt.Rows[i]["JobHeader"].ToString()),
                        ExperienceRequired = Convert.ToString(dt.Rows[i]["ExperienceRequired"].ToString()),
                        KeySkillsRequired = Convert.ToString(dt.Rows[i]["KeySkillsRequired"].ToString()),
                        JobLocation = Convert.ToString(dt.Rows[i]["JobLocation"]),
                        JobDescription = Convert.ToString(dt.Rows[i]["JobDescription"].ToString()),
                        PostedDate = Convert.ToString(dt.Rows[i]["PostedDate"].ToString()),
                        PostedBy = Convert.ToString(dt.Rows[i]["PostedBy"].ToString()),
                        CasinoId = Convert.ToInt32(dt.Rows[i]["CasinoId"].ToString()),
                        CasinoName = (dt.Rows[i]["CasinoName"].ToString()),

                    });
                }
            }
            return _JobPostings;
        }


        public IEnumerable<DisplayImage> GetMainHomeBigBGSlide()
        {
            List<DisplayImage> _ImageHeader = new List<DisplayImage>();
            var dsTable = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetMainHomeBigBGSlide]", null);
            if (SqlHelper.HasRows(dsTable))
            {
                for (int i = 0; i < dsTable.Rows.Count; i++)
                {
                    _ImageHeader.Add(new DisplayImage
                    {
                        PromoId = Convert.ToInt32(dsTable.Rows[i]["PromoId"].ToString()),
                        PromoTypeID = Convert.ToInt32(dsTable.Rows[i]["PromoTypeID"].ToString()),
                        CasinoID = Convert.ToInt32(dsTable.Rows[i]["CasinoID"].ToString()),
                        PromoName = Convert.ToString(dsTable.Rows[i]["PromoName"].ToString()),
                        ImagePath = Convert.ToString(dsTable.Rows[i]["ImagePath"].ToString()),
                        ShowDuration = Convert.ToInt32(dsTable.Rows[i]["ShowDurationInSecs"].ToString()),
                        Date = Convert.ToString(dsTable.Rows[i]["Date"].ToString()),
                        IsActive = Convert.ToBoolean(dsTable.Rows[i]["IsActive"].ToString()),
                    });
                }
            }
            return _ImageHeader;
        }

        public IEnumerable<DisplayImage> GetMainHomePageSmallPromoSlide()
        {
            List<DisplayImage> _ImageHeader = new List<DisplayImage>();
            var dsTable = SqlHelper.ExecuteStoredProcedureWithDataTable(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetMainHomePageSmallPromoSlide]", null);
            if (SqlHelper.HasRows(dsTable))
            {
                for (int i = 0; i < dsTable.Rows.Count; i++)
                {
                    _ImageHeader.Add(new DisplayImage
                    {
                        PromoId = Convert.ToInt32(dsTable.Rows[i]["PromoId"].ToString()),
                        PromoTypeID = Convert.ToInt32(dsTable.Rows[i]["PromoTypeID"].ToString()),
                        CasinoID = Convert.ToInt32(dsTable.Rows[i]["CasinoID"].ToString()),
                        PromoName = Convert.ToString(dsTable.Rows[i]["PromoName"].ToString()),
                        ImagePath = Convert.ToString(dsTable.Rows[i]["ImagePath"].ToString()),
                        ShowDuration = Convert.ToInt32(dsTable.Rows[i]["ShowDurationInSecs"].ToString()),
                        Date = Convert.ToString(dsTable.Rows[i]["Date"].ToString()),
                        IsActive = Convert.ToBoolean(dsTable.Rows[i]["IsActive"].ToString()),
                    });
                }
            }
            return _ImageHeader;
        }

        public DayAndWeekleyEvents GetEventDetails(int CasinoID)
        {

            List<OneTimeEvent> _OneTimeEventList = new List<OneTimeEvent>();
            List<WeeklyEvent> _WeeklyEventList = new List<WeeklyEvent>();

            DayAndWeekleyEvents dayAndWeekleyEvents = new DayAndWeekleyEvents();

            List<SqlParameter> param = new List<SqlParameter>();
            SqlParameter sqlParameter = new SqlParameter("@CasinoID", CasinoID);
            param.Add(sqlParameter);
            DataSet ds = SqlHelper.ExecuteStoredProcedureWithDataSet(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetEventDetails]", param);
            var dsTable = ds.Tables[0];
            var dsTable1 = ds.Tables[1];


            //--------------------One Time Event List------------------//
            if (SqlHelper.HasRows(dsTable))
            {
                for (int i = 0; i < dsTable.Rows.Count; i++)
                {
                    _OneTimeEventList.Add(new OneTimeEvent
                    {
                        EventID = Convert.ToInt32(dsTable.Rows[i]["EventID"].ToString()),
                        CasinoID = Convert.ToInt32(dsTable.Rows[i]["CasinoID"].ToString()),
                        EventTypeID = Convert.ToInt32(dsTable.Rows[i]["EventTypeID"].ToString()),
                        CasinoName = Convert.ToString(dsTable.Rows[i]["CasinoName"].ToString()),
                        EventHeader = Convert.ToString(dsTable.Rows[i]["EventHeader"].ToString()),
                        EventDescription = Convert.ToString(dsTable.Rows[i]["EventDescription"].ToString()),
                        EventDate = Convert.ToDateTime(dsTable.Rows[i]["EventDate"].ToString()),
                        EventTime = Convert.ToString(dsTable.Rows[i]["EventTime"].ToString()),
                        EventTypeName = Convert.ToString(dsTable.Rows[i]["EventTypeName"].ToString()),
                        IsActive = Convert.ToBoolean(dsTable.Rows[i]["IsActive"].ToString()),
                    });

                }


            }
            //--------------------Weekly Event List------------------//
            if (SqlHelper.HasRows(dsTable1))
            {
                for (int i = 0; i < dsTable1.Rows.Count; i++)
                {
                    _WeeklyEventList.Add(new WeeklyEvent
                    {
                        EventID = Convert.ToInt32(dsTable1.Rows[i]["EventID"].ToString()),
                        CasinoID = Convert.ToInt32(dsTable1.Rows[i]["CasinoID"].ToString()),
                        EventTypeID = Convert.ToInt32(dsTable1.Rows[i]["EventTypeID"].ToString()),
                        CasinoName = Convert.ToString(dsTable1.Rows[i]["CasinoName"].ToString()),
                        EventHeader = Convert.ToString(dsTable1.Rows[i]["EventHeader"].ToString()),
                        EventDescription = Convert.ToString(dsTable1.Rows[i]["EventDescription"].ToString()),
                        DayID = Convert.ToInt32(dsTable1.Rows[i]["DayID"].ToString()),
                        Day = Convert.ToString(dsTable1.Rows[i]["Day"].ToString()),
                        EventTime = Convert.ToString(dsTable1.Rows[i]["EventTime"].ToString()),
                        EventTypeName = Convert.ToString(dsTable1.Rows[i]["EventTypeName"].ToString()),
                        IsActive = Convert.ToBoolean(dsTable1.Rows[i]["IsActive"].ToString()),
                    });

                }
            }

            dayAndWeekleyEvents.OneTimeEvent = _OneTimeEventList;
            dayAndWeekleyEvents.WeeklyEvent = _WeeklyEventList;
            return dayAndWeekleyEvents;
        }



        public CasinoContacts GetCasinoContacts()
        {
            List<Casinos> _Casinos = new List<Casinos>();
            List<CasinoContents> _CasinoContact = new List<CasinoContents>();
            CasinoContacts getCasinoContacts = new CasinoContacts();
            DataSet ds = SqlHelper.ExecuteStoredProcedureWithDataSet(_appSettings.ConnectionString.OnlineSlotDb, "[PGPromo].[Casino_GetCasinoContactDetails]", null);
            var dsTable = ds.Tables[0];
            var dsTable1 = ds.Tables[1];


            //--------------------One Time Event List------------------//
            if (SqlHelper.HasRows(dsTable))
            {
                for (int i = 0; i < dsTable.Rows.Count; i++)
                {
                    _Casinos.Add(new Casinos
                    {
                        CasinoID = Convert.ToInt32(dsTable.Rows[i]["CasinoID"].ToString()),
                        ImagePath = Convert.ToString(dsTable.Rows[i]["LogoImagePathForMenu"].ToString()),
                    });

                }


            }
            //--------------------Weekly Event List------------------//
            if (SqlHelper.HasRows(dsTable1))
            {
                for (int i = 0; i < dsTable1.Rows.Count; i++)
                {
                    _CasinoContact.Add(new CasinoContents
                    {
                        CasinoID = Convert.ToInt32(dsTable1.Rows[i]["CasinoID"].ToString()),
                        CasinoName = Convert.ToString(dsTable1.Rows[i]["CasinoName"].ToString()),
                        ContentCategoryID = Convert.ToInt32(dsTable1.Rows[i]["ContentCategoryID"].ToString()),
                        ContentCategoryName = Convert.ToString(dsTable1.Rows[i]["ContentCategoryName"].ToString()),
                        ContentData = Convert.ToString(dsTable1.Rows[i]["ContentData"].ToString()),
                    });
                }
            }
            getCasinoContacts.Casinos = _Casinos;
            getCasinoContacts.CasinoContents = _CasinoContact;
            return getCasinoContacts;
        }
    }
}
