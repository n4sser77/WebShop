# WebShop

## Manager classes 
Manager classes are services that handle orders, users, products,
and user carts. They have direct access to the database,
which the Shop class mainly uses to control the flow of the app.

## Shop class 
This is the main app logic. It is poorly decoupled with some Console UI rendering
and business logic.

## AppDbContext & MongoDBContext
Both contexts inherit EF's DbContext base class that provides 
different database contexts. MongoDB is used for logging user logins and signups,
and AppDbContext is the main database.

## Helpers
There is helpers for UI rendering and input handling
such as password input method, password hashing etc.

## Seeding
Database seeder is using mainly bogus to generate user data
and orders.

Product seeding is hard coded.

## Notes
Notes contains the plans for the project such as models,
database tables and their relationships.

