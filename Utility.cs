using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DaVinciAdminApi.Helper
{
    public static class Utility
    {
        /// <summary>
        /// Writes exception details to file
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strMessage"></param>
        public static void WriteToFile(Exception ex, string strMessage = null)
        {
            StringBuilder sbMsg = null;
            try
            {
                string strPath = AppSettings.DaVinciAdminApiLogs;

                if (strPath == null)
                {
                    strPath = AppDomain.CurrentDomain.BaseDirectory;

                    strPath = Path.Combine(strPath, "DaVinciAdminApiLogs");

                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }
                }
                else
                {
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }
                }
                string strLogFile = Path.Combine(strPath, String.Format("{0:dd}_{1:MM}_{2:yyyy}.log", DateTime.Today, DateTime.Today, DateTime.Today));
                sbMsg = new StringBuilder();

                //Log error messages to be highlighted in the log file
                if (ex != null)
                {
                    sbMsg.Append("########################################### ERROR #########################################" +
                    Environment.NewLine + "Log Date : " + DateTime.Now + Environment.NewLine
                                        + strMessage + Environment.NewLine
                                        + "Additional Info :" + Environment.NewLine + ex.Message + Environment.NewLine
                                        + ex.StackTrace + Environment.NewLine);

                    if (ex.InnerException != null)
                    {
                        sbMsg.Append("Inner Exception :" + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                                       ex.InnerException.Source + Environment.NewLine + ex.InnerException.StackTrace + Environment.NewLine);
                    }
                    sbMsg.Append("###########################################################################################");
                }

                else if (ex == null)
                {
                    sbMsg.Append("########################################### ERROR #########################################" +
                    Environment.NewLine + "Log Date : " + DateTime.Now + Environment.NewLine
                                        + strMessage + Environment.NewLine);
                    sbMsg.Append("###########################################################################################");
                }

                sbMsg.Append(Environment.NewLine + Environment.NewLine);

                //Write to file
                File.AppendAllText(strLogFile, sbMsg.ToString());

                //Uncomment the below if sending exception mail
                //new Thread(() =>
                //{
                //    Thread.CurrentThread.IsBackground = true;

                //    //Send exception email
                //    SendExceptionMail(ex, strMessage);
                //}).Start();


                sbMsg = null;
            }
            catch
            {
                //Do nothing                
            }
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }



        public static byte[] compressProto<T>(T data)
        {
            using (MemoryStream msi = new MemoryStream())
            {
                Serializer.Serialize<T>(msi, data);

                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress, false))
                    {
                        msi.Position = 0;
                        CopyTo(msi, gs);

                    }
                    return mso.ToArray();
                }
            }
        }



        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt;
            try
            {
                src.Position = 0;
            }
            catch { }
            finally { GC.Collect(); }
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
