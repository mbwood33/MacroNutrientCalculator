const express = require('express');
const bodyParser = require('body-parser');
const { Pool } = require('pg');

// Create an Express app
const app = express();
const PORT = process.env.PORT || 5000;

// Middleware to parse JSON
app.use(bodyParser.json());

// PostgreSQL Database Configuration
const pool = new Pool({
    user: 'user', // Replace with your database user
    host: 'db', // Replace with your database host
    database: 'weighttracker', // Replace with your database name
    password: 'password', // Replace with your database password
    port: 5432, // Default PostgreSQL port
});

// Test database connection
pool.connect((err) => {
    if (err) {
        console.error('Database connection error:', err.stack);
    } else {
        console.log('Connected to the database');
    }
});

// Routes
app.get('/', (req, res) => {
    res.send('Weight Tracker Backend is running!');
});

// Get all users
app.get('/api/users', async (req, res) => {
    try {
        const result = await pool.query('SELECT * FROM users');
        res.json(result.rows);
    } catch (err) {
        console.error(err);
        res.status(500).send('Server error');
    }
});

// Add a new user
app.post('/api/users', async (req, res) => {
    const { username, password, dateOfBirth, height } = req.body;
    try {
        const result = await pool.query(
            'INSERT INTO users (username, password, date_of_birth, height) VALUES ($1, $2, $3, $4) RETURNING *',
            [username, password, dateOfBirth, height]
        );
        res.status(201).json(result.rows[0]);
    } catch (err) {
        console.error(err);
        res.status(500).send('Server error');
    }
});

// Get weight logs for a specific user
app.get('/api/users/:userId/weightlogs', async (req, res) => {
    const { userId } = req.params;
    try {
        const result = await pool.query('SELECT * FROM weight_logs WHERE user_id = $1', [userId]);
        res.json(result.rows);
    } catch (err) {
        console.error(err);
        res.status(500).send('Server error');
    }
});

// Add a weight log for a user
app.post('/api/users/:userId/weightlogs', async (req, res) => {
    const { userId } = req.params;
    const { weight, bodyFatPercentage, timestamp } = req.body;
    try {
        const result = await pool.query(
            'INSERT INTO weight_logs (user_id, weight, body_fat_percentage, timestamp) VALUES ($1, $2, $3, $4) RETURNING *',
            [userId, weight, bodyFatPercentage, timestamp]
        );
        res.status(201).json(result.rows[0]);
    } catch (err) {
        console.error(err);
        res.status(500).send('Server error');
    }
});

// Delete a weight log
app.delete('/api/weightlogs/:logId', async (req, res) => {
    const { logId } = req.params;
    try {
        await pool.query('DELETE FROM weight_logs WHERE id = $1', [logId]);
        res.status(204).send();
    } catch (err) {
        console.error(err);
        res.status(500).send('Server error');
    }
});

// Start the server
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
