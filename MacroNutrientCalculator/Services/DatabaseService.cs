using Npgsql;   // Import Npgsql to interact with PostgreSQL
using Microsoft.Extensions.Configuration;   // Import to access configuration settings, like connection strings
using System;
using System.Collections.Generic;
using MacroNutrientCalc.Models;
using System.ComponentModel.DataAnnotations; // Ensure this namespace contains RegisterModel
// *** (May need to include additional using statements as needed) ***

namespace MacroNutrientCalc.Services
{
    public class DatabaseService    // Service responsible for managing database operations
    {
        private readonly string _connectionString = null!;  // Holds the connection string for managing database operations

        public DatabaseService(IConfiguration configuration)    // Constructor that initializes the DatabaseService with configuration settings
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); // Retrieve the "DefaultConnection" string from configuration (e.g., appsettings.json)
            InitializeDatabase();   // Ensure the users tables is initialized
            InitializeMeasurementsTable();  // Ensure the user measurements table is initialized

        }

        // Initializes the database by creating the 'users' table if it doesn't already exist
        private void InitializeDatabase()
        {
            using var conn = new NpgsqlConnection(_connectionString);   // Create a new connection using the connection string
            conn.Open();    // Open the database connection
            
            // Define the SQL query to create the users table if it doesn't exist
            string query = @"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    email VARCHAR(100) NOT NULL UNIQUE,
                    password VARCHAR(100) NOT NULL,
                    name VARCHAR(100),
                    date_of_birth DATE,
                    height NUMERIC
                );
            ";
            using var cmd = new NpgsqlCommand(query, conn); // Create a command with the SQL query and connection
            cmd.ExecuteNonQuery();  // Execute the command to create the table
        }

        // Initializes the database by creating the 'users measurements' table if it doesn't already exist
        private void InitializeMeasurementsTable()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"
                CREATE TABLE IF NOT EXISTS user_measurements (
                    id SERIAL PRIMARY KEY,
                    user_id INTEGER NOT NULL,
                    weight NUMERIC,
                    body_fat_percentage NUMERIC,
                    measurement_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                );
            ";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        // Adds a new measurement record for the given user. True if a record was inserted; otherwise, false.
        // This method creates the user_measurements table if it doesn't exist.
        public bool AddMeasurement(int userId, double? weight, double? bodyFatPercentage, DateTime timestamp)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Insert a new measurement record linked to the given user
            string insertQuery = @"
                INSERT INTO user_measurements (user_id, weight, body_fat_percentage, measurement_date)
                VALUES (@user_id, @weight, @body_fat_percentage, @timestamp);
            ";
            using var insertCmd = new NpgsqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("user_id", userId);
            // Use DBNull.Value if the parameter has no value
            insertCmd.Parameters.AddWithValue("weight", weight.HasValue ? (object)weight.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("body_fat_percentage", bodyFatPercentage.HasValue ? (object)bodyFatPercentage.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("timestamp", timestamp);  // Pass the user-specified timestamp

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        // Retrieves all measurement records for a given user (by user_id)
        public List<WeightLog> GetMeasurements(int userId)
        {
            var measurements = new List<WeightLog>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            string query = @"
                SELECT id, user_id, weight, body_fat_percentage, measurement_date
                FROM user_measurements
                WHERE user_id = @user_id
                ORDER BY measurement_date DESC;
            ";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("user_id", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // Convert NUMERIC fields to double? using Convert.ToDouble
                double? weight = reader.IsDBNull(2) ? (double?)null : Convert.ToDouble(reader.GetDecimal(2));
                double? bodyFatPercentage = reader.IsDBNull(3) ? (double?)null : Convert.ToDouble(reader.GetDecimal(3));
                // measurements.Add(new WeightLog *** Changed to code line below 4/8/25
                var log = new WeightLog
                {
                    Id = reader.GetInt32(0),        // Measurement record ID
                    UserId = reader.GetInt32(1),    // User ID (foreign key)
                    Weight = weight,                // Weight value
                    BodyFatPercentage = bodyFatPercentage,  // Body fat percentage value
                    Timestamp = reader.GetDateTime(4)       // Date/time of measurement
                };
                measurements.Add(log);
            }
            return measurements;
        }

        // Deletes a measurement record by its record ID
        public bool DeleteMeasurement(int measurementId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "DELETE FROM user_measurements WHERE id = @id;";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", measurementId);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        // Retrieves a user's information from the 'users' table by ID
        public RegisterModel? GetUserById(int userId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT email, password, name, date_of_birth, height FROM users WHERE id = @id;";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("id", userId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new RegisterModel
                {
                    Email = reader.GetString(0),
                    Password = reader.GetString(1),
                    Name = reader.GetString(2),
                    DateOfBirth = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    Height = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4)
                };
            }
            return null;
        }

        // Updates the user's information in the 'users' table
        // The userId is passed separately to ensure the correct record is updated
        public bool UpdateUser(RegisterModel user, int userId)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"
                UPDATE users
                SET email = @Email, password = @Password, name = @Name, date_of_birth = @DateOfBirth, height = @Height
                WHERE id = @Id;
            ";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("Email", user.Email);
            cmd.Parameters.AddWithValue("Password", user.Password);
            cmd.Parameters.AddWithValue("Name", user.Name);
            cmd.Parameters.AddWithValue("DateOfBirth", user.DateOfBirth.HasValue ? (object)user.DateOfBirth.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("Height", user.Height.HasValue ? (object)user.Height.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("Id", userId);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        // Creates a new user record in the database
        // Returns true if the user is created successfully, false if the user already exists
        public bool CreateUser(string email, string password, string name, DateTime? dateOfBirth, decimal? height)
        {
            // Create and open a new connection to the database
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // SQL query to check if a user with the given email already exists
            string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @email;";
            using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("email", email);   // Add the email parameter to the query
                long count = (long)checkCmd.ExecuteScalar();    // Execute the query and retrieve the count of matching users
                if (count > 0)  // If the user exists, return false to indicate creation failure
                {
                    return false;
                }
            }

            // SQL query to insert a new user record into the users table
            string insertQuery = @"
                INSERT INTO users (email, password, name, date_of_birth, height)
                VALUES (@email, @password, @name, @dateOfBirth, @height);
            ";
            using var insertCmd = new NpgsqlCommand(insertQuery, conn);     // Create a command for the INSERT query
            // Add parameters for each column in the insert statement
            insertCmd.Parameters.AddWithValue("email", email);
            insertCmd.Parameters.AddWithValue("password", password);
            insertCmd.Parameters.AddWithValue("name", (object?)name ?? DBNull.Value);   // Use DBNull.Value if 'name' is null
            insertCmd.Parameters.AddWithValue("dateOfBirth", dateOfBirth.HasValue ? (object)dateOfBirth.Value : DBNull.Value);  // If dateOfBirth has a value, use it; otherwise, use DBNull.Value
            insertCmd.Parameters.AddWithValue("height", height.HasValue ? (object)height.Value : DBNull.Value); // If height has a value, use it; otherwise, use DBNull.Value
            insertCmd.ExecuteNonQuery();    // Execute the insert command to create the new user record
            return true;    // Return true to indicate successful user creation
        }

        // Validates user credentials by checking if a user exists with the given email and password
        // Returns true if credentials are valid, otherwise false
        public bool ValidateUser(string email, string password)
        {
            // Create and open a new connection to the database
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // SQL query to count users that match both email and password
            string query = "SELECT COUNT(*) FROM users WHERE email = @email AND password = @password;";
            using var cmd = new NpgsqlCommand(query, conn);

            // Add email and password parameters to the query
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);
            
            long count = (long)cmd.ExecuteScalar(); // Execute the query and retrieve the count of matching users
            return count > 0;   // Return true if at least one matching user is found; otherwise, return false
        }

        // Retrieves a user's information from the database by email
        // Returns a RegisterModel populated with the user's data if found; otherwise, returns null.
        public RegisterModel? GetUserByEmail(string email)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // SQL query to select user info by email
            string query = "SELECT id, email, password, name, date_of_birth, height FROM users WHERE email = @email;";
            using var cmd = new NpgsqlCommand(query, conn);

            // Add parameter to prevent SQL injection
            cmd.Parameters.AddWithValue("email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Create and return a new RegisterModel instance populated with data from the database
                return new RegisterModel
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Password = reader.GetString(2),
                    Name = reader.GetString(3),
                    DateOfBirth = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Height = reader.IsDBNull(5) ? null : reader.GetDecimal(5)
                };
            }
            return null;
        }
    }
}
