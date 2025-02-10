# C# Accepted Assessment app

An application for C# (.net) knowledge assessment

## Description

This is a web application that interacts with a 3nd party service (<https://fakeapi.platzi.com>/<https://api.escuelajs.co>) and serves data

We have to do some code refactoring and implement some new features

## Code refactoring

Seems that the use of http client is not so much efficient

Let's make a different, more solid, approach/implementation

## New features

**#1**

Right now only the **getAll** method supported for **products**

We have to implement **getOne** and **create** methods also

**#2**

Add implementation for **categories**

**#3**

3nd party service supports JWT auth. We have to implement and support it. Use the credentials provided to appsettings.json file.

**#4**

We must measure and log the performance of the requests. Create a middleware to achieve this.

## Implementation

* Try to understand and keep the architectural approach.
* Add unit testing.
* Add docker support.
* Using CQRS pattern will be considered as a strong plus.
* The attached collections (postman/insomnia) will help you with the requests.

## Running the Service with Docker Compose

To build and run the service using Docker Compose, follow these steps:

### Step 1: Build the Docker Images

Navigate to the directory containing the `docker-compose.yml` file and run the following command to build the Docker images:

```
docker-compose build
```
### Step 2: Run the Services

After the images have been built, you can start the services by running:

```
docker-compose up
```

## API Documentation

When running the application with **Docker Compose**, you can access the Swagger UI at:

[http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)
