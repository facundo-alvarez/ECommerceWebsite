# ECommerceWebsite

Goals:
- The goals of this project is to create a fully implemented Ecommerce solution for a market.

Tools:
- ASP.NET Core.
- Razor Pages.
- Clean Architecture for organization of the project.
- Entity Framework Core.
- Entitity Framework Core Identity.
- Braintree payment gateway.

Functionality:
- Ajax filtering of products, adding to cart, adding to favorites, validation, sorting.
- Dynamic cart with ajax quantity and delete.
- Authorization for checkout for clients authenticated and with items on the order.
- Checkout with system for detect lower than 0 balances.
- Repository, and specification patterns for accessing the database using Entity Framework Core.
- Using of session.
- Possibility of adding to cart without beeing logged using session. If logged in, adding to the database with automatic syncronization if you had items in your cart without being autenticated.
- Discounts with expire date and different types of systems (value or percentage).
- Related products based on category.
- Load more button.
- Admin panel for products management.
- Braintree payment gateway for processing payments.
