using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FirebirdSql.Data.Client;
using FirebirdSql.Data.FirebirdClient;
using Tmds.DBus.Protocol;

namespace VendingMachine.Models
{
    public class DataWorker : IDisposable
    {
        private const string connectionString = 
        @"DataSource=localhost;Port=3050;"
        +"Database=C:/Users/user/Desktop/Arina/IT/DataBase/VENDINGMACHINEDATABASE.FDB;"
        +"User=SYSDBA;Password=VryjvB6vjWn2GNV;Charset=NONE;";

        private FbConnection _connection;

        private static Random _random = new Random();
        public DataWorker()
        {
            try
            {
                _connection = new FbConnection(connectionString);
                _connection.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
            }
        }

        public List<User> GetAllUser()
        {
            List<User> users = new List<User>();
            string sql = "SELECT U.ID_USER, U.USERNAME, U.SURNAME, U.MIDDLE_NAME, U.EMAIL, U.PHONE, UR.USER_ROLE, U.USER_PASSWORD, U.ICON, U.IS_BLOCKED FROM USER_TABLE U JOIN USER_ROLE UR ON U.ROLE_ID = UR.ID_ROLE;";
            using (var command = new FbCommand(sql, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string surname = reader.GetString(2);
                    string middlename = reader.GetString(3);
                    string email = reader.GetString(4);
                    string phone = reader.GetString(5);
                    string role = reader.GetString(6);
                    string password = reader.GetString(7);
                    byte[] picture = null;
                    if (!reader.IsDBNull(8))
                    {
                        long length = reader.GetBytes(8, 0, null, 0, 0);
                        picture = new byte[length];
                        reader.GetBytes(8, 0, picture, 0, (int)length);
                    }
                    bool isBlocked = reader.GetBoolean(9);
                    users.Add(new User(id, name, surname, middlename, email, phone, role, password, picture, isBlocked));
                    
                }
            }
            return users;
        }
        public List<VM> GetAllVM()
        {
            List<VM> vMs = new List<VM>();
            string sql = "SELECT VM.ID_VENDING_MACHINE, L.FULL_ADDRESS, L.PLACE_DESCRIPTION, M.MODEL, M.BRAND, S.STATUS, VM.VENDING_MACHINE_NAME, C.COMPANY_CODE, C.COMPANY_NAME, VM.MODEM, VM.INSTALLATION_DATE FROM VENDING_MACHINE VM " 
            +"JOIN LOCATION L ON VM.LOCATION_ID = L.ID_LOCATION "
            +"JOIN VENDING_MACHINE_MODEL M ON VM.MODEL_ID = M.ID_MODEL "
            +"JOIN VENDING_MACHINE_STATUS S ON VM.STATUS_ID = S.ID_STATUS "
            +"JOIN COMPANY C ON VM.COMPANY_ID = C.COMPANY_CODE;";
            using (var command = new FbCommand(sql, _connection))
            using (var reader = command.ExecuteReader())
            { 
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string address = reader.GetString(1);
                    string placeDescription = reader.GetString(2);
                    string model = reader.GetString(3);
                    string brand = reader.GetString(4);
                    string status = reader.GetString(5);
                    string name = reader.GetString(6);
                    int companyCode = reader.GetInt32(7);
                    string companyName = reader.GetString(8);
                    int modem = reader.GetInt32(9);
                    DateOnly installationDate = DateOnly.FromDateTime(reader.GetDateTime(10));

                    vMs.Add(new VM(id, address, placeDescription, model, brand, status, name, companyCode, companyName, modem, installationDate));
                }
            }
            return vMs;
        }
        public List<Sale> GetAllSales()
        {
            List<Sale> sales = new List<Sale>();
            string sql = "SELECT S.ID_SALE, P.PRICE, S.SALED_PRODUCT_COUNT, S.SALE_DATETIME "
            +"FROM SALE S JOIN PRODUCT P ON S.PRODUCT_ID = P.ID_PRODUCT;";
            using(var command = new FbCommand(sql, _connection))
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    float price = reader.GetFloat(1);
                    int count = reader.GetInt32(2);
                    DateTime dateTime = reader.GetDateTime(3);

                    sales.Add(new Sale(id, count, (float)price*count, dateTime));
                }
            }
            return sales;
        }
        public List<Model> GetAllModel()
        {
            List<Model> models = new List<Model>();
            string sql = "SELECT ID_MODEL, BRAND, MODEL "
            +"FROM VENDING_MACHINE_MODEL;";
            using(var command = new FbCommand(sql, _connection))
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string brand = reader.GetString(1);
                    string model = reader.GetString(2);
                
                    models.Add(new Model(id, brand, model));
                }
            }
            return models;
        }
        public List<Critical_Threshold_Rule> GetAllRule ()
        {
            List<Critical_Threshold_Rule> rules = new List<Critical_Threshold_Rule>();
            string sql = "SELECT ID_CRITICAL_THRESHOLD_RULE, NAME "
            +"FROM CRITICAL_THRESHOLD_RULE;";
            using(var command = new FbCommand(sql, _connection))
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                
                    rules.Add(new Critical_Threshold_Rule(id, name));
                }
            }
            return rules;
        }
        public List<Product_Matrix> GetAllProductMatrix ()
        {
            List<Product_Matrix> matrices = new List<Product_Matrix>();
            string sql = "SELECT ID_PRODUCT_MATRIX, NAME "
            +"FROM PRODUCT_MATRIX;";
            using(var command = new FbCommand(sql, _connection))
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                
                    matrices.Add(new Product_Matrix(id, name));
                }
            }
            return matrices;
        }
        public List<Notification_Template> GetAllNotificationTemplate ()
        {
            List<Notification_Template> templates = new List<Notification_Template>();
            string sql = "SELECT ID_NOTIFICATION_TEMPLATE, NAME "
            +"FROM NOTIFICATION_TEMPLATE;";
            using(var command = new FbCommand(sql, _connection))
            using(var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                
                    templates.Add(new Notification_Template(id, name));
                }
            }
            return templates;
        }
        public List<Company> GetAllCompanies()
        {
            List<Company> companies = new List<Company>();
            string sql = "SELECT C.COMPANY_CODE, C.COMPANY_NAME, C.ADDRESS, C.WEBSITE, C.RESPONSIBLE_PERSON_ID, U.USERNAME, U.SURNAME, U.MIDDLE_NAME, U.PHONE, U.EMAIL, C.STATUS "
            +"FROM COMPANY C JOIN USER_TABLE U ON C.RESPONSIBLE_PERSON_ID = ID_USER;";
            using (var command = new FbCommand(sql, _connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    string address = reader.GetString(2);
                    string website = reader.GetString(3);
                    int userId = reader.GetInt32(4);
                    string uName = reader.GetString(5);
                    string uSurname = reader.GetString(6);
                    string uMiddlename = reader.GetString(7);
                    string phone = reader.GetString(8);
                    string email = reader.GetString(9);
                    string status = reader.GetString(10);

                    companies.Add(new Company(id, name, address, website, userId, uName, uSurname, uMiddlename, phone, email, status));
                }
            }
            return companies;
        }
        public void DeleteCompany(int companyCode)
        {
            string sql = @"DELETE FROM COMPANY WHERE COMPANY_CODE = @companyCode;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@companyCode", companyCode);
            int affected = command.ExecuteNonQuery();
        }
        public void DeleteVM(int id)
        {
            string sql = @"DELETE FROM VENDING_MACHINE WHERE ID_VENDING_MACHINE = @id;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@id", id);
            int affected = command.ExecuteNonQuery();
        }
        public void UpdateIsBlocked(int id, bool isBlocked)
        {
            string sql = @"UPDATE USER_TABLE SET IS_BLOCKED = @isBlocked WHERE ID_USER = @id;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@isBlocked", isBlocked);
            command.Parameters.AddWithValue("@id", id);
            int affected = command.ExecuteNonQuery();
        }
        public void UpdateUserPassword(int id, string password)
        {
            string sql = @"UPDATE USER_TABLE SET USER_PASSWORD = @password WHERE ID_USER = @id;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@id", id);
            int affected = command.ExecuteNonQuery();
        }
        public void UpdateCompanyInfo(int code, string name, string address, string website)
        {
            string sql = "UPDATE COMPANY SET COMPANY_NAME = @name, ADDRESS = @address, WEBSITE = @website WHERE COMPANY_CODE = @code;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@address", address);
            command.Parameters.AddWithValue("@website", website);
            command.Parameters.AddWithValue("@code", code);
            int affected = command.ExecuteNonQuery();
        }
        public void UpdateCompanyStatus(int code, string status)
        {
            string sql = "UPDATE COMPANY SET STATUS = @status WHERE COMPANY_CODE = @code;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@code", code);
            int affected = command.ExecuteNonQuery();
        }
        public void UpdateModem(int id)
        {
            string sql = @"UPDATE VENDING_MACHINE SET MODEM = -1 WHERE ID_VENDING_MACHINE = @id;";
            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@id", id);
            int affected = command.ExecuteNonQuery();
        }
        public int GeneratedUniqueCompanyCode()
        {
            int newCode = 0;
            bool isUnique = false;
            int maxAttempts = 100;
            int attepmt = 0;
            while (!isUnique && attepmt < maxAttempts)
            {
                newCode = _random.Next(100000000, 1000000000);
                using (var command = new FbCommand("SELECT COUNT(*) FROM COMPANY WHERE COMPANY_CODE = @code", _connection))
                {
                    command.Parameters.AddWithValue("@code", newCode);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count == 0)
                    {
                        isUnique = true;
                    }
                }
                attepmt++;
            }
            if (!isUnique)
            {
                throw new Exception($"Не удалось сгенерировать уникальный код после {attepmt} попыток");
                
            }
            return newCode;
        }
        public void CreateCompany(int companyCode, string companyName, string companyAddress, string companyWebsite, string status,
        string name, string surname, string middlename, string email, string phone)
        {
            int userId;
            using var command1 = new FbCommand("SELECT FIRST 1 ID_USER FROM USER_TABLE ORDER BY ID_USER DESC;", _connection);
            userId = Convert.ToInt32(command1.ExecuteScalar()) + 1;
            Console.WriteLine(userId);
            
            string sql = "INSERT INTO USER_TABLE (ID_USER, USERNAME, SURNAME, MIDDLE_NAME, EMAIL, PHONE, ROLE_ID)"
            +"VALUES(@userId, @name, @surname, @middlename, @email, @phone, 2);";
            using var command2 = new FbCommand(sql, _connection);
            command2.Parameters.AddWithValue("@userId", userId);
            command2.Parameters.AddWithValue("@name", name);
            command2.Parameters.AddWithValue("@surname", surname);
            command2.Parameters.AddWithValue("@middlename", middlename);
            command2.Parameters.AddWithValue("@email", email);
            command2.Parameters.AddWithValue("@phone", phone);
            int affected = command2.ExecuteNonQuery();

            sql = "INSERT INTO COMPANY (COMPANY_CODE, COMPANY_NAME, ADDRESS, WEBSITE, RESPONSIBLE_PERSON_ID, STATUS)"
            +"VALUES (@companyCode, @companyName, @companyAddress, @companyWebsite, @userId, @status)";

            using var command = new FbCommand(sql, _connection);
            command.Parameters.AddWithValue("@companyCode", companyCode);
            command.Parameters.AddWithValue("@companyName", companyName);
            command.Parameters.AddWithValue("@companyAddress", companyAddress);
            command.Parameters.AddWithValue("@companyWebsite", companyWebsite);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@status", status);
            affected = command.ExecuteNonQuery();
        }
        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}