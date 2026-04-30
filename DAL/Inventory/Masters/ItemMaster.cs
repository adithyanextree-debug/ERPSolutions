using ERPSample.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ERPSample.DAL.Inventory.Masters
{
    public class ItemMaster
    {

        String ConnectionString;
        public ItemMaster(String ConnectionString = "")
        {
            this.ConnectionString = ConnectionString;
        }
        //   private static IConfigurationRoot configuration = new ConfigurationBuilder()
        //.SetBasePath(Directory.GetCurrentDirectory())
        //.AddJsonFile("appsettings.json")
        //.Build();

        //   public static string MasterConnectionString
        //   {
        //       get
        //       {
        //           return $"Data Source={configuration.GetConnectionString("Master Data Source")};" +
        //                  $"Initial Catalog={configuration.GetConnectionString("Master Database")};" +
        //                  "User ID=sa;Password=yourPassword;";
        //       }
        //   }

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        //return $"Data Source=HP\\SQLEXPRESS;Initial Catalog=NextreeSystemMAIN;User ID=nextree;Password=Nextree@4313$;";
        //        return $"Data Source=HP\\SQLEXPRESS;Initial Catalog=NextreeSystemMAIN;User ID=nextree;Password=Nextree@4313$;TrustServerCertificate=True;";
        //    }
        //}

        public DataTable Fill(string? item, string? category, string? brand, string? ModelNo, string? Barcode)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("ItemMasterExtSP", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Item", item ?? null);
                cmd.Parameters.AddWithValue("@Category", category ?? null);
                cmd.Parameters.AddWithValue("@Brand", brand ?? null);
                cmd.Parameters.AddWithValue("@ModelNo", ModelNo ?? null);
                cmd.Parameters.AddWithValue("@Barcode", Barcode ?? null);
                cmd.Parameters.AddWithValue("@Mode", 5);

                con.Open();
                da.Fill(dt);
            }

            return dt;
        }


        public DataSet NewProductDetails()
        {
            DataSet ds = new DataSet();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("ItemMasterExtSP", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", 8);

                con.Open();
                da.Fill(ds);
            }

            return ds;
        }


        public DataTable UnitNewRow()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("ItemMasterExtSP", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", 9);

                con.Open();
                da.Fill(dt);
            }

            return dt;
        }

        public string SaveUnitDetails(
    ERPSample.Models.Inventory.Masters.InvItemUnits entry,
    SqlConnection Conn,
    SqlTransaction Tx)
        {
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Conn, Tx))
            {
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Mode", 1);
                Cmd.Parameters.AddWithValue("@ID", entry.ID);
                Cmd.Parameters.AddWithValue("@Active", entry.Active);
                Cmd.Parameters.AddWithValue("@IsDefault", entry.IsDefault);
                Cmd.Parameters.AddWithValue("@ItemID", entry.ItemID);
                Cmd.Parameters.AddWithValue("@Unit", entry.Unit);
                Cmd.Parameters.AddWithValue("@BasicUnit", entry.BasicUnit);
                Cmd.Parameters.AddWithValue("@Factor", 1);
                Cmd.Parameters.AddWithValue("@SellingPrice", entry.SellingPrice);

                Cmd.Parameters.AddWithValue("@Barcode",
                    string.IsNullOrWhiteSpace(entry.Barcode) ? DBNull.Value : entry.Barcode);

                Cmd.Parameters.AddWithValue("@PurchaseRate",
                    string.IsNullOrWhiteSpace(entry.PurchaseRate?.ToString()) ? DBNull.Value : entry.PurchaseRate);

                Cmd.Parameters.AddWithValue("@PromotionPrice",
                    string.IsNullOrWhiteSpace(entry.PromotionPrice?.ToString()) ? DBNull.Value : entry.PromotionPrice);

                Cmd.Parameters.AddWithValue("@OnlinePrice",
                    string.IsNullOrWhiteSpace(entry.OnlinePrice?.ToString()) ? DBNull.Value : entry.OnlinePrice);

                Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.InputOutput
                });

                Cmd.ExecuteNonQuery();
            }

            return "true";
        }


        public string SaveBarcodeDetails(
            ERPSample.Models.Inventory.Masters.InvItemBarcodes barcode,
            SqlConnection Conn,
            SqlTransaction Tx)
        {
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Conn, Tx))
            {
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Mode", 2);
                Cmd.Parameters.AddWithValue("@ID", barcode.ID);
                Cmd.Parameters.AddWithValue("@Active", barcode.Active);
                Cmd.Parameters.AddWithValue("@ItemID", barcode.ItemID);
                Cmd.Parameters.AddWithValue("@UnitID", barcode.UnitID);
                Cmd.Parameters.AddWithValue("@Barcode", barcode.Barcode);

                Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.InputOutput
                });

                Cmd.ExecuteNonQuery();
            }

            return "true";
        }

        //public string SaveImageDetails(Int64 ID, string Column, string Value)
        //{
        //    SqlConnection Con = new SqlConnection(ConnectionString);
        //    try
        //    {
        //        SqlCommand cmd1 = new SqlCommand();
        //        cmd1.Connection = new SqlConnection(ConnectionString);
        //        if (Column == "Title")
        //        {
        //            cmd1.CommandText = "UPDATE InvItemImages SET Title =  @Value WHERE ID = @ID";

        //        }
        //        else if (Column == "ArabicTitle")
        //        {
        //            cmd1.CommandText = "UPDATE InvItemImages SET ArabicTitle = @Value WHERE ID = @ID";

        //        }
        //        else if (Column == "IsDefault")
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = new SqlConnection(ConnectionString);
        //            cmd.CommandText = "select ItemID from InvItemImages where ID = @ItemId; SELECT SCOPE_IDENTITY()";
        //            cmd.Parameters.AddWithValue("@ItemId", ID);
        //            cmd.Connection.Open();
        //            object Details = cmd.ExecuteScalar();
        //            cmd.Parameters.Clear();
        //            cmd.Connection.Close();
        //            cmd1.CommandText = "UPDATE InvItemImages set IsDefault = 0 where ItemID = @ItemID;UPDATE InvItemImages SET IsDefault = @Value WHERE ID = @ID";
        //            cmd1.Parameters.AddWithValue("@ItemID", int.Parse(Details.ToString()));
        //        }
        //        else if (Column == "Active")
        //        {
        //            cmd1.CommandText = "UPDATE InvItemImages SET Active = @Value WHERE ID = @ID";
        //        }
        //        else if (Column == "unitActive")
        //        {
        //            cmd1.CommandText = "UPDATE InvItemUnits SET Active = @Value WHERE ID = @ID";
        //        }
        //        else if (Column == "unitDefault")
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = new SqlConnection(ConnectionString);
        //            cmd.CommandText = "select ItemID from InvItemUnits where ID = @ItemId; SELECT SCOPE_IDENTITY()";
        //            cmd.Parameters.AddWithValue("@ItemId", ID);
        //            cmd.Connection.Open();
        //            object Details = cmd.ExecuteScalar();
        //            cmd.Parameters.Clear();
        //            cmd.Connection.Close();
        //            cmd1.CommandText = "UPDATE InvItemUnits set IsDefault = 0 where ItemID = @ItemID;UPDATE InvItemUnits SET IsDefault = @Value WHERE ID = @ID";
        //            cmd1.Parameters.AddWithValue("@ItemID", int.Parse(Details.ToString()));
        //        }
        //        cmd1.Parameters.AddWithValue("@Value", Value);
        //        cmd1.Parameters.AddWithValue("@ID", ID);
        //        cmd1.Connection.Open();
        //        object data = cmd1.ExecuteNonQuery();
        //        cmd1.Parameters.Clear();
        //        cmd1.Connection.Close();
        //        if (data != null)
        //        {
        //            return "true";
        //        }
        //        else
        //        {
        //            return "Unable to process the request";
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        if (Con != null && Con.State == ConnectionState.Open)
        //        {
        //            Con.Close();
        //            return Ex.Message;

        //        }
        //        return Ex.Message;
        //    }
        //}
        // ------------------------------------------------------------
        // 1) Save Item Images
        // ------------------------------------------------------------
        public string SaveItemImages(
            ERPSample.Models.Inventory.Masters.InvItemImages entry,
            SqlConnection Conn,
            SqlTransaction Tx)
        {
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Conn, Tx))
            {
                Cmd.CommandType = CommandType.StoredProcedure;

                Cmd.Parameters.AddWithValue("@Mode", 6);
                Cmd.Parameters.AddWithValue("@ID", entry.ID);
                Cmd.Parameters.AddWithValue("@Active", entry.Active);
                Cmd.Parameters.AddWithValue("@IsDefault", entry.IsDefault);
                Cmd.Parameters.AddWithValue("@ItemID", entry.ItemID);
                Cmd.Parameters.AddWithValue("@Title", entry.Title);
                Cmd.Parameters.AddWithValue("@ArabicTitle", entry.ArabicTitle);
                Cmd.Parameters.AddWithValue("@ImageSize", entry.ImageSize);
                Cmd.Parameters.AddWithValue("@ImagePath", entry.ImagePath);
                Cmd.Parameters.AddWithValue("@OrderNo", entry.OrderNo);

                Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.InputOutput
                });

                Cmd.ExecuteNonQuery();
            }

            return "true";
        }



        // ------------------------------------------------------------
        // 2) Insert Entry (Item Master Insert)
        // ------------------------------------------------------------
        public string InsertEntry(Models.Inventory.Masters.ItemMaster ItemMaster,SqlConnection Conn,SqlTransaction Tx)
        {
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Conn, Tx))
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@Mode", 7);

                // Required
                Cmd.Parameters.AddWithValue("@ID", ItemMaster.InvItemMaster.ID);
                Cmd.Parameters.AddWithValue("@ItemName", ItemMaster.InvItemMaster.ItemName);
                Cmd.Parameters.AddWithValue("@ItemCode", ItemMaster.InvItemMaster.ItemCode);
                Cmd.Parameters.AddWithValue("@Active", ItemMaster.InvItemMaster.Active);

                // Optional
                Cmd.Parameters.AddWithValue("@PartNo", ItemMaster.InvItemMaster.PartNo);
                Cmd.Parameters.AddWithValue("@OEMNo", ItemMaster.InvItemMaster.OEMNo);
                Cmd.Parameters.AddWithValue("@Unit", ItemMaster.InvItemMaster.Unit);
                Cmd.Parameters.AddWithValue("@ArabicName", ItemMaster.InvItemMaster.ItemName);
                Cmd.Parameters.AddWithValue("@CategoryID", ItemMaster.InvItemMaster.CategoryID);
                Cmd.Parameters.AddWithValue("@BrandID", ItemMaster.InvItemMaster.BrandID);
                Cmd.Parameters.AddWithValue("@ColorID", ItemMaster.InvItemMaster.ColorID);
                Cmd.Parameters.AddWithValue("@ArticleID", ItemMaster.InvItemMaster.ArticleID);
                Cmd.Parameters.AddWithValue("@SizeID", ItemMaster.InvItemMaster.SizeID);
                Cmd.Parameters.AddWithValue("@ModelNo", ItemMaster.InvItemMaster.ModelNo);
                Cmd.Parameters.AddWithValue("@Manufacturer", ItemMaster.InvItemMaster.Manufacturer);
                Cmd.Parameters.AddWithValue("@PurchaseUnit", ItemMaster.InvItemMaster.PurchaseUnit);
                Cmd.Parameters.AddWithValue("@SellingUnit", ItemMaster.InvItemMaster.SellingUnit);
                Cmd.Parameters.AddWithValue("@StockItem", ItemMaster.InvItemMaster.StockItem);
                Cmd.Parameters.AddWithValue("@IsExpiry", ItemMaster.InvItemMaster.IsExpiry);
                Cmd.Parameters.AddWithValue("@Weight", ItemMaster.InvItemMaster.Weight);
                Cmd.Parameters.AddWithValue("@ExpiryPeriod", ItemMaster.InvItemMaster.ExpiryPeriod);
                Cmd.Parameters.AddWithValue("@Remarks", ItemMaster.InvItemMaster.Remarks);
                Cmd.Parameters.AddWithValue("@LongDescription", ItemMaster.InvItemMaster.LongDescription);
                Cmd.Parameters.AddWithValue("@ArabicLongDescription", ItemMaster.InvItemMaster.ArabicLongDescription);
                Cmd.Parameters.AddWithValue("@UrlName", ItemMaster.InvItemMaster.UrlName);
                Cmd.Parameters.AddWithValue("@SellOnEcommerce", ItemMaster.InvItemMaster.SellOnEcommerce);
                Cmd.Parameters.AddWithValue("@ShortDescription", ItemMaster.InvItemMaster.ShortDescription);
                Cmd.Parameters.AddWithValue("@ShortDescriptionArabic", ItemMaster.InvItemMaster.ShortDescriptionArabic);

                Cmd.Parameters.Add(new SqlParameter("@NewID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.InputOutput
                });

                Cmd.ExecuteNonQuery();

                string NewID = (ItemMaster.InvItemMaster.ID == null || ItemMaster.InvItemMaster.ID == 0)
                    ? Convert.ToString(Cmd.Parameters["@NewID"].Value)
                    : ItemMaster.InvItemMaster.ID.ToString();

                return NewID;
            }
        }



        // ------------------------------------------------------------
        // 3) Product Details
        // ------------------------------------------------------------
        public DataSet ProuctDetails(long ID)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Conn))
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@ID", ID);
                Cmd.Parameters.AddWithValue("@Mode", 0);

                DataSet Results = new DataSet();
                using (SqlDataAdapter Sda = new SqlDataAdapter(Cmd))
                {
                    Sda.Fill(Results);
                }

                return Results;
            }
        }



        // ------------------------------------------------------------
        // 4) Update Entry
        // ------------------------------------------------------------
        public string UpdateEntry(Models.Inventory.Masters.ItemMaster ItemMaster)
        {
            using (SqlConnection Conn = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand())
            {
                Conn.Open();
                Cmd.Connection = Conn;

                // Helper: handle nulls
                void AddParameter(string name, object value)
                {
                    Cmd.Parameters.AddWithValue(
                        name,
                        (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        ? DBNull.Value
                        : value
                    );
                }

                Cmd.CommandText = @"
            UPDATE InvItemMaster SET
                ItemName = @ItemName,
                ShortDescription = @ShortDescription,
                ShortDescriptionArabic = @ShortDescriptionArabic,
                ItemCode = @ItemCode,
                Active = @Active,
                PartNo = @PartNo,
                OEMNo = @OEMNo,
                Unit = @Unit,
                ArabicName = @ArabicName,
                CategoryID = @CategoryID,
                BrandID = @BrandID,
                ModelNo = @ModelNo,
                Manufacturer = @Manufacturer,
                PurchaseUnit = @PurchaseUnit,
                SellingUnit = @SellingUnit,
                StockItem = @StockItem,
                IsExpiry = @IsExpiry,
                Weight = @Weight,
                ExpiryPeriod = @ExpiryPeriod,
                Remarks = @Remarks,
                LongDescription = @LongDescription,
                ArabicLongDescription = @ArabicLongDescription,
                SellOnEcommerce = @SellOnEcommerce,
                UrlName = @UrlName
            WHERE ID = @ID;
        ";

                Cmd.Parameters.AddWithValue("@ID", ItemMaster.InvItemMaster.ID);
                Cmd.Parameters.AddWithValue("@ItemName", ItemMaster.InvItemMaster.ItemName);
                Cmd.Parameters.AddWithValue("@ItemCode", ItemMaster.InvItemMaster.ItemCode);
                Cmd.Parameters.AddWithValue("@Active", ItemMaster.InvItemMaster.Active);

                AddParameter("@ShortDescription", ItemMaster.InvItemMaster.ShortDescription);
                AddParameter("@ShortDescriptionArabic", ItemMaster.InvItemMaster.ShortDescriptionArabic);
                AddParameter("@PartNo", ItemMaster.InvItemMaster.PartNo);
                AddParameter("@OEMNo", ItemMaster.InvItemMaster.OEMNo);
                AddParameter("@Unit", ItemMaster.InvItemMaster.Unit);
                AddParameter("@ArabicName", ItemMaster.InvItemMaster.ArabicName);
                AddParameter("@CategoryID", ItemMaster.InvItemMaster.CategoryID);
                AddParameter("@BrandID", ItemMaster.InvItemMaster.BrandID);
                AddParameter("@ModelNo", ItemMaster.InvItemMaster.ModelNo);
                AddParameter("@Manufacturer", ItemMaster.InvItemMaster.Manufacturer);
                AddParameter("@PurchaseUnit", ItemMaster.InvItemMaster.PurchaseUnit);
                AddParameter("@SellingUnit", ItemMaster.InvItemMaster.SellingUnit);
                AddParameter("@StockItem", ItemMaster.InvItemMaster.StockItem);
                AddParameter("@IsExpiry", ItemMaster.InvItemMaster.IsExpiry);
                AddParameter("@Weight", ItemMaster.InvItemMaster.Weight);
                AddParameter("@ExpiryPeriod", ItemMaster.InvItemMaster.ExpiryPeriod);
                AddParameter("@Remarks", ItemMaster.InvItemMaster.Remarks);
                AddParameter("@LongDescription", ItemMaster.InvItemMaster.LongDescription);
                AddParameter("@ArabicLongDescription", ItemMaster.InvItemMaster.ArabicLongDescription);
                AddParameter("@SellOnEcommerce", ItemMaster.InvItemMaster.SellOnEcommerce);
                AddParameter("@UrlName", ItemMaster.InvItemMaster.UrlName);

                Cmd.ExecuteNonQuery();
                return ItemMaster.InvItemMaster.ID.ToString();
            }
        }


        // Delete Units
        public string DeletInvItemUnits(long ID)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Con))
            {
                try
                {
                    Con.Open();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Mode", 3);
                    Cmd.Parameters.AddWithValue("@ID", ID);

                    Cmd.ExecuteNonQuery();
                    return "true";
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }
            }
        }

        // Delete Barcodes
        public string DeletInvItemBarcodes(long ID)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Con))
            {
                try
                {
                    Con.Open();
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Mode", 4);
                    Cmd.Parameters.AddWithValue("@ID", ID);

                    Cmd.ExecuteNonQuery();
                    return "true";
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }
            }
        }

        // Delete Image Details
        public string DeleteImageDetails(long ID, string ImagePath)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Con))
            {
                try
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Mode", 10);
                    Cmd.Parameters.AddWithValue("@ID", ID);

                    SqlDataAdapter Adapter = new SqlDataAdapter(Cmd);
                    DataSet Results = new DataSet();
                    Adapter.Fill(Results);

                    if (Results.Tables.Count > 0)
                    {
                        foreach (DataRow dr in Results.Tables[0].Rows)
                        {
                            string file = Path.Combine(ImagePath, dr["ImagePath"].ToString());
                            if (File.Exists(file))
                                File.Delete(file);
                        }
                    }

                    return "true";
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }
            }
        }

        // Delete Item Master
        public string DeleteItemMaster(long ID, string ImagePath)
        {
            using (SqlConnection Con = new SqlConnection(ConnectionString))
            using (SqlCommand Cmd = new SqlCommand("ItemMasterExtSP", Con))
            {
                try
                {
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Mode", 11);
                    Cmd.Parameters.AddWithValue("@ID", ID);

                    SqlDataAdapter Adapter = new SqlDataAdapter(Cmd);
                    DataSet Results = new DataSet();
                    Adapter.Fill(Results);

                    DataTable DtDetails = Results.Tables.Count > 0 ? Results.Tables[0] : null;
                    DataTable DtImages = Results.Tables.Count > 1 ? Results.Tables[1] : null;

                    if (DtDetails == null || DtDetails.Rows.Count == 0)
                        return "Unable to track this record";

                    if (DtImages != null && DtImages.Rows.Count > 0)
                    {
                        foreach (DataRow dr in DtImages.Rows)
                        {
                            string file = Path.Combine(ImagePath, dr["ImagePath"].ToString());
                            if (File.Exists(file))
                                File.Delete(file);
                        }
                    }

                    return "true";
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }
            }
        }



        //For excel upload insert 

        public string InsertExcelEntry(Models.Inventory.Masters.ItemMaster ItemMaster, SqlConnection Conn, SqlTransaction Tx)
        {
            SqlCommand Cmd = new SqlCommand("ItemMasterExcelUploadSP", Conn, Tx);
            Cmd.CommandType = CommandType.StoredProcedure;

            Cmd.Parameters.AddWithValue("@Mode", 1);
            Cmd.Parameters.AddWithValue("@ProductName", ItemMaster.InvItemMasterExcel.ProductName);
            Cmd.Parameters.AddWithValue("@SKU", ItemMaster.InvItemMasterExcel.SKU);
            Cmd.Parameters.AddWithValue("@Category", ItemMaster.InvItemMasterExcel.Category);
            Cmd.Parameters.AddWithValue("@Brand", ItemMaster.InvItemMasterExcel.Brand);
            Cmd.Parameters.AddWithValue("@ENGLISHProductDescription", ItemMaster.InvItemMasterExcel.ENGLISHProductDescription);
            Cmd.Parameters.AddWithValue("@ARABICProductDescription", ItemMaster.InvItemMasterExcel.ARABICProductDescription);
            Cmd.Parameters.AddWithValue("@PRICE", ItemMaster.InvItemMasterExcel.PRICE);
            Cmd.Parameters.AddWithValue("@BarCode", ItemMaster.InvItemMasterExcel.BarCode);

            // InputOutput parameter for ItemID
            SqlParameter ItemIDParam = new SqlParameter("@ItemID", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.InputOutput,
                Value = ItemMaster.InvItemMasterExcel.ID == 0 ? (object)DBNull.Value : ItemMaster.InvItemMasterExcel.ID
            };
            Cmd.Parameters.Add(ItemIDParam);

            Cmd.ExecuteNonQuery();

            // Return new or existing ID
            string NewID = Convert.ToString(ItemIDParam.Value);
            return NewID;
        }


        public string SaveExcelItemImages(ERPSample.Models.Inventory.Masters.InvItemImages entry, SqlConnection Conn, SqlTransaction Tx)
        {
            SqlCommand Cmd = new SqlCommand("ItemMasterExcelUploadSP", Conn, Tx);
            Cmd.CommandType = CommandType.StoredProcedure;

            Cmd.Parameters.AddWithValue("@Mode", 2);
            Cmd.Parameters.AddWithValue("@IsDefault", entry.IsDefault);
            Cmd.Parameters.AddWithValue("@ItemID1", entry.ItemID);
            Cmd.Parameters.AddWithValue("@Title", entry.Title);
            Cmd.Parameters.AddWithValue("@ImageSize", entry.ImageSize);
            Cmd.Parameters.AddWithValue("@ImagePath", entry.ImagePath);
            Cmd.Parameters.AddWithValue("@OrderNo", entry.OrderNo);

            SqlParameter ItemIDParam = new SqlParameter("@ItemID", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.InputOutput,
                Value = entry.ItemID
            };
            Cmd.Parameters.Add(ItemIDParam);

            Cmd.ExecuteNonQuery();
            return "true";
        }

    }
}
