# Use a Node.js image
FROM node:14

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the application
COPY . .

# Expose the port for the backend service
EXPOSE 3000

# Command to run the application
CMD ["node", "index.js"]
