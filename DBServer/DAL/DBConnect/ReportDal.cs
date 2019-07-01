//using Model.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace DAL
{

    //[Serializable]
    //public class DataModel
    //{
    //    public string name;
    //    public string path;
    //    public byte[] content;
    //    public string saveTime;
    //}
    [Serializable]
    class DataModel
    {
        public DateTime timeCreate;
        public DateTime timeModify;
        public DateTime timeAccess;
        public string name;
        public string path;
        public byte[] content;
    }

    public class ReportDal
    {
        public bool Insertreport(string name, byte[] content, string path, DateTime createTime, DateTime modifyTime, DateTime accessTime)
        {
            if (name == null || content == null)
            {
                return false;
            }
            string cmd = "DELETE FROM TestData WHERE Name = @Name";
            int re1 = SQLiteHelper.ExecuteNonQuery(cmd, new SQLiteParameter("@Name", name));

            int re = SQLiteHelper.ExecuteNonQuery("INSERT INTO TestData (Name,Content,Path,CreateTime,ModifyTime,AccessTime)"
            + "  VALUES  (@Name,@Content,@Path,@CreateTime,@ModifyTime,@AccessTime)",
            new SQLiteParameter("@Name", name),
            new SQLiteParameter("@Content", content),
            new SQLiteParameter("@Path", path),
             new SQLiteParameter("@CreateTime", createTime),
             new SQLiteParameter("@ModifyTime", modifyTime),
             new SQLiteParameter("@AccessTime", accessTime)
            );

            //int re = SQLiteHelper.ExecuteNonQuery("INSERT INTO report (Name,Path,Content,AccessTime)"
            //+ "  VALUES  (@Name,@Path,@Content,@AccessTime)",
            //new SQLiteParameter("@Name", name),
            //new SQLiteParameter("@Path", path),
            //new SQLiteParameter("@Content", content),
            // new SQLiteParameter("@AccessTime", accessTime)
            //);
            if (re > 0)
                return true;
            return false;
        }

        public DataSet GetreportLimitN(int n)
        {
            List<Tuple<string, string>> datas = new List<Tuple<string, string>>();
            //string cmd = "SELECT * FROM report ORDER BY ID DESC LIMIT 0," + n.ToString();
            string cmd = "SELECT ID, Name, Path, CreateTime,ModifyTime,AccessTime FROM TestData ORDER BY ID DESC LIMIT 0," + n.ToString();
            DataSet ds = SQLiteHelper.ExecuteDataSet(cmd);
            return ds;
        }

        public object GetReportSingleContent(string name = "")
        {
            //string cmd = "SELECT Path FROM report where Name = @Name";
            //object obj = SQLiteHelper.ExecuteScalar(cmd, new SQLiteParameter("@Name", name));

            //List<byte[]> list = new List<byte[]>();

            string cmd = "SELECT Content FROM TestData where Name = @Name ORDER BY ID DESC";
            //string cmd = "SELECT ID FROM TestData where Name = @Name ORDER BY ID DESC";
            object ob = SQLiteHelper.ExecuteScalar(cmd, new SQLiteParameter("@Name", name));

            return ob;
        }

        public DataSet GetReportSingle(string name = "")
        {
            string cmd = "SELECT * FROM TestData where Name = @Name ORDER BY ID DESC ";
            DataSet ds = SQLiteHelper.ExecuteDataSet(cmd, new SQLiteParameter("@Name", name));
            return ds;
        }


        public List<byte[]> GetReport()
        {
            List<byte[]> list = new List<byte[]>();
            string cmd = "SELECT * FROM report ";
            DataSet ds = SQLiteHelper.ExecuteDataSet(cmd);
            if (ds == null || ds.Tables.Count == 0)
                return null;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var b = ds.Tables[0].Rows[i]["Content"] as byte[];
                list.Add(b);
            }
            return list;
        }

        //public List<ReportDbModel> GetReportEx()
        //{
        //    List<ReportDbModel> list = new List<ReportDbModel>();
        //    string cmd = "SELECT * FROM report";
        //    DataSet ds = SQLiteHelper.ExecuteDataSet(cmd);
        //    if (ds == null || ds.Tables.Count == 0)
        //        return null;
        //    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //    {
        //        ReportDbModel rdbm = new ReportDbModel();
        //        rdbm.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"]);
        //        rdbm.Name = ds.Tables[0].Rows[i]["Name"].ToString();
        //        rdbm.Date = ds.Tables[0].Rows[i]["SaveTime"].ToString();
        //        //rdbm.Content = ds.Tables[0].Rows[i]["Content"] as byte[];
        //        list.Add(rdbm);
        //    }
        //    return list;
        //}

    }
}
