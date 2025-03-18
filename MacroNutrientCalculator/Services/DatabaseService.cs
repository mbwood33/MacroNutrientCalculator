using Npgsql;
using Microsoft.Extensions.Configuration;
using System;

namespace MacroNutrientCalc.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            InitializeDatabase();
        }

        // Create the users table if it doesn't exist
        private void InitializeDatabase()
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"
                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    email VARCHAR(100) NOT NULL UNIQUE,)
                    password VARCHAR(100) NOT NULL,
                    name VARCHAR(100),
                    date_of_birth DATE,
                    height NUMERIC,
                    weight NUMERIC,
                    body_composition TEXT
                );
            ";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        // Creates a new user with additional information
        public bool CreateUser(string email, string password, string name, DateTime? dateOfBirth, decimal? height)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // Check if the user already exists
            string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @email;";
            using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("email", email);
                long count = (long)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    return false;
                }
            }

            // Inser the new user
            string insertQuery = @"
                INSERT INTO users (email, password, name, date_of_birth, height)
                VALUES (@email, @password, @name, @dateOfBirth, @height);
            ";
            using var insertCmd = new NpgsqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("email", email);
            insertCmd.Parameters.AddWithValue("password", password);
            insertCmd.Parameters.AddWithValue("name", (object?)name ?? DBNull.Value);
            insertCmd.Parameters.AddWithValue("dateOfBirth", dateOfBirth.HasValue ? (object)dateOfBirth.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("height", height.HasValue ? (object)height.Value : DBNull.Value);
            insertCmd.ExecuteNonQuery();
            return true;
        }

        // Validates the user's credentials
        public bool ValidateUser(string email, string password)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT COUNT(*) FROM users WHERE email = @email AND password = @password;";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);
            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
