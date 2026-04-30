using Microsoft.Data.SqlClient;
using System.Data;
using ERPSample.Models;
namespace ERPSample.DAL.General.Companies
{
    public class CompaniesOperations
    {

        //This is for getting connection string for all functions in this main class
        String ConnectionString;
        public CompaniesOperations(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }

        //Inorder to display the main table when we click on the menu
        public DataTable Fill()
        {
            SqlCommand Cmd = new SqlCommand();
            Cmd.Connection = new SqlConnection(ConnectionString);
            Cmd.CommandText = "SELECT ID,Nature,Company,TelephoneNo,MobileNo,Convert(bit,ActiveFlag) AS Active FROM MaCompanies";
            DataTable dt = new DataTable();
            new SqlDataAdapter(Cmd).Fill(dt);
            return dt;
        }

        //When we click on new entry we will get the users by default using this function
        //Users or Contact person ID is FK from MaEmployees table 
        public DataSet ContactPersonNew()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT ID,FirstName FROM MaEmployees", new SqlConnection(ConnectionString));
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        //For inserting into database
        public int Insert(ERPSample.Models.General.Companies.MainCompanies maincompanies)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("INSERT INTO MaCompanies(Company,Nature,TelephoneNo,MobileNo,AddressLineOne,AddressLineTwo,UniqueID,Reference,BankCode,ContactPersonID,ActiveFlag,Country,CreatedOn,HeaderImage,FooterImage,BranchCompanyID,CreatedBy) VALUES " +
                    "(@Company,@Nature,@TelephoneNo,@MobileNo,@AddressLineOne,@AddressLineTwo,@UniqueID,@Reference,@BankCode,@ContactPersonID,@ActiveFlag,@Country,@CreatedOn,@HeaderImage,@FooterImage,@BranchCompanyID,@CreatedBy)", conn);
                Cmd.Parameters.AddWithValue("@Company", maincompanies.Company);
                Cmd.Parameters.AddWithValue("@AddressLineOne", maincompanies.AddressLineOne);
                Cmd.Parameters.AddWithValue("@ActiveFlag", maincompanies.ActiveFlag);

                //The not null fields in the DB that we will not provide any values in our project will set as DbNull.value
                //and also all the other non manadatory ields ar aslo set to DBnull.value.Then only we could insert the data to DB.
                if (maincompanies.Country == null)
                {
                    Cmd.Parameters.AddWithValue("@Country", maincompanies.Country = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Country", maincompanies.Country);
                }
                if (maincompanies.CreatedOn == null)
                {
                    Cmd.Parameters.AddWithValue("@CreatedOn", DBNull.Value);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@CreatedOn", maincompanies.CreatedOn = DateTime.Now);
                }
                if (maincompanies.Nature == null)
                {
                    Cmd.Parameters.AddWithValue("@Nature", maincompanies.Nature = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Nature", maincompanies.Nature);
                }
                if (maincompanies.TelephoneNo == null)
                {
                    Cmd.Parameters.AddWithValue("@TelephoneNo", maincompanies.TelephoneNo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TelephoneNo", maincompanies.TelephoneNo);
                }
                if (maincompanies.MobileNo == null)
                {
                    Cmd.Parameters.AddWithValue("@MobileNo", maincompanies.MobileNo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@MobileNo", maincompanies.MobileNo);
                }
                if (maincompanies.AddressLineTwo == null)
                {
                    Cmd.Parameters.AddWithValue("@AddressLineTwo", maincompanies.AddressLineTwo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@AddressLineTwo", maincompanies.AddressLineTwo);
                }
                if (maincompanies.UniqueID == null)
                {
                    Cmd.Parameters.AddWithValue("@UniqueID", maincompanies.UniqueID = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@UniqueID", maincompanies.UniqueID);
                }
                if (maincompanies.Reference == null)
                {
                    Cmd.Parameters.AddWithValue("@Reference", maincompanies.Reference = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Reference", maincompanies.Reference);
                }
                if (maincompanies.BankCode == null)
                {
                    Cmd.Parameters.AddWithValue("@BankCode", maincompanies.BankCode = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@BankCode", maincompanies.BankCode);
                }
                if (maincompanies.BankCode == null)
                {
                    Cmd.Parameters.AddWithValue("@ContactPersonID", maincompanies.ContactPersonID = Convert.ToInt32(DBNull.Value));
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ContactPersonID", maincompanies.ContactPersonID);
                }
                if (maincompanies.HeaderImage == null)
                {
                    Cmd.Parameters.AddWithValue("@HeaderImage", maincompanies.HeaderImage = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@HeaderImage", maincompanies.HeaderImage);
                }
                if (maincompanies.FooterImage == null)
                {
                    Cmd.Parameters.AddWithValue("@FooterImage", maincompanies.FooterImage = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@FooterImage", maincompanies.FooterImage);
                }
                Cmd.Parameters.AddWithValue("@BranchCompanyID", maincompanies.BranchCompanyID);
                Cmd.Parameters.AddWithValue("@CreatedBy", maincompanies.CreatedBy);
                Cmd.Connection.Open();
                int x = Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                return x;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //When double click on row the form should be shown and the values of that row also should be shown
        public DataSet UpdateRow(int ID)
        {
            SqlCommand Cmd = new SqlCommand();
            Cmd.Connection = new SqlConnection(ConnectionString);
            Cmd.CommandText = "SELECT ID,Company,Nature,TelephoneNo,MobileNo,AddressLineOne,AddressLineTwo,UniqueID,Reference,BankCode,ContactPersonID,ActiveFlag,HeaderImage,FooterImage FROM MaCompanies where ID=@ID;" +
                "SELECT * FROM MaEmployees ;";  //This Select command is for getting the same contact person in that row at the time of edit
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataSet ds = new DataSet();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }

        //For updating data base
        public int Update(Models.General.Companies.MainCompanies maincompanies)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand Cmd = new SqlCommand("UPDATE MaCompanies SET Company=@Company,Nature=@Nature,TelephoneNo=@TelephoneNo,MobileNo=@MobileNo,AddressLineOne=@AddressLineOne,AddressLineTwo=@AddressLineTwo,UniqueID=@UniqueID,Reference=@Reference,BankCode=@BankCode,ContactPersonID=@ContactPersonID,ActiveFlag=@ActiveFlag,HeaderImage=@HeaderImage,FooterImage=@FooterImage Where ID=@ID;", conn);
                Cmd.Parameters.AddWithValue("@ID", maincompanies.ID);
                Cmd.Parameters.AddWithValue("@Company", maincompanies.Company);
                Cmd.Parameters.AddWithValue("@AddressLineOne", maincompanies.AddressLineOne);
                Cmd.Parameters.AddWithValue("@ActiveFlag", maincompanies.ActiveFlag);

                //The not null fields in the DB that we will not provide any values in our project will set as DbNull.value
                //and also all the other non manadatory fields ar aslo set to DBnull.value.Then only we could insert the data to DB.
                if (maincompanies.Country == null)
                {
                    Cmd.Parameters.AddWithValue("@Country", maincompanies.Country = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Country", maincompanies.Country);
                }
                if (maincompanies.CreatedOn == null)
                {
                    Cmd.Parameters.AddWithValue("@CreatedOn", DBNull.Value);
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@CreatedOn", maincompanies.CreatedOn = DateTime.Now);
                }
                if (maincompanies.Nature == null)
                {
                    Cmd.Parameters.AddWithValue("@Nature", maincompanies.Nature = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Nature", maincompanies.Nature);
                }
                if (maincompanies.TelephoneNo == null)
                {
                    Cmd.Parameters.AddWithValue("@TelephoneNo", maincompanies.TelephoneNo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@TelephoneNo", maincompanies.TelephoneNo);
                }
                if (maincompanies.MobileNo == null)
                {
                    Cmd.Parameters.AddWithValue("@MobileNo", maincompanies.MobileNo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@MobileNo", maincompanies.MobileNo);
                }
                if (maincompanies.AddressLineTwo == null)
                {
                    Cmd.Parameters.AddWithValue("@AddressLineTwo", maincompanies.AddressLineTwo = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@AddressLineTwo", maincompanies.AddressLineTwo);
                }
                if (maincompanies.UniqueID == null)
                {
                    Cmd.Parameters.AddWithValue("@UniqueID", maincompanies.UniqueID = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@UniqueID", maincompanies.UniqueID);
                }
                if (maincompanies.Reference == null)
                {
                    Cmd.Parameters.AddWithValue("@Reference", maincompanies.Reference = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@Reference", maincompanies.Reference);
                }
                if (maincompanies.BankCode == null)
                {
                    Cmd.Parameters.AddWithValue("@BankCode", maincompanies.BankCode = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@BankCode", maincompanies.BankCode);
                }
                if (maincompanies.BankCode == null)
                {
                    Cmd.Parameters.AddWithValue("@ContactPersonID", maincompanies.ContactPersonID = Convert.ToInt32(DBNull.Value));
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@ContactPersonID", maincompanies.ContactPersonID);
                }
                if (maincompanies.HeaderImage == null)
                {
                    Cmd.Parameters.AddWithValue("@HeaderImage", maincompanies.HeaderImage = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@HeaderImage", maincompanies.HeaderImage);
                }
                if (maincompanies.FooterImage == null)
                {
                    Cmd.Parameters.AddWithValue("@FooterImage", maincompanies.FooterImage = DBNull.Value.ToString());
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@FooterImage", maincompanies.FooterImage);
                }
                Cmd.Connection.Open();
                int x = Cmd.ExecuteNonQuery();
                Cmd.Connection.Close();
                return x;
            }
            catch
            {
                return 0;
            }
        }

        //For deleting data from DB
        public int Delete(Models.General.Companies.MainCompanies maincompanies, string ImagePath)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.Connection = new SqlConnection(ConnectionString);
                Cmd.CommandText = "DELETE FROM MaCompanies WHERE ID= @ID";
                Cmd.Parameters.AddWithValue("@ID", maincompanies.ID);
                Cmd.Connection.Open();
                var headerimage = "";
                var footerimage = "";
                if (maincompanies.HeaderImage != DBNull.Value.ToString())
                    headerimage = DBNull.Value.ToString();
                if (maincompanies.FooterImage != DBNull.Value.ToString())
                    footerimage = DBNull.Value.ToString();
                int x = Cmd.ExecuteNonQuery();
                string file1 = Path.Combine(ImagePath, maincompanies.HeaderImage);
                string file2 = Path.Combine(ImagePath, maincompanies.FooterImage);
                if (File.Exists(file1))
                    File.Delete(file1);
                if (File.Exists(file2))
                    File.Delete(file2);
                Cmd.Connection.Close();
                return x;
            }
            catch (Exception) { throw; }
        }

        //To get company header and footer in account statement and trial balance 12/7/2023 
        public DataSet GetCompanyImages(int ID)
        {
            SqlCommand Cmd = new SqlCommand();
            Cmd.Connection = new SqlConnection(ConnectionString);
            Cmd.CommandText = "SELECT HeaderImage,FooterImage FROM MaCompanies where ID=@ID;";
            Cmd.Parameters.AddWithValue("@ID", ID);
            DataSet ds = new DataSet();
            new SqlDataAdapter(Cmd).Fill(ds);
            return ds;
        }
    }
}
